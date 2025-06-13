using Microsoft.EntityFrameworkCore;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

app.UseCors("AllowClientApp");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "iPhone 14", Descripcion = "Apple iPhone 14 128GB", Precio = 999, Stock = 20, ImagenUrl = " " },
            new Producto { Nombre = "Samsung Galaxy S22", Descripcion = "Samsung Galaxy S22 5G", Precio = 899, Stock = 15, ImagenUrl = "https://example.com/galaxy.jpg" },
            new Producto { Nombre = "Coca Cola 2L", Descripcion = "Botella de gaseosa 2L", Precio = 1.99M, Stock = 100, ImagenUrl = "https://example.com/coca.jpg" },
            new Producto { Nombre = "Auriculares Sony", Descripcion = "Auriculares Bluetooth", Precio = 49.99M, Stock = 30, ImagenUrl = "https://example.com/sony.jpg" },
            new Producto { Nombre = "Mouse Logitech G502", Descripcion = "Mouse gamer Logitech", Precio = 59.99M, Stock = 25, ImagenUrl = "https://example.com/g502.jpg" },
            new Producto { Nombre = "Teclado Mecánico", Descripcion = "Teclado retroiluminado RGB", Precio = 79.99M, Stock = 12, ImagenUrl = "https://example.com/teclado.jpg" },
            new Producto { Nombre = "Notebook Dell", Descripcion = "Laptop Dell Inspiron 15", Precio = 1199, Stock = 10, ImagenUrl = "https://example.com/dell.jpg" },
            new Producto { Nombre = "Smartwatch Xiaomi", Descripcion = "Reloj inteligente Mi Band", Precio = 39.99M, Stock = 50, ImagenUrl = "https://example.com/xiaomi.jpg" },
            new Producto { Nombre = "Cargador Anker", Descripcion = "Cargador rápido USB-C", Precio = 29.99M, Stock = 40, ImagenUrl = "https://example.com/anker.jpg" },
            new Producto { Nombre = "Tablet Lenovo", Descripcion = "Tablet Android 10.1\"", Precio = 199.99M, Stock = 18, ImagenUrl = "https://example.com/lenovo.jpg" },
        });
        db.SaveChanges();
    }
}

// ENDPOINTS MINIMAL API

// GET /productos (+ búsqueda por query)
app.MapGet("/productos", async (AppDbContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        productos = productos.Where(p => p.Nombre.Contains(q));
    return await productos.ToListAsync();
});

// POST /carritos (inicializa el carrito)
app.MapPost("/carritos", async (AppDbContext db) =>
{
    var carrito = new Compra { Fecha = DateTime.Now, Total = 0 };
    db.Compras.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(carrito.Id);
});

// GET /carritos/{carrito} → Trae los ítems del carrito
app.MapGet("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Compras.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    return Results.Ok(carrito);
});

// DELETE /carritos/{carrito} → Vacía el carrito
app.MapDelete("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    db.ItemsCompra.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carrito}/confirmar (detalle + datos cliente)
app.MapPut("/carritos/{carritoId}/confirmar", async (AppDbContext db, int carritoId, Compra datos) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    // Validar stock
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay stock suficiente para {producto?.Nombre ?? "producto desconocido"}");
    }
    // Descontar stock y guardar datos cliente
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
    }
    carrito.NombreCliente = datos.NombreCliente;
    carrito.ApellidoCliente = datos.ApellidoCliente;
    carrito.EmailCliente = datos.EmailCliente;
    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carrito}/{producto} → Agrega producto o actualiza cantidad
app.MapPut("/carritos/{carritoId}/{productoId}", async (AppDbContext db, int carritoId, int productoId, int cantidad) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    var producto = await db.Productos.FindAsync(productoId);
    if (carrito == null || producto == null) return Results.NotFound();
    if (producto.Stock < cantidad) return Results.BadRequest("No hay stock suficiente");
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        item = new ItemCompra { ProductoId = productoId, Cantidad = cantidad, PrecioUnitario = producto.Precio };
        carrito.Items.Add(item);
        db.ItemsCompra.Add(item);
    }
    else
    {
        item.Cantidad = cantidad;
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

// DELETE /carritos/{carrito}/{producto} → Elimina producto o reduce cantidad
app.MapDelete("/carritos/{carritoId}/{productoId}", async (AppDbContext db, int carritoId, int productoId) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();
    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();