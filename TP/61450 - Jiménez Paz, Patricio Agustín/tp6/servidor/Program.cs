using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Base de datos usando Entity Framework Core y SQLite
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));

// Configurar opciones JSON para usar camelCase
builder.Services.Configure<JsonOptions>(opt =>
{
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowClientApp",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5177", "https://localhost:7221")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Servir archivos estáticos desde la carpeta "wwwroot"
app.UseStaticFiles();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Endpoints productos
app.MapGet(
    "/productos",
    async (AppDb db, string? search) =>
    {
        if (string.IsNullOrWhiteSpace(search))
            return await db.Productos.ToListAsync();

        return await db
            .Productos.Where(p =>
                p.Nombre.ToLower().Contains(search.ToLower())
                || p.Descripcion.ToLower().Contains(search.ToLower())
            )
            .ToListAsync();
    }
);

// Endpoints carrito
// Inicializar un carrito vacío
app.MapPost(
    "/carritos",
    async (AppDb db) =>
    {
        Carrito carrito = new Carrito();
        db.Carritos.Add(carrito);
        await db.SaveChangesAsync();
        return Results.Created($"/carrito/{carrito.Id}", carrito);
    }
);

// Obtener productos del carrito
app.MapGet(
    "/carritos/{id:int}",
    async (AppDb db, int id) =>
    {
        var carrito = await db
            .Carritos.Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (carrito == null)
            return Results.NotFound(new { message = "Carrito no encontrado." });

        return Results.Ok(carrito);
    }
);

// Vaciar el carrito
app.MapDelete(
    "/carritos/{id:int}",
    async (AppDb db, int id) =>
    {
        var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);

        if (carrito == null)
            return Results.NotFound(new { message = "Carrito no encontrado." });

        if (!carrito.Items.Any())
            return Results.BadRequest(new { message = "El carrito ya está vacío." });

        db.ItemsCarrito.RemoveRange(carrito.Items);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

// Agregar un producto al carrito
app.MapPut(
    "/carritos/{id:int}/{productoId:int}",
    async (AppDb db, int id, int productoId) =>
    {
        var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);

        if (carrito == null)
            return Results.NotFound(new { message = "Carrito no encontrado." });

        var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
        if (producto == null || producto.Stock == 0)
            return Results.BadRequest(
                new { message = "Producto no disponible o stock insuficiente." }
            );

        var itemCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (itemCarrito != null)
        {
            itemCarrito.Cantidad += 1;
        }
        else
        {
            itemCarrito = new ItemCarrito
            {
                Producto = producto,
                Cantidad = 1,
                Carrito = carrito,
            };
            carrito.Items.Add(itemCarrito);
        }

        producto.Stock -= 1; // Reducir el stock del producto
        await db.SaveChangesAsync();

        // Recargar el carrito con los productos actualizados
        var carritoActualizado = await db
            .Carritos.Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(c => c.Id == id);

        return Results.Ok(carritoActualizado);
    }
);

// Eliminar un producto del carrito
app.MapDelete(
    "/carritos/{id:int}/{productoId:int}",
    async (AppDb db, int id, int productoId) =>
    {
        var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);

        if (carrito == null)
            return Results.NotFound(new { message = "Carrito no encontrado." });

        var itemCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (itemCarrito == null)
            return Results.NotFound(new { message = "Producto no encontrado en el carrito." });

        if (itemCarrito.Cantidad > 1)
        {
            itemCarrito.Cantidad -= 1;
        }
        else
        {
            carrito.Items.Remove(itemCarrito);
            db.ItemsCarrito.Remove(itemCarrito);
        }

        var producto = await db.Productos.FindAsync(productoId);
        if (producto != null)
        {
            producto.Stock += 1; // Devolver el stock del producto
        }

        await db.SaveChangesAsync();
        // Recargar el carrito con los productos actualizados
        var carritoActualizado = await db
            .Carritos.Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(c => c.Id == id);

        return Results.Ok(carritoActualizado);
    }
);

// Endpoints compras (carrito a compra)
app.MapPut(
    "/carritos/{id:int}/confirmar",
    async (AppDb db, int id, CompraDto compraDto) =>
    {
        var carrito = await db
            .Carritos.Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (carrito == null)
            return Results.NotFound(new { message = "Carrito no encontrado." });

        if (!carrito.Items.Any())
            return Results.BadRequest(new { message = "El carrito está vacío." });

        // Validar datos del cliente
        if (
            string.IsNullOrWhiteSpace(compraDto.Cliente.Nombre)
            || string.IsNullOrWhiteSpace(compraDto.Cliente.Apellido)
            || string.IsNullOrWhiteSpace(compraDto.Cliente.Email)
        )
        {
            return Results.BadRequest(new { message = "Datos del cliente incompletos." });
        }
        if (!compraDto.Cliente.Email.Contains("@"))
        {
            return Results.BadRequest(new { message = "Email del cliente inválido." });
        }

        // Crear la compra
        Compra compra = new Compra
        {
            Total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio),
            Items = carrito
                .Items.Select(i => new ItemCompra
                {
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.Producto.Precio,
                    ProductoId = i.ProductoId,
                })
                .ToList(),
            NombreCliente = compraDto.Cliente.Nombre,
            ApellidoCliente = compraDto.Cliente.Apellido,
            EmailCliente = compraDto.Cliente.Email,
            Fecha = compraDto.Fecha ?? DateTime.Now,
        };

        db.Compras.Add(compra);

        // Eliminar el carrito y sus items
        db.Carritos.Remove(carrito);
        db.ItemsCarrito.RemoveRange(carrito.Items);

        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Compra confirmada.", compra });
    }
);

