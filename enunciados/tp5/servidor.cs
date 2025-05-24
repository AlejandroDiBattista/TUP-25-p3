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

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Producto> Productos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=tienda.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Laptop", Stock = 10 },
            new Producto { Id = 2, Nombre = "Mouse", Stock = 10 },
            new Producto { Id = 3, Nombre = "Teclado", Stock = 10 },
            new Producto { Id = 4, Nombre = "Monitor", Stock = 10 },
            new Producto { Id = 5, Nombre = "Auriculares", Stock = 10 },
            new Producto { Id = 6, Nombre = "Silla Gamer", Stock = 10 },
            new Producto { Id = 7, Nombre = "Impresora", Stock = 10 },
            new Producto { Id = 8, Nombre = "Router", Stock = 10 },
            new Producto { Id = 9, Nombre = "Disco SSD", Stock = 10 },
            new Producto { Id = 10, Nombre = "Pendrive", Stock = 10 }
        );
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();

app.MapGet("/productos", async (AppDbContext db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reposicion", async (AppDbContext db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/agregar/{id}/{cantidad}", async (AppDbContext db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto != null)
    {
        producto.Stock += cantidad;
        await db.SaveChangesAsync();
        return Results.Ok(producto);
    }
    return Results.NotFound();
});

app.MapPost("/productos/quitar/{id}/{cantidad}", async (AppDbContext db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto != null && producto.Stock - cantidad >= 0)
    {
        producto.Stock -= cantidad;
        await db.SaveChangesAsync();
        return Results.Ok(producto);
    }
    return Results.BadRequest();
});

app.Run();
