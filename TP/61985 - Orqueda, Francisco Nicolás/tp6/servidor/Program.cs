using Microsoft.EntityFrameworkCore;
using servidor.Models;
using servidor.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Obtener todos los productos
app.MapGet("/api/productos", async ([FromServices] TiendaContext db) =>
    await db.Productos.ToListAsync());

// Obtener un producto por ID
app.MapGet("/api/productos/{id}", async (int id, [FromServices] TiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto
        ? Results.Ok(producto)
        : Results.NotFound());

// Crear un nuevo producto
app.MapPost("/api/productos", async ([FromBody] Producto prod, [FromServices] TiendaContext db) =>
{
    db.Productos.Add(prod);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{prod.Id}", prod);
});

// Actualizar un producto existente
app.MapPut("/api/productos/{id}", async (int id, [FromBody] Producto prodActualizado, [FromServices] TiendaContext db) =>
{
    var prodExistente = await db.Productos.FindAsync(id);
    if (prodExistente is null)
        return Results.NotFound();

    prodExistente.Nombre = prodActualizado.Nombre;
    prodExistente.Descripcion = prodActualizado.Descripcion;
    prodExistente.Precio = prodActualizado.Precio;
    prodExistente.Stock = prodActualizado.Stock;
    prodExistente.ImagenUrl = prodActualizado.ImagenUrl;

    await db.SaveChangesAsync();
    return Results.Ok(prodExistente);
});

// Eliminar un producto
app.MapDelete("/api/productos/{id}", async (int id, [FromServices] TiendaContext db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null)
        return Results.NotFound();

    db.Productos.Remove(prod);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Obtener todas las compras
app.MapGet("/api/compras", async ([FromServices] TiendaContext db) =>
    await db.Compras.ToListAsync());

// Obtener una compra por ID
app.MapGet("/api/compras/{id}", async (int id, [FromServices] TiendaContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    return compra is not null ? Results.Ok(compra) : Results.NotFound();
});

// Crear una nueva compra
app.MapPost("/api/compras", async ([FromBody] Compra compra, [FromServices] TiendaContext db) =>
{
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Created($"/api/compras/{compra.Id}", compra);
});

app.MapPut("/api/compras/{id}", async (int id, [FromBody] Compra compraActualizada, [FromServices] TiendaContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    if (compra is null) return Results.NotFound();

    compra.Fecha = compraActualizada.Fecha;
    compra.Total = compraActualizada.Total;
    // compra.ProductoId = compraActualizada.ProductoId; // Solo si tu modelo lo tiene

    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

app.MapDelete("/api/compras/{id}", async (int id, [FromServices] TiendaContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    if (compra is null) return Results.NotFound();

    db.Compras.Remove(compra);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Ejemplo endpoint extra
app.MapPost("/algo", ([FromServices] TiendaContext db) => {
    // Lógica para el endpoint /algo
    return Results.Ok();
});

// Endpoint de ejemplo para productos (no necesario si ya tienes /api/productos)
app.MapGet("/productos", ([FromServices] TiendaContext db) =>
{
    return db.Productos.ToList();
});

// Diccionario en memoria para simular carritos (puedes mejorar esto con EF)
var carritos = new Dictionary<Guid, List<CarritoItem>>();

// Crear un nuevo carrito
app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid();
    carritos[id] = new List<CarritoItem>();
    return Results.Ok(id);
});

app.MapGet("/carritos/{carritoId}", async (Guid carritoId, [FromServices] TiendaContext db) =>
{
    if (!carritos.TryGetValue(carritoId, out var items))
        return Results.NotFound();

    // Cargar los datos completos del producto para cada item
    var result = new List<servidor.Models.CarritoItem>();
    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        result.Add(new servidor.Models.CarritoItem
        {
            Id = item.Id,
            ProductoId = item.ProductoId,
            Producto = producto,
            Cantidad = item.Cantidad
        });
    }
    return Results.Ok(result);
});

// Agregar o actualizar producto en carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (Guid carritoId, int productoId, [FromBody] int cantidad, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null || producto.Stock < cantidad)
        return Results.BadRequest("Producto no disponible o stock insuficiente.");

    var items = carritos[carritoId];
    var item = items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        items.Add(new CarritoItem { ProductoId = productoId, Producto = producto, Cantidad = cantidad });
    }
    else
    {
        item.Cantidad = cantidad;
    }
    return Results.Ok();
});

// Vaciar carrito
app.MapDelete("/carritos/{carritoId}", (Guid carritoId) =>
{
    if (carritos.ContainsKey(carritoId))
        carritos[carritoId].Clear();
    return Results.Ok();
});

// Quitar producto del carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", (Guid carritoId, int productoId) =>
{
    if (carritos.TryGetValue(carritoId, out var items))
    {
        var item = items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item != null)
            items.Remove(item);
    }
    return Results.Ok();
});

app.MapPut("/carritos/{carritoId}/confirmar", async (Guid carritoId, [FromBody] CompraConfirmacionDto datos, [FromServices] TiendaContext db) =>
{
    if (!carritos.TryGetValue(carritoId, out var items) || items.Count == 0)
        return Results.BadRequest("El carrito está vacío.");

    decimal total = items.Sum(i => i.Producto.Precio * i.Cantidad);

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = datos.Nombre,
        ApellidoCliente = datos.Apellido,
        EmailCliente = datos.Email,
        Items = items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    items.Clear();

    return Results.Ok();
});



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}

app.Run();
