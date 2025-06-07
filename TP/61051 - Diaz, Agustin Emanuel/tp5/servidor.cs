#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 8.0.0"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 8.0.0"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.6"

using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Json;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<TiendaDb>(opt =>
    opt.UseSqlite("Data Source=productos.db"));
builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/productos", async (TiendaDb db) =>
    await db.Productos.OrderBy(p => p.Nombre).ToListAsync());

app.MapGet("/productos/reponer", async (TiendaDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos", async (Producto nuevo, TiendaDb db) => {
    db.Productos.Add(nuevo);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{nuevo.Id}", nuevo);
});

app.MapPost("/productos/agregar", async (MovimientoStock mov, TiendaDb db) => {
    var prod = await db.Productos.FindAsync(mov.Id);
    if (prod is null) return Results.NotFound(new { mensaje = "Producto no encontrado" });
    prod.Stock += mov.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.MapPost("/productos/quitar", async (MovimientoStock mov, TiendaDb db) => {
    var prod = await db.Productos.FindAsync(mov.Id);
    if (prod is null) return Results.NotFound(new { mensaje = "Producto no encontrado" });
    if (prod.Stock < mov.Cantidad) return Results.BadRequest(new { mensaje = "Stock insuficiente" });
    prod.Stock -= mov.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

app.Run("http://localhost:5000");

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
}

class MovimientoStock {
    public int Id { get; set; }
    public int Cantidad { get; set; }
}

class TiendaDb : DbContext {
    public TiendaDb(DbContextOptions<TiendaDb> opt) : base(opt) { }
    public DbSet<Producto> Productos => Set<Producto>();
}