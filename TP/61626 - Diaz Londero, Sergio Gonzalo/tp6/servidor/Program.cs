var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

//conexion sqlite
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite("Data Source=tiendaonline.db"));

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
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // aplica migraciones automáticamente
}


app.Run();


// Clases de dominio

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }
}

public class Compra
{
    public int Id { get; set; }
    public int Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }

    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int CompraId { get; set; }
    public Compra Compra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Botín de Fútbol", Descripcion = "Botín profesional con tapones de goma", Precio = 199999m, Stock = 15, ImagenUrl = "botin2.jpg" },
            new Producto { Id = 2, Nombre = "Buzo Deportivo", Descripcion = "Buzo de algodón con capucha", Precio = 80000m, Stock = 25, ImagenUrl = "buzo.jpg" },
            new Producto { Id = 3, Nombre = "Campera de Abrigo", Descripcion = "Campera impermeable con interior polar", Precio = 130000m, Stock = 18, ImagenUrl = "campera.jpg" },
            new Producto { Id = 4, Nombre = "Pantalón Jogger", Descripcion = "Pantalón cómodo estilo urbano", Precio = 60000m, Stock = 30, ImagenUrl = "pantalon.jpg" },
            new Producto { Id = 5, Nombre = "Remera Estampada", Descripcion = "Remera de algodón con diseño gráfico", Precio = 35000m, Stock = 40, ImagenUrl = "remera.jpg" },
            new Producto { Id = 6, Nombre = "Remera Básica", Descripcion = "Remera lisa color blanco", Precio = 30000m, Stock = 50, ImagenUrl = "remera2.jpg" },
            new Producto { Id = 7, Nombre = "Short Deportivo", Descripcion = "Short de secado rápido para entrenamiento", Precio = 30000m, Stock = 35, ImagenUrl = "short.jpg" },
            new Producto { Id = 8, Nombre = "Zapatilla Urbana", Descripcion = "Zapatilla moderna y cómoda para el día a día", Precio = 180000m, Stock = 20, ImagenUrl = "zapatilla.jpg" },
            new Producto { Id = 9, Nombre = "Zapatilla Running", Descripcion = "Zapatilla ideal para correr largas distancias", Precio = 190000m, Stock = 22, ImagenUrl = "zapatilla2 (1).jpg" },
            new Producto { Id = 10, Nombre = "Zapatilla Clásica", Descripcion = "Zapatilla estilo retro de lona", Precio = 70000m, Stock = 28, ImagenUrl = "zapatilla2.jpg" }
        );
    }
}
