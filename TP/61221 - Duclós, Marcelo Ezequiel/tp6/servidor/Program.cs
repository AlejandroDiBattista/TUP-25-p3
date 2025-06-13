using servidor.Data;
using servidor.Models;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");
app.UseStaticFiles();

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoints CRUD para productos

app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

app.MapGet("/api/productos/{id:int}", async (int id, TiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto
        ? Results.Ok(producto)
        : Results.NotFound()
);

app.MapPost("/api/productos", async (Producto producto, TiendaContext db) =>
{
    if (string.IsNullOrWhiteSpace(producto.Nombre) ||
        string.IsNullOrWhiteSpace(producto.Descripcion) ||
        producto.Precio < 0 ||
        producto.Stock < 0 ||
        string.IsNullOrWhiteSpace(producto.ImagenUrl))
    {
        return Results.BadRequest("Todos los campos son obligatorios y los valores numéricos deben ser no negativos.");
    }

    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{producto.Id}", producto);
});

app.MapPut("/api/productos/{id:int}", async (int id, Producto input, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(input.Nombre) ||
        string.IsNullOrWhiteSpace(input.Descripcion) ||
        input.Precio < 0 ||
        input.Stock < 0 ||
        string.IsNullOrWhiteSpace(input.ImagenUrl))
    {
        return Results.BadRequest("Todos los campos son obligatorios y los valores numéricos deben ser no negativos.");
    }

    producto.Nombre = input.Nombre;
    producto.Descripcion = input.Descripcion;
    producto.Precio = input.Precio;
    producto.Stock = input.Stock;
    producto.ImagenUrl = input.ImagenUrl;

    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapDelete("/api/productos/{id:int}", async (int id, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoints CRUD para clientes

app.MapGet("/api/clientes", async (TiendaContext db) =>
    await db.Clientes.ToListAsync()
);

app.MapGet("/api/clientes/{id:int}", async (int id, TiendaContext db) =>
    await db.Clientes.FindAsync(id) is Cliente cliente
        ? Results.Ok(cliente)
        : Results.NotFound()
);

app.MapGet("/api/clientes/email/{email}", async (string email, TiendaContext db) =>
    await db.Clientes.FirstOrDefaultAsync(c => c.Email == email) is Cliente cliente
        ? Results.Ok(cliente)
        : Results.NotFound()
);

app.MapPost("/api/clientes", async (Cliente cliente, TiendaContext db) =>
{
    if (string.IsNullOrWhiteSpace(cliente.Nombre) ||
        string.IsNullOrWhiteSpace(cliente.Apellido) ||
        string.IsNullOrWhiteSpace(cliente.Email))
    {
        return Results.BadRequest("Nombre, Apellido y Email son obligatorios.");
    }

    db.Clientes.Add(cliente);
    await db.SaveChangesAsync();
    return Results.Created($"/api/clientes/{cliente.Id}", cliente);
});

app.MapPut("/api/clientes/{id:int}", async (int id, Cliente input, TiendaContext db) =>
{
    var cliente = await db.Clientes.FindAsync(id);
    if (cliente is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(input.Nombre) ||
        string.IsNullOrWhiteSpace(input.Apellido) ||
        string.IsNullOrWhiteSpace(input.Email))
    {
        return Results.BadRequest("Nombre, Apellido y Email son obligatorios.");
    }

    cliente.Nombre = input.Nombre;
    cliente.Apellido = input.Apellido;
    cliente.Email = input.Email;

    await db.SaveChangesAsync();
    return Results.Ok(cliente);
});

app.MapDelete("/api/clientes/{id:int}", async (int id, TiendaContext db) =>
{
    var cliente = await db.Clientes.FindAsync(id);
    if (cliente is null) return Results.NotFound();

    db.Clientes.Remove(cliente);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoint POST para ventas con validación de stock y de entrada

app.MapPost("/api/ventas", async (RegistrarVentaRequest ventaRequest, TiendaContext db) =>
{
    using var transaction = await db.Database.BeginTransactionAsync();
    try
    {
        // 1. Validar datos básicos
        if (string.IsNullOrWhiteSpace(ventaRequest.EmailCliente))
            return Results.BadRequest("El email es obligatorio");

        // 2. Crear o actualizar cliente
        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Email == ventaRequest.EmailCliente);
        if (cliente == null)
        {
            cliente = new Cliente
            {
                Nombre = ventaRequest.NombreCliente,
                Apellido = ventaRequest.ApellidoCliente,
                Email = ventaRequest.EmailCliente
            };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();
        }

        // 3. Validar productos y stock
        var detalles = new List<DetalleVenta>();
        decimal total = 0;

        foreach (var detalle in ventaRequest.Detalles)
        {
            var producto = await db.Productos.FindAsync(detalle.ProductoId);
            if (producto == null)
                return Results.NotFound($"Producto {detalle.ProductoId} no encontrado");
            
            if (producto.Stock < detalle.Cantidad)
                return Results.BadRequest($"Stock insuficiente para {producto.Nombre}");

            producto.Stock -= detalle.Cantidad;

            var detalleVenta = new DetalleVenta
            {
                ProductoId = producto.Id,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = producto.Precio
            };

            detalles.Add(detalleVenta);
            total += producto.Precio * detalle.Cantidad;
        }

        // 4. Crear la venta
        var venta = new Venta
        {
            Fecha = DateTime.Now,
            NombreCliente = ventaRequest.NombreCliente,
            ApellidoCliente = ventaRequest.ApellidoCliente,
            EmailCliente = ventaRequest.EmailCliente,
            Total = total,
            Detalles = detalles
        };

        db.Ventas.Add(venta);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return Results.Ok(new { Message = "Venta registrada con éxito", VentaId = venta.Id });
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return Results.BadRequest($"Error al procesar la venta: {ex.Message}");
    }
});

// Agregar endpoint para consultar historial
app.MapGet("/api/ventas/historial/{email}", async (string email, TiendaContext db) =>
{
    try
    {
        var ventas = await db.Ventas
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .Where(v => v.EmailCliente.ToLower().Trim() == email.ToLower().Trim())
            .OrderByDescending(v => v.Fecha)
            .Select(v => new
            {
                v.Id,
                v.Fecha,
                v.Total,
                v.NombreCliente,
                v.ApellidoCliente,
                v.EmailCliente,
                Detalles = v.Detalles.Select(d => new
                {
                    d.Id,
                    d.Cantidad,
                    d.PrecioUnitario,
                    Producto = new
                    {
                        d.Producto.Id,
                        d.Producto.Nombre,
                        d.Producto.Precio,
                        d.Producto.Stock,
                        d.Producto.ImagenUrl
                    }
                }).ToList()
            })
            .ToListAsync();

        return Results.Ok(ventas);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al buscar historial: {ex.Message}");
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto
            {
                Nombre = "Smartphone Samsung Galaxy A54",
                Descripcion = "Smartphone 5G de gama media, pantalla AMOLED de 6.4\", 6 GB RAM, 128 GB almacenamiento, cámara trasera de 50+12+5 MP, batería 5000 mAh, resistencia IP67.",
                Precio = 173,
                Stock = 10,
                ImagenUrl = "/img/Smartphone-Samsung-Galaxy-A54.jpg",
                Categoria = "Smartphones"
            },
            new Producto
            {
                Nombre = "Notebook Lenovo IdeaPad 3",
                Descripcion = "Laptop de uso general, pantalla de 15\" FHD, procesador Intel o AMD, buen equilibrio entre rendimiento y precio, ideal para tareas diarias.",
                Precio = 409,
                Stock = 10,
                ImagenUrl = "/img/Notebook-Lenovo-IdeaPad-3.jpg",
                Categoria = "Notebooks"
            },
            new Producto
            {
                Nombre = "Auriculares Bluetooth JBL Tune 510BT",
                Descripcion = "Auriculares inalámbricos supraaurales, batería de hasta 40h, entrada auxiliar 3.5mm, audio JBL Pure Bass.",
                Precio = 59,
                Stock = 10,
                ImagenUrl = "/img/Auriculares-Bluetooth-JBL-Tune-510BT.jpg",
                Categoria = "Audio"
            },
            new Producto
            {
                Nombre = "Mouse inalambrico Logitech M185",
                Descripcion = "Mouse óptico inalámbrico de 3 botones, nano receptor USB, batería duradera (~1 año).",
                Precio = 18,
                Stock = 10,
                ImagenUrl = "/img/Mouse-inalámbrico-Logitech-M185.jpg",
                Categoria = "Perifericos"
            },
            new Producto
            {
                Nombre = "Teclado mecanico Redragon Kumara",
                Descripcion = "Teclado compacto (87 teclas), retroiluminación RGB, switches mecánicos Outemu, resistente.",
                Precio = 45,
                Stock = 10,
                ImagenUrl = "/img/Teclado-mecánico-Redragon-Kumara.jpg",
                Categoria = "Perifericos"
            },
            new Producto
            {
                Nombre = "Monitor LG 24 Full HD",
                Descripcion = "Monitor LED de 24\", resolución 1920x1080, tiempo de respuesta 5ms, conexiones HDMI y VGA.",
                Precio = 130,
                Stock = 10,
                ImagenUrl = "/img/Monitor-LG-24-Pulgadas-Full-HD.jpg",
                Categoria = "Monitores"
            },
            new Producto
            {
                Nombre = "Tablet Samsung Galaxy Tab A8",
                Descripcion = "Tablet de 10.5\", 3 GB RAM, 32/64 GB almacenamiento, batería de 7040 mAh, ideal para multimedia.",
                Precio = 230,
                Stock = 10,
                ImagenUrl = "/img/Tablet-Samsung-Galaxy-Tab-A8.jpg",
                Categoria = "Tablets"
            },
            new Producto
            {
                Nombre = "Smartwatch Xiaomi Mi Band 7",
                Descripcion = "Pulsera de actividad con pantalla AMOLED 1.62\", monitoreo de salud (ritmo, sueño), duración de batería 14 días.",
                Precio = 50,
                Stock = 10,
                ImagenUrl = "/img/Smartwatch-Xiaomi-Mi-Band-7.jpg",
                Categoria = "Wearables"
            },
            new Producto
            {
                Nombre = "Parlante portatil JBL Flip 6",
                Descripcion = "Altavoz Bluetooth portátil, resistente al agua (IPX7), 12h de batería, sonido JBL Pro Sound.",
                Precio = 130,
                Stock = 10,
                ImagenUrl = "/img/Parlante-portátil-JBL-Flip-6.jpg",
                Categoria = "Audio"
            },
            new Producto
            {
                Nombre = "Disco SSD Kingston 480GB",
                Descripcion = "SSD SATA de 480GB, velocidades de lectura/escritura de hasta 550/520 MB/s, hasta 3 años de garantía.",
                Precio = 45,
                Stock = 10,
                ImagenUrl = "/img/Disco-SSD-Kingston-480-GB.jpg",
                Categoria = "Almacenamiento"
            }
        );
        db.SaveChanges();
    }
}

app.Run();