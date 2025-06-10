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

app.MapDelete("/carritos/{carritoId:guid}", async (Guid carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    db.ItemsCarrito.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();

    return Results.Ok("Carrito vaciado");
});

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
        if (producto.Stock < item.Cantidad + 1)
            return Results.BadRequest("No hay suficiente stock disponible");

        item.Cantidad++;
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
    }

    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado o actualizado en el carrito");
});

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

    if (item.Cantidad > 1)
    {
        item.Cantidad--;
    }
    else
    {
        db.ItemsCarrito.Remove(item);
    }

    await db.SaveChangesAsync();
    return Results.Ok("Producto actualizado o eliminado del carrito");
});

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
        if (item.Cantidad > item.Producto.Stock)
        {
            return Results.BadRequest($"No hay suficiente stock para {item.Producto.Nombre}");
        }
    }

    foreach (var item in carrito.Items)
    {
        item.Producto.Stock -= item.Cantidad;
    }

    var compra = new Compra
    {
        Cliente = "Anónimo", // valor fijo porque no se recibe del cliente
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

app.Run();
