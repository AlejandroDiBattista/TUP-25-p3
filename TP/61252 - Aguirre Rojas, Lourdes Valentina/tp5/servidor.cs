#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<TiendaDb>(opt =>
    opt.UseSqlite("Data Source=./tienda.db"));

builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/prendas", async (TiendaDb db) =>
    await db.Prendas.ToListAsync());

app.MapGet("/prendas/reponer", async (TiendaDb db) =>
    await db.Prendas.Where(p => p.Stock < 5).ToListAsync());

app.MapPost("/prendas/agregar", async (TiendaDb db, StockUpdate data) =>
{
    var prenda = await db.Prendas.FindAsync(data.Id);
    if (prenda is null)
        return Results.NotFound($"Prenda con ID {data.Id} no encontrada.");

    prenda.Stock += data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prenda);
});

app.MapPost("/prendas/quitar", async (TiendaDb db, StockUpdate data) =>
{
    var prenda = await db.Prendas.FindAsync(data.Id);
    if (prenda is null)
        return Results.NotFound($"Prenda con ID {data.Id} no encontrada.");

    if (prenda.Stock < data.Cantidad)
        return Results.BadRequest("No se puede quitar más stock del disponible.");

    prenda.Stock -= data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prenda);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();

    if (!db.Prendas.Any())
    {
        db.Prendas.AddRange(new[]
        {
            new Prenda { Nombre = "Remera blanca", Descripcion = "Remera de algodón ", Precio = 3500, Stock = 10, Categoria = "Ropa casual" },
            new Prenda { Nombre = "Pantalón jeans", Descripcion = "Pantalón de jean azul", Precio = 6000, Stock = 8, Categoria = "Ropa casual" },
            new Prenda { Nombre = "Camisa formal", Descripcion = "Camisa manga larga", Precio = 5000, Stock = 12, Categoria = "Ropa formal" },
            new Prenda { Nombre = "Vestido floral", Descripcion = "Vestido verano", Precio = 7000, Stock = 6, Categoria = "Ropa femenina" },
            new Prenda { Nombre = "Buzo con capucha", Descripcion = "Buzo polar", Precio = 9000, Stock = 4, Categoria = "Abrigo" },
            new Prenda { Nombre = "Falda negra", Descripcion = "Falda corta negra", Precio = 4000, Stock = 10, Categoria = "Ropa femenina" },
            new Prenda { Nombre = "Campera de cuero", Descripcion = "Campera negra", Precio = 8000, Stock = 3, Categoria = "Abrigo" },
            new Prenda { Nombre = "Short deportivo", Descripcion = "Short para entrenamiento", Precio = 3000, Stock = 9, Categoria = "Ropa deportiva" },
            new Prenda { Nombre = "Zapatillas urbanas", Descripcion = "Zapatillas blancas", Precio = 1000, Stock = 5, Categoria = "Calzado" },
            new Prenda { Nombre = "Medias", Descripcion = "Par de medias de algodón", Precio = 1000, Stock = 20, Categoria = "Accesorios" }
        });

        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");

class TiendaDb : DbContext
{
    public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }

    public DbSet<Prenda> Prendas => Set<Prenda>();
}

class Prenda
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; } = string.Empty;
}

class StockUpdate
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
}