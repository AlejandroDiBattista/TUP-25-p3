using Microsoft.EntityFrameworkCore;
using Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de EF Core con SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// ENDPOINTS API TIENDA ONLINE

// GET /productos (+ búsqueda por query)
app.MapGet("/productos", async (TiendaDbContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        productos = productos.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    return await productos.ToListAsync();
});

// POST /carritos (inicializa el carrito)
app.MapPost("/carritos", async (TiendaDbContext db) =>
{
    var compra = new Compra { Fecha = DateTime.Now, Total = 0 };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok(compra.Id);
});

// GET /carritos/{carrito} (trae los ítems del carrito)
app.MapGet("/carritos/{carritoId}", async (TiendaDbContext db, int carritoId) =>
{
    var compra = await db.Compras.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound();
    return Results.Ok(compra);
});

// DELETE /carritos/{carrito} (vacía el carrito)
app.MapDelete("/carritos/{carritoId}", async (TiendaDbContext db, int carritoId) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound();
    db.ItemsCompra.RemoveRange(compra.Items);
    compra.Items.Clear();
    compra.Total = 0;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carrito}/confirmar (detalle + datos cliente)
app.MapPut("/carritos/{carritoId}/confirmar", async (TiendaDbContext db, int carritoId, Compra datos) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound();
    compra.NombreCliente = datos.NombreCliente;
    compra.ApellidoCliente = datos.ApellidoCliente;
    compra.EmailCliente = datos.EmailCliente;
    compra.Fecha = DateTime.Now;
    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carrito}/{producto} (agrega producto o actualiza cantidad)
app.MapPut("/carritos/{carritoId}/{productoId}", async (TiendaDbContext db, int carritoId, int productoId, int cantidad) =>
{
    if (cantidad < 1) return Results.BadRequest("Cantidad inválida");
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    var producto = await db.Productos.FindAsync(productoId);
    if (compra == null || producto == null) return Results.NotFound();
    if (producto.Stock < cantidad) return Results.BadRequest("Sin stock suficiente");
    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        item = new ItemCompra { ProductoId = productoId, Cantidad = cantidad, PrecioUnitario = producto.Precio };
        compra.Items.Add(item);
    }
    else
    {
        if (producto.Stock < cantidad) return Results.BadRequest("Sin stock suficiente");
        item.Cantidad = cantidad;
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

// DELETE /carritos/{carrito}/{producto} (elimina producto o reduce cantidad)
app.MapDelete("/carritos/{carritoId}/{productoId}", async (TiendaDbContext db, int carritoId, int productoId) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound();
    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();
    compra.Items.Remove(item);
    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Cargar productos de ejemplo al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Monitor Samsung 27''", Descripcion = "Monitor LED Full HD 27 pulgadas", Precio = 120000, Stock = 8, ImagenUrl = "monitor.webp.webp" },
            new Producto { Nombre = "Teclado Mecánico HyperX Alloy", Descripcion = "Teclado mecánico RGB para gaming", Precio = 45000, Stock = 15, ImagenUrl = "teclado.webp.webp" },
            new Producto { Nombre = "Mouse Gamer Razer DeathAdder", Descripcion = "Mouse óptico 16000 DPI", Precio = 35000, Stock = 20, ImagenUrl = "mouse.webp.webp" },
            new Producto { Nombre = "Auriculares Corsair HS50", Descripcion = "Auriculares gaming con micrófono", Precio = 38000, Stock = 10, ImagenUrl = "auris.webp.webp" },
            new Producto { Nombre = "Placa de Video NVIDIA RTX 4060", Descripcion = "8GB GDDR6, Ray Tracing", Precio = 650000, Stock = 4, ImagenUrl = "grafica.webp.webp" },
            new Producto { Nombre = "Disco SSD Samsung 980 PRO 1TB", Descripcion = "NVMe PCIe Gen4", Precio = 95000, Stock = 12, ImagenUrl = "ssd.webp.webp" },
            new Producto { Nombre = "Router TP-Link Archer AX10", Descripcion = "Wi-Fi 6, triple banda", Precio = 42000, Stock = 9, ImagenUrl = "router.webp.webp" },
            new Producto { Nombre = "Tablet Samsung Galaxy Tab S6 Lite", Descripcion = "10.4'' 64GB WiFi", Precio = 210000, Stock = 6, ImagenUrl = "tablet.webp.webp" },
            new Producto { Nombre = "Impresora HP Ink Tank 415", Descripcion = "Multifunción WiFi", Precio = 85000, Stock = 7, ImagenUrl = "impresora.webp.webp" },
            new Producto { Nombre = "Webcam Logitech C920", Descripcion = "Full HD 1080p", Precio = 32000, Stock = 14, ImagenUrl = "webcam.webp.webp" }
        });
        db.SaveChanges();
    }
}

app.Run();
