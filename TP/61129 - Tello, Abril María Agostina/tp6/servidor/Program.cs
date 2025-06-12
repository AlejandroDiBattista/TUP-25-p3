using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Configura EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configura CORS para permitir el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5177",
            "https://localhost:7221",
            "http://localhost:5180"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Crea la base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}

// Usa CORS
app.UseCors("AllowClientApp");

// Endpoints Minimal API

// GET /api/productos
app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

// Aquí puedes agregar los demás endpoints para carrito y compras según lo que pida el TP

app.Run();
