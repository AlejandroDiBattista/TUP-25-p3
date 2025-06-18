using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.models;
#nullable enable

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaDb>(options =>
    options.UseSqlite("Data Source=tienda.db"));

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

var app = builder.Build();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new List<Producto> {
            new Producto { Nombre = "Auriculares", Descripcion = "Ares H120 RGB", Precio = 120000, Stock = 10, ImagenUrl = "http://localhost:5184/img/ARES-H120-RGB.png" },
            new Producto { Nombre = "Auriculares", Descripcion = "Pandora H350", Precio = 110000, Stock = 20, ImagenUrl = "http://localhost:5184/img/PANDORA-H350.png" },
            new Producto { Nombre = "Auriculares", Descripcion = "Zeus x Wireless H510", Precio = 250000, Stock = 6, ImagenUrl = "http://localhost:5184/img/ZEUS-X-WIRELESS-H510.png" },
            new Producto { Nombre = "Mouse", Descripcion = "Storm Elite", Precio = 20500, Stock = 30, ImagenUrl = "http://localhost:5184/img/STORM-ELITE.png" },
            new Producto { Nombre = "Mouse", Descripcion = "Invader M719", Precio = 16000, Stock = 14, ImagenUrl = "http://localhost:5184/img/INVADER-M719.png" },
            new Producto { Nombre = "Mouse", Descripcion = "Impact M908", Precio = 15000, Stock = 8, ImagenUrl = "http://localhost:5184/img/IMPACT-M908.png" },
            new Producto { Nombre = "Teclado", Descripcion = "Kumara K522", Precio = 10000, Stock = 24, ImagenUrl = "http://localhost:5184/img/KUMARA-K552.png" },
            new Producto { Nombre = "Teclado", Descripcion = "Deimos K599", Precio = 19000, Stock = 12, ImagenUrl = "http://localhost:5184/img/DEIMOS-K599.png" },
            new Producto { Nombre = "Teclado", Descripcion = "Ziggs K669", Precio = 20000, Stock = 8, ImagenUrl = "http://localhost:5184/img/ZIGGS-K669.png" },
            new Producto { Nombre = "Teclado", Descripcion = "DRAGONBORN K630", Precio = 25000, Stock = 16, ImagenUrl = "http://localhost:5184/img/DRAGONBORN-K630.png" }
        });
        db.SaveChanges();
    }
}

app.MapGet("/productos", async ([FromServices] TiendaDb db, [FromQuery] string? busqueda) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        var filtro = busqueda.Trim().ToLower();
        query = query.Where(p =>
            p.Nombre.ToLower().Contains(filtro) ||
            p.Descripcion.ToLower().Contains(filtro));
    }

    return await query.ToListAsync();
});
app.MapPost("/carritos", async ([FromServices] TiendaDb db) =>
{
    var compra = new Compra { Fecha = DateTime.Now, Total = 0 };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok(compra.Id);
});
app.MapGet("/carritos/{carritoId:int}", async ([FromServices] TiendaDb db, int carritoId) =>
{
    var items = await db.ItemsCompra
        .Include(i => i.Producto)
        .Where(i => i.CompraId == carritoId)
        .ToListAsync();
    return Results.Ok(items);
});
app.MapDelete("/carritos/{carritoId:int}", async ([FromServices] TiendaDb db, int carritoId) =>
{
    var items = await db.ItemsCompra.Where(i => i.CompraId == carritoId).ToListAsync();
    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            producto.Stock += item.Cantidad;
        }
    }

    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{carritoId:int}/confirmar", async ([FromServices] TiendaDb db, int carritoId, [FromBody] ConfirmarCompra confirmacion) =>
{
    if (string.IsNullOrWhiteSpace(confirmacion.NombreCliente) ||
        string.IsNullOrWhiteSpace(confirmacion.ApellidoCliente) ||
        string.IsNullOrWhiteSpace(confirmacion.EmailCliente))
    {
        return Results.BadRequest("Debe completar nombre, apellido y email.");
    }

    var compra = await db.Compras.FindAsync(carritoId);
    if (compra == null)
    {
        return Results.NotFound("Carrito no encontrado.");
    }

    compra.NombreCliente = confirmacion.NombreCliente;
    compra.ApellidoCliente = confirmacion.ApellidoCliente;
    compra.EmailCliente = confirmacion.EmailCliente;
    compra.Fecha = DateTime.Now;
    compra.Total = await db.ItemsCompra.Where(i => i.CompraId == carritoId).SumAsync(i => i.Cantidad * i.PrecioUnitario);
    await db.SaveChangesAsync();

    return Results.Ok();
});
app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async ([FromServices] TiendaDb db, int carritoId, int productoId, [FromBody] int cantidad) =>
{
    var compra = await db.Compras.FindAsync(carritoId);
    if (compra == null)
    {
        compra = new Compra { Fecha = DateTime.Now, Total = 0 };
        db.Compras.Add(compra);
        await db.SaveChangesAsync();
        carritoId = compra.Id;
    }

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.BadRequest("Producto no encontrado");

    if (cantidad < 1) return Results.BadRequest("Cantidad inválida");

    var item = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);
    int cantidadActual = item?.Cantidad ?? 0;
    int diferencia = cantidad - cantidadActual;

    if (producto.Stock < diferencia)
        return Results.BadRequest("Stock insuficiente");

    // Actualizar stock temporal
    producto.Stock -= diferencia;

    if (item == null)
    {
        item = new Compras { CompraId = carritoId, ProductoId = productoId, Cantidad = cantidad, PrecioUnitario = producto.Precio };
        db.ItemsCompra.Add(item);
    }
    else
    {
        item.Cantidad = cantidad;
    }

    await db.SaveChangesAsync();
    return Results.Ok(new { carritoId });
});

app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async ([FromServices] TiendaDb db, int carritoId, int productoId) =>
{
    var item = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += item.Cantidad; // restaurar stock
    }

    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

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

app.Run();
