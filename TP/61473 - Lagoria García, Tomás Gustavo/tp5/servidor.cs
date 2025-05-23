#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 7.0.21"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 7.0.21"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.10"

using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
SQLitePCL.Batteries_V2.Init();

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); // agregar servicios : Instalar EF Core y SQLite
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) => await db.Productos.Where(p => p.Cantidad < 3).ToListAsync());

app.MapGet("/productos/porID/{id}", async (int id, AppDb db) => await db.Productos.FindAsync(id) is Producto p ? Results.Ok(p) : Results.NotFound());

app.MapPost("/productos/agregarNuevo", async (Producto p, AppDb db) =>
{
    db.Productos.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{p.Id}", p);
});
app.MapPost("/productos/{id}/agregar/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
    {
        return Results.NotFound();
    }
    producto.Cantidad += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});
app.MapPost("/productos/{id}/quitar/{cantidad}", async (int id, int cantidad, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
    {
        return Results.NotFound();
    }
    if (producto.Cantidad - cantidad < 0) 
    {
        return Results.BadRequest("Stock insuficiente.");
    }
         producto.Cantidad -= cantidad;
        await db.SaveChangesAsync();
        return Results.Ok(producto);

    });

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}