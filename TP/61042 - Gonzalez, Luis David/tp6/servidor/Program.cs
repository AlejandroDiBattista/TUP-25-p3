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

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
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

// Crear la base de datos y cargar productos de ejemplo si está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Zapatilla Nike Air Max", Descripcion = "Running, Talle 42", Precio = 120000, Stock = 10, ImagenUrl = "https://images.nike.com/zapatilla-airmax.jpg" },
            new Producto { Nombre = "Zapatilla Adidas Ultraboost", Descripcion = "Training, Talle 41", Precio = 135000, Stock = 8, ImagenUrl = "https://images.adidas.com/zapatilla-ultraboost.jpg" },
            new Producto { Nombre = "Zapatilla Puma RS-X", Descripcion = "Casual, Talle 43", Precio = 110000, Stock = 12, ImagenUrl = "https://images.puma.com/zapatilla-rsx.jpg" },
            new Producto { Nombre = "Zapatilla Converse Chuck Taylor", Descripcion = "Clásica, Talle 40", Precio = 95000, Stock = 15, ImagenUrl = "https://images.converse.com/chuck-taylor.jpg" },
            new Producto { Nombre = "Zapatilla Reebok Classic", Descripcion = "Urbanas, Talle 42", Precio = 98000, Stock = 9, ImagenUrl = "https://images.reebok.com/classic.jpg" },
            new Producto { Nombre = "Zapatilla Vans Old Skool", Descripcion = "Skate, Talle 41", Precio = 90000, Stock = 11, ImagenUrl = "https://images.vans.com/oldskool.jpg" },
            new Producto { Nombre = "Zapatilla Fila Disruptor", Descripcion = "Moda, Talle 39", Precio = 85000, Stock = 7, ImagenUrl = "https://images.fila.com/disruptor.jpg" },
            new Producto { Nombre = "Zapatilla New Balance 574", Descripcion = "Running, Talle 44", Precio = 105000, Stock = 6, ImagenUrl = "https://images.newbalance.com/574.jpg" },
            new Producto { Nombre = "Zapatilla Asics Gel", Descripcion = "Deportiva, Talle 42", Precio = 115000, Stock = 10, ImagenUrl = "https://images.asics.com/gel.jpg" },
            new Producto { Nombre = "Zapatilla Topper Urbana", Descripcion = "Diaria, Talle 40", Precio = 70000, Stock = 20, ImagenUrl = "https://images.topper.com/urbana.jpg" }
        });
        db.SaveChanges();
    }
}

app.Run();
