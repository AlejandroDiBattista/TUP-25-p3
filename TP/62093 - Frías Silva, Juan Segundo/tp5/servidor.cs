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

// CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt =>
    opt.UseSqlite("Data Source=./stock.db")); // base de datos nueva
builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// ENDPOINTS

// Listar todos los productos
app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.OrderBy(p => p.Id).ToListAsync());

// Listar productos con stock < 3
app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Agregar stock
app.MapPost("/productos/{id}/agregar-stock", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Quitar stock (sin dejarlo negativo)
app.MapPost("/productos/{id}/quitar-stock", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad)
        return Results.BadRequest("No hay suficiente stock para quitar");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// SEEDING DE DATOS
var scope = app.Services.CreateScope();
var scopedDb = scope.ServiceProvider.GetRequiredService<AppDb>();
scopedDb.Database.EnsureCreated();

if (!scopedDb.Productos.Any())
{
   var productosIniciales = new List<Producto>
    {
        new Producto { Nombre = "mouse inalambrico",        Precio = 50000, Stock = 10 },
        new Producto { Nombre = "teclado",   Precio = 30000, Stock = 10 },
        new Producto { Nombre = "mouse pad",        Precio = 10000, Stock = 10 },
        new Producto { Nombre = "auriculared",Precio = 15000, Stock = 10 },
        new Producto { Nombre = "monitor",  Precio = 70000, Stock = 10 },
        new Producto { Nombre = "procesador",       Precio = 90000, Stock = 10 },
        new Producto { Nombre = "motherboard",       Precio = 60000, Stock = 10 },
        new Producto { Nombre = "placa de video",     Precio = 80000, Stock = 10 },
        new Producto { Nombre = "fuente", Precio = 30000, Stock = 10 },
        new Producto { Nombre = "memorias ram",             Precio = 10000, Stock = 10 }
    };


    scopedDb.Productos.AddRange(productosIniciales);
    scopedDb.SaveChanges();
    Console.WriteLine(" Productos agregados.");
}


app.Run("http://localhost:5000");

// MODELOS

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