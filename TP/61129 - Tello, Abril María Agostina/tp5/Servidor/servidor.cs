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

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); // agregar servicios : Instalar EF Core y SQLite
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated(); // crear BD si no existe

    if (!db.Productos.Any()) {
        var productosIniciales = new List<Producto> {
            new() { Nombre = "Mouse", Precio = 10.0m, Stock = 5 },
            new() { Nombre = "Teclado", Precio = 20.0m, Stock = 2 },
            new() { Nombre = "Monitor", Precio = 100.0m, Stock = 0 },
            new() { Nombre = "CPU", Precio = 500.0m, Stock = 1 },
            new() { Nombre = "Impresora", Precio = 150.0m, Stock = 0 },
            new() { Nombre = "Escaner", Precio = 80.0m, Stock = 3 },
            new() { Nombre = "Proyector", Precio = 200.0m, Stock = 0 },
            new() { Nombre = "Parlantes", Precio = 50.0m, Stock = 4 },
            new() { Nombre = "Webcam", Precio = 30.0m, Stock = 2 },
            new() { Nombre = "Microfono", Precio = 25.0m, Stock = 1 }
        };
        db.Productos.AddRange(productosIniciales);
        db.SaveChanges();
    }
}
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) => 
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPut("/productos/{id}/agregar-stock/{cantidad}", async (int id, int cantidad, AppDb db) => 
{
    var producto = await db.Productos.FindAsync(id);

    if(producto == null) {
        return Results.NotFound("Producto no encontrado")
    }
    if (cantidad < 0) {
        return Results.BadRequest("No se puede agregar un stock negativo");
    }
        producto.Stock += cantidad;
        await db.SaveChangesAsync();

        return Results.Ok(producto);
    
});

app.MapPut("/productos/{id}/quitar-stock/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) {
        return Results.NotFound("Producto no encontrado");
    }
    if (cantidad <= 0) {
        return Results.BadRequest("La cantidad a quitar debe ser mayor a 0");
    }
    if (producto.Stock - cantidad < 0) {
        return Results.BadRequest("No hay suficiente stock para quitar");
    }

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();

    return Results.Ok(producto);
});


app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}