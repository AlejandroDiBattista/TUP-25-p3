using servidor.Models;
using Microsoft.EntityFrameworkCore;
using servidor.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));
    
// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    DbInitializer.Inicializar(db);
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (TiendaContext db, string? nombre) =>
{
    var productos = string.IsNullOrWhiteSpace(nombre)
        ? await db.Productos.ToListAsync()
        : await db.Productos
            .Where(p => p.Nombre.Contains(nombre))
            .ToListAsync();

    return Results.Ok(productos);
});

app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito { FechaCreacion = DateTime.UtcNow };
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(carrito.Id);
});

app.MapGet("/carritos/{carritoId:guid}", async (Guid carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    return Results.Ok(carrito.Items.Select(i => new {
        ProductoId = i.ProductoId,
        Nombre = i.Producto.Nombre,
        Precio = i.Producto.Precio,
        Cantidad = i.Cantidad,
        Subtotal = i.Cantidad * i.Producto.Precio
    }));
});

// VACÍAR CARRITO Y DEVOLVER STOCK
app.MapDelete("/carritos/{carritoId:guid}", async (Guid carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    // Devolver stock de todos los productos del carrito
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
            producto.Stock += item.Cantidad;
    }

    db.ItemsCarrito.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();

    return Results.Ok("Carrito vaciado");
});

// AGREGAR PRODUCTO AL CARRITO Y DESCONTAR STOCK
app.MapPut("/carritos/{carritoId:guid}/{productoId:int}", async (Guid carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);

    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (item != null)
    {
        if (producto.Stock < 1)
            return Results.BadRequest("No hay suficiente stock disponible");

        item.Cantidad++;
        producto.Stock--; // Descontar stock
    }
    else
    {
        if (producto.Stock < 1)
            return Results.BadRequest("Producto sin stock");

        carrito.Items.Add(new ItemCarrito
        {
            ProductoId = productoId,
            Cantidad = 1
        });
        producto.Stock--; // Descontar stock
    }

    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado o actualizado en el carrito");
});

// QUITAR PRODUCTO DEL CARRITO Y DEVOLVER STOCK
app.MapDelete("/carritos/{carritoId:guid}/{productoId:int}", async (Guid carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (item == null)
        return Results.NotFound("Producto no está en el carrito");

    var producto = await db.Productos.FindAsync(productoId);

    if (item.Cantidad > 1)
    {
        item.Cantidad--;
        if (producto != null)
            producto.Stock++; // Devolver stock
    }
    else
    {
        db.ItemsCarrito.Remove(item);
        if (producto != null)
            producto.Stock++; // Devolver stock
    }

    await db.SaveChangesAsync();
    return Results.Ok("Producto actualizado o eliminado del carrito");
});

// CONFIRMAR COMPRA (ya no descuenta stock aquí, solo verifica)
app.MapPut("/carritos/{carritoId:guid}/confirmar", async (Guid carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    if (!carrito.Items.Any())
        return Results.BadRequest("El carrito está vacío");

    foreach (var item in carrito.Items)
    {
        if (item.Cantidad > item.Producto.Stock + item.Cantidad) // El stock ya fue descontado al agregar
        {
            return Results.BadRequest($"No hay suficiente stock para {item.Producto.Nombre}");
        }
    }

    var compra = new Compra
    {
        Cliente = "Anónimo", 
        Fecha = DateTime.UtcNow,
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };

    db.Compras.Add(compra);
    db.ItemsCarrito.RemoveRange(carrito.Items);
    
    await db.SaveChangesAsync();

    return Results.Ok("Compra confirmada");
});

app.MapDelete("/carritos/{carritoId:guid}/{productoId:int}/todo", async (Guid carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no está en el carrito");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
        producto.Stock += item.Cantidad; // Devuelve todo el stock

    db.ItemsCarrito.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok("Producto eliminado completamente del carrito");
});

app.Run();