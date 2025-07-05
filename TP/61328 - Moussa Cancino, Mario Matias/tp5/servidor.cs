#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 7.0.0"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 7.0.0"
#r "nuget: Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, 7.0.0"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.7"


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;


SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
var app = builder.Build();

// Crear base y cargar datos si no existen
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(ProductoSeeder.ObtenerProductos());
        db.SaveChanges();
    }
}

// Rutas
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/bajo-stock", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPut("/productos/agregar/{id}/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPut("/productos/quitar/{id}/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    if (producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente");

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.Run("http://localhost:5000");

// Clases y datos

class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(ProductoSeeder.ObtenerProductos());
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

static class ProductoSeeder
{
    public static List<Producto> ObtenerProductos() =>
        new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Producto 1", Precio = 100m, Stock = 10 },
            new Producto { Id = 2, Nombre = "Producto 2", Precio = 200m, Stock = 10 },
            new Producto { Id = 3, Nombre = "Producto 3", Precio = 300m, Stock = 10 },
            new Producto { Id = 4, Nombre = "Producto 4", Precio = 400m, Stock = 10 },
            new Producto { Id = 5, Nombre = "Producto 5", Precio = 500m, Stock = 10 },
            new Producto { Id = 6, Nombre = "Producto 6", Precio = 600m, Stock = 10 },
            new Producto { Id = 7, Nombre = "Producto 7", Precio = 700m, Stock = 10 },
            new Producto { Id = 8, Nombre = "Producto 8", Precio = 800m, Stock = 10 },
            new Producto { Id = 9, Nombre = "Producto 9", Precio = 900m, Stock = 10 },
            new Producto { Id = 10, Nombre = "Producto 10", Precio = 1000m, Stock = 10 },
        };
}
