#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(options => options.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(options => {
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Rutas de la API
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reposicion", async (AppDb db) => {
    return await db.Productos.Where(p => p.Stock < 3).ToListAsync();
});

app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, AppDb db) => {
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound();
    prod.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, AppDb db) => {
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound();
    if (prod.Stock < cantidad) return Results.BadRequest("No hay suficiente stock");
    prod.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Crear base de datos y cargar datos si es la primera vez
var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();
if (!db.Productos.Any()) {
    db.Productos.Add(new Producto { Nombre = "Lapicera", Precio = 100, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Cuaderno", Precio = 500, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Regla", Precio = 200, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Goma", Precio = 50, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Cartuchera", Precio = 1500, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Resaltador", Precio = 300, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Compás", Precio = 400, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Tijera", Precio = 250, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Pegamento", Precio = 120, Stock = 10 });
    db.Productos.Add(new Producto { Nombre = "Corrector", Precio = 350, Stock = 10 });
    db.SaveChanges();
}

app.Run("http://localhost:5000");

// Modelo
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> opt) : base(opt) {}
    public DbSet<Producto> Productos => Set<Producto>();
}
