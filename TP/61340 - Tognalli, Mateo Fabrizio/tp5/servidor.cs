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

// Endpoints API REST
// 1. Listar todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// 2. Listar productos con stock bajo (menor a 3 unidades)
app.MapGet("/productos/reponer", async (AppDb db) => 
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// 3. Agregar stock a un producto
app.MapPut("/productos/{id}/agregar-stock", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound($"Producto con ID {id} no encontrado");
    
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    
    return Results.Ok(producto);
});

// 4. Quitar stock a un producto
app.MapPut("/productos/{id}/quitar-stock", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound($"Producto con ID {id} no encontrado");
    
    if (producto.Stock < cantidad)
        return Results.BadRequest($"Stock insuficiente. Stock actual: {producto.Stock}");
        
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    
    return Results.Ok(producto);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe

// Verificar si hay productos en la base de datos
if (!db.Productos.Any())
{    // Agregar 10 productos de ejemplo con stock inicial de 10 unidades    
    var productos = new List<Producto>
    {
        new Producto { Nombre = "Zapatillas deportivas", Precio = 40000, Stock = 10 },
        new Producto { Nombre = "Remera algodón", Precio = 10000, Stock = 10 },
        new Producto { Nombre = "Pantalón jean", Precio = 25000, Stock = 10 },
        new Producto { Nombre = "Campera invierno", Precio = 60000, Stock = 10 },
        new Producto { Nombre = "Medias pack x3", Precio = 4000, Stock = 10 },
        new Producto { Nombre = "Gorra", Precio = 8000, Stock = 10 },
        new Producto { Nombre = "Bufanda", Precio = 6000, Stock = 10 },
        new Producto { Nombre = "Buzo Algodón", Precio = 29000, Stock = 10 },
        new Producto { Nombre = "Shorts deportivos", Precio = 13000, Stock = 10 },
        new Producto { Nombre = "Zapatillas casual", Precio = 35000, Stock = 10 }
    };
    
    db.Productos.AddRange(productos);
    db.SaveChanges();
    Console.WriteLine("Base de datos poblada con 10 productos");
}

Console.WriteLine("Servidor iniciado en http://localhost:5000");
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
    public int Precio { get; set; }
    public int Stock { get; set; }
}