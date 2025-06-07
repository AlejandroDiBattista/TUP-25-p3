using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/agregar-stock/{id:int}/{cantidad:int}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/quitar-stock/{id:int}/{cantidad:int}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");
    if (producto.Stock < cantidad) return Results.BadRequest("No hay suficiente stock");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

if (!db.Productos.Any())
{
    db.Productos.AddRange(
        new Producto { Nombre = "Lápiz", Precio = 100, Stock = 10 },
        new Producto { Nombre = "Cuaderno", Precio = 500, Stock = 10 },
        new Producto { Nombre = "Goma", Precio = 80, Stock = 10 },
        new Producto { Nombre = "Regla", Precio = 200, Stock = 10 },
        new Producto { Nombre = "Cartuchera", Precio = 800, Stock = 10 },
        new Producto { Nombre = "Mochila", Precio = 5000, Stock = 10 },
        new Producto { Nombre = "Resaltador", Precio = 150, Stock = 10 },
        new Producto { Nombre = "Compás", Precio = 600, Stock = 10 },
        new Producto { Nombre = "Lapicera", Precio = 120, Stock = 10 },
        new Producto { Nombre = "Pegamento", Precio = 90, Stock = 10 }
    );
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