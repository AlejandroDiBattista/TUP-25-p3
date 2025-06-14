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
            new Producto { Nombre = "Google Pixel 8 Pro", Descripcion = "Google Pixel 8 Pro 128GB - Obsidiana", Precio = 999, Stock = 15, ImagenUrl = "https://celularesindustriales.com.ar/wp-content/uploads/71h9zq4viSL._AC_UF8941000_QL80_.jpg" },
            new Producto { Nombre = "Google Pixel Watch 2", Descripcion = "Google Pixel Watch 2 con LTE - Plata/Azul Celeste", Precio = 399, Stock = 20, ImagenUrl = "https://http2.mlstatic.com/D_609452-MLU77279991570_072024-C.jpg" },
            new Producto { Nombre = "Google Pixel Buds Pro", Descripcion = "Google Pixel Buds Pro - Porcelana", Precio = 199, Stock = 25, ImagenUrl = "https://i5.walmartimages.com/seo/Google-Pixel-Buds-Pro-Wireless-Earbuds-with-Active-Noise-Cancellation-Bluetooth-Earbuds-Fog_5f8d8e03-bfe9-4099-994c-cea7552ce02d.9cdcbd2e072b93f0fb8ec60dcfc98ca7.jpeg" },
            new Producto { Nombre = "Google Nest Hub Max", Descripcion = "Google Nest Hub Max con Asistente de Google", Precio = 229, Stock = 10, ImagenUrl = "https://lh3.googleusercontent.com/uQZNPuGyf7dKvtGZWjoiyGcPg_A44yUS2tx-o2--dyuwp9A1vR4Efh1UF28KKLpGUg=w895" },
            new Producto { Nombre = "Google Nest Doorbell (Batería)", Descripcion = "Google Nest Doorbell con batería", Precio = 179, Stock = 12, ImagenUrl = "https://i5.walmartimages.com/seo/Google-Nest-Doorbell-Battery-Video-Doorbell-Camera-Wireless-Doorbell-Security-Camera-Snow_807fb01e-45c2-43b7-815c-69a78955ee7f.aacdb8a51e4600aabdfdddb3d729343c.jpeg" },
            new Producto { Nombre = "Google Chromecast con Google TV (4K)", Descripcion = "Chromecast con Google TV - Nieve", Precio = 49, Stock = 30, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_820731-MEC49046529482_022022-O.webp" },
            new Producto { Nombre = "Google Nest Mini", Descripcion = "Google Nest Mini (2da gen.) - Tiza", Precio = 49, Stock = 40, ImagenUrl = "https://www.cdmarket.com.ar/image/0/1000_1300-nestmininegro.jpg" },
            new Producto { Nombre = "Google Wifi (Pack de 3)", Descripcion = "Sistema Wi-Fi en malla Google Wifi (3 unidades)", Precio = 199, Stock = 8, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_890254-MLA32739750061_112019-O.webp" },
            new Producto { Nombre = "Fitbit Charge 6", Descripcion = "Fitbit Charge 6 - Negro Obsidiana", Precio = 159, Stock = 18, ImagenUrl = "https://m.media-amazon.com/images/I/61ZtqtvoD2L.jpg" },
            new Producto { Nombre = "Google Pixel Tablet", Descripcion = "Google Pixel Tablet con base de carga y altavoz", Precio = 499, Stock = 7, ImagenUrl = "https://storage.googleapis.com/support-kms-prod/DwjEEz9EqLvL0HHbIZsdtjj2uMWg5KttRFxa" }
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
    var carrito = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound();

    var result = new {
        Id = carrito.Id,
        Items = carrito.Items.Select(i => new {
            i.Id,
            Producto = new {
                i.Producto.Id,
                i.Producto.Nombre,
                i.Producto.Descripcion,
                i.Producto.ImagenUrl,
                i.Producto.Precio,
                Stock = i.Producto.Stock
            },
            i.Cantidad,
            i.PrecioUnitario
        }).ToList()
    };

    return Results.Ok(result);
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

// PUT /carritos/{carritoId}/{productoId}?cantidad=1
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

// GET /carritos/{carritoId}/cantidad → Devuelve la cantidad total de productos en el carrito
app.MapGet("/carritos/{carritoId}/cantidad", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    int cantidad = carrito.Items.Sum(i => i.Cantidad);
    return Results.Ok(cantidad);
});

app.Run();