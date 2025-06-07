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

// CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.Configure<JsonOptions>(opt => 
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// TEST
//app.MapGet("/", () => "Servidor funcionando correctamente.");
 //app.MapGet("/", () => @"
// Menú de Cliente:
// 1. Listar todos los productos:              GET /productos
// 2. Listar productos con stock bajo (<3):    GET /productos/stock
// 3. Agregar stock a un producto:             PUT /productos/agregar/{id}/{cantidad}
// 4. Quitar stock de un producto:             PUT /productos/quitar/{id}/{cantidad}
// ");

// ENDPOINTS

// Listar todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Listar productos con stock < 3
app.MapGet("/productos/stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

// Agregar stock
app.MapPut("/productos/agregar/{id}/{cantidad}", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Quitar stock
app.MapPut("/productos/quitar/{id}/{cantidad}", async (int id, int cantidad, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock - cantidad < 0)
        return Results.BadRequest("El stock no puede ser negativo.");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Inicializar base de datos con productos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        var productos = new[] {
            new Producto { Nombre = "Laptop", Precio = 999.99m, Stock = 10 },
            new Producto { Nombre = "Mouse", Precio = 29.99m, Stock = 10 },
            new Producto { Nombre = "Teclado", Precio = 49.99m, Stock = 10 },
            new Producto { Nombre = "Monitor", Precio = 299.99m, Stock = 10 },
            new Producto { Nombre = "Auriculares", Precio = 79.99m, Stock = 10 },
            new Producto { Nombre = "Webcam", Precio = 59.99m, Stock = 10 },
            new Producto { Nombre = "Impresora", Precio = 199.99m, Stock = 1 },
            new Producto { Nombre = "Disco Duro", Precio = 89.99m, Stock = 10 },
            new Producto { Nombre = "Memoria RAM", Precio = 69.99m, Stock = 2 },
            new Producto { Nombre = "Tarjeta Gráfica", Precio = 399.99m, Stock = 10 }
        };

        db.Productos.AddRange(productos);
        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

// DB Context y Modelo
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
// Fin del código