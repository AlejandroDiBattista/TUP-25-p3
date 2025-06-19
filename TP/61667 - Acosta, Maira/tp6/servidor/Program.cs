using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Conexión a SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Habilitar CORS para el cliente Blazor
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowClientApp");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new List<Producto>
        {
            new Producto {Nombre = "Smar TV Led",Descripcion = "Samsung 55 Un55k6500 Fhd 1080p Quad Core",Precio = 900000,Stock = 100,ImagenUrl  = "http://localhost:5184/img/televisor.png" },
            new Producto {Nombre = "Heladera French Door",Descripcion = "3 puertas con Spacemax 690L",Precio = 1200000,Stock = 150,ImagenUrl = "http://localhost:5184/img/heladera.png" },
            new Producto {Nombre = "Cocina Whirlpool",Descripcion = " Hierro 56 cm",Precio = 200000,Stock = 200,ImagenUrl = "http://localhost:5184/img/cocina.jpg" },
            new Producto {Nombre = "Secador Philips ",Descripcion = "DryCare Pro 2200WW",Precio = 50000,Stock = 300,ImagenUrl = "http://localhost:5184/img/secapelo.jpg" },
            new Producto {Nombre = "Microondas Samsung",Descripcion = "20L BLANCO ME731K-KD",Precio = 250000,Stock = 50,ImagenUrl = "http://localhost:5184/img/microondas.jpg" },
            new Producto {Nombre = "Freidora de Aire Aiwa",Descripcion = "Fa35 - 3,6l - 1400w - 360°c",Precio = 130000,Stock = 12,ImagenUrl = "http://localhost:5184/img/freidora.jpg" },
            new Producto {Nombre = "SecaRopa Khoinoor",Descripcion = "Centrífugador 5,5 Kg. Acero Inoxidable A-655/2",Precio = 350000,Stock = 20,ImagenUrl = "http://localhost:5184/img/secaropa.jpg" },
            new Producto {Nombre = "LavaRopa Whirlpool",Descripcion = "Wnq90as Frontal 9kg Inverter Titanium",Precio = 450000,Stock = 25,ImagenUrl = "http://localhost:5184/img/lavaropa.jpg" },
            new Producto {Nombre = "Plancha para Ropa Sokany",Descripcion = "A vapor ",Precio = 60000,Stock = 30,ImagenUrl = "http://localhost:5184/img/plancharopa.jpg" },
            new Producto {Nombre = "Pava Electrica Liliana",Descripcion = "Tempomate Ap175 Negra 1.7l 2kw",Precio = 20000,Stock = 25,ImagenUrl = "http://localhost:5184/img/pava.jpg" },
        });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/productos", async (TiendaContext db, string? query) =>
{
    var productos = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(query))
    {
        productos = productos.Where(p =>
            p.Nombre.Contains(query) || p.Descripcion.Contains(query));
    }

    return await productos.ToListAsync();
});

app.MapPut("/api/carritos/{carritoId}/{productoId}", async (
    Guid carritoId,
    int productoId,
    TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(productoId);

    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock <= 0)
        return Results.BadRequest("No hay más stock disponible");

    var itemExistente = await db.CarritoItems
        .FirstOrDefaultAsync(ci => ci.CarritoId == carritoId && ci.ProductoId == productoId);

    if (itemExistente != null)
    {
        itemExistente.Cantidad += 1;
    }
    else
    {
        db.CarritoItems.Add(new CarritoItem
        {
            CarritoId = carritoId,
            ProductoId = productoId,
            Cantidad = 1
        });
    }

    producto.Stock -= 1;

    await db.SaveChangesAsync();

    return Results.Ok("Producto agregado al carrito");
});

    Guid carritoId,
    int productoId,
    int delta,
    TiendaContext db) =>
{
    var item = await db.CarritoItems
        .Include(ci => ci.Producto)
        .FirstOrDefaultAsync(ci => ci.CarritoId == carritoId && ci.ProductoId == productoId);

    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");

    if (delta > 0)
    {
        if (item.Producto.Stock < delta)
            return Results.BadRequest("No hay stock suficiente");
        item.Cantidad += delta;
        item.Producto.Stock -= delta;
    }
    else if (delta < 0)
    {
        int resta = Math.Min(item.Cantidad - 1, -delta);
        if (resta > 0)
        {
            item.Cantidad -= resta;
            item.Producto.Stock += resta;
        }
    }

    await db.SaveChangesAsync();
    return Results.Ok("Cantidad actualizada");
});

app.MapGet("/api/carritos/{carritoId}", async (Guid carritoId, TiendaContext db) =>
{
    var items = await db.CarritoItems
        .Where(ci => ci.CarritoId == carritoId)
        .Include(ci => ci.Producto)
        .ToListAsync();

    if (items == null || items.Count == 0)
        return Results.NotFound("Carrito vacío o no encontrado");

    var resultado = items.Select(ci => new
    {
        ci.Id,
        ci.ProductoId,
        ProductoNombre = ci.Producto.Nombre,
        ci.Cantidad,
        ProductoPrecio = ci.Producto.Precio,
        ProductoImagenUrl = ci.Producto.ImagenUrl
    });

    return Results.Ok(resultado);
});

app.MapDelete("/api/carritos/{carritoId}", async (Guid carritoId, TiendaContext db) =>
{
    var items = await db.CarritoItems
        .Where(ci => ci.CarritoId == carritoId)
        .Include(ci => ci.Producto)
        .ToListAsync();

    if (items == null || items.Count == 0)
        return Results.NotFound("Carrito vacío o no encontrado");

    foreach (var item in items)
    {
        item.Producto.Stock += item.Cantidad;
    }

    db.CarritoItems.RemoveRange(items);
    await db.SaveChangesAsync();

    return Results.Ok("Carrito vaciado correctamente");
});

app.MapPost("/api/carritos", () =>
{
    var nuevoCarritoId = Guid.NewGuid();
    return Results.Ok(nuevoCarritoId);
});

app.MapDelete("/api/carritos/{carritoId}/{productoId}", async (
    Guid carritoId,
    int productoId,
    TiendaContext db) =>
{
    var item = await db.CarritoItems
        .Include(ci => ci.Producto)
        .FirstOrDefaultAsync(ci => ci.CarritoId == carritoId && ci.ProductoId == productoId);

    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");

    // Acá eliminamos el item entero y devolvemos todo el stock
    item.Producto.Stock += item.Cantidad;
    db.CarritoItems.Remove(item);

    await db.SaveChangesAsync();

    return Results.Ok("Producto eliminado del carrito completamente");
});

app.MapPut("/api/carritos/{carritoId}/confirmar", async (
    Guid carritoId,
    Compra compra,
    TiendaContext db) =>
{
    var itemsCarrito = await db.CarritoItems
        .Include(ci => ci.Producto)
        .Where(ci => ci.CarritoId == carritoId)
        .ToListAsync();

    if (itemsCarrito.Count == 0)
        return Results.BadRequest("El carrito está vacío");

    decimal total = itemsCarrito.Sum(ci => ci.Cantidad * ci.Producto.Precio);

    compra.Id = Guid.NewGuid();
    compra.Fecha = DateTime.UtcNow;
    compra.Total = total;

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    foreach (var item in itemsCarrito)
    {
        db.ItemsCompra.Add(new ItemCompra
        {
            CompraId = compra.Id,
            ProductoId = item.ProductoId,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.Producto.Precio
        });
    }

    db.CarritoItems.RemoveRange(itemsCarrito);

    await db.SaveChangesAsync();

    return Results.Ok("Compra confirmada");
});

app.Run();
