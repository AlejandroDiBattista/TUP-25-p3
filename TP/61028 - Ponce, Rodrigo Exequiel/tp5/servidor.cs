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
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// ENDPOINTS

app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.OrderBy(p => p.Nombre).ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).OrderBy(p => p.Nombre).ToListAsync());

app.MapPut("/productos/agregar-stock/{id:int}", async (AppDb db, int id, int cantidad) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound("Producto no encontrado");
    prod.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.MapPut("/productos/quitar-stock/{id:int}", async (AppDb db, int id, int cantidad) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound("Producto no encontrado");
    if (prod.Stock < cantidad) return Results.BadRequest("No hay suficiente stock");
    prod.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
}); 

// INICIALIZAR BD CON PRODUCTOS DE EJEMPLO
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any()) {
        db.Productos.AddRange(
            new Producto { Nombre = "Leche", Precio = 250, Stock = 10 },
            new Producto { Nombre = "Pan", Precio = 150, Stock = 10 },
            new Producto { Nombre = "Queso", Precio = 600, Stock = 10 },
            new Producto { Nombre = "Café", Precio = 950, Stock = 10 },
            new Producto { Nombre = "Yerba", Precio = 750, Stock = 10 },
            new Producto { Nombre = "Azúcar", Precio = 300, Stock = 10 },
            new Producto { Nombre = "Arroz", Precio = 400, Stock = 10 },
            new Producto { Nombre = "Fideos", Precio = 350, Stock = 10 },
            new Producto { Nombre = "Aceite", Precio = 1100, Stock = 10 },
            new Producto { Nombre = "Harina", Precio = 280, Stock = 10 }
        );
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

// CLASES

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
