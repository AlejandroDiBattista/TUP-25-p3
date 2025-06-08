#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.OrderBy(p => p.Nombre).ToListAsync());

app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/agregar-stock", async (AppDb db, MovimientoStock mov) =>
{
    var prod = await db.Productos.FindAsync(mov.ProductoId);
    if (prod is null) return Results.NotFound("Producto no encontrado");
    prod.Stock += mov.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.MapPost("/productos/quitar-stock", async (AppDb db, MovimientoStock mov) =>
{
    var prod = await db.Productos.FindAsync(mov.ProductoId);
    if (prod is null) return Results.NotFound("Producto no encontrado");
    if (prod.Stock < mov.Cantidad) return Results.BadRequest("Stock insuficiente");
    prod.Stock -= mov.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();
if (!db.Productos.Any()) {
    db.Productos.AddRange(Enumerable.Range(1, 10).Select(i =>
        new Producto {
            Nombre = $"Producto {i}",
            Precio = 10 + i,
            Stock = 10
        }));
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

class MovimientoStock {
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
