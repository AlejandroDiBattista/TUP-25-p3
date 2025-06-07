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

// CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// ENDPOINTS

app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());


app.MapPost("/productos/agregar-stock", async (AppDb db, ActualizarStockDto data) =>
{
    var producto = await db.Productos.FindAsync(data.Id);
    if (producto == null)
        return Results.NotFound($"No existe el producto con ID {data.Id}");
    producto.Stock += data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok($"Se agregaron {data.Cantidad} unidades al producto {producto.Nombre}");
});

// Quitar stock
app.MapPost("/productos/quitar-stock", async (AppDb db, ActualizarStockDto data) =>
{
    var producto = await db.Productos.FindAsync(data.Id);
    if (producto == null)
        return Results.NotFound($"No existe el producto con ID {data.Id}");
    if (producto.Stock < data.Cantidad)
        return Results.BadRequest($"No hay suficiente stock para quitar {data.Cantidad} unidades del producto {producto.Nombre}");
    producto.Stock -= data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok($"Se quitaron {data.Cantidad} unidades del producto {producto.Nombre}");
});

// CREAR BASE Y AGREGAR PRODUCTOS DE EJEMPLO
var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

if (!db.Productos.Any())
{
    var ejemplo = Enumerable.Range(1, 10).Select(i => new Producto
    {
        Nombre = $"Producto {i}",
        Precio = i * 10,
        Stock = 10
    });
    db.Productos.AddRange(ejemplo);
    db.SaveChanges();
}

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

class ActualizarStockDto
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
}
