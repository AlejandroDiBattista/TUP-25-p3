#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"


using System.Linq;
using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;


CultureInfo.CurrentCulture = new CultureInfo("es-AR");


var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); 
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());
app.MapGet("/productos/faltantes", async (AppDb db) => await db.Productos.Where(p => p.Stock < 3).ToListAsync());
app.MapPost("/productos/{id}/sumar", async (int id, AppDb db) =>
{
    var prod  = await db.Productos.FindAsync(id);
    if (prod  == null) return Results.NotFound();

    prod.Stock += 1;
    await db.SaveChangesAsync();
    return Results.Ok(prod );
});	

app.MapPost("/productos/{id}/restar", async (int id, AppDb db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod  == null) return Results.NotFound();

    prod.Stock -= 1;
    await db.SaveChangesAsync();
    return Results.Ok(prod );

});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); 


if (!db.Productos.Any()) {
    db.Productos.AddRange(
        new Producto { Nombre = "Café", Precio = 100, Stock = 10 },
    new Producto { Nombre = "Azúcar", Precio = 80, Stock = 10 },
    new Producto { Nombre = "Arroz", Precio = 120, Stock = 10 },
    new Producto { Nombre = "Yerba", Precio = 200, Stock = 10 },
    new Producto { Nombre = "Pan", Precio = 60, Stock = 10 },
    new Producto { Nombre = "Fideos", Precio = 90, Stock = 10 },
    new Producto { Nombre = "Aceite", Precio = 300, Stock = 10 },
    new Producto { Nombre = "Sal", Precio = 50, Stock = 10 },
    new Producto { Nombre = "Harina", Precio = 70, Stock = 10 },
    new Producto { Nombre = "Galletitas", Precio = 150, Stock = 10 }
    );
    
    db.SaveChanges();
    foreach (var p in db.Productos)
    {
        Console.WriteLine($"Producto agregado: {p.Nombre} - ${p.Precio}");
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