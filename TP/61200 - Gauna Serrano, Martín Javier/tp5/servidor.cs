#r "nuget: Microsoft.EntityFrameworkCore.Sqlite"
#r "nuget: Microsoft.AspNetCore.App"
#r "nuget: Microsoft.AspNetCore.Routing"

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// Crear DB y poblar datos si está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            db.Productos.Add(new Producto { Nombre = $"Producto {i}", Stock = 10 });
        }
        db.SaveChanges();
    }
}

// ENDPOINTS

// Listar todos los productos
app.MapGet("/productos", async (TiendaDbContext db) =>
    await db.Productos.ToListAsync());

// Listar productos con stock < 3
app.MapGet("/productos/reponer", async (TiendaDbContext db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Agregar stock
app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, TiendaDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Quitar stock
app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, TiendaDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("Producto no encontrado");
    if (producto.Stock - cantidad < 0)
        return Results.BadRequest("No se puede dejar stock negativo");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.Run();

// ----------------------
// CLASES Y DB CONTEXT
// ----------------------

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
}

class TiendaDbContext : DbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
}
