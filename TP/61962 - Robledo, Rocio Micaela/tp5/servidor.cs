#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();

// Base de datos SQLite llamada "libreria.db"
builder.Services.AddDbContext<LibreriaDb>(opt =>
    opt.UseSqlite("Data Source=./libreria.db"));

builder.Services.Configure<JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/articulos", async (LibreriaDb db) =>
    await db.Articulos.ToListAsync());

app.MapGet("/articulos/reponer", async (LibreriaDb db) =>
    await db.Articulos.Where(a => a.Stock < 5).ToListAsync());

app.MapPost("/articulos/agregar", async (LibreriaDb db, StockUpdate data) =>
{
    var articulo = await db.Articulos.FindAsync(data.Id);
    if (articulo is null)
        return Results.NotFound($"Artículo con ID {data.Id} no encontrado.");

    articulo.Stock += data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(articulo);
});

app.MapPost("/articulos/quitar", async (LibreriaDb db, StockUpdate data) =>
{
    var articulo = await db.Articulos.FindAsync(data.Id);
    if (articulo is null)
        return Results.NotFound($"Artículo con ID {data.Id} no encontrado.");

    if (articulo.Stock < data.Cantidad)
        return Results.BadRequest("No se puede quitar más stock del disponible.");

    articulo.Stock -= data.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(articulo);
});

// Inicializar base de datos con datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibreriaDb>();
    db.Database.EnsureCreated();

    if (!db.Articulos.Any())
    {
        db.Articulos.AddRange(new[]
        {
            new Articulo { Nombre = "Cuaderno A4", Descripcion = "Cuaderno de 80 hojas", Precio = 2.5m, Stock = 10, Categoria = "Papelería" },
            new Articulo { Nombre = "Lápiz HB", Descripcion = "Lápiz de grafito", Precio = 0.5m, Stock = 20, Categoria = "Escritura" },
            new Articulo { Nombre = "Goma de borrar", Descripcion = "Goma blanca", Precio = 0.3m, Stock = 15, Categoria = "Escritura" },
            new Articulo { Nombre = "Regla 30cm", Descripcion = "Regla de plástico", Precio = 1.0m, Stock = 12, Categoria = "Instrumentos" },
            new Articulo { Nombre = "Tijeras escolares", Descripcion = "Tijeras con punta redonda", Precio = 1.8m, Stock = 8, Categoria = "Instrumentos" },
            new Articulo { Nombre = "Resaltador", Descripcion = "Marcador fluorescente", Precio = 0.9m, Stock = 18, Categoria = "Escritura" },
            new Articulo { Nombre = "Bolígrafo azul", Descripcion = "Bolígrafo tinta azul", Precio = 0.7m, Stock = 25, Categoria = "Escritura" },
            new Articulo { Nombre = "Cartulina", Descripcion = "Cartulina de colores", Precio = 1.2m, Stock = 10, Categoria = "Papelería" },
            new Articulo { Nombre = "Pegamento en barra", Descripcion = "Adhesivo escolar", Precio = 1.1m, Stock = 14, Categoria = "Instrumentos" },
            new Articulo { Nombre = "Compás", Descripcion = "Compás para dibujo", Precio = 2.0m, Stock = 6, Categoria = "Instrumentos" },
        });

        db.SaveChanges();
    }
}

app.Run("http://localhost:5000");
class LibreriaDb : DbContext
{
    public LibreriaDb(DbContextOptions<LibreriaDb> options) : base(options) { }

    public DbSet<Articulo> Articulos => Set<Articulo>();
}

class Articulo
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
