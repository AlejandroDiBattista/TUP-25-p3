using Microsoft.EntityFrameworkCore;
using Servidor.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging más limpio
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Error;
});

// Configurar filtros de logging
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.Hosting", LogLevel.Information);


builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();


if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowClientApp");


app.MapGet("/", () => "Servidor API está en funcionamiento");


app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });




app.MapGet("/productos", async (TiendaDbContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        var queryLower = q.ToLower();
        productos = productos.Where(p => p.Nombre.ToLower().Contains(queryLower));
    }
    return await productos.ToListAsync();
});


app.MapPost("/carritos", async (TiendaDbContext db) =>
{
    try
    {
        var compra = new Compra { Fecha = DateTime.Now, Total = 0 };
        db.Compras.Add(compra);
        await db.SaveChangesAsync();
        return Results.Ok(compra.Id);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al crear carrito: {ex.Message}");
        return Results.Problem("Error interno del servidor");
    }
});


app.MapGet("/carritos/{carritoId}", async (TiendaDbContext db, int carritoId) =>
{
    try
    {
        var compra = await db.Compras
            .Where(c => c.Id == carritoId)
            .Select(c => new 
            {
                c.Id,
                c.Fecha,
                c.Total,
                c.NombreCliente,
                c.ApellidoCliente,
                c.EmailCliente,
                Items = c.Items.Select(i => new 
                {
                    i.Id,
                    i.ProductoId,
                    i.Cantidad,
                    i.PrecioUnitario,
                    Producto = new 
                    {
                        i.Producto!.Id,
                        i.Producto.Nombre,
                        i.Producto.Descripcion,
                        i.Producto.Precio,
                        i.Producto.Stock,
                        i.Producto.ImagenUrl
                    }
                }).ToList()
            })
            .FirstOrDefaultAsync();
            
        if (compra == null) 
        {
            return Results.NotFound($"Carrito con ID {carritoId} no encontrado");
        }
        
        return Results.Ok(compra);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al obtener carrito {carritoId}: {ex.Message}");
        return Results.Problem($"Error interno del servidor: {ex.Message}");
    }
});


app.MapDelete("/carritos/{carritoId}", async (TiendaDbContext db, int carritoId) =>
{
    try
    {
        var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
        if (compra == null) return Results.NotFound();
        
        foreach (var item in compra.Items)
        {            var producto = await db.Productos.FindAsync(item.ProductoId);
            if (producto != null)
            {
                producto.Stock += item.Cantidad;
                // Console.WriteLine($"Stock restaurado para producto {producto.Nombre}: {producto.Stock} (+{item.Cantidad})");
            }
        }
        
        db.ItemsCompra.RemoveRange(compra.Items);
        compra.Items.Clear();
        compra.Total = 0;
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al vaciar carrito {carritoId}: {ex.Message}");
        return Results.Problem("Error interno del servidor");
    }
});


app.MapPut("/carritos/{carritoId}/confirmar", async (TiendaDbContext db, int carritoId, Compra datos) =>
{
    try
    {
        var compra = await db.Compras.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId);
        if (compra == null) return Results.NotFound($"Carrito con ID {carritoId} no encontrado");
        
        foreach (var item in compra.Items)
        {
            if (item.Producto == null)
            {
                var producto = await db.Productos.FindAsync(item.ProductoId);
                if (producto == null) return Results.BadRequest($"Producto con ID {item.ProductoId} no encontrado");
                item.Producto = producto;
            }
            
            if (item.Producto.Stock < 0)
            {
                return Results.BadRequest($"Error de stock para {item.Producto.Nombre}. Stock actual: {item.Producto.Stock}");
            }
        }
        
        compra.NombreCliente = datos.NombreCliente;
        compra.ApellidoCliente = datos.ApellidoCliente;
        compra.EmailCliente = datos.EmailCliente;
        compra.Fecha = DateTime.Now;
        compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
        
        await db.SaveChangesAsync();
        
        Console.WriteLine($"Compra confirmada exitosamente. ID: {carritoId}, Cliente: {datos.NombreCliente} {datos.ApellidoCliente}, Total: ${compra.Total}");
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al confirmar compra {carritoId}: {ex.Message}");
        return Results.Problem($"Error interno del servidor: {ex.Message}");
    }
});


