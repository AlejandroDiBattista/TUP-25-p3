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

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

app.MapPost("/productos/{id}/agregar-stock/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null)
        return Results.NotFound("Producto no encontrado");
    prod.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.MapPost("/productos/{id}/quitar-stock/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound("Producto no encontrado");
    if (prod.Stock < cantidad) return Results.BadRequest("Stock insuficiente");
    prod.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

using (var scope = app.Services.CreateScope()) {
    var dbScoped = scope.ServiceProvider.GetRequiredService<AppDb>();
    dbScoped.Database.EnsureCreated();

    if (!dbScoped.Productos.Any())
    {
        dbScoped.Productos.AddRange(new[]{
            new Producto { Nombre = "Lapicera", Precio = 50, Stock = 10 },
            new Producto { Nombre = "Cuaderno", Precio = 200, Stock = 10 },
            new Producto { Nombre = "Goma", Precio = 200, Stock = 10 },
            new Producto { Nombre = "Regla", Precio = 60, Stock = 10 },
            new Producto { Nombre = "Mochila", Precio = 1550, Stock = 10 },
            new Producto { Nombre = "Cartuchera", Precio = 400, Stock = 10 },
            new Producto { Nombre = "Tijera", Precio = 100, Stock = 10 },
            new Producto { Nombre = "Marcador", Precio = 80, Stock = 10 },
            new Producto { Nombre = "Pegamento", Precio = 90, Stock = 10 },
            new Producto { Nombre = "Papel", Precio = 10, Stock = 10 },
        });
        dbScoped.SaveChanges();
    }
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