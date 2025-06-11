using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt =>
    opt.UseSqlite("Data Source=tienda.db"));

builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Endpoints REST

app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Cargar productos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "cable USB", Precio = 30, Stock = 10 },
            new Producto { Nombre = "Linterna", Precio = 150, Stock = 10 },
            new Producto { Nombre = "Computadora ", Precio = 20, Stock = 10 },
            new Producto { Nombre = "Mochila", Precio = 250, Stock = 10 },
            new Producto { Nombre = "Resaltador", Precio = 80, Stock = 10 },
            new Producto { Nombre = "TV", Precio = 2000, Stock = 10 },
            new Producto { Nombre = "Tijeras", Precio = 100, Stock = 10 },
            new Producto { Nombre = "Placa Mother", Precio = 60, Stock = 10 },
            new Producto { Nombre = "GTA VI", Precio = 120, Stock = 10 },
            new Producto { Nombre = "PS5", Precio = 50, Stock = 10 }
        );
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

// Modelos

class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
