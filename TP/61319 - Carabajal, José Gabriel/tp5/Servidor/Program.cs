using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");

// Configurar EF Core + SQLite
builder.Services.AddDbContext<AppDb>(opts =>
    opts.UseSqlite("Data Source=productos.db"));

var app = builder.Build();

// Seed inicial
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    if (!db.Products.Any())
    {
        for(int i = 1; i <= 10; i++)
        {
            db.Products.Add(new Product {
                Name = $"Producto {i}",
                Stock = 10,
                Price = 10m * i
            });
        }
        db.SaveChanges();
    }
}

// Endpoints
app.MapGet("/products", async (AppDb db) =>
    await db.Products.ToListAsync());

app.MapGet("/products/reorder", async (AppDb db) =>
    await db.Products.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/products/{id:int}/add", async (int id, int quantity, AppDb db) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    p.Stock += quantity;
    await db.SaveChangesAsync();
    return Results.Ok(p);
});

app.MapPost("/products/{id:int}/remove", async (int id, int quantity, AppDb db) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    if (p.Stock - quantity < 0)
        return Results.BadRequest("Stock no puede quedar negativo");
    p.Stock -= quantity;
    await db.SaveChangesAsync();
    return Results.Ok(p);
});

app.Run();

public class Product
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = default!;
    public int Stock { get; set; }
    [Required] public decimal Price { get; set; }
}

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> opts) : base(opts) { }
    public DbSet<Product> Products => Set<Product>();
}