// Inicializar la base de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated(); // Asegura que la base de datos y las tablas existan

    if (!db.Productos.Any())
    {
        // Agregar algunos productos de ejemplo si la tabla está vacía
        db.Productos.AddRange(
            new List<Producto>
            {
                new Producto
                {
                    Nombre = "Xiaomi Redmi 14C 256 GB",
                    Descripcion =
                        "El Xiaomi Redmi 14C combina diseño moderno con rendimiento eficiente. Su pantalla LCD de 6,88\" con refresco de 120 Hz ofrece una experiencia visual fluida. Equipado con un procesador MediaTek Helio G81-Ultra y 4 GB de RAM, garantiza un rendimiento ágil en multitarea. La cámara principal de 50 MP captura fotos nítidas, mientras que la batería de 5.160 mAh asegura autonomía para todo el día. Con 256 GB de almacenamiento, tendrás espacio amplio para tus aplicaciones y archivos. Además, cuenta con sensor de huellas lateral y sistema operativo Android 14 con HyperOS.",
                    Precio = 313499,
                    Stock = 10,
                    ImagenUrl = "/images/Xiaomi_Redmi_14_C.webp",
                },
                new Producto
                {
                    Nombre = "Xiaomi Redmi Note 13 128 GB",
                    Descripcion =
                        "El Xiaomi Redmi Note 13 destaca por su pantalla AMOLED de 6,67\" con resolución FHD+ que ofrece colores vibrantes y negros profundos. Su cámara principal de 108 MP, acompañada de lentes ultra gran angular y macro, permite capturar imágenes detalladas en diversas condiciones. La batería de 5.000 mAh proporciona una duración excepcional, superando la de muchos competidores. Con 128 GB de almacenamiento, es ideal para usuarios que buscan un equilibrio entre rendimiento y precio.",
                    Precio = 379999,
                    Stock = 5,
                    ImagenUrl = "/images/Xiaomi_Redmi_Note_13.webp",
                },
                new Producto
                {
                    Nombre = "Samsung Galaxy A06 128 GB",
                    Descripcion =
                        "El Samsung Galaxy A06 ofrece una pantalla de 6,5\" con resolución HD+ y una tasa de refresco de 90 Hz, brindando una experiencia visual fluida. Su cámara principal de 50 MP captura fotos claras y detalladas. Con 128 GB de almacenamiento y una batería de 5.000 mAh, es una opción sólida para usuarios que buscan un dispositivo confiable sin comprometer el presupuesto.",
                    Precio = 219999,
                    Stock = 1,
                    ImagenUrl = "/images/Samsung_Galaxy_A06.webp",
                },
                new Producto
                {
                    Nombre = "Motorola Moto G35 5G 128 GB",
                    Descripcion =
                        "El Motorola Moto G35 5G se destaca por su pantalla de 6,7\" con resolución FHD+ y tasa de refresco de 120 Hz, ideal para contenidos multimedia. Equipado con un procesador Unisoc T760 y 4 GB de RAM, ofrece un rendimiento fluido en tareas diarias. La cámara principal de 50 MP y la batería de 5.000 mAh aseguran fotos de calidad y autonomía prolongada. Con 128 GB de almacenamiento, es perfecto para usuarios que buscan conectividad 5G a un precio accesible.",
                    Precio = 309999,
                    Stock = 8,
                    ImagenUrl = "/images/Motorola_Moto_G35_5G.webp",
                },
                new Producto
                {
                    Nombre = "Samsung Galaxy A36 5G 256 GB",
                    Descripcion =
                        "El Samsung Galaxy A36 5G presenta una pantalla AMOLED de 6,7\" con tasa de refresco de 120 Hz, ofreciendo colores vibrantes y transiciones suaves. Su cámara principal de 50 MP captura imágenes nítidas, mientras que la batería de 5.000 mAh garantiza un uso prolongado. Con 256 GB de almacenamiento, es ideal para usuarios que buscan un equilibrio entre rendimiento y capacidad. Sin embargo, su procesador Snapdragon 6 Gen 3 ha mostrado algunos problemas de optimización, lo que puede afectar el rendimiento en tareas exigentes.",
                    Precio = 499999,
                    Stock = 3,
                    ImagenUrl = "/images/Samsung_Galaxy_A36_5G.webp",
                },
                new Producto
                {
                    Nombre = "Motorola Moto G75 5G 128 GB",
                    Descripcion =
                        "El Motorola Moto G75 5G destaca por su pantalla de 6,8\" con resolución FHD+ y tasa de refresco de 120 Hz, brindando una experiencia visual inmersiva. Equipado con el procesador Snapdragon 6 Gen 3 y 4 GB de RAM, ofrece un rendimiento fluido en multitarea. La cámara principal de 50 MP y la batería de 5.000 mAh aseguran fotos de calidad y autonomía prolongada. Con 128 GB de almacenamiento, es una opción sólida para usuarios que buscan conectividad 5G y rendimiento equilibrado.",
                    Precio = 469999,
                    Stock = 7,
                    ImagenUrl = "/images/Motorola_Moto_G75_5G.webp",
                },
                new Producto
                {
                    Nombre = "Motorola Razr 50 Ultra 512 GB",
                    Descripcion =
                        "El Motorola Razr 50 Ultra es un smartphone plegable que combina diseño innovador con tecnología avanzada. Su pantalla interna pOLED de 6,9\" con tasa de refresco de 165 Hz ofrece una experiencia visual fluida. Equipado con el procesador Snapdragon 8s Gen 3 y 12 GB de RAM, garantiza un rendimiento excepcional. Las cámaras traseras de 50 MP y la frontal de 32 MP permiten capturar fotos de alta calidad. La batería de 4.000 mAh y la carga inalámbrica de 15W aseguran autonomía y conveniencia. Con 512 GB de almacenamiento, es ideal para usuarios que buscan lo último en tecnología y diseño.",
                    Precio = 1499999,
                    Stock = 4,
                    ImagenUrl = "/images/Motorola_Razr_50_Ultra.webp",
                },
                new Producto
                {
                    Nombre = "Samsung Galaxy S25+ 256 GB",
                    Descripcion =
                        "El Samsung Galaxy S25+ es un smartphone de gama alta que ofrece una pantalla Dynamic AMOLED de 6,7\" con resolución QHD+ y tasa de refresco de 120 Hz, brindando colores vibrantes y transiciones suaves. Equipado con el procesador Snapdragon 8 Elite y 12 GB de RAM, ofrece un rendimiento excepcional. Las cámaras traseras de 50 MP, 12 MP y 10 MP permiten capturar fotos de alta calidad. La batería de 4.900 mAh y la carga rápida de 45W aseguran autonomía y conveniencia. Con 256 GB de almacenamiento, es ideal para usuarios que buscan lo último en tecnología y rendimiento.",
                    Precio = 2999999,
                    Stock = 5,
                    ImagenUrl = "/images/Samsung_Galaxy_S25_Plus.webp",
                },
                new Producto
                {
                    Nombre = "iPhone 14 128 GB + Cable USB-C Lightning",
                    Descripcion =
                        "El iPhone 14 ofrece una pantalla Super Retina XDR de 6,1\" con tecnología OLED, brindando colores precisos y negros profundos. Equipado con el chip A15 Bionic y 4 GB de RAM, ofrece un rendimiento fluido en multitarea. Las cámaras de 12 MP permiten capturar fotos y videos de alta calidad. La batería proporciona autonomía para todo el día y es compatible con carga rápida. Con 128 GB de almacenamiento, es ideal para usuarios que buscan un dispositivo premium con el ecosistema de Apple.",
                    Precio = 1999999,
                    Stock = 3,
                    ImagenUrl = "/images/iPhone_14_128_GB_Cable_USB_C_Lightning.webp",
                },
                new Producto
                {
                    Nombre = "Samsung Galaxy Z Flip6 512 GB",
                    Descripcion =
                        "El Samsung Galaxy Z Flip6 es un smartphone plegable que combina diseño compacto con tecnología avanzada. Su pantalla interna Dynamic AMOLED 2X de 6,7\" con tasa de refresco de 120 Hz ofrece una experiencia visual fluida. Equipado con el procesador Snapdragon 8 Gen 3 y 12 GB de RAM, garantiza un rendimiento excepcional. Las cámaras de 50 MP y 12 MP permiten capturar fotos de alta calidad. La batería de 4.000 mAh y la carga inalámbrica de 15W aseguran autonomía y conveniencia. Con 512 GB de almacenamiento, es ideal para usuarios que buscan lo último en tecnología y diseño.",
                    Precio = 2649999,
                    Stock = 2,
                    ImagenUrl = "/images/Samsung_Galaxy_Z_Flip6.webp",
                },
            }
        );
        db.SaveChanges();
    }
}

app.Run();

// Modelo de datos
class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options)
        : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<ItemCarrito> ItemsCarrito => Set<ItemCarrito>();
}

// Modelo de datos
class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; } = 0;
    public string ImagenUrl { get; set; } = null!;
}

class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCompra> Items { get; set; } = new List<ItemCompra>();
}

record CompraDto(ClienteDto Cliente, DateTime? Fecha);

record ClienteDto(string Nombre, string Apellido, string Email);

class ItemCompra
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int CompraId { get; set; }

    [JsonIgnore]
    public Compra Compra { get; set; } = null!;
}

class Carrito
{
    public int Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
}

class ItemCarrito
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int CarritoId { get; set; }

    [JsonIgnore]
    public Carrito Carrito { get; set; } = null!;
}
