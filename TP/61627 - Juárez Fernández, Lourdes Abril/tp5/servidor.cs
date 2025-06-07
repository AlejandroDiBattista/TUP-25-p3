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

#r "nuget: Microsoft.EntityFrameworkCore.Sqlite"
#r "nuget: Microsoft.AspNetCore.App"

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<StockContext>(options => options.UseSqlite("Data Source=stock.db"));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StockContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            db.Products.Add(new Product { Name = $"Producto {i}", Stock = 10 });
        }
        db.SaveChanges();
    }
}

app.MapGet("/productos", async (StockContext db) => await db.Products.ToListAsync());

app.MapGet("/productos/reponer", async (StockContext db) =>
    await db.Products.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, StockContext db) =>
{
    var producto = await db.Products.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, StockContext db) =>
{
    var producto = await db.Products.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.Run();

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Stock { get; set; }
}

public class StockContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public StockContext(DbContextOptions<StockContext> options) : base(options) { }
}
