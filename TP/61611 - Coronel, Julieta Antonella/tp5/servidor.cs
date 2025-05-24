#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.11"




using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using SQLitePCL;
Batteries_V2.Init();



var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=stock.db"));
builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.ToListAsync()
);

app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
        return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Resma A4", Precio = 1900, Stock = 10 },
            new Producto { Nombre = "Lapicera Azul", Precio = 250, Stock = 10 },
            new Producto { Nombre = "Corrector", Precio = 430, Stock = 10 },
            new Producto { Nombre = "Cuaderno Rivadavia", Precio = 1200, Stock = 10 },
            new Producto { Nombre = "Marcador", Precio = 620, Stock = 10 },
            new Producto { Nombre = "Papel glasé", Precio = 300, Stock = 10 },
            new Producto { Nombre = "Cartulina", Precio = 450, Stock = 10 },
            new Producto { Nombre = "Pegamento", Precio = 370, Stock = 10 },
            new Producto { Nombre = "Goma blanca", Precio = 220, Stock = 10 },
            new Producto { Nombre = "Sacapuntas", Precio = 180, Stock = 10 }
        });
        db.SaveChanges();
    }
}

Console.WriteLine("Servidor iniciado en http://localhost:5000");
app.Run("http://localhost:5000");

class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> opts) : base(opts) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
