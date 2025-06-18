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
    var query = from p in db.Productos
                select new
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    ImagenUrl = p.ImagenUrl,
                    Stock = p.Stock - db.ItemsCompra
                        .Where(i => i.ProductoId == p.Id)
                        .Sum(i => (int?)i.Cantidad) ?? 0
                };

    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q));

    var result = await query.ToListAsync();
    return Results.Ok(result);
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
{    var carrito = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    db.ItemsCompra.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carrito}/confirmar (detalle + datos cliente)
app.MapPut("/carritos/{carritoId}/confirmar", async (AppDbContext db, int carritoId, Compra datos) =>
{
    var carrito = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    
    if (carrito == null) return Results.NotFound();

    // Validar stock actual considerando otros carritos
    foreach (var item in carrito.Items)
    {
        var cantidadEnOtrosCarritos = await db.ItemsCompra
            .Where(i => i.ProductoId == item.ProductoId && i.Compra.Id != carritoId)
            .SumAsync(i => i.Cantidad);

        var stockDisponible = item.Producto.Stock - cantidadEnOtrosCarritos;

        if (stockDisponible < item.Cantidad)
        {
            return Results.BadRequest($"No hay stock suficiente para {item.Producto?.Nombre ?? "producto desconocido"}. Stock disponible: {stockDisponible}");
        }
    }

    // Descontar stock permanentemente y guardar datos cliente
    foreach (var item in carrito.Items)
    {
        item.Producto.Stock -= item.Cantidad; // Reducir stock permanentemente
    }

    carrito.NombreCliente = datos.NombreCliente;
    carrito.ApellidoCliente = datos.ApellidoCliente;
    carrito.EmailCliente = datos.EmailCliente;
    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
    carrito.Fecha = DateTime.Now; // Registrar fecha de confirmación
    
    await db.SaveChangesAsync();
    return Results.Ok();
});

// GET /productos/{productoId}/stock-disponible → Obtiene el stock real disponible
app.MapGet("/productos/{productoId}/stock-disponible", async (AppDbContext db, int productoId) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound();

    // Calcular cuántas unidades están en carritos
    var cantidadEnCarritos = await db.ItemsCompra
        .Where(i => i.ProductoId == productoId)
        .SumAsync(i => i.Cantidad);

    return Results.Ok(new { 
        StockTotal = producto.Stock,
        StockEnCarritos = cantidadEnCarritos,
        StockDisponible = producto.Stock - cantidadEnCarritos
    });
});

// PUT /carritos/{carritoId}/{productoId}?cantidad=1
app.MapPut("/carritos/{carritoId}/{productoId}", async (AppDbContext db, int carritoId, int productoId, int cantidad) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    var producto = await db.Productos.FindAsync(productoId);
    if (carrito == null || producto == null) return Results.NotFound();

    // Obtener la cantidad total en todos los carritos excepto el actual
    var cantidadEnOtrosCarritos = await db.ItemsCompra
        .Where(i => i.ProductoId == productoId && i.Compra.Id != carritoId)
        .SumAsync(i => i.Cantidad);

    var stockRealDisponible = producto.Stock; // Stock total del producto
    var cantidadActual = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId)?.Cantidad ?? 0;
    var stockDisponibleParaEsteCarrito = stockRealDisponible - cantidadEnOtrosCarritos;

    // Verificar si hay suficiente stock disponible
    if (cantidad > stockDisponibleParaEsteCarrito + cantidadActual)
    {
        return Results.BadRequest($"No hay suficiente stock. Stock disponible: {stockDisponibleParaEsteCarrito + cantidadActual}");
    }

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        item = new ItemCompra { 
            ProductoId = productoId, 
            Cantidad = cantidad, 
            PrecioUnitario = producto.Precio 
        };
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

// DELETE /carritos/{carritoId}/{productoId} → Elimina producto o reduce cantidad
app.MapDelete("/carritos/{carritoId}/{productoId}", async (AppDbContext db, int carritoId, int productoId) =>
{    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    // Restaurar el stock del producto
    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += item.Cantidad;
    }
    
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