#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 7.0.0"
#r "nuget: Microsoft.AspNetCore.App, 7.0.0"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.2"

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<TiendaDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
var app = builder.Build();


public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
}

public class TiendaDb : DbContext
{
    public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }
    public DbSet<Producto> Productos { get; set; }
}


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        var productosBelleza = new List<Producto>
        {
            new Producto { Nombre = "Base líquida", Stock = 10 },
            new Producto { Nombre = "Corrector de ojeras", Stock = 10 },
            new Producto { Nombre = "Rubor en polvo", Stock = 10 },
            new Producto { Nombre = "Sombra para ojos", Stock = 10 },
            new Producto { Nombre = "Delineador líquido", Stock = 10 },
            new Producto { Nombre = "Máscara de pestañas", Stock = 10 },
            new Producto { Nombre = "Labial mate", Stock = 10 },
            new Producto { Nombre = "Gloss labial", Stock = 10 },
            new Producto { Nombre = "Polvo compacto", Stock = 10 },
            new Producto { Nombre = "Spray fijador de maquillaje", Stock = 10 }
        };

        db.Productos.AddRange(productosBelleza);
        db.SaveChanges();
    }
}
    



app.MapGet("/productos", (TiendaDb db) => db.Productos.ToList());
app.MapGet("/productos/reponer", (TiendaDb db) => db.Productos.Where(p => p.Stock < 3).ToList());
app.MapPost("/productos/{id}/agregar", (TiendaDb db, int id, int cantidad) =>
{
    var prod = db.Productos.Find(id);
    if (prod is null) return Results.NotFound();
    prod.Stock += cantidad;
    db.SaveChanges();
    return Results.Ok(prod);
});
app.MapPost("/productos/{id}/quitar", (TiendaDb db, int id, int cantidad) =>
{
    var prod = db.Productos.Find(id);
    if (prod is null) return Results.NotFound();
    if (prod.Stock < cantidad) return Results.BadRequest("Stock insuficiente");
    prod.Stock -= cantidad;
    db.SaveChanges();
    return Results.Ok(prod);
});

app.Run("http://localhost:5000");

