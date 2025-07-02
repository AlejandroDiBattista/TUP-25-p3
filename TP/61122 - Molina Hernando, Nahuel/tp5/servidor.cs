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

// Configuración de servicios
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => 
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Endpoints

// Listar todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Listar productos a reponer (stock < 3)
app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

// Agregar stock a un producto
app.MapPost("/productos/{id}/agregar", async (AppDb db, int id, int cantidad) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound("Producto no encontrado.");
    prod.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

// Quitar stock a un producto (sin dejar negativo)
app.MapPost("/productos/{id}/quitar", async (AppDb db, int id, int cantidad) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound("Producto no encontrado.");
    if (prod.Stock < cantidad) return Results.BadRequest("No hay suficiente stock.");
    prod.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

// Inicializar base de datos y productos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            db.Productos.Add(new Producto
            {
                Nombre = $"Producto {i}",
                Precio = 100 + i * 10,
                Stock = 10
            });
        }
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

// Clases de modelo y contexto
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
