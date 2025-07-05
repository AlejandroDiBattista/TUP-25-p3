using Microsoft.EntityFrameworkCore;
using tp6.Data;
using tp6.Models;
using Microsoft.AspNetCore.Mvc ;
var builder = WebApplication.CreateBuilder(args);

// CORS para Blazor WebAssembly
var policyName = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// EF Core con SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();
app.UseCors(policyName);

// Inicializar la base de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();
}

// ------------------ ENDPOINTS ------------------

// Ruta de prueba
app.MapGet("/", () => "Servidor API funcionando correctamente");

// GET /productos?q=nombre
app.MapGet("/productos", async (TiendaDbContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        productos = productos.Where(p => p.Nombre.StartsWith(q));

    return await productos.ToListAsync();
});

// POST /carrito - crear nuevo carrito
app.MapPost("/carrito", async (TiendaDbContext db) =>
{
    var nuevo = new Carrito
    {
        CarritoId = Guid.NewGuid(),
        CarritoItems = new List<CarritoItem>()
    };
    db.Carritos.Add(nuevo);
    await db.SaveChangesAsync();
    return Results.Ok(nuevo.CarritoId);
});

// PUT /carrito/{carritoId}/{productoId}?cantidad=x
app.MapPut("/carrito/{carritoId:guid}/{productoId:int}", async (
    Guid carritoId, int productoId, [FromQuery] int cantidad, TiendaDbContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.CarritoItems)
        .FirstOrDefaultAsync(c => c.CarritoId == carritoId);

    if (carrito is null)
        return Results.NotFound();

    var item = carrito.CarritoItems.FirstOrDefault(ci => ci.ProductoId == productoId);

    if (item != null)
    {
        item.Cantidad += cantidad;
        if (item.Cantidad <= 0)
            carrito.CarritoItems.Remove(item);
    }
    else if (cantidad > 0)
    {
        carrito.CarritoItems.Add(new CarritoItem
        {
            ProductoId = productoId,
            Cantidad = cantidad
        });
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});

// GET /carrito/{carritoId}/items
app.MapGet("/carrito/{carritoId:guid}/items", async (Guid carritoId, TiendaDbContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.CarritoItems)
        .ThenInclude(ci => ci.Producto)
        .FirstOrDefaultAsync(c => c.CarritoId == carritoId);

    if (carrito is null)
        return Results.NotFound();

    var resultado = carrito.CarritoItems.Select(ci => new
    {
        ci.ProductoId,
        Nombre = ci.Producto?.Nombre ?? "",
        Precio = ci.Producto?.Precio ?? 0,
        ci.Cantidad,
        Subtotal = (ci.Producto?.Precio ?? 0) * ci.Cantidad
    });

    return Results.Ok(resultado);
});

// GET /carrito/{carritoId}/cantidad
app.MapGet("/carrito/{carritoId:guid}/cantidad", async (Guid carritoId, TiendaDbContext db) =>
{
    var total = await db.CarritoItems
        .Where(ci => ci.CarritoId == carritoId)
        .SumAsync(ci => ci.Cantidad);

    return Results.Ok(total);
});

// DELETE /carrito/{carritoId}
app.MapDelete("/carrito/{carritoId:guid}", async (Guid carritoId, TiendaDbContext db) =>
{
    var items = db.CarritoItems.Where(ci => ci.CarritoId == carritoId);
    db.CarritoItems.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE /carrito/{carritoId}/{productoId}
app.MapDelete("/carrito/{carritoId:guid}/{productoId:int}", async (Guid carritoId, int productoId, TiendaDbContext db) =>
{
    var item = await db.CarritoItems
        .FirstOrDefaultAsync(ci => ci.CarritoId == carritoId && ci.ProductoId == productoId);

    if (item != null)
    {
        db.CarritoItems.Remove(item);
        await db.SaveChangesAsync();
    }

    return Results.NoContent();
});

// PUT /carrito/{carritoId}/confirmar
app.MapPut("/carrito/{carritoId:guid}/confirmar", async (Guid carritoId, TiendaDbContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.CarritoItems)
        .ThenInclude(ci => ci.Producto)
        .FirstOrDefaultAsync(c => c.CarritoId == carritoId);

    if (carrito == null || !carrito.CarritoItems.Any())
        return Results.BadRequest("El carrito está vacío.");

    foreach (var item in carrito.CarritoItems)
    {
        if (item.Producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {item.Producto.Nombre}");

        item.Producto.Stock -= item.Cantidad;
    }

    var compra = new Compra
    {
        Id = Guid.NewGuid(),
        Fecha = DateTime.UtcNow,
        Total = carrito.CarritoItems.Sum(i => i.Producto.Precio * i.Cantidad),
        Items = carrito.CarritoItems.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };

    db.Compras.Add(compra);
    db.CarritoItems.RemoveRange(carrito.CarritoItems);
    await db.SaveChangesAsync();

    return Results.Ok("Compra confirmada correctamente.");
});

app.Run();