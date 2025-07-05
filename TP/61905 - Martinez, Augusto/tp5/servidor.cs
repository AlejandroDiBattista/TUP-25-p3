#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 7.0.0"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 7.0.0"
#r "nuget: Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, 7.0.0"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.7"

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;

Batteries.Init();

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    
    if (!db.Productos.Any()) {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "Laptop HP", Precio = 15000.00m, Stock = 10 },
            new Producto { Nombre = "Mouse Logitech", Precio = 800.00m, Stock = 10 },
            new Producto { Nombre = "Teclado Mecánico", Precio = 1200.00m, Stock = 10 },
            new Producto { Nombre = "Monitor 24\"", Precio = 8000.00m, Stock = 10 },
            new Producto { Nombre = "Auriculares", Precio = 600.00m, Stock = 10 },
            new Producto { Nombre = "Webcam HD", Precio = 900.00m, Stock = 10 },
            new Producto { Nombre = "Disco SSD 1TB", Precio = 2500.00m, Stock = 10 },
            new Producto { Nombre = "Memoria RAM 16GB", Precio = 1800.00m, Stock = 10 },
            new Producto { Nombre = "Smartphone", Precio = 12000.00m, Stock = 10 },
            new Producto { Nombre = "Tablet", Precio = 7000.00m, Stock = 10 }
        };
        
        db.Productos.AddRange(productos);
        db.SaveChanges();
        Console.WriteLine("Base de datos inicializada con 10 productos de ejemplo.");
    }
}

Console.WriteLine("Servidor iniciado en: http://localhost:5000");
Console.WriteLine("Endpoints disponibles:");
Console.WriteLine("- GET /productos");
Console.WriteLine("- GET /productos/reponer");
Console.WriteLine("- POST /productos/{id}/agregar");
Console.WriteLine("- POST /productos/{id}/quitar");
Console.WriteLine("\nPresione Ctrl+C para detener el servidor.\n");


app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());


app.MapGet("/productos/reponer", async (AppDb db) => 
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar", async (int id, HttpRequest request, AppDb db) => {
    var requestData = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(request.Body);
    if (!requestData.TryGetValue("cantidad", out int cantidad))
        return Results.BadRequest("Se requiere el campo 'cantidad'");
    
    if (cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a 0");
    
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) 
        return Results.NotFound("Producto no encontrado");
    
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    
    Console.WriteLine($"Stock agregado: {producto.Nombre} - Nuevo stock: {producto.Stock}");
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar", async (int id, HttpRequest request, AppDb db) => {
    var requestData = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(request.Body);
    if (!requestData.TryGetValue("cantidad", out int cantidad))
        return Results.BadRequest("Se requiere el campo 'cantidad'");
    
    if (cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a 0");
    
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) 
        return Results.NotFound("Producto no encontrado");
    
    if (producto.Stock < cantidad) 
        return Results.BadRequest($"Stock insuficiente. Stock actual: {producto.Stock}");
    
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    
    Console.WriteLine($"Stock descontado: {producto.Nombre} - Nuevo stock: {producto.Stock}");
    return Results.Ok(producto);
});

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