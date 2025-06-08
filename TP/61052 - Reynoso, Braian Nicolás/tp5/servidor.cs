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


var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();




app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.ToListAsync());


app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());


app.MapPost("/productos/agregar", async (AppDb db, StockUpdate data) =>
{
    var producto = await db.Productos.FindAsync(data.Id);
    if (producto is null)
        return Results.NotFound($"Producto con ID {data.Id} no encontrado.");

    producto.Stock += data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


app.MapPost("/productos/quitar", async (AppDb db, StockUpdate data) =>
{
    var producto = await db.Productos.FindAsync(data.Id);
    if (producto is null)
        return Results.NotFound($"Producto con ID {data.Id} no encontrado.");

    if (producto.Stock < data.Cantidad)
        return Results.BadRequest("No se puede quitar más stock del disponible.");

    producto.Stock -= data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Id = 1, Nombre = "Botella de agua", Stock = 10 },
            new Producto { Id = 2, Nombre = "Aceite", Stock = 10 },
            new Producto { Id = 3, Nombre = "Gaseosa", Stock = 10 },
            new Producto { Id = 4, Nombre = "Fideos", Stock = 10 },
            new Producto { Id = 5, Nombre = "Arroz", Stock = 10 },
            new Producto { Id = 6, Nombre = "Harina", Stock = 10 },
            new Producto { Id = 7, Nombre = "Yerba mate", Stock = 10 },
            new Producto { Id = 8, Nombre = "Azúcar", Stock = 10 },
            new Producto { Id = 9, Nombre = "Sal fina", Stock = 10 },
            new Producto { Id = 10, Nombre = "Leche", Stock = 10 },
        });

        db.SaveChanges();
    }
}

app.Run("http://localhost:5000"); 



class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int Stock { get; set; } 
}

class StockUpdate
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
}
