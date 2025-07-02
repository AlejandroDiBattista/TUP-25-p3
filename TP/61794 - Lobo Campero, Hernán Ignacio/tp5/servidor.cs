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

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Listar productos a reponer (stock < 3)
app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

// Agregar stock
app.MapPost("/productos/{id}/agregar", async (int id, HttpRequest req, AppDb db) => {
    var data = await JsonSerializer.DeserializeAsync<CantidadDto>(req.Body);
    if (data == null) return Results.BadRequest();
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound();
    prod.Stock += data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

// Quitar stock (validando que no quede negativo)
app.MapPost("/productos/{id}/quitar", async (int id, HttpRequest req, AppDb db) => {
    var data = await JsonSerializer.DeserializeAsync<CantidadDto>(req.Body);
    if (data == null) return Results.BadRequest();
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound();
    if (prod.Stock < data.Cantidad) return Results.BadRequest(new { mensaje = "Stock insuficiente" });
    prod.Stock -= data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated(); // crear BD si no existe
    if (!db.Productos.Any()) {
        db.Productos.AddRange(new[] {
            new Producto { Nombre = "Arroz", Precio = 150, Stock = 10 },
            new Producto { Nombre = "Fideos", Precio = 120, Stock = 10 },
            new Producto { Nombre = "Aceite", Precio = 500, Stock = 10 },
            new Producto { Nombre = "Azúcar", Precio = 200, Stock = 10 },
            new Producto { Nombre = "Sal", Precio = 80, Stock = 10 },
            new Producto { Nombre = "Yerba", Precio = 600, Stock = 10 },
            new Producto { Nombre = "Leche", Precio = 250, Stock = 10 },
            new Producto { Nombre = "Harina", Precio = 110, Stock = 10 },
            new Producto { Nombre = "Galletitas", Precio = 180, Stock = 10 },
            new Producto { Nombre = "Mermelada", Precio = 300, Stock = 10 }
        });
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; } // NUEVO
}

class CantidadDto
{
    public int Cantidad { get; set; }
}