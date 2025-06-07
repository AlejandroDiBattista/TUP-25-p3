#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.10"

using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();


app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/agregar/{id:int}/{cantidad:int}", async (AppDb db, int id, int cantidad) => {
    var p = await db.Productos.FindAsync(id);
    if (p is null) return Results.NotFound();
    p.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(p);
});

app.MapPost("/productos/quitar/{id:int}/{cantidad:int}", async (AppDb db, int id, int cantidad) => {
    var p = await db.Productos.FindAsync(id);
    if (p is null) return Results.NotFound();
    if (p.Stock < cantidad) return Results.BadRequest("No hay suficiente stock.");
    p.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(p);
});


var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

if (!db.Productos.Any()) {
    var nombres = new[] {
        "Batidora", "Horno eléctrico", "Extractor de jugos", "Cafetera espresso", "Freidora",
        "Aire acondicionado", "Calefactor", "Lámpara LED", "Purificador de aire", "Máquina de coser"
    };

    db.Productos.AddRange(
        nombres.Select((nombre, i) =>
            new Producto {
                Nombre = nombre,
                Precio = 100 + i * 50, 
                Stock = 10
            }
        )
    );
    db.SaveChanges();
}

app.Run("http://localhost:5000");


class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
