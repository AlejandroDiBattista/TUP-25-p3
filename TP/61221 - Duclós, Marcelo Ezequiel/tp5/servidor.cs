#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPut("/productos/{id}/agregar/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPut("/productos/{id}/quitar/{cantidad}", async (int id, int cantidad, AppDb db) =>
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
    db.Productos.AddRange(
        new Producto { Nombre = "Notebook Lenovo", Precio = 850000, Stock = 10 },
        new Producto { Nombre = "Mouse Logitech", Precio = 12000, Stock = 10 },
        new Producto { Nombre = "Teclado Mecánico", Precio = 25000, Stock = 10 },
        new Producto { Nombre = "Monitor Samsung 24\"", Precio = 95000, Stock = 10 },
        new Producto { Nombre = "Auriculares Bluetooth", Precio = 18000, Stock = 10 },
        new Producto { Nombre = "Disco SSD 1TB", Precio = 60000, Stock = 10 },
        new Producto { Nombre = "Placa de Video RTX 4060", Precio = 520000, Stock = 10 },
        new Producto { Nombre = "Impresora HP", Precio = 70000, Stock = 10 },
        new Producto { Nombre = "Tablet Samsung", Precio = 120000, Stock = 10 },
        new Producto { Nombre = "Smartwatch Xiaomi", Precio = 35000, Stock = 10 }
    );
    db.SaveChanges();
}

app.Run("http://localhost:5000");

class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
