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

//Endpoint
//lista todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

//lista productos con bajo stock(stock < 3)
app.MapGet("/productos/reponer", async (AppDb db) => await db.Productos.Where(p => p.Stock < 3).ToListAsync());
// agregar stock a un producto
app.MapPut("/productos/{id}/agregar-stock", async (AppDb db, int id, HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var body = await reader.ReadToEndAsync();
    var data = JsonSerializer.Deserialize<StockDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound("no ");
    producto.Stock += data?.Cantidad ?? 0;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// quitar stock a un producto (sin dejar negativo)
app.MapPut("/productos/{id}/quitar-stock", async (AppDb db, int id, HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var body = await reader.ReadToEndAsync();
    var data = JsonSerializer.Deserialize<StockDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if ((data?.Cantidad ?? 0) > producto.Stock)
        return Results.BadRequest(new { error = "No hay suficiente stock." });
    producto.Stock -= data?.Cantidad ?? 0;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated();
// Crear la base de datos y agregar productos si no existen
if (!db.Productos.Any()) {
    var productos = new List<Producto>
    {
        new Producto { Nombre = "Coca 2L", Precio = 2500, Stock = 10 },
        new Producto { Nombre = "Pepsi 2L", Precio = 2000, Stock = 10 },
        new Producto { Nombre = "Galletas Variedades", Precio = 1200, Stock = 10 },
        new Producto { Nombre = "Pepitos", Precio = 1300, Stock = 10 },
        new Producto { Nombre = "Chocolate Block 250mg", Precio = 1500, Stock = 10 },
        new Producto { Nombre = "Oreos", Precio = 1400, Stock = 10 },
        new Producto { Nombre = "Chocolina", Precio = 2000, Stock = 10 },
        new Producto { Nombre = "Azucar 1kg", Precio = 2000, Stock = 10 },
        new Producto { Nombre = "Chocolate en polvo 1kg", Precio = 1500, Stock = 10 },
        new Producto { Nombre = "Aceite 1l", Precio = 1200, Stock = 10 }
    };
    db.Productos.AddRange(productos);
    db.SaveChanges();
}

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



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