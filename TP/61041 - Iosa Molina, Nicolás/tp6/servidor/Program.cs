using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
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

// Crear la base de datos y cargar datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated(); // <-- Aquí se crea la base de datos si no existe

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "iPhone 15", Descripcion = "Celular Apple", Precio = 1200, Stock = 10, ImagenUrl = "https://store.storeimages.cdn-apple.com/iphone15.jpg" },
            new Producto { Nombre = "Samsung Galaxy S24", Descripcion = "Celular Samsung", Precio = 1100, Stock = 8, ImagenUrl = "https://images.samsung.com/galaxy-s24.jpg" },
            new Producto { Nombre = "Xiaomi Redmi Note 13", Descripcion = "Celular Xiaomi", Precio = 400, Stock = 15, ImagenUrl = "https://xiaomi.com/redmi-note-13.jpg" },
            new Producto { Nombre = "Auriculares JBL", Descripcion = "Auriculares inalámbricos", Precio = 150, Stock = 20, ImagenUrl = "https://jbl.com/auriculares.jpg" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido", Precio = 30, Stock = 50, ImagenUrl = "https://images.cargadorusb-c.jpg" },
            new Producto { Nombre = "Coca Cola 2L", Descripcion = "Gaseosa 2 litros", Precio = 3, Stock = 100, ImagenUrl = "https://cocacola.com/2l.jpg" },
            new Producto { Nombre = "Pepsi 2L", Descripcion = "Gaseosa 2 litros", Precio = 2.8, Stock = 90, ImagenUrl = "https://pepsi.com/2l.jpg" },
            new Producto { Nombre = "Funda iPhone 15", Descripcion = "Funda silicona", Precio = 25, Stock = 30, ImagenUrl = "https://fundas.com/iphone15.jpg" },
            new Producto { Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico", Precio = 40, Stock = 25, ImagenUrl = "https://logitech.com/mouse.jpg" },
            new Producto { Nombre = "Teclado Redragon", Descripcion = "Teclado mecánico", Precio = 60, Stock = 18, ImagenUrl = "https://redragon.com/teclado.jpg" }
        );
        db.SaveChanges();
    }
}

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

app.Run();

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public double Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }
    public List<ItemCompra> ItemsCompra { get; set; }
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public double Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCompra> ItemsCompra { get; set; }
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int CompraId { get; set; }
    public Compra Compra { get; set; }
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
}

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }
}
