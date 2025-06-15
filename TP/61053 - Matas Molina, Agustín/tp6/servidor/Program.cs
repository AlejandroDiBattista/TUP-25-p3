using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json.Serialization;
using TuProyecto.Models;
var builder = WebApplication.CreateBuilder(args);

// Agrega esta línea:
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

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
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));


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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(

           new Producto { Nombre = "Procesador Intel Core i7-14700K", Descripcion = "CPU de alto rendimiento para gaming y creación de contenido, 20 núcleos, 28 hilos.", Precio = 350000, Stock = 15, ImagenUrl = "https://ejemplo.com/imagenes/intel-i7.jpg" },
           new Producto { Nombre = "Tarjeta Gráfica NVIDIA GeForce RTX 4070 SUPER", Descripcion = "Potente GPU para juegos en 1440p y 4K con Ray Tracing y DLSS.", Precio = 600000, Stock = 10, ImagenUrl = "https://ejemplo.com/imagenes/rtx4070super.jpg" },
           new Producto { Nombre = "Placa Base ASUS ROG Strix Z790-E Gaming WiFi II", Descripcion = "Placa madre ATX de alta gama para procesadores Intel de 12ª, 13ª y 14ª generación.", Precio = 280000, Stock = 8, ImagenUrl = "https://ejemplo.com/imagenes/asus-z790.jpg" },
           new Producto { Nombre = "Memoria RAM Corsair Vengeance RGB Pro 32GB (2x16GB) DDR5", Descripcion = "Kit de memoria de alta velocidad con iluminación RGB para un rendimiento óptimo.", Precio = 180000, Stock = 25, ImagenUrl = "https://ejemplo.com/imagenes/corsair-ddr5.jpg" },
           new Producto { Nombre = "Disco SSD Samsung 990 PRO 1TB NVMe PCIe 4.0", Descripcion = "Unidad de estado sólido ultrarrápida para cargas de sistema y juegos.", Precio = 150000, Stock = 20, ImagenUrl = "https://ejemplo.com/imagenes/samsung-990pro.jpg" },
           new Producto { Nombre = "Fuente de Alimentación EVGA Supernova 850W G6 80 Plus Gold", Descripcion = "Fuente modular de alta eficiencia para sistemas de gaming exigentes.", Precio = 120000, Stock = 12, ImagenUrl = "https://ejemplo.com/imagenes/evga-850w.jpg" },
           new Producto { Nombre = "Gabinete Cooler Master MasterBox TD500 Mesh V2", Descripcion = "Chasis con excelente flujo de aire y paneles de malla para una refrigeración eficiente.", Precio = 85000, Stock = 18, ImagenUrl = "https://ejemplo.com/imagenes/td500-mesh.jpg" },
           new Producto { Nombre = "Monitor Gamer LG UltraGear 27GR95QE-B OLED", Descripcion = "Monitor QHD de 27 pulgadas con panel OLED, 240Hz de tasa de refresco.", Precio = 700000, Stock = 7, ImagenUrl = "https://ejemplo.com/imagenes/lg-oled.jpg" },
           new Producto { Nombre = "Teclado Mecánico HyperX Alloy Origins 60", Descripcion = "Teclado compacto al 60% con switches HyperX Red y retroiluminación RGB.", Precio = 55000, Stock = 30, ImagenUrl = "https://ejemplo.com/imagenes/hyperx-origins60.jpg" },
           new Producto { Nombre = "Mouse Gamer Logitech G Pro X Superlight 2 Lightspeed", Descripcion = "Mouse inalámbrico ultraligero con sensor HERO 25K para eSports.", Precio = 45000, Stock = 35, ImagenUrl = "https://ejemplo.com/imagenes/logitech-superlight.jpg" },
           new Producto { Nombre = "Cooler CPU Noctua NH-D15 chromax.black", Descripcion = "Disipador de aire de doble torre de alto rendimiento con ventiladores silenciosos y diseño en negro.", Precio = 145000, Stock = 10, ImagenUrl = "https://ejemplo.com/imagenes/noctua-d15-black.jpg" },
           new Producto { Nombre = "Disco Duro Seagate Barracuda 4TB 5400 RPM", Descripcion = "Almacenamiento masivo fiable para juegos y archivos, ideal para complementar un SSD.", Precio = 95000, Stock = 15, ImagenUrl = "https://ejemplo.com/imagenes/seagate-barracuda-4tb.jpg" },
           new Producto { Nombre = "Tarjeta de Red TP-Link Archer TX20E Wi-Fi 6", Descripcion = "Adaptador PCIe para conectividad Wi-Fi 6 ultrarrápida y Bluetooth 5.2.", Precio = 48000, Stock = 12, ImagenUrl = "https://ejemplo.com/imagenes/tp-link-tx20e.jpg" },
           new Producto { Nombre = "Auriculares Gamer SteelSeries Arctis Nova 7 Wireless", Descripcion = "Auriculares inalámbricos multiplataforma con audio de alta fidelidad y batería de larga duración.", Precio = 160000, Stock = 8, ImagenUrl = "https://ejemplo.com/imagenes/steelseries-nova7.jpg" },
           new Producto { Nombre = "Pasta Térmica Arctic MX-6 (4g)", Descripcion = "Compuesto térmico de alto rendimiento para mejorar la transferencia de calor entre el CPU/GPU y el disipador.", Precio = 18000, Stock = 30, ImagenUrl = "https://ejemplo.com/imagenes/arctic-mx6.jpg" }
        );
        db.SaveChanges();
    }

}
app.MapPost("/api/carrito", async (TiendaDbContext db) =>
{
    var compra = new Compra { Items = new List<ItemCompra>() };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok(new { carritoId = compra.Id }); // <-- Aquí debe devolver el ID real
});
app.MapGet("/api/carrito/{carritoId=int}", async (int carritoId, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
            .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null)
        return Results.NotFound();

    // Usa Results.Json para evitar el ciclo de referencias
    return Results.Json(compra, new System.Text.Json.JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    });
});
app.MapPut("/api/carrito/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, int cantidad, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
            .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null)
        return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound();

    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        item = new ItemCompra
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio,
            Producto = producto
        };
        compra.Items.Add(item);
    }
    else
    {
        item.Cantidad += cantidad;
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/api/carrito/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, TiendaDbContext db) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound();

    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    compra.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.MapDelete("/api/carrito/{carritoId:int}", async (int carritoId, TiendaDbContext db) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound();

    compra.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.MapPut("/api/carrito/{carritoId:int}/confirmar", async (int carritoId, Compra datos, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound();

    // Validar stock antes de descontar
    foreach (var item in compra.Items)
    {
        if (item.Producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay suficiente stock para {item.Producto.Nombre}");
    }

    // Descontar stock
    foreach (var item in compra.Items)
    {
        item.Producto.Stock -= item.Cantidad;
    }

    compra.NombreCliente = datos.NombreCliente;
    compra.ApellidoCliente = datos.ApellidoCliente;
    compra.EmailCliente = datos.EmailCliente;
    compra.Fecha = DateTime.Now;
    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();

    return Results.Ok(compra);
});
app.MapGet("/api/productos", async (TiendaDbContext db) =>
    await db.Productos.ToListAsync());
app.MapControllers();
app.Run();




