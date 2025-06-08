#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: System.Diagnostics.DiagnosticSource, 8.0.0"
#r "nuget: Microsoft.EntityFrameworkCore, 8.0.0"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 8.0.0"

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


// Todos Los Productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Listado de productos que necesitan reposición (stock < 3)
app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

// Agregar el stock a un producto
app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
        return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok($"Se agregaron {cantidad} unidades al producto '{producto.Nombre}'. Nuevo stock: {producto.Stock}");
});

// Quita el stock a un producto
app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad)
        return Results.BadRequest("No hay suficiente stock para quitar esa cantidad.");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok($"Se quitaron {cantidad} unidades del producto '{producto.Nombre}'. Nuevo stock: {producto.Stock}");
});

// Crear la BD y datos iniciales
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        var productosEjemplo = new List<Producto>
        {
            new Producto { Nombre = "Lapicera", Precio = 50, Stock = 10 },
            new Producto { Nombre = "Cuaderno", Precio = 200, Stock = 10 },
            new Producto { Nombre = "Regla", Precio = 80, Stock = 10 },
            new Producto { Nombre = "Goma", Precio = 30, Stock = 10 },
            new Producto { Nombre = "Cartuchera", Precio = 300, Stock = 10 },
            new Producto { Nombre = "Lápiz", Precio = 40, Stock = 10 },
            new Producto { Nombre = "Tijera", Precio = 150, Stock = 10 },
            new Producto { Nombre = "Pegamento", Precio = 90, Stock = 10 },
            new Producto { Nombre = "Marcador", Precio = 120, Stock = 10 },
            new Producto { Nombre = "Corrector", Precio = 60, Stock = 10 }
        };

        db.Productos.AddRange(productosEjemplo);
        db.SaveChanges();
        Console.WriteLine("✅ Ejemplos de Productos.");
    }
}

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