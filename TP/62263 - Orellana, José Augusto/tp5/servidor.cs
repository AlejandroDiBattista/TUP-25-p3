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

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Endpoint para reponer productos menores a 3
app.MapGet("/productos/stock-bajo", async (AppDb db) =>
{
    var productos = await db.Productos
        .Where(p => p.Stock < 3)
        .ToListAsync();
    return productos;
});

// Endpoint para agregar stock a un producto
app.MapPut("/productos/{id}/agregar-stock", async (int id, ModificarStock datos, AppDb db) =>
{
    if (datos.Cantidad <= 0) return Results.BadRequest("La cantidad a aumentar debe ser mayor cero");

    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    producto.Stock += datos.Cantidad;
    await db.SaveChangesAsync();

    return Results.Ok(producto);
});

// Endpoint para eliminar stock de un producto
app.MapPut("/productos/{id}/quitar", async (int id, ModificarStock datos, AppDb db) =>
{
    if (datos.Cantidad <= 0) return Results.BadRequest("La cantidad a quitar debe ser mayor cero");

    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado");
    if (producto.Stock < datos.Cantidad) return Results.BadRequest("No se puede eliminar más stock del que hay disponible");

    producto.Stock -= datos.Cantidad;
    await db.SaveChangesAsync();

    return Results.Ok(producto);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos
if (!db.Productos.Any()) {
    var productosEjemplo = new[] {
        new Producto { Nombre = "Producto 1", Precio = 10000, Stock = 10 },
        new Producto { Nombre = "Producto 2", Precio = 20000, Stock = 10 },
        new Producto { Nombre = "Producto 3", Precio = 30000, Stock = 10 },
        new Producto { Nombre = "Producto 4", Precio = 40000, Stock = 10 },
        new Producto { Nombre = "Producto 5", Precio = 50000, Stock = 10 },
        new Producto { Nombre = "Producto 6", Precio = 60000, Stock = 10 },
        new Producto { Nombre = "Producto 7", Precio = 70000, Stock = 10 },
        new Producto { Nombre = "Producto 8", Precio = 80000, Stock = 10 },
        new Producto { Nombre = "Producto 9", Precio = 90000, Stock = 10 },
        new Producto { Nombre = "Producto 10", Precio = 10000, Stock = 10 }
    };
    db.Productos.AddRange(productosEjemplo);
    await db.SaveChangesAsync();
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

class ModificarStock {
    public int Cantidad { get; set; }
}