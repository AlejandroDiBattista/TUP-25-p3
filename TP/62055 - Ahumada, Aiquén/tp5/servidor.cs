#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.0.7"

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

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

   
    if (!db.Productos.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            db.Productos.Add(new Producto
            {
                Nombre = $"Producto {i}",
                Precio = 10m * i,  
                Stock = 10
            });
        }
        db.SaveChanges();
    }
}
app.MapGet("/productos", async (AppDb db) =>
{
    var lista = await db.Productos.ToListAsync();
    return Results.Ok(lista);
});
app.MapGet("/productos/reponer", async (AppDb db) =>
{
    var bajos = await db.Productos
        .Where(p => p.Stock < 3)
        .ToListAsync();
    return Results.Ok(bajos);
});
app.MapPost("/productos/{id:int}/agregar-stock", async (int id, CantidadDTO dto, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound(new { mensaje = $"Producto con ID {id} no existe." });

    if (dto.Cantidad <= 0)
        return Results.BadRequest(new { mensaje = "La cantidad a agregar debe ser mayor que cero." });

    producto.Stock += dto.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});
app.MapPost("/productos/{id:int}/quitar-stock", async (int id, CantidadDTO dto, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound(new { mensaje = $"Producto con ID {id} no existe." });

    if (dto.Cantidad <= 0)
        return Results.BadRequest(new { mensaje = "La cantidad a quitar debe ser mayor que cero." });

    if (producto.Stock - dto.Cantidad < 0)
        return Results.BadRequest(new { mensaje = "Operación inválida: stock insuficiente." });

    producto.Stock -= dto.Cantidad;
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
public class CantidadDTO
{
    public int Cantidad { get; set; }
}