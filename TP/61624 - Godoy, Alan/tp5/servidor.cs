#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

app.MapPost("/productos/agregar", async (AppDb db, ProductoCambio cambio) => {
    var producto = await db.Productos.FindAsync(cambio.Id);
    if (producto is null) return Results.NotFound("Producto no encontrado");
    producto.Stock += cambio.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/quitar", async (AppDb db, ProductoCambio cambio) => {
    var producto = await db.Productos.FindAsync(cambio.Id);
    if (producto is null) return Results.NotFound("Producto no encontrado");
    if (producto.Stock < cambio.Cantidad)
        return Results.BadRequest("No hay suficiente stock");
    producto.Stock -= cambio.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDb>();
db.Database.EnsureCreated();
if (!db.Productos.Any()) {
    for (int i = 1; i <= 10; i++) {
        db.Productos.Add(new Producto {
            Nombre = $"Producto {i}",
            Precio = 10 + i,
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
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

class ProductoCambio {
    public int Id { get; set; }
    public int Cantidad { get; set; }
}
