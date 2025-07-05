#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => 
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
var app = builder.Build();

app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPut("/productos/{id}/agregar/{cantidad}", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPut("/productos/{id}/quitar/{cantidad}", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDb>();
db.Database.EnsureCreated();

if (!db.Productos.Any()) {
    db.Productos.AddRange(
        new Producto { Nombre = "Arroz", Precio = 150, Stock = 10 },
        new Producto { Nombre = "Fideos", Precio = 100, Stock = 10 },
        new Producto { Nombre = "Leche", Precio = 250, Stock = 10 },
        new Producto { Nombre = "Yerba", Precio = 400, Stock = 10 },
        new Producto { Nombre = "Galletas", Precio = 120, Stock = 10 },
        new Producto { Nombre = "Azúcar", Precio = 130, Stock = 10 },
        new Producto { Nombre = "Café", Precio = 500, Stock = 10 },
        new Producto { Nombre = "Aceite", Precio = 700, Stock = 10 },
        new Producto { Nombre = "Sal", Precio = 80, Stock = 10 },
        new Producto { Nombre = "Harina", Precio = 90, Stock = 10 }
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
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
