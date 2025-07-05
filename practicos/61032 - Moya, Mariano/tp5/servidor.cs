#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"
#r "nuget: Microsoft.Data.Sqlite, 9.0.4"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.10"

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;

Batteries_V2.Init();

// CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

app.MapPost("/productos/{id}/agregar-stock", async (int id, StockDto dto, AppDb db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound();
    prod.Stock += dto.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.MapPost("/productos/{id}/quitar-stock", async (int id, StockDto dto, AppDb db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound();
    if (prod.Stock < dto.Cantidad) return Results.BadRequest(new { error = "Stock insuficiente" });
    prod.Stock -= dto.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

// Agregar productos de ejemplo al crear la base de datos
if (!db.Productos.Any())
{
    db.Productos.AddRange(
        new Producto { Nombre = "Resma de hojas", Precio = 2500, Stock = 15 },
        new Producto { Nombre = "Bolígrafo azul", Precio = 120, Stock = 30 },
        new Producto { Nombre = "Lápiz negro", Precio = 90, Stock = 25 },
        new Producto { Nombre = "Cuaderno universitario", Precio = 800, Stock = 20 },
        new Producto { Nombre = "Corrector líquido", Precio = 350, Stock = 12 },
        new Producto { Nombre = "Carpeta A4", Precio = 600, Stock = 18 },
        new Producto { Nombre = "Papel glasé", Precio = 200, Stock = 40 },
        new Producto { Nombre = "Sacapuntas", Precio = 75, Stock = 22 },
        new Producto { Nombre = "Fibra color", Precio = 220, Stock = 16 },
        new Producto { Nombre = "Block de dibujo", Precio = 950, Stock = 10 }
    );
    db.SaveChanges();
}

app.Run("http://localhost:5000");

// Modelo de datos
class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

class StockDto
{
    public int Cantidad { get; set; }
}