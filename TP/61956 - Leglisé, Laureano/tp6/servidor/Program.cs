using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar EF Core con SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Habilitar CORS para el cliente Blazor
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurar JSON para ignorar ciclos de referencia
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

app.UseCors();

// Crear la base de datos y poblarla con productos de ejemplo si está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new List<Producto>
        {
            new Producto { Nombre = "Celular Samsung A54", Descripcion = "Pantalla 6.4\", 128GB, 8GB RAM", Precio = 350000, Stock = 10, ImagenUrl = "/imagenes/celular.jpg" },
            new Producto { Nombre = "Notebook Lenovo", Descripcion = "Intel i5, 8GB RAM, 512GB SSD", Precio = 600000, Stock = 5, ImagenUrl = "/imagenes/notebook.jpg" },
            new Producto { Nombre = "Airpods Pro", Descripcion = "Inalámbricos, cancelación de ruido", Precio = 45000, Stock = 20, ImagenUrl = "/imagenes/auriculares.jpg" },
            new Producto { Nombre = "Mouse Logitech Pro", Descripcion = "RGB, 7200 DPI", Precio = 15000, Stock = 15, ImagenUrl = "/imagenes/mouse.jpg" },
            new Producto { Nombre = "Teclado HyperX", Descripcion = "Switch Red, retroiluminado", Precio = 25000, Stock = 12, ImagenUrl = "/imagenes/teclado.jpg" },
            new Producto { Nombre = "Monitor 24\"", Descripcion = "Full HD, 75Hz", Precio = 80000, Stock = 8, ImagenUrl = "/imagenes/monitor.jpg" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Carga rápida 25W", Precio = 8000, Stock = 30, ImagenUrl = "/imagenes/cargador.jpg" },
            new Producto { Nombre = "Smartwatch", Descripcion = "Monitor de ritmo cardíaco", Precio = 50000, Stock = 10, ImagenUrl = "/imagenes/reloj.jpg" },
            new Producto { Nombre = "Parlante JBL", Descripcion = "Portátil, resistente al agua", Precio = 22000, Stock = 18, ImagenUrl = "/imagenes/parlante.jpg" },
            new Producto { Nombre = "Disco Externo 1TB", Descripcion = "USB 3.0", Precio = 65000, Stock = 7, ImagenUrl = "/imagenes/disco.jpg" }
        });
        db.SaveChanges();
    }
}

// Endpoints Productos
app.MapGet("/productos", async (ApplicationDbContext db, string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    return await query.ToListAsync();
});

// Endpoint para actualizar/modificar un producto
app.MapPut("/productos/{id}", async (int id, Producto productoActualizado, ApplicationDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound();

    producto.Nombre = productoActualizado.Nombre;
    producto.Descripcion = productoActualizado.Descripcion;
    producto.Precio = productoActualizado.Precio;
    producto.Stock = productoActualizado.Stock;
    producto.ImagenUrl = productoActualizado.ImagenUrl;

    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Carrito en memoria (por simplicidad, usar diccionario)
var carritos = new Dictionary<Guid, List<(int ProductoId, int Cantidad)>>();

// Inicializa un carrito
app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid();
    carritos[id] = new List<(int, int)>();
    return Results.Ok(id);
});

// Trae los ítems del carrito
app.MapGet("/carritos/{carritoId}", (Guid carritoId, ApplicationDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound();

    var items = carritos[carritoId]
        .Select(ci =>
        {
            var producto = db.Productos.Find(ci.ProductoId);
            return new
            {
                Producto = producto,
                ci.Cantidad,
                Importe = producto != null ? producto.Precio * ci.Cantidad : 0
            };
        }).ToList();

    return Results.Ok(items);
});

// Vacía el carrito
app.MapDelete("/carritos/{carritoId}", (Guid carritoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound();
    carritos[carritoId].Clear();
    return Results.NoContent();
});

// Agrega o actualiza producto en carrito
app.MapPut("/carritos/{carritoId}/{productoId}", (Guid carritoId, int productoId, int cantidad, ApplicationDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound();

    var producto = db.Productos.Find(productoId);
    if (producto == null || producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente");

    var items = carritos[carritoId];
    var idx = items.FindIndex(i => i.ProductoId == productoId);
    if (idx >= 0)
        items[idx] = (productoId, cantidad);
    else
        items.Add((productoId, cantidad));

    return Results.Ok();
});

// Elimina o reduce producto del carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", (Guid carritoId, int productoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound();

    var items = carritos[carritoId];
    var idx = items.FindIndex(i => i.ProductoId == productoId);
    if (idx >= 0)
        items.RemoveAt(idx);

    return Results.NoContent();
});

// Confirmar compra
app.MapPut("/carritos/{carritoId}/confirmar", async (Guid carritoId, Compra datos, ApplicationDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound();

    var items = carritos[carritoId];
    if (!items.Any())
        return Results.BadRequest("Carrito vacío");

    // Validar stock
    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {producto?.Nombre}");
    }

    // Registrar compra
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.NombreCliente,
        ApellidoCliente = datos.ApellidoCliente,
        EmailCliente = datos.EmailCliente,
        Total = 0,
        Items = new List<ItemCompra>()
    };

    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
        compra.Items.Add(new ItemCompra
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        });
        compra.Total += producto.Precio * item.Cantidad;
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carritos[carritoId].Clear();

    // Devuelve solo un resumen plano para evitar ciclos de referencia
    return Results.Ok(new
    {
        compra.Id,
        compra.Fecha,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        compra.Total
    });
});

app.UseStaticFiles();
app.Run();