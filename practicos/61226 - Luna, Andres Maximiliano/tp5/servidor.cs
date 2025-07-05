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

var contexto = app.Services.GetRequiredService<AppDb>();
contexto.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// los productos menor a 3
app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// agregar stock
app.MapPost("/productos/agregar", async (AppDb db, int id, int cantidad) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// quitar stock
app.MapPost("/productos/quitar", async (AppDb db, int id, int cantidad) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null || producto.Stock < cantidad)
        return Results.BadRequest();
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

// productos de ej
if (!db.Productos.Any())
{
    db.Productos.AddRange(
        new Producto { Nombre = "Lapicera", Precio = 100, Stock = 10 },
        new Producto { Nombre = "Cuaderno", Precio = 500, Stock = 10 },
        new Producto { Nombre = "Regla", Precio = 150, Stock = 10 },
        new Producto { Nombre = "Goma", Precio = 80, Stock = 10 },
        new Producto { Nombre = "Cartuchera", Precio = 1200, Stock = 10 },
        new Producto { Nombre = "Tijera", Precio = 300, Stock = 10 },
        new Producto { Nombre = "Pegamento", Precio = 250, Stock = 10 },
        new Producto { Nombre = "Marcador", Precio = 200, Stock = 10 },
        new Producto { Nombre = "Corrector", Precio = 180, Stock = 10 },
        new Producto { Nombre = "Papel A4", Precio = 900, Stock = 10 }
    );
    db.SaveChanges();
}

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
    public int Stock { get; set; }
}