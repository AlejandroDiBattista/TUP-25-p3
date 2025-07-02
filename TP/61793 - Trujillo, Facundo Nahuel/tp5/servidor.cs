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

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        var productosIniciales = Enumerable.Range(1, 10)
            .Select(i => new Producto
            {
                Nombre = $"Articulo {i}",
                Stock = 50
            }).ToList();

        db.Productos.AddRange(productosIniciales);
        db.SaveChanges();
    }
}


app.MapGet("/productos", async (AppDbContext db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDbContext db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());


app.MapPut("/productos/{id}/agregarstock", async (int id, int cantidad, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    if (cantidad <= 0)
        return Results.BadRequest("Cantidad debe ser mayor que 0");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPut("/productos/{id}/quitastock", async (int id, int cantidad, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    if (cantidad <= 0)
        return Results.BadRequest("Cantidad debe ser mayor que 0");

    if (producto.Stock - cantidad < 0)
        return Results.BadRequest("Stock insuficiente");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.Run();

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
}
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
}
