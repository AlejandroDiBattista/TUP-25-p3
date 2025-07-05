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

app.MapGet("/productos", async (AppDb db, int id) =>
{
    var prod = await db.Productos.FindAsync(id);
    return prod is not null ? Results.Ok(prod) : Results.NotFound();
});

app.MapPost("/productos", async (AppDb db, Producto p) =>
{
    db.Productos.Add(p);
    await db.SaveChangesAsync();
    return Results.Ok(p);
});

app.MapPut("/productos", async (AppDb db, Producto p) =>
{
    var prod = await db.Productos.FindAsync(p.Id);
    if (prod is null) return Results.NotFound();
    if (!string.IsNullOrEmpty(p.Nombre)) prod.Nombre = p.Nombre;
    if (p.Precio > 0) prod.Precio = p.Precio;
    prod.Stock = p.Stock;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any()) {
        db.Productos.AddRange(
            new Producto { Nombre = "Lapicera",   Precio =  100, Stock = 10 },
            new Producto { Nombre = "Cuaderno",   Precio =  250, Stock = 15 },
            new Producto { Nombre = "Regla",      Precio =   80, Stock = 20 },
            new Producto { Nombre = "Goma",       Precio =   50, Stock = 30 },
            new Producto { Nombre = "Cartuchera", Precio =  400, Stock =  5 },
            new Producto { Nombre = "Mochila",    Precio = 2000, Stock =  3 },
            new Producto { Nombre = "Compás",     Precio =  300, Stock =  8 },
            new Producto { Nombre = "Tijera",     Precio =  120, Stock = 12 },
            new Producto { Nombre = "Pegamento",  Precio =   90, Stock = 25 },
            new Producto { Nombre = "Marcador",   Precio =   60, Stock = 18 }
        );
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
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}