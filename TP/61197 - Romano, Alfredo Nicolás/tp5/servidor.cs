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

SQLitePCL.Batteries_V2.Init();

// CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); 
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build(); 

// ENDPOINTS

// Listar todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Listar productos con stock < 3
app.MapGet("/productos/stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

// Agregar stock
app.MapPut("/productos/agregar/{id}/{cantidad}", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Quitar stock
app.MapPut("/productos/quitar/{id}/{cantidad}", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock - cantidad < 0)
        return Results.BadRequest("El stock no puede ser negativo.");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Inicializar base de datos con productos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        var productos = new[] {
            new Producto { Nombre = "Coca-Cola", Precio = 1200, Stock = 10},
            new Producto { Nombre = "Pepsi", Precio = 1100, Stock = 8},
            new Producto { Nombre = "Fanta", Precio = 1150, Stock = 7},
            new Producto { Nombre = "Sprite", Precio = 1150, Stock = 5},
            new Producto { Nombre = "Fernet Branca", Precio = 10000, Stock = 3},
            new Producto { Nombre = "Heineken", Precio = 4000, Stock = 6},
            new Producto { Nombre = "Mirinda", Precio = 1000, Stock = 12},
            new Producto { Nombre = "Agua mineral", Precio = 1000, Stock = 4},
            new Producto { Nombre = "Gaseosa Manaos", Precio = 800, Stock = 15},
            new Producto { Nombre = "Cigarrillos Malboro", Precio = 3500, Stock = 20}
        };

        db.Productos.AddRange(productos);
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

// Modelo de datos

// DB Context y Modelo
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
// Fin del código