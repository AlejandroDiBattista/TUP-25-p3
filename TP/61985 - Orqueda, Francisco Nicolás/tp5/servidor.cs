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

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();


app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.ToListAsync());


app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPut("/productos/{id}/agregar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


app.MapPut("/productos/{id}/quitar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad)
        return Results.BadRequest("No se puede quitar más stock del disponible");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); 


if (!db.Productos.Any())
{
    var productos = new[]
    {
        new Producto { Nombre = "Lapicera",     Precio = 100, Stock = 10 },
        new Producto { Nombre = "Cuaderno",     Precio = 500, Stock = 10 },
        new Producto { Nombre = "Regla",        Precio = 200, Stock = 10 },
        new Producto { Nombre = "Goma",         Precio = 50,  Stock = 10 },
        new Producto { Nombre = "Compás",       Precio = 350, Stock = 10 },
        new Producto { Nombre = "Carpeta",      Precio = 700, Stock = 10 },
        new Producto { Nombre = "Tijera",       Precio = 300, Stock = 10 },
        new Producto { Nombre = "Cartuchera",   Precio = 800, Stock = 10 },
        new Producto { Nombre = "Corrector",    Precio = 150, Stock = 10 },
        new Producto { Nombre = "Pegamento",    Precio = 120, Stock = 10 }
    };
    db.Productos.AddRange(productos);
    db.SaveChanges();
}
Console.Clear();
app.Run("http://localhost:5000");


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
