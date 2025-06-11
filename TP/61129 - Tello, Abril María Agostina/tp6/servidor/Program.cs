using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");
app.UseRouting();

app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// --- Productos CRUD ---

app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync());

app.MapGet("/api/productos/{id}", async (int id, TiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto ? Results.Ok(producto) : Results.NotFound());

app.MapPost("/api/productos", async (Producto producto, TiendaContext db) =>
{
    if (string.IsNullOrWhiteSpace(producto.Nombre) || producto.Precio < 0 || producto.Stock < 0)
        return Results.BadRequest("Datos inválidos.");

    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{producto.Id}", producto);
});

app.MapPut("/api/productos/{id}", async (int id, Producto inputProducto, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(inputProducto.Nombre) || inputProducto.Precio < 0 || inputProducto.Stock < 0)
        return Results.BadRequest("Datos inválidos.");

    producto.Nombre = inputProducto.Nombre;
    producto.Descripcion = inputProducto.Descripcion;
    producto.Precio = inputProducto.Precio;
    producto.Stock = inputProducto.Stock;
    producto.ImagenUrl = inputProducto.ImagenUrl;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/productos/{id}", async (int id, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// --- Compras CRUD ---

app.MapGet("/api/compras", async (TiendaContext db) =>
    await db.Compras
            .Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .ToListAsync());

app.MapGet("/api/compras/{id}", async (int id, TiendaContext db) =>
{
    var compra = await db.Compras
                        .Include(c => c.Items)
                        .ThenInclude(i => i.Producto)
                        .FirstOrDefaultAsync(c => c.Id == id);
    return compra is not null ? Results.Ok(compra) : Results.NotFound();
});

app.MapPost("/api/compras", async (Compra compra, TiendaContext db) =>
{
    if (compra.Items == null || compra.Items.Count == 0)
        return Results.BadRequest("La compra debe tener al menos un item.");

    compra.Fecha = DateTime.UtcNow;
    // Calculamos total automáticamente
    compra.Total = 0;
    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null)
            return Results.BadRequest($"Producto con id {item.ProductoId} no existe.");

        if (item.Cantidad <= 0)
            return Results.BadRequest("La cantidad debe ser mayor a cero.");

        if (producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay stock suficiente para el producto {producto.Nombre}.");

        item.PrecioUnitario = producto.Precio;
        compra.Total += item.PrecioUnitario * item.Cantidad;

        producto.Stock -= item.Cantidad; // Reducimos stock
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Created($"/api/compras/{compra.Id}", compra);
});

app.MapPut("/api/compras/{id}", async (int id, Compra inputCompra, TiendaContext db) =>
{
    var compra = await db.Compras
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.Id == id);

    if (compra == null) return Results.NotFound();

    // No permitimos cambiar la fecha ni el total directamente
    // Solo actualizar cliente o items (por simplicidad, no se actualizan items aquí)

    compra.ClienteId = inputCompra.ClienteId;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/compras/{id}", async (int id, TiendaContext db) =>
{
    var compra = await db.Compras
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.Id == id);

    if (compra == null) return Results.NotFound();

    // Al borrar la compra, devolvemos el stock
    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
            producto.Stock += item.Cantidad;
    }

    db.ItemsCompra.RemoveRange(compra.Items);
    db.Compras.Remove(compra);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// --- ItemsCompra CRUD ---

app.MapGet("/api/itemscompra", async (TiendaContext db) =>
    await db.ItemsCompra.Include(i => i.Producto).Include(i => i.Compra).ToListAsync());

app.MapGet("/api/itemscompra/{id}", async (int id, TiendaContext db) =>
    await db.ItemsCompra.Include(i => i.Producto).Include(i => i.Compra).FirstOrDefaultAsync(i => i.Id == id)
    is ItemCompra item ? Results.Ok(item) : Results.NotFound());

app.MapPost("/api/itemscompra", async (ItemCompra itemCompra, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(itemCompra.ProductoId);
    var compra = await db.Compras.FindAsync(itemCompra.CompraId);

    if (producto == null) return Results.BadRequest("Producto no encontrado.");
    if (compra == null) return Results.BadRequest("Compra no encontrada.");
    if (itemCompra.Cantidad <= 0) return Results.BadRequest("Cantidad inválida.");

    if (producto.Stock < itemCompra.Cantidad)
        return Results.BadRequest("No hay stock suficiente.");

    itemCompra.PrecioUnitario = producto.Precio;
    producto.Stock -= itemCompra.Cantidad;

    db.ItemsCompra.Add(itemCompra);
    await db.SaveChangesAsync();

    // Actualizamos total de la compra
    compra.Total += itemCompra.PrecioUnitario * itemCompra.Cantidad;
    await db.SaveChangesAsync();

    return Results.Created($"/api/itemscompra/{itemCompra.Id}", itemCompra);
});

app.MapPut("/api/itemscompra/{id}", async (int id, ItemCompra inputItem, TiendaContext db) =>
{
    var itemCompra = await db.ItemsCompra.FindAsync(id);
    if (itemCompra == null) return Results.NotFound();

    var producto = await db.Productos.FindAsync(inputItem.ProductoId);
    if (producto == null) return Results.BadRequest("Producto no encontrado.");
    if (inputItem.Cantidad <= 0) return Results.BadRequest("Cantidad inválida.");

    // Ajustar stock: devolver el anterior y restar el nuevo
    var productoAnterior = await db.Productos.FindAsync(itemCompra.ProductoId);
    if (productoAnterior != null)
        productoAnterior.Stock += itemCompra.Cantidad;

    if (producto.Stock < inputItem.Cantidad)
        return Results.BadRequest("No hay stock suficiente.");

    producto.Stock -= inputItem.Cantidad;

    // Actualizamos campos
    itemCompra.ProductoId = inputItem.ProductoId;
    itemCompra.Cantidad = inputItem.Cantidad;
    itemCompra.PrecioUnitario = producto.Precio;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/itemscompra/{id}", async (int id, TiendaContext db) =>
{
    var itemCompra = await db.ItemsCompra.FindAsync(id);
    if (itemCompra == null) return Results.NotFound();

    var producto = await db.Productos.FindAsync(itemCompra.ProductoId);
    if (producto != null)
        producto.Stock += itemCompra.Cantidad;

    var compra = await db.Compras.FindAsync(itemCompra.CompraId);
    if (compra != null)
    {
        compra.Total -= itemCompra.PrecioUnitario * itemCompra.Cantidad;
    }

    db.ItemsCompra.Remove(itemCompra);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
