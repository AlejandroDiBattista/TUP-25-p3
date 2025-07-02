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
using System.Linq;

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated(); 

    // Si no hay productos, se agregan los de ejemplo.
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Remera de Entrenamiento", Precio = 45000.00m, Stock = 15 },
            new Producto { Nombre = "Zapatillas de Running", Precio = 120000.50m, Stock = 8 },
            new Producto { Nombre = "Calzas Deportivas", Precio = 75000.00m, Stock = 12 },
            new Producto { Nombre = "Buzo con Capucha", Precio = 98000.75m, Stock = 2 }, // Stock bajo
            new Producto { Nombre = "Shorts de Fútbol", Precio = 55000.00m, Stock = 20 },
            new Producto { Nombre = "Medias Deportivas (Par)", Precio = 15000.00m, Stock = 1 }  // Stock bajo
        );
        db.SaveChanges();
    }
}




app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());


app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());


app.MapPut("/productos/{id}/agregar/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
    {
        return Results.NotFound(new { message = $"Producto con ID {id} no encontrado." });
    }

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


app.MapPut("/productos/{id}/quitar/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
    {
        return Results.NotFound(new { message = $"Producto con ID {id} no encontrado." });
    }

    if (producto.Stock < cantidad)
    {
        return Results.BadRequest(new { message = $"No se puede quitar {cantidad} unidades. Stock insuficiente ({producto.Stock} disponibles)." });
    }

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

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
    public int Stock { get; set; } // Propiedad requerida por la consigna
}