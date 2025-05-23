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

// Endpoint para productos a reponer (stock < 3)
app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

// Endpoint para agregar stock
app.MapPost("/productos/{id}/agregar", async (int id, AppDb db) => {
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound();
    prod.Stock++;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

// Endpoint para quitar stock (no dejar negativo)
app.MapPost("/productos/{id}/quitar", async (int id, AppDb db) => {
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound();
    if (prod.Stock == 0) return Results.BadRequest("Stock insuficiente");
    prod.Stock--;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos
if (!db.Productos.Any()) {
    var productosEjemplo = new[] {
        new Producto { Nombre = "Auriculares", Precio = 1200, Stock = 10 },
        new Producto { Nombre = "Procesadores", Precio = 900, Stock = 10 },
        new Producto { Nombre = "Teclados", Precio = 2500, Stock = 10 },
        new Producto { Nombre = "Pendrives", Precio = 1100, Stock = 10 },
        new Producto { Nombre = "Cables HDMI", Precio = 1800, Stock = 10 },
        new Producto { Nombre = "Luces LED", Precio = 500, Stock = 10 },
        new Producto { Nombre = "MicroSD", Precio = 800, Stock = 10 },
        new Producto { Nombre = "Monitores", Precio = 1500, Stock = 10 },
        new Producto { Nombre = "Placas de video", Precio = 2200, Stock = 10 },
        new Producto { Nombre = "Parlantes", Precio = 700, Stock = 10 }
    };
    db.Productos.AddRange(productosEjemplo);
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