#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/", () => "Servidor de productos en funcionamiento.");
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());
app.MapGet("/productos/reponer", async (AppDb db) => await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar", async (AppDb db, string id, int cantidad) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado.");
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar", async (AppDb db, string id, int cantidad) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado.");
    if (producto.Stock < cantidad) return Results.BadRequest("No hay suficiente stock.");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});
    
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any()) {
        var productosEjemplo = new List<Producto>
        {
            new Producto { Id = "P0001", Nombre = "Heladera", Precio = 320000.00m, Stock = 10 },
            new Producto { Id = "P0002", Nombre = "Lavarropas", Precio = 280000.00m, Stock = 10 },
            new Producto { Id = "P0003", Nombre = "Microondas", Precio = 85000.00m, Stock = 10 },
            new Producto { Id = "P0004", Nombre = "Cocina", Precio = 150000.00m, Stock = 10 },
            new Producto { Id = "P0005", Nombre = "Aire Acondicionado", Precio = 260000.00m, Stock = 10 },
            new Producto { Id = "P0006", Nombre = "Calefactor", Precio = 72000.00m, Stock = 10 },
            new Producto { Id = "P0007", Nombre = "Ventilador", Precio = 25000.00m, Stock = 10 },
            new Producto { Id = "P0008", Nombre = "Televisor", Precio = 300000.00m, Stock = 10 },
            new Producto { Id = "P0009", Nombre = "Licuadora", Precio = 18000.00m, Stock = 10 },
            new Producto { Id = "P0010", Nombre = "Horno Eléctrico", Precio = 65000.00m, Stock = 10 }
        };

        db.Productos.AddRange(productosEjemplo);
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto {
    public string Id { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }

}