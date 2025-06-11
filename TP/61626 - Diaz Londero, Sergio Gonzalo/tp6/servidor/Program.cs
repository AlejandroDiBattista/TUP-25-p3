using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS para permitir al cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177") // Puerto del cliente Blazor
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Para usar controladores si necesitas (aunque en este ejemplo no los usas)
builder.Services.AddControllers();

// Configurar SQLite (necesitas crear AppDbContext si quieres BD real)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tiendaonline.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

// Endpoint simple para probar API
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Lista de productos en memoria para demo
var productos = new List<Producto>
{
    new Producto { Id = 1, Nombre = "Botín de Fútbol", Descripcion = "Botín profesional con tapones de goma", Precio = 199999m, Stock = 15, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/botin2.jpg" },
    new Producto { Id = 2, Nombre = "Buzo Deportivo", Descripcion = "Buzo de algodón con capucha", Precio = 80000m, Stock = 25, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/buzo.jpg" },
    new Producto { Id = 3, Nombre = "Campera de Abrigo", Descripcion = "Campera impermeable con interior polar", Precio = 130000m, Stock = 18, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/campera.jpg" },
    new Producto { Id = 4, Nombre = "Pantalón Jogger", Descripcion = "Pantalón cómodo estilo urbano", Precio = 60000m, Stock = 30, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/pantalon.jpg" },
    new Producto { Id = 5, Nombre = "Remera Estampada", Descripcion = "Remera de algodón con diseño gráfico", Precio = 35000m, Stock = 40, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/remera2.jpg" },
    new Producto { Id = 6, Nombre = "Remera Básica", Descripcion = "Remera lisa color blanco", Precio = 30000m, Stock = 50, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/remera.jpg" },
    new Producto { Id = 7, Nombre = "Short Deportivo", Descripcion = "Short de secado rápido para entrenamiento", Precio = 30000m, Stock = 35, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/short.jpg" },
    new Producto { Id = 8, Nombre = "Zapatilla Urbana", Descripcion = "Zapatilla moderna y cómoda para el día a día", Precio = 180000m, Stock = 20, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/zapatilla2.jpg" },
    new Producto { Id = 9, Nombre = "Zapatilla Running", Descripcion = "Zapatilla ideal para correr largas distancias", Precio = 190000m, Stock = 22, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/zapatilla.jpg" },
    new Producto { Id = 10, Nombre = "Zapatilla Clásica", Descripcion = "Zapatilla estilo retro de lona", Precio = 70000m, Stock = 28, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/zapatilla.jpg" }
};

app.MapGet("/api/productos", () => productos);

app.Run();

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }
}

// Opcional: si tienes AppDbContext para EF Core, crea la clase
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }
}
