using Microsoft.EntityFrameworkCore;
using Servidor.Datos;
using Servidor.Modelos;
using Tienda.Modelos;
using Cliente.Modelos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:5184",
            "https://localhost:7221",
            "http://localhost:5177"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
var app = builder.Build();
app.UseCors();
// var testCarrito = new Tienda.Modelos.Carrito();
var carritos = new Dictionary<Guid, Carrito>();

app.MapGet("/productos", async (TiendaDbContext db, string? query) =>
{
    var productos = string.IsNullOrWhiteSpace(query)
        ? await db.Productos.ToListAsync()
        : await db.Productos
            .Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query))
            .ToListAsync();

    return Results.Ok(productos);
});
// Buscar Imagenes de productos
app.MapPost("/productos", async (TiendaDbContext db) =>
{
    db.Productos.RemoveRange(db.Productos);
    await db.SaveChangesAsync();

    return Results.Ok("Productos cargados correctamente");
});
app.MapPost("/carritos", () =>
{
    var carrito = new Carrito();
    carritos[carrito.Id] = carrito;
    return Results.Ok(carrito.Id);
});

app.MapGet("/carritos/{id:guid}", (Guid id) =>
{
    if (carritos.TryGetValue(id, out var carrito))
        return Results.Ok(carrito);

    return Results.NotFound();
});

app.MapDelete("/carritos/{id:guid}", (Guid id) =>
{
    if (carritos.Remove(id))
        return Results.Ok();

    return Results.NotFound();
});

app.MapPut("/carritos/{id:guid}/{productoId:int}", async (Guid id, int productoId, TiendaDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null || producto.Stock <= 0)
        return Results.BadRequest("Producto no disponible");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item != null)
    {
        if (producto.Stock <= item.Cantidad)
            return Results.BadRequest("No hay stock disponible para aumentar la cantidad");

        item.Cantidad++;
    }
    else
    {
        carrito.Items.Add(new ItemCarrito
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            Cantidad = 1,
            PrecioUnitario = producto.Precio
        });
    }

    return Results.Ok(carrito);
});

app.MapDelete("/carritos/{id:guid}/{productoId:int}", (Guid id, int productoId) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no encontrado en carrito");

    item.Cantidad--;
    if (item.Cantidad <= 0)
        carrito.Items.Remove(item);

    return Results.Ok(carrito);
});

app.MapPut("/carritos/{id:guid}/confirmar", async (Guid id, ClienteDto cliente, TiendaDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    if (!carrito.Items.Any())
        return Results.BadRequest("El carrito está vacío");
        
    if (string.IsNullOrWhiteSpace(cliente.Email) || !cliente.Email.Contains("@"))
        return Results.BadRequest("Email inválido");

    var compra = new Compra
    {
        Fecha = DateTime.UtcNow,
        NombreCliente = cliente.Nombre,
        ApellidoCliente = cliente.Apellido,
        EmailCliente = cliente.Email,
        Total = carrito.Items.Sum(i => i.PrecioUnitario * i.Cantidad)
    };

    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {item.Nombre}");

        producto.Stock -= item.Cantidad;

        compra.ItemsCompra.Add(new ItemCompra
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.PrecioUnitario
        });
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    carritos.Remove(id);

    return Results.Ok("Compra confirmada");
});

app.MapGet("/compras", async (TiendaDbContext db) =>
{
    var compras = await db.Compras
        .Include(c => c.ItemsCompra)
        .ThenInclude(ic => ic.Producto)
        .ToListAsync();

    return Results.Ok(compras);
});

app.MapGet("/compras/{id:int}", async (int id, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.ItemsCompra)
        .ThenInclude(ic => ic.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (compra == null)
        return Results.NotFound("Compra no encontrada");

    return Results.Ok(compra);
});

app.UseStaticFiles();
app.Run();
