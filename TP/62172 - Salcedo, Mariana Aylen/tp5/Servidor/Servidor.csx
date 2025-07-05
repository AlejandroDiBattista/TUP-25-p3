#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.7"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 8.0.0"
#r "nuget: Microsoft.EntityFrameworkCore.InMemory, 8.0.0"
#r "nuget: Microsoft.EntityFrameworkCore, 8.0.0"
#r "nuget: Microsoft.AspNetCore.App, 8.0.0"
#r "nuget: Microsoft.AspNetCore.OpenApi, 8.0.0"

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<StockContext>(options =>
    options.UseSqlite("Data Source=stock.db"));

var app = builder.Build();

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
}

public class StockContext : DbContext
{
    public StockContext(DbContextOptions<StockContext> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StockContext>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "Heladera", Stock = 10 },
            new Producto { Nombre = "Lavarropas", Stock = 10 },
            new Producto { Nombre = "Microondas", Stock = 10 },
            new Producto { Nombre = "Aire Acondicionado", Stock = 10 },
            new Producto { Nombre = "Televisor", Stock = 10 },
            new Producto { Nombre = "Cafetera", Stock = 10 },
            new Producto { Nombre = "Tostadora", Stock = 10 },
            new Producto { Nombre = "Licuadora", Stock = 10 },
            new Producto { Nombre = "Plancha", Stock = 10 },
            new Producto { Nombre = "Ventilador", Stock = 10 }
        };
        db.Productos.AddRange(productos);
        db.SaveChanges();
    }
}

app.MapGet("/productos", async (StockContext db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (StockContext db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, StockContext db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound();
    prod.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, StockContext db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound();
    if (prod.Stock - cantidad < 0) return Results.BadRequest("Stock insuficiente");
    prod.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.Run("http://localhost:5000");
