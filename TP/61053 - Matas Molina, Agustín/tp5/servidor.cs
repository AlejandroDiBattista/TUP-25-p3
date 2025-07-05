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
            new Producto { Nombre = "Yerba Mate", Precio = 1500, Stock = 10 },
            new Producto { Nombre = "Azúcar", Precio = 800, Stock = 10 },
            new Producto { Nombre = "Café", Precio = 2000, Stock = 10 },
            new Producto { Nombre = "Galletitas", Precio = 600, Stock = 10 },
            new Producto { Nombre = "Leche", Precio = 1200, Stock = 10 },
            new Producto { Nombre = "Harina", Precio = 750, Stock = 10 },
            new Producto { Nombre = "Aceite", Precio = 2500, Stock = 10 },
            new Producto { Nombre = "Sal", Precio = 300, Stock = 10 },
            new Producto { Nombre = "Fideos", Precio = 900, Stock = 10 },
            new Producto { Nombre = "Arroz", Precio = 1000, Stock = 10 },
            new Producto { Nombre = "Pan", Precio = 500, Stock = 10 },
            new Producto { Nombre = "Mermelada", Precio = 1400, Stock = 10 },
            new Producto { Nombre = "Queso", Precio = 2800, Stock =10 },
            new Producto { Nombre = "Huevos", Precio = 1600, Stock = 10 },
            new Producto { Nombre = "Manteca", Precio = 1700, Stock = 10 }

    
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


