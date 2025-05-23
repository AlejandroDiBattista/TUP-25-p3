#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// 🧹 Eliminar base de datos si existe
if (File.Exists("tienda.db"))
    File.Delete("tienda.db");

// Configuración y servicios
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Rutas
app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.OrderBy(p => p.Id).ToListAsync()); // ✅ Orden por ID

app.MapGet("/reposicion", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/agregar-stock", async (AppDb db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/quitar-stock", async (AppDb db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Inicializar BD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(Enumerable.Range(1, 10).Select(i =>
            new Producto { Nombre = $"Producto {i}", Precio = 100 + i, Stock = 10 }));
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

// Modelos
class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
