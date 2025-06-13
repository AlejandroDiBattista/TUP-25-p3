using Microsoft.EntityFrameworkCore;
using servidor.Models;
using servidor.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Base de datos SQLite
builder.Services.AddDbContext<TiendaContext>(opt =>
    opt.UseSqlite("Data Source=tienda.db"));

// CORS para permitir conexiones del frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors();

// Endpoint simple de prueba
app.MapGet("/", () => "API Tienda Online funcionando");

// Obtener todos los productos
app.MapGet("/productos", async (TiendaContext db, string? search) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(p => p.Nombre.Contains(search));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{carrito.Id}", carrito);
});

app.MapGet("/carritos/{carritoId}", async (int carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
            .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound();

    // Opcional: devolver solo la info necesaria (productos y cantidades)
    var carritoDto = new
    {
        carrito.Id,
        Items = carrito.Items.Select(i => new
        {
            ProductoId = i.ProductoId,
            Nombre = i.Producto.Nombre,
            PrecioUnitario = i.PrecioUnitario,
            Cantidad = i.Cantidad,
            Subtotal = i.PrecioUnitario * i.Cantidad
        }),
        Total = carrito.Items.Sum(i => i.PrecioUnitario * i.Cantidad)
    };

    return Results.Ok(carritoDto);
});

app.MapDelete("/carritos/{carritoId}", async (int carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound();

    db.CarritoItems.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();

    return Results.Ok(new { mensaje = "Carrito vaciado correctamente." });
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (
    int carritoId, int productoId, TiendaContext db) =>
{
    // Buscar el carrito
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    // Buscar el producto
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    // Verificar si hay stock disponible
    var cantidadActualEnCarrito = carrito.Items
        .FirstOrDefault(i => i.ProductoId == productoId)?.Cantidad ?? 0;

    if (producto.Stock <= cantidadActualEnCarrito)
        return Results.BadRequest("No hay suficiente stock disponible");

    // Buscar si ya existe ese producto en el carrito
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item != null)
    {
        item.Cantidad++;
    }
    else
    {
        item = new CarritoItem
        {
            ProductoId = productoId,
            Cantidad = 1,
            PrecioUnitario = producto.Precio
        };
        carrito.Items.Add(item);
    }

    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Producto agregado al carrito." });
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (
    int carritoId, int productoId, TiendaContext db) =>
{
    // Buscar el carrito
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    // Buscar el item en el carrito
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no está en el carrito");

    // Reducir la cantidad o eliminar
    if (item.Cantidad > 1)
    {
        item.Cantidad--;
    }
    else
    {
        carrito.Items.Remove(item);
        db.CarritoItems.Remove(item); // También lo quitamos de la base
    }

    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Producto actualizado en el carrito" });
});


app.MapPost("/api/compras", async (CompraDto dto, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == dto.CarritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var compra = new Compra
    {
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        Total = carrito.Items.Sum(i => i.PrecioUnitario * i.Cantidad),
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };

    db.Compras.Add(compra);

    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
            producto.Stock -= item.Cantidad;
    }

    db.CarritoItems.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();

    return Results.Ok(new { mensaje = "Compra registrada exitosamente." });
});

app.Run();
