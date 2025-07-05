// Recordar borrar la base de datos desde consola con: rm tienda.db
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
app.MapGet("/productos/reponer", async (AppDb db) => await db.Productos.Where(p => p.Stock < 3).ToListAsync());
app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});
app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();
if (!db.Productos.Any()) {
    var productos = new List<Producto>
    {
        new Producto { Nombre = "Lata de atun", Precio = 2129, Stock = 10 },
        new Producto { Nombre = "Lata de caballa", Precio = 2490, Stock = 10 },
        new Producto { Nombre = "Lata de lenteja", Precio = 350, Stock = 10 },
        new Producto { Nombre = "Lata de arveja", Precio = 800, Stock = 10 },
        new Producto { Nombre = "Lata de poroto", Precio = 750, Stock = 10 },
        new Producto { Nombre = "Lata de choclo", Precio = 1090, Stock = 10 },
        new Producto { Nombre = "Lata de tomate", Precio = 729, Stock = 10 },
        new Producto { Nombre = "Lata de palmito", Precio = 2499, Stock = 10 },
        new Producto { Nombre = "Lata de espageti", Precio = 950, Stock = 10 },
        new Producto { Nombre = "Lata de bife??", Precio = 666, Stock = 10 }
    };
    db.Productos.AddRange(productos);
    db.SaveChanges();
}

app.Run("http://localhost:5000");

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}