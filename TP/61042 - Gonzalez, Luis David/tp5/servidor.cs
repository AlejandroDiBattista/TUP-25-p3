#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); // agregar servicios : Instalar EF Core y SQLite
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Zapatillas de running", Stock = 10 },
            new Producto { Nombre = "Camiseta deportiva", Stock = 10 },
            new Producto { Nombre = "Short de entrenamiento", Stock = 10 },
            new Producto { Nombre = "Buzo térmico", Stock = 10 },
            new Producto { Nombre = "Calza deportiva", Stock = 10 },
            new Producto { Nombre = "Guantes de gimnasio", Stock = 10 },
            new Producto { Nombre = "Gorra para correr", Stock = 10 },
            new Producto { Nombre = "Medias técnicas", Stock = 10 },
            new Producto { Nombre = "Chaqueta impermeable", Stock = 10 },
            new Producto { Nombre = "Bolso deportivo", Stock = 10 }
        });
        db.SaveChanges();
    }
}

app.MapGet("/productos", async (AppDbContext db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDbContext db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar/{cantidad}", async (int id, int cantidad, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar/{cantidad}", async (int id, int cantidad, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    if (producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente.");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Stock { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}
}