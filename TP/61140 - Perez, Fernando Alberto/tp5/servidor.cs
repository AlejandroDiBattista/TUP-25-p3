#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<StoreContext>();
var app = builder.Build();

// Modelos y DB Context
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Stock { get; set; }
}

public class StoreContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=store.db");
}

// Inicialización
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<StoreContext>();
db.Database.EnsureCreated();
if (!db.Products.Any())
{
    for (int i = 1; i <= 10; i++)
        db.Products.Add(new Product { Name = $"Producto {i}", Stock = 10 });
    db.SaveChanges();
}

// Endpoints
app.MapGet("/products", async (StoreContext db) => await db.Products.ToListAsync());

app.MapGet("/products/replenish", async (StoreContext db) =>
    await db.Products.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/products/{id}/add/{amount:int}", async (StoreContext db, int id, int amount) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();
    product.Stock += amount;
    await db.SaveChangesAsync();
    return Results.Ok(product);
});

app.MapPost("/products/{id}/remove/{amount:int}", async (StoreContext db, int id, int amount) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();
    if (product.Stock < amount) return Results.BadRequest("Stock insuficiente.");
    product.Stock -= amount;
    await db.SaveChangesAsync();
    return Results.Ok(product);
});

app.Run();
