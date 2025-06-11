using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Conexión a SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Habilitar CORS para el cliente Blazor
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Crear DB y cargar datos de ejemplo si está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated(); // crea la base si no existe

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new List<Producto>
        {
            new Producto { Nombre = "Celular A1", Descripcion = "Smartphone gama media", Precio = 120000, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Producto 2", Descripcion = "Descripción del producto 2", Precio = 200, Stock = 5, ImagenUrl = "https://example.com/producto2.jpg" }
        });
        db.SaveChanges();
    }
}

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

// Endpoint de prueba
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint para obtener productos (puede servirte como primer API real)
app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

app.Run();
