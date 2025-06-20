// Ensure the required dependencies are included in your project file (.csproj).

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

// ENDPOINTS EXISTENTES
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// NUEVOS ENDPOINTS

// Listar productos con stock bajo (<3)
app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Sumar stock
app.MapPut("/productos/{id}/sumar", async (AppDb db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Restar stock (no dejar en negativo)
app.MapPut("/productos/{id}/restar", async (AppDb db, int id, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock < cantidad)
        return Results.BadRequest("No se puede restar más stock del disponible.");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// CREAR BD Y AGREGAR PRODUCTOS
var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

// Agregar productos de ejemplo solo si no existen
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
    public int Stock { get; set; } // Agregado para manejar el stock
}
