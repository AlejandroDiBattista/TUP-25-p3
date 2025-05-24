#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Ruta para listar todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Ruta para listar productos con stock menor a 3
app.MapGet("/productos/faltantes", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Ruta para agregar stock a un producto
app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado");
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Ruta para quitar stock a un producto
app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado");
    if (producto.Stock - cantidad < 0)
        return Results.BadRequest("No se puede dejar stock negativo");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Crear base de datos y cargar 10 productos iniciales con stock 10 si no existen
var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDb>();
db.Database.EnsureCreated();
if (!db.Productos.Any()) {
    for (int i = 1; i <= 10; i++) {
        db.Productos.Add(new Producto {
            Nombre = $"Producto {i}",
            Precio = 100 + i * 10,
            Stock = 10
        });
    }
    db.SaveChanges();
}

app.Run("http://localhost:5000");

class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
