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

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db")
);

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Habilitar archivos estáticos
app.UseStaticFiles();

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/productos", async (TiendaDbContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
    {
        productos = productos.Where(p =>
            p.Nombre.Contains(q) ||
            p.Descripcion.Contains(q)
        );
    }

    return await productos.ToListAsync();
});

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Se inicializa la base de datos y se cargan los productos.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Aceite Natura 900cc", Descripcion = "Aceite vegetal comestible", Precio = 1800, Stock = 15, ImagenUrl = "/images/Aceite Natura 900cc.jpg" },
            new Producto { Nombre = "Arroz Lucchetti 500grs", Descripcion = "Arroz blanco premium", Precio = 950, Stock = 20, ImagenUrl = "/images/Arroz Lucchetti 500grs.jpg" },
            new Producto { Nombre = "Azucar Ledesma 1kg", Descripcion = "Azúcar refinada", Precio = 1300, Stock = 10, ImagenUrl = "/images/Azucar Ledesma 1kg.jpg" },
            new Producto { Nombre = "Coca-Cola 1.5 ltrs", Descripcion = "Bebida gaseosa cola", Precio = 2800, Stock = 25, ImagenUrl = "/images/Coca-Cola 1.5 ltrs.jpg" },
            new Producto { Nombre = "Fernet Branca 750cc", Descripcion = "Fernet Branca", Precio = 11000, Stock = 18, ImagenUrl = "/images/Fernet Branca 750cc.jpg" },
            new Producto { Nombre = "Galleta TerrabuSi 400grs", Descripcion = "Galletas surtidas", Precio = 2700, Stock = 30, ImagenUrl = "/images/Galleta TerrabuSi.jpg" },
            new Producto { Nombre = "Harina 000 Cañuelas 1kg", Descripcion = "Harina 000 Cañuelas", Precio = 800, Stock = 22, ImagenUrl = "/images/Harina 000 Cañuelas 1kg.jpg" },
            new Producto { Nombre = "Pure de Tomate Mora 520grs", Descripcion = "Pure de tomate", Precio = 600, Stock = 12, ImagenUrl = "/images/Pure de Tomate Mora 520grs.jpg" },
            new Producto { Nombre = "Spaghetti La Providencia 500grs", Descripcion = "Fideo tipo Spaghetti", Precio = 750, Stock = 16, ImagenUrl = "/images/Spaghetti La Providencia 500grs.jpg" },
            new Producto { Nombre = "Té La Virginia x 25 unidades", Descripcion = "Té La Virginia x 25 unidades", Precio = 650, Stock = 8, ImagenUrl = "/images/Té La Virginia x 25 unidades.jpg" }
        );
        db.SaveChanges();
    }
}

app.Run();


public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = null!;
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = null!;
    public string ApellidoCliente { get; set; } = null!;
    public string EmailCliente { get; set; } = null!;
    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int CompraId { get; set; }
    public Compra? Compra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class TiendaDbContext : DbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
}
