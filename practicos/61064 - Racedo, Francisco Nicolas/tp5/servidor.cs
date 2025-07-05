#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"
#r "nuget: Microsoft.Extensions.Logging, 9.0.4"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.10"


using System;
using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

// Inicializar SQLite explícitamente con manejo de errores mejorado
try {
    SQLitePCL.Batteries_V2.Init();
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
    Console.WriteLine("SQLite inicializado correctamente");
} catch (Exception ex) {
    Console.WriteLine($"Error al inicializar SQLite: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
    if (ex.InnerException != null)
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
}

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();

// Configurar opciones de SQLite con ruta absoluta
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "tienda.db");
Console.WriteLine($"Ruta de la base de datos: {dbPath}");

// Configuración de EF Core 9.0.4 con manejo de errores y logging
builder.Services.AddDbContext<AppDb>(opt => {
    opt.UseSqlite($"Data Source={dbPath}");
    opt.EnableSensitiveDataLogging(); // Ayuda para depuración
    opt.EnableDetailedErrors(); // Mostrar errores detallados
    opt.LogTo(Console.WriteLine, LogLevel.Information); // Logging a consola
});

builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// Agregamos logging explícito
builder.Services.AddLogging(logging => {
    logging.ClearProviders();
    logging.AddConsole();
});

var app = builder.Build();

// Endpoint para obtener todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Endpoint para obtener productos que necesitan reposición (stock < 3)
app.MapGet("/productos/reponer", async (AppDb db) => 
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Endpoint para agregar stock a un producto
app.MapPut("/productos/{id}/agregar", async (int id, StockRequest stockRequest, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound();
    
    producto.Stock += stockRequest.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

// Endpoint para quitar stock a un producto (con validación para evitar stock negativo)
app.MapPut("/productos/{id}/quitar", async (int id, StockRequest stockRequest, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound();
    
    if (producto.Stock < stockRequest.Cantidad)
        return Results.BadRequest(new { error = "No hay suficiente stock disponible" });
    
    producto.Stock -= stockRequest.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

var db = app.Services.GetRequiredService<AppDb>();
// db.Database.EnsureDeleted(); // Comentamos esta línea para permitir que los datos persistan
db.Database.EnsureCreated(); // crear BD si no existe

// Agregar productos de ejemplo al crear la base de datos
if (!db.Productos.Any())
{   
    var productosEjemplo = new List<Producto>
    {   
        new Producto { Nombre = "Camiseta", Precio = 25999.99m, Stock = 10 },
        new Producto { Nombre = "Pantalón", Precio = 25999.99m, Stock = 10 },
        new Producto { Nombre = "Zapatillas", Precio = 59000.99m, Stock = 10 },
        new Producto { Nombre = "Gorra", Precio = 9000.99m, Stock = 10 },
        new Producto { Nombre = "Calcetines", Precio = 4000.99m, Stock = 10 },
        new Producto { Nombre = "Chaqueta", Precio = 49999.99m, Stock = 10 },
        new Producto { Nombre = "Bufanda", Precio = 14999.99m, Stock = 10 },
        new Producto { Nombre = "Guantes", Precio = 12999m, Stock = 10 },
        new Producto { Nombre = "Cinturón", Precio = 15999.99m, Stock = 10 },
        new Producto { Nombre = "Reloj", Precio = 99999.99m, Stock = 10 }
    };
    
    db.Productos.AddRange(productosEjemplo);
    db.SaveChanges();
}

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.

// Modelo de datos
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    
    public DbSet<Producto> Productos => Set<Producto>();

    // Configuración adicional para EF Core 9.0.4
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Solo agregar configuraciones adicionales si no están ya configuradas
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración explícita para la tabla Producto
        modelBuilder.Entity<Producto>().ToTable("Productos");
        modelBuilder.Entity<Producto>().HasKey(p => p.Id);
        modelBuilder.Entity<Producto>().Property(p => p.Nombre).IsRequired();
        modelBuilder.Entity<Producto>().Property(p => p.Precio).HasPrecision(18, 2);
        
        base.OnModelCreating(modelBuilder);
    }
}

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// Clase para recibir solicitudes de modificación de stock
class StockRequest
{   
    public int Cantidad { get; set; }
}