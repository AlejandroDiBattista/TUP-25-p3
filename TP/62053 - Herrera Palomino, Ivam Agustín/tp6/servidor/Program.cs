using Microsoft.EntityFrameworkCore;
using TiendaOnline.Backend.Data;
using TiendaOnline.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaContext>(opts =>
    opts.UseSqlite("Data Source=tienda.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();      
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        var ejemplos = new[]
        {
            new Producto { Nombre = "Celular X",          Descripcion = "Smartphone 6.5\"", Precio = 250m, Stock = 15, ImagenUrl = "https://i.ibb.co/kb3NyKt/Celular-4.webp" },
            new Producto { Nombre = "Auriculares",        Descripcion = "In-ear Bluetooth",     Precio = 50m,  Stock = 30, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_816116-MLU73465844524_122023-F.webp" },
            new Producto { Nombre = "Power Bank",         Descripcion = "10000 mAh",            Precio = 35m,  Stock = 25, ImagenUrl = "https://static.tp-link.com/01_large_1601428433247z.jpg" },
            new Producto { Nombre = "Notebook",           Descripcion = "Fast Charge",         Precio = 20m,  Stock = 40, ImagenUrl = "https://fastly.picsum.photos/id/6/200/200.jpg?hmac=g4Q9Vcu5Ohm8Rwap3b6HSIxUfIALZ6BasESHhw7VjLE" },
            new Producto { Nombre = "Mouse óptico",       Descripcion = "Wireless",             Precio = 25m,  Stock = 20, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_983029-MLA83597560797_042025-F.webp" },
            new Producto { Nombre = "Teclado Mecánico",   Descripcion = "Clasico",                  Precio = 80m,  Stock = 10, ImagenUrl = "https://fastly.picsum.photos/id/403/200/200.jpg?hmac=GkAppJlJ6MSNvKNo7Hj3Z_I3QTbiwzOtyOJbh6jcZ0U" },
            new Producto { Nombre = "Smartwatch",         Descripcion = "Monitor cardíaco",     Precio = 120m, Stock = 12, ImagenUrl = "https://i.ibb.co/PGtBbtK9/Smartwatch.webp" },
            new Producto { Nombre = "Altavoz Bluetooth",  Descripcion = "Portátil 10 W",       Precio = 60m,  Stock = 18, ImagenUrl = "https://i.ibb.co/B2B7wyVs/Altavozbluetooth.webp" },
            new Producto { Nombre = "Cable HDMI",         Descripcion = "2 m",                 Precio = 15m,  Stock = 50, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_790639-MLA82176442619_012025-F.webp" },
            new Producto { Nombre = "Gorra",              Descripcion = "Roja",               Precio = 18m,  Stock = 35, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_664330-MLA78279435294_082024-O.webp" }
        };
        db.Productos.AddRange(ejemplos);
        db.SaveChanges();
    }
}

// GET /productos
app.MapGet("/productos", async (TiendaContext db, string? search) =>
{
    var q = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(search))
        q = q.Where(p => EF.Functions.Like(p.Nombre,$"%{search}%") ||
                         EF.Functions.Like(p.Descripcion,$"%{search}%"));

    return await q.ToListAsync();
});

// POST /carritos
app.MapPost("/carritos", async (TiendaContext db) =>
{
    var c = new Carrito();
    db.Carritos.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{c.Id}", c);
});

// GET /carritos/{id}
app.MapGet("/carritos/{id:guid}", async (TiendaContext db, Guid id) =>
{
    var car = await db.Carritos
                      .Include(c => c.Items).ThenInclude(i => i.Producto)
                      .AsNoTracking()
                      .FirstOrDefaultAsync(c => c.Id == id);
    return car is null ? Results.NotFound() : Results.Ok(car);
});

// DELETE /carritos/{id}  (vaciar)
app.MapDelete("/carritos/{id:guid}", async (TiendaContext db, Guid id) =>
{
    // Buscamos el carrito con sus items para borrarlos
    var car = await db.Carritos.Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == id);
    
    if (car is null) return Results.NotFound();

    // Borramos el carrito 
    db.Carritos.Remove(car);
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// PUT /carritos/{id}/{productoId}?qty=2
app.MapPut("/carritos/{id:guid}/{productoId:int}", async (
    TiendaContext db, Guid id, int productoId, int qty) =>
{
    if (qty <= 0) return Results.BadRequest("qty debe ser > 0");

    var car = await db.Carritos.Include(c => c.Items)
                               .FirstOrDefaultAsync(c => c.Id == id && !c.Cerrado);
    if (car is null) return Results.NotFound();

    var prod = await db.Productos.FindAsync(productoId);
    if (prod is null) return Results.NotFound("Producto inexistente");

    var item = car.Items.FirstOrDefault(i => i.ProductoId == productoId);
    var nuevaCant = (item?.Cantidad ?? 0) + qty;

    if (nuevaCant > prod.Stock)
        return Results.Conflict($"Solo hay {prod.Stock} unidades");

    if (item is null)
        car.Items.Add(new CarritoItem { ProductoId = productoId, Cantidad = qty });
    else
        item.Cantidad = nuevaCant;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE /carritos/{id}/{productoId}?qty=1
app.MapDelete("/carritos/{id:guid}/{productoId:int}", async (
    TiendaContext db, Guid id, int productoId, int qty) =>
{
    var car = await db.Carritos.Include(c => c.Items)
                               .FirstOrDefaultAsync(c => c.Id == id && !c.Cerrado);
    if (car is null) return Results.NotFound();

    var item = car.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null) return Results.NotFound();

    item.Cantidad -= qty <= 0 ? 1 : qty;
    if (item.Cantidad <= 0)
    {
        db.ItemsCarrito.Remove(item);
        
        await db.SaveChangesAsync();

        var itemsRestantes = await db.ItemsCarrito.CountAsync(i => i.CarritoId == id);
        if (itemsRestantes == 0)
            db.Carritos.Remove(car);
    }

    await db.SaveChangesAsync();
    return Results.NoContent();
});


// PUT /carritos/{id}/confirmar
app.MapPut("/carritos/{id:guid}/confirmar", async (
    TiendaContext db, Guid id,
    string nombre, string apellido, string email) =>      // ← sin DTO
{
    var car = await db.Carritos.Include(c=>c.Items)
                               .ThenInclude(i=>i.Producto)
                               .FirstOrDefaultAsync(c=>c.Id==id && !c.Cerrado);
    if (car is null) return Results.NotFound();
    if (!car.Items.Any()) return Results.BadRequest("Carrito vacío");

    foreach (var it in car.Items)
    {
        if (it.Cantidad > it.Producto!.Stock)
            return Results.Conflict($"Sin stock para {it.Producto.Nombre}");
        it.Producto.Stock -= it.Cantidad;
    }

    var compra = new Compra
    {
        NombreCliente = nombre,
        ApellidoCliente = apellido,
        EmailCliente = email,
        Total = car.Items.Sum(i => i.Cantidad * i.Producto!.Precio),
        Items = car.Items.Select(i => new ItemCompra
        {
            ProductoId     = i.ProductoId,
            Cantidad       = i.Cantidad,
            PrecioUnitario = i.Producto!.Precio
        }).ToList()
    };

    db.Compras.Add(compra);
    car.Cerrado = true;
    await db.SaveChangesAsync();

    return Results.Ok(compra); 
});


app.Run();

