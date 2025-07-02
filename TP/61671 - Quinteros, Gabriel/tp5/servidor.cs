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


var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    
    if (!db.Productos.Any())
    {
        var productos = new List<Producto>
        {
            new() { Nombre = "Laptop Dell", Precio = 800000, Stock = 10 },
            new() { Nombre = "Mouse Logitech", Precio = 15000, Stock = 10 },
            new() { Nombre = "Teclado Mecánico", Precio = 25000, Stock = 10 },
            new() { Nombre = "Monitor 24\"", Precio = 120000, Stock = 10 },
            new() { Nombre = "Auriculares", Precio = 35000, Stock = 10 },
            new() { Nombre = "Webcam HD", Precio = 20000, Stock = 10 },
            new() { Nombre = "Disco SSD 1TB", Precio = 45000, Stock = 10 },
            new() { Nombre = "Memoria RAM 16GB", Precio = 30000, Stock = 10 },
            new() { Nombre = "Impresora HP", Precio = 85000, Stock = 10 },
            new() { Nombre = "Router WiFi", Precio = 18000, Stock = 10 }
        };
        
        db.Productos.AddRange(productos);
        db.SaveChanges();
        Console.WriteLine(" Productos de ejemplo creados");
    }
}


app.MapGet("/productos", async (AppDb db) => 
{
    var productos = await db.Productos.ToListAsync();
    return Results.Ok(productos);
});


app.MapGet("/productos/reponer", async (AppDb db) => 
{
    var productos = await db.Productos.Where(p => p.Stock < 3).ToListAsync();
    return Results.Ok(productos);
});


app.MapPut("/productos/{id}/agregar-stock", async (int id, StockRequest request, AppDb db) => 
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound($"Producto con ID {id} no encontrado");
    
    if (request.Cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a 0");
    
    producto.Stock += request.Cantidad;
    await db.SaveChangesAsync();
    
    return Results.Ok(new { 
        mensaje = $"Se agregaron {request.Cantidad} unidades al producto {producto.Nombre}",
        producto = producto 
    });
});


app.MapPut("/productos/{id}/quitar-stock", async (int id, StockRequest request, AppDb db) => 
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null)
        return Results.NotFound($"Producto con ID {id} no encontrado");
    
    if (request.Cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a 0");
    
    if (producto.Stock < request.Cantidad)
        return Results.BadRequest($"Stock insuficiente. Stock actual: {producto.Stock}");
    
    producto.Stock -= request.Cantidad;
    await db.SaveChangesAsync();
    
    return Results.Ok(new { 
        mensaje = $"Se quitaron {request.Cantidad} unidades del producto {producto.Nombre}",
        producto = producto 
    });
});

Console.WriteLine(" Servidor iniciado en http://localhost:5000");
Console.WriteLine(" Endpoints disponibles:");
Console.WriteLine("   GET /productos - Listar todos los productos");
Console.WriteLine("   GET /productos/reponer - Productos que necesitan reposición");
Console.WriteLine("   PUT /productos/{id}/agregar-stock - Agregar stock");
Console.WriteLine("   PUT /productos/{id}/quitar-stock - Quitar stock");

app.Run("http://localhost:5000");

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

class StockRequest
{
    public int Cantidad { get; set; }
}