app.MapPut("/carritos/{carritoId}/{productoId}", async (TiendaDbContext db, int carritoId, int productoId, int cantidad) =>
{
    try
    {
        if (cantidad < 1) return Results.BadRequest("Cantidad inválida");
        
        var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
        if (compra == null) return Results.NotFound($"Carrito con ID {carritoId} no encontrado");
        
        var producto = await db.Productos.FindAsync(productoId);
        if (producto == null) return Results.NotFound($"Producto con ID {productoId} no encontrado");
        
        var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
        
        if (item == null)
        {
            // Producto no existe en el carrito, agregarlo
            if (producto.Stock < cantidad) 
            {
                return Results.BadRequest($"Stock insuficiente. Stock disponible: {producto.Stock}, solicitado: {cantidad}");
            }
              item = new ItemCompra { ProductoId = productoId, Cantidad = cantidad, PrecioUnitario = producto.Precio };
            compra.Items.Add(item);
            producto.Stock -= cantidad;
            // Console.WriteLine($"Producto {producto.Nombre} agregado al carrito. Cantidad: {cantidad}, Stock restante: {producto.Stock}");
        }
        else
        {
            // Producto ya existe, agregar a la cantidad existente
            if (producto.Stock < cantidad) 
            {
                return Results.BadRequest($"Stock insuficiente. Stock disponible: {producto.Stock}, solicitado: {cantidad}");
            }
              item.Cantidad += cantidad;
            producto.Stock -= cantidad;
            // Console.WriteLine($"Cantidad de {producto.Nombre} aumentada en {cantidad}. Nueva cantidad: {item.Cantidad}, Stock restante: {producto.Stock}");
        }
        
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al agregar producto {productoId} al carrito {carritoId}: {ex.Message}");
        return Results.Problem($"Error interno del servidor: {ex.Message}");
    }
});


app.MapDelete("/carritos/{carritoId}/{productoId}", async (TiendaDbContext db, int carritoId, int productoId) =>
{
    try
    {
        var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
        if (compra == null) return Results.NotFound($"Carrito con ID {carritoId} no encontrado");
        
        var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null) return Results.NotFound($"Producto con ID {productoId} no encontrado en el carrito");
          var producto = await db.Productos.FindAsync(productoId);
        if (producto != null)
        {
            producto.Stock += item.Cantidad;
            // Console.WriteLine($"Stock restaurado para producto {producto.Nombre}: {producto.Stock} (+{item.Cantidad})");
        }
        
        compra.Items.Remove(item);
        db.ItemsCompra.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al eliminar producto {productoId} del carrito {carritoId}: {ex.Message}");
        return Results.Problem($"Error interno del servidor: {ex.Message}");
    }
});


app.MapPost("/carritos/limpiar-abandonados", async (TiendaDbContext db, int minutosInactividad = 30) =>
{
    try
    {
        var fechaLimite = DateTime.Now.AddMinutes(-minutosInactividad);
        var carritosAbandonados = await db.Compras
            .Include(c => c.Items)
            .Where(c => c.Fecha < fechaLimite && string.IsNullOrEmpty(c.NombreCliente))
            .ToListAsync();

        var stockRestaurado = 0;
        var carritosLimpiados = 0;

        foreach (var carrito in carritosAbandonados)
        {
            foreach (var item in carrito.Items)
            {
                var producto = await db.Productos.FindAsync(item.ProductoId);                if (producto != null)
                {
                    producto.Stock += item.Cantidad;
                    stockRestaurado += item.Cantidad;
                    // Console.WriteLine($"Stock restaurado para producto {producto.Nombre}: {producto.Stock} (+{item.Cantidad})");
                }
            }
            
            db.ItemsCompra.RemoveRange(carrito.Items);
            db.Compras.Remove(carrito);
            carritosLimpiados++;
        }

        await db.SaveChangesAsync();
        
        Console.WriteLine($"Limpieza completada: {carritosLimpiados} carritos eliminados, {stockRestaurado} unidades de stock restauradas");
        return Results.Ok(new { CarritosLimpiados = carritosLimpiados, StockRestaurado = stockRestaurado });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al limpiar carritos abandonados: {ex.Message}");
        return Results.Problem($"Error interno del servidor: {ex.Message}");
    }
});


