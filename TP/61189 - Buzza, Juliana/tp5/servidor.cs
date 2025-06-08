#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 7.0.11"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 7.0.11"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.4"


using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPut("/productos/{id}/agregar/{cantidad}", async (AppDb db, int id, int cantidad) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPut("/productos/{id}/quitar/{cantidad}", async (AppDb db, int id, int cantidad) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    if (producto.Stock - cantidad < 0)
        return Results.BadRequest("El stock no puede ser negativo");

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
        new Producto { Nombre = "Laptop", Precio = 999.99m, Stock = 10 },
        new Producto { Nombre = "Mouse", Precio = 29.99m, Stock = 10 },
        new Producto { Nombre = "Teclado", Precio = 49.99m, Stock = 10 },
        new Producto { Nombre = "Monitor", Precio = 299.99m, Stock = 10 },
        new Producto { Nombre = "Auriculares", Precio = 79.99m, Stock = 10 },
        new Producto { Nombre = "Webcam", Precio = 59.99m, Stock = 10 },
        new Producto { Nombre = "Impresora", Precio = 199.99m, Stock = 10 },
        new Producto { Nombre = "Disco Duro", Precio = 89.99m, Stock = 10 },
        new Producto { Nombre = "Memoria RAM", Precio = 69.99m, Stock = 10 },
        new Producto { Nombre = "Tarjeta Gráfica", Precio = 399.99m, Stock = 10 }
    };
    db.Productos.AddRange(productos);
    db.SaveChanges();
}

app.Run("http://localhost:5000");
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}