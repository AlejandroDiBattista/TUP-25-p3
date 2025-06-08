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

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Migraciones y datos iniciales
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.Migrate();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(Enumerable.Range(1, 10).Select(i =>
            new Producto
            {
                Nombre = $"Producto {i}",
                Precio = 10 + i,
                Stock = 10
            }));
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// === Endpoints ===

// Listar todos los productos
app.MapGet("/productos", async (AppDb db) =>
    await db.Productos.ToListAsync());

// Listar productos con stock menor a 3
app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

// Agregar stock
app.MapPut("/productos/{id}/agregar", async (int id, DatosStock datos, AppDb db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound();

    prod.Stock += datos.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Quitar stock (con validación)
app.MapPut("/productos/{id}/quitar", async (int id, DatosStock datos, AppDb db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound();
    if (prod.Stock < datos.Cantidad)
        return Results.BadRequest("Stock insuficiente.");

    prod.Stock -= datos.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();

// === Modelos y contexto ===

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

class DatosStock
{
    public int Cantidad { get; set; }
}
