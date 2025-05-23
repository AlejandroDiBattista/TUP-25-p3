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
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); 
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Listar productos con stock menor a 3
app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Agregar stock
app.MapPost("/productos/{id}/agregar", async (int id, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    producto.Stock++;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Quitar stock
app.MapPost("/productos/{id}/quitar", async (int id, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    if (producto.Stock == 0)
        return Results.BadRequest("No se puede dejar el stock en negativo.");

    producto.Stock--;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); 

if (!db.Productos.Any()) {
    for (int i = 1; i <= 10; i++) {
        db.Productos.Add(new Producto { Nombre = $"Producto {i}", Stock = 10 });
    }
    db.SaveChanges();
}


app.Run("http://localhost:5000"); 


// Modelo de datos
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int Stock { get; set; }  
}