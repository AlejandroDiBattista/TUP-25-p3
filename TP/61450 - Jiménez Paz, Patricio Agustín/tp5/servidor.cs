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

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Obtener productos cuyo stock es menor a 3
app.MapGet("/productos/stockCritico", async (AppDb db) => await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Obtener un producto por id
app.MapGet("/productos/{id}", async (int id, AppDb db) => await db.Productos.FindAsync(id) is Producto p ? Results.Ok(p) : Results.NotFound());

// Crear un producto
app.MapPost("/productos", async (Producto p, AppDb db) => {
    try
    {
        if (p.Stock < 0) return Results.BadRequest("El stock no puede ser negativo");
        if (p.Precio < 0) return Results.BadRequest("El precio no puede ser negativo");
        using var tran = await db.Database.BeginTransactionAsync();

        db.Productos.Add(p);
        await db.SaveChangesAsync();
        await tran.CommitAsync();
        return Results.Created($"/productos/{p.Id}", p);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Actualizar un producto
app.MapPut("/productos/{id}", async (int id, Producto p, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    using var tran = await db.Database.BeginTransactionAsync();
    try
    {
        if (p.Stock < 0) return Results.BadRequest("El stock no puede ser negativo");
        if (p.Precio < 0) return Results.BadRequest("El precio no puede ser negativo");

        producto.Nombre = p.Nombre;
        producto.Precio = p.Precio;
        producto.Stock = p.Stock;

        await db.SaveChangesAsync();
        await tran.CommitAsync();
        return Results.Ok(producto);
    }
    catch (Exception ex)
    {
        await tran.RollbackAsync();
        return Results.Problem(ex.Message);
    }
});

// Actualizar el stock de un producto, tanto para aumentar como para disminuir
// Se espera un JSON con la cantidad de stock a modificar
app.MapPut("/productos/{id}/stock", async (int id, StockDto stockDto, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    using var tran = await db.Database.BeginTransactionAsync();
    try
    {
        if (stockDto.Stock > 0)
        {
            producto.Stock += stockDto.Stock;
        }
        else
        {
            if (producto.Stock + stockDto.Stock < 0) return Results.BadRequest("No se puede reducir el stock a un valor negativo");
            producto.Stock += stockDto.Stock;
        }
        await db.SaveChangesAsync();
        await tran.CommitAsync();
        return Results.Ok(producto);
    }
    catch (Exception ex)
    {
        await tran.RollbackAsync();
        return Results.Problem(ex.Message);
    }
});

// Eliminar un producto
app.MapDelete("/productos/{id}", async (int id, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    using var tran = await db.Database.BeginTransactionAsync();
    try
    {
        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        await tran.CommitAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        await tran.RollbackAsync();
        return Results.Problem(ex.Message);
    }
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos
if (!db.Productos.Any())
{
    db.Productos.AddRange(new List<Producto>
    {
        new Producto { Nombre = "Producto 1", Precio = 10.0m, Stock = 10 },
        new Producto { Nombre = "Producto 2", Precio = 20.0m, Stock = 10 },
        new Producto { Nombre = "Producto 3", Precio = 30.0m, Stock = 10 },
        new Producto { Nombre = "Producto 4", Precio = 40.0m, Stock = 10 },
        new Producto { Nombre = "Producto 5", Precio = 50.0m, Stock = 10 },
        new Producto { Nombre = "Producto 6", Precio = 60.0m, Stock = 10 },
        new Producto { Nombre = "Producto 7", Precio = 70.0m, Stock = 10 },
        new Producto { Nombre = "Producto 8", Precio = 80.0m, Stock = 10 },
        new Producto { Nombre = "Producto 9", Precio = 90.0m, Stock = 10 },
        new Producto { Nombre = "Producto 10", Precio = 100.0m, Stock = 10 },
    });
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
    public int Stock { get; set; } = 0;
}

record StockDto(int Stock); // DTO para actualizar el stock