app.MapPost("/carritos/{carritoId}/{productoId}/modificar", async (TiendaDbContext db, int carritoId, int productoId, int cantidad) =>
{
    try
    {
        if (cantidad < 1) return Results.BadRequest("Cantidad inválida");
        
        var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
        if (compra == null) return Results.NotFound($"Carrito con ID {carritoId} no encontrado");
        
        var producto = await db.Productos.FindAsync(productoId);
        if (producto == null) return Results.NotFound($"Producto con ID {productoId} no encontrado");
        
        var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null) return Results.NotFound($"Producto con ID {productoId} no encontrado en el carrito");
        
        var cantidadActual = item.Cantidad;
        var diferenciaCantidad = cantidad - cantidadActual;
        
        if (diferenciaCantidad > 0 && producto.Stock < diferenciaCantidad) 
        {
            return Results.BadRequest($"Stock insuficiente. Stock disponible: {producto.Stock}, necesario: {diferenciaCantidad}");
        }
        
        item.Cantidad = cantidad;
        producto.Stock -= diferenciaCantidad;
          await db.SaveChangesAsync();
        // Console.WriteLine($"Cantidad de {producto.Nombre} modificada a {cantidad}. Stock ajustado: {producto.Stock} (diferencia: {-diferenciaCantidad})");
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al modificar cantidad del producto {productoId} en carrito {carritoId}: {ex.Message}");
        return Results.Problem($"Error interno del servidor: {ex.Message}");
    }
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    

    try
    {
        scopedLogger.LogInformation("Verificando/creando base de datos...");
        db.Database.EnsureCreated();
        scopedLogger.LogInformation("Base de datos verificada correctamente");
    }
    catch (Exception ex)
    {
        scopedLogger.LogError(ex, "Error al crear la base de datos");

        try
        {
            scopedLogger.LogInformation("Intentando aplicar migraciones...");
            db.Database.Migrate();
            scopedLogger.LogInformation("Migraciones aplicadas correctamente");
        }
        catch (Exception migrationEx)
        {
            scopedLogger.LogCritical(migrationEx, "Error crítico al aplicar migraciones");
            throw;
        }
    }
    

    var productCount = await db.Productos.CountAsync();
    if (productCount == 0)
    {
        scopedLogger.LogInformation("Agregando productos de ejemplo...");
        
        var productos = new[]
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
            new Producto { Nombre = "Webcam Logitech C920", Descripcion = "Full HD 1080p", Precio = 32000, Stock = 14, ImagenUrl = "webcam.webp.webp" },
            new Producto { Nombre = "Smartphone iPhone 15", Descripcion = "128GB, Cámara 48MP, iOS 17", Precio = 1250000, Stock = 5, ImagenUrl = "iphone.webp" },
            new Producto { Nombre = "Laptop Lenovo ThinkPad E14", Descripcion = "AMD Ryzen 5, 8GB RAM, 256GB SSD", Precio = 780000, Stock = 3, ImagenUrl = "laptop.webp" },
            new Producto { Nombre = "Memoria RAM Corsair 16GB DDR4", Descripcion = "3200MHz RGB Pro", Precio = 55000, Stock = 18, ImagenUrl = "ram.webp" },
            new Producto { Nombre = "Procesador Intel Core i5-13400F", Descripcion = "10 núcleos, 4.6GHz Turbo", Precio = 280000, Stock = 7, ImagenUrl = "cpu.webp" },
            new Producto { Nombre = "Fuente Corsair CV550", Descripcion = "550W 80+ Bronze", Precio = 72000, Stock = 11, ImagenUrl = "fuente.webp" },
            new Producto { Nombre = "Gabinete Cooler Master MasterBox", Descripcion = "ATX Mid Tower RGB", Precio = 48000, Stock = 9, ImagenUrl = "gabinete.webp" },
            new Producto { Nombre = "Smartphone Samsung Galaxy S24", Descripcion = "256GB, Triple cámara 50MP", Precio = 1100000, Stock = 4, ImagenUrl = "samsung.webp" },
            new Producto { Nombre = "Parlantes Logitech Z313", Descripcion = "Sistema 2.1 con subwoofer", Precio = 28000, Stock = 16, ImagenUrl = "parlantes.webp" },
            new Producto { Nombre = "Cargador Inalámbrico Anker", Descripcion = "15W Fast Charging", Precio = 18500, Stock = 22, ImagenUrl = "cargador.webp" },
            new Producto { Nombre = "Disco Duro Seagate 2TB", Descripcion = "7200 RPM, SATA III", Precio = 62000, Stock = 13, ImagenUrl = "hdd.webp" },
            new Producto { Nombre = "Microfono Blue Yeti", Descripcion = "USB Condensador para streaming", Precio = 145000, Stock = 6, ImagenUrl = "micro.webp" },
            new Producto { Nombre = "Smartwatch Apple Watch SE", Descripcion = "GPS, 44mm, Correa deportiva", Precio = 390000, Stock = 8, ImagenUrl = "watch.webp" },
            new Producto { Nombre = "Nintendo Switch OLED", Descripcion = "Consola híbrida con pantalla OLED 7''", Precio = 420000, Stock = 5, ImagenUrl = "switch.webp" },
            new Producto { Nombre = "Cámara Canon EOS M50 Mark II", Descripcion = "Mirrorless 24.1MP, 4K Video", Precio = 850000, Stock = 3, ImagenUrl = "camara.webp" }
        };
        
        db.Productos.AddRange(productos);
        await db.SaveChangesAsync();
        
        scopedLogger.LogInformation("Productos de ejemplo agregados correctamente. Total: {ProductCount}", productos.Length);
    }
    else
    {
        scopedLogger.LogInformation("Base de datos ya contiene {ProductCount} productos", productCount);
    }
}

app.Run();
