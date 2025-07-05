using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Modelos;
var builder = WebApplication.CreateBuilder(args);

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


// Configura EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// Carga productos de ejemplo al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Teclado Mecánico Redragon Kumara", Descripcion = "Teclado mecánico compacto con retroiluminación RGB.", Precio = 45000, Stock = 12, ImagenUrl = "http://localhost:5184/img/TecladoKumara.jfif" },
            new Producto { Id = 2, Nombre = "Mouse Logitech G203", Descripcion = "Mouse gamer con sensor óptico de alta precisión.", Precio = 30000, Stock = 30, ImagenUrl = "http://localhost:5184/img/MouseG203.jfif" },
            new Producto { Id = 3, Nombre = "Auriculares HyperX Cloud Stinger", Descripcion = "Auriculares con micrófono y sonido envolvente.", Precio = 60000, Stock = 10, ImagenUrl = "http://localhost:5184/img/AuricularesHyperex.jfif" },
            new Producto { Id = 4, Nombre = "Monitor Samsung 24''", Descripcion = "Monitor LED Full HD de 24 pulgadas.", Precio = 150000, Stock = 12, ImagenUrl = "http://localhost:5184/img/MonitorSamsung.jfif" },
            new Producto { Id = 5, Nombre = "Webcam Logitech C920", Descripcion = "Webcam HD 1080p para streaming y videollamadas.", Precio = 80000, Stock = 15, ImagenUrl = "http://localhost:5184/img/CamaraLogitech.jfif" },
            new Producto { Id = 6, Nombre = "Mousepad Redragon Flick", Descripcion = "Mousepad gamer XL con superficie de tela.", Precio = 18000, Stock = 25, ImagenUrl = "http://localhost:5184/img/mousepad.jfif" },
            new Producto { Id = 7, Nombre = "Parlantes Genius SP-HF1800A", Descripcion = "Parlantes estéreo de madera para PC.", Precio = 55000, Stock = 9, ImagenUrl = "http://localhost:5184/img/ParlantesGenius.jfif" },
            new Producto { Id = 8, Nombre = "Micrófono Fifine K669B", Descripcion = "Micrófono USB para streaming y grabación.", Precio = 55000, Stock = 14, ImagenUrl = "http://localhost:5184/img/Microfono.jfif" },
            new Producto { Id = 9, Nombre = "Hub USB 3.0 Orico", Descripcion = "Hub USB de 4 puertos con alimentación externa.", Precio = 22000, Stock = 18, ImagenUrl = "http://localhost:5184/img/HubUsb.jfif" },
            new Producto { Id = 10, Nombre = "Soporte para Monitor", Descripcion = "Soporte ajustable para monitor de escritorio.", Precio = 25000, Stock = 11, ImagenUrl = "http://localhost:5184/img/Soporte.png" }
        
        });
        db.SaveChanges();
    }
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Habilitar el uso de archivos estáticos
app.UseStaticFiles();

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Aquí irán los endpoints de la API

app.MapGet("/productos", async (TiendaContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        productos = productos.Where(p => p.Nombre.Contains(q));
    return await productos.ToListAsync();
});

// Inicializar un carrito
app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(new { CarritoId = carrito.Id });
});

// Obtener los ítems de un carrito
app.MapGet("/carritos/{carritoId}", async (TiendaContext db, string carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound();

    var itemsDto = carrito.Items.Select(i =>
    {
        // Obtener el stock global del producto desde DB
        var productoDb = db.Productos.First(p => p.Id == i.ProductoId);
        int stockGlobal = productoDb.Stock;

        // Stock disponible = lo que hay en la base + lo que el usuario ya tiene en el carrito
        int stockDisponible = stockGlobal + i.Cantidad;

        return new
        {
            i.ProductoId,
            i.Producto.Nombre,
            i.Producto.Precio,
            i.Cantidad,
            Importe = i.Cantidad * i.Producto.Precio,
            Stock = i.Producto.Stock,
        };
    });

    return Results.Ok(itemsDto);
});

// Vaciar un carrito
app.MapDelete("/carritos/{carritoId}", async (TiendaContext db, string carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound();

    db.ItemsCarrito.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// Agregar o actualizar producto en el carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (TiendaContext db, string carritoId, int productoId, int cantidad) =>
{
    if (cantidad < 1)
        return Results.BadRequest("Cantidad debe ser mayor a 0");

    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    var producto = await db.Productos.FindAsync(productoId);

    if (carrito == null || producto == null)
        return Results.NotFound();

    var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    int cantidadActualEnCarrito = itemExistente?.Cantidad ?? 0;

    // Calculamos stock disponible real: stock actual + la cantidad que ya tiene el carrito para ese producto
    int stockDisponible = producto.Stock + cantidadActualEnCarrito;

    if (cantidad > stockDisponible)
        return Results.BadRequest("No hay suficiente stock");

    var item = itemExistente;
    if (item == null)
    {
        item = new ItemCarrito { ProductoId = productoId, Cantidad = cantidad, CarritoId = carritoId };
        db.ItemsCarrito.Add(item);
    }
    else
    {
        item.Cantidad = cantidad;
        db.ItemsCarrito.Update(item);
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});


// Eliminar o reducir cantidad de producto en el carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", async (TiendaContext db, string carritoId, int productoId) =>
{
    var item = await db.ItemsCarrito
        .FirstOrDefaultAsync(i => i.CarritoId == carritoId && i.ProductoId == productoId);

    if (item == null)
        return Results.NotFound();

    db.ItemsCarrito.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Confirmar compra
app.MapPut("/carritos/{carritoId}/confirmar", async (TiendaContext db, string carritoId, Compra compra) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null || !carrito.Items.Any())
        return Results.BadRequest("Carrito vacío o no encontrado");

    //  Verificar si hay stock suficiente para cada producto
    foreach (var item in carrito.Items)
    {
        if (item.Producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay suficiente stock para {item.Producto.Nombre}");
    }

    //  Descontar el stock real de cada producto
    foreach (var item in carrito.Items)
    {
        item.Producto.Stock -= item.Cantidad;
    }

    // 💾 Registrar la compra
    var nuevaCompra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = compra.NombreCliente,
        ApellidoCliente = compra.ApellidoCliente,
        EmailCliente = compra.EmailCliente,
        Total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio),
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };

    db.Compras.Add(nuevaCompra);

    // 🧹 Vaciar carrito (eliminar ítems)
    db.ItemsCarrito.RemoveRange(carrito.Items);

    // 💾 Guardar todo
    await db.SaveChangesAsync();

    return Results.Ok(new { Mensaje = "Compra confirmada" });
});

app.Run();
