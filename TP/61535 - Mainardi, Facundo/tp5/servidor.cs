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
        for (int i = 1; i <= 10; i++) {
            db.Productos.Add(new Producto { 
                Nombre = $"Producto {i}", 
                Precio = i * 100, 
                Stock = 10 
            });
        }
        db.SaveChanges();
    }
}

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) => 
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar", async (int id, HttpRequest request, AppDb db) => {
    var requestData = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(request.Body);
    if (!requestData.TryGetValue("cantidad", out int cantidad))
        return Results.BadRequest("Se requiere el campo 'cantidad'");
    
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();
    
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar", async (int id, HttpRequest request, AppDb db) => {
    var requestData = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(request.Body);
    if (!requestData.TryGetValue("cantidad", out int cantidad))
        return Results.BadRequest("Se requiere el campo 'cantidad'");
    
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();
    
    if (producto.Stock - cantidad < 0) 
        return Results.BadRequest("Stock no puede ser negativo");
    
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
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