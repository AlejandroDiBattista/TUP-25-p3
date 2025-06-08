#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

#r "nuget: Microsoft.EntityFrameworkCore, 7.0.11"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 7.0.11"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.4"


using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;


SQLitePCL.Batteries_V2.Init();

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); // agregar servicios : Instalar EF Core y SQLite
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/{stock}", async (int stock, AppDb db) => await db.Productos.Where(x => x.Stock <= stock).ToListAsync());

app.MapPut("/productos/agregar/{id}/{stock}", async (int id, int stock, AppDb db) =>
{
    var res = await db.Productos.FindAsync(id);
    res.Stock += stock;
    await db.SaveChangesAsync();
    return Results.Ok(res);
});

app.MapPut("/productos/quitar/{id}/{stock}", async (int id, int stock, AppDb db) =>
{
    var res = await db.Productos.FindAsync(id);
    if (res.Stock < stock)
        return Results.NotFound($"Producto con ID {id} no encontrado.");

    res.Stock -= stock;
    await db.SaveChangesAsync();
    return Results.Ok(res);
});

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos

app.Run("http://localhost:5000");
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(ProductoSeeder.ObtenerProductos());
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public static class ProductoSeeder
{
    public static List<Producto> ObtenerProductos()
    {
        return new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Producto 1", Precio = 100.00m, Stock = 4 },
            new Producto { Id = 2, Nombre = "Producto 2", Precio = 150.00m, Stock = 4 },
            new Producto { Id = 3, Nombre = "Producto 3", Precio = 200.00m, Stock = 4 },
            new Producto { Id = 4, Nombre = "Producto 4", Precio = 250.00m, Stock = 4 },
            new Producto { Id = 5, Nombre = "Producto 5", Precio = 300.00m, Stock = 3 },
            new Producto { Id = 6, Nombre = "Producto 6", Precio = 350.00m, Stock = 3 },
            new Producto { Id = 7, Nombre = "Producto 7", Precio = 400.00m, Stock = 4 },
            new Producto { Id = 8, Nombre = "Producto 8", Precio = 450.00m, Stock = 2 },
            new Producto { Id = 9, Nombre = "Producto 9", Precio = 500.00m, Stock = 4 },
            new Producto { Id = 10, Nombre = "Producto 10", Precio = 550.00m, Stock = 1 }
        };
    }
}