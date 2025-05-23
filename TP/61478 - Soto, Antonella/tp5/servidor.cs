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

using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

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
                Nombre = $"Producto {i}",
                Stock = 10
            }).ToList();

        db.Productos.AddRange(productosIniciales);
        db.SaveChanges();
    }
}

// Listar todos los productos
app.MapGet("/productos", async (AppDbContext db) =>
    await db.Productos.ToListAsync());

// Listar productos a reponer (stock < 3)
app.MapGet("/productos/reponer", async (AppDbContext db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Agregar stock a producto
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

// Quitar stock a producto (validar no negativo)
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

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
}