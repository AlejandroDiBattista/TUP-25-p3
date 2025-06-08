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
                                        //ENDPOINTS
// TRAER PRODUCTOS

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// TRAER PRODUCTOS CON STOCK BAJO

app.MapGet("/productos/bajo-stock", async (AppDb db) => await db.Productos.Where(p => p.Stock < 3).ToListAsync());

//AGREGAR STOCK

app.MapPost("/productos/{id}/agregar/{cantidad}", async (AppDb db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);

});

//QUITAR STOCK

app.MapPost("/productos/{id}/quitar/{cantidad}", async (AppDb db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos

if (!db.Productos.Any())
{
    for (int i = 1; i <= 10; i++)
    {
        db.Productos.Add(new Producto
        {
            Nombre = $"Producto {i}",
            Precio = 100 + i * 10,
            Stock = 10
        });
    }
    db.SaveChanges();
    Console.WriteLine("Base de datos inicializada con productos de ejemplo.");
}

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
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