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


app.MapGet("/productos", async (AppDb db) => await db.Productos.OrderBy(p => p.Id).ToListAsync());


app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).OrderBy(p => p.Id).ToListAsync()
);


app.MapGet("/productos/{id:int}", async (int id, AppDb db) =>
    await db.Productos.FindAsync(id) is Producto p ? Results.Ok(p) : Results.NotFound()
);


app.MapPut("/productos/{id:int}", async (int id, Producto prod, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (prod.Stock < 0) return Results.BadRequest("El stock no puede ser negativo.");
    producto.Nombre = prod.Nombre;
    producto.Precio = prod.Precio;
    producto.Stock = prod.Stock;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "iPhone 15", Precio = 1200000, Stock = 10 },
            new Producto { Nombre = "Samsung Galaxy S24", Precio = 1100000, Stock = 10 },
            new Producto { Nombre = "Xiaomi Redmi Note 13", Precio = 450000, Stock = 10 },
            new Producto { Nombre = "Motorola Moto G84", Precio = 380000, Stock = 10 },
            new Producto { Nombre = "OnePlus 12", Precio = 950000, Stock = 10 },
            new Producto { Nombre = "Google Pixel 8", Precio = 1050000, Stock = 10 },
            new Producto { Nombre = "Realme 11 Pro", Precio = 420000, Stock = 10 },
            new Producto { Nombre = "Nokia G42", Precio = 250000, Stock = 10 },
            new Producto { Nombre = "Sony Xperia 5 V", Precio = 980000, Stock = 10 },
            new Producto { Nombre = "Huawei P60 Pro", Precio = 990000, Stock = 10 }
        );
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

