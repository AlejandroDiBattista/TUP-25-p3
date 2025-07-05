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
builder.Services.AddDbContext<TiendaContexto>(opt => opt.UseSqlite("Data Source=stock_tienda.db"));
builder.Services.Configure<JsonOptions>(opt => {
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Inicialización de BD con productos distintos
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<TiendaContexto>();
    db.Database.EnsureCreated();

    if (!db.Articulos.Any()) {
        db.Articulos.AddRange(new List<Articulo> {
            new Articulo { Nombre = "Cámara DSLR", Precio = 95000, Stock = 7 },
            new Articulo { Nombre = "Lente 50mm", Precio = 15000, Stock = 5 },
            new Articulo { Nombre = "Trípode Pro", Precio = 8200, Stock = 2 },
            new Articulo { Nombre = "Estuche rígido", Precio = 3200, Stock = 1 },
            new Articulo { Nombre = "Tarjeta SD 128GB", Precio = 4000, Stock = 4 },
            new Articulo { Nombre = "Kit de limpieza", Precio = 1200, Stock = 3 },
            new Articulo { Nombre = "Batería extra", Precio = 2500, Stock = 6 },
            new Articulo { Nombre = "Flash externo", Precio = 7300, Stock = 0 },
            new Articulo { Nombre = "Filtro UV", Precio = 900, Stock = 9 },
            new Articulo { Nombre = "Disco duro portátil", Precio = 11500, Stock = 10 }
        });

        db.SaveChanges();
    }
}

app.MapGet("/articulos", async (TiendaContexto db) =>
    await db.Articulos.ToListAsync());

app.MapGet("/articulos/bajo-stock", async (TiendaContexto db) =>
    await db.Articulos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/articulos/{id}/incrementar", async (int id, HttpRequest req, TiendaContexto db) => {
    var data = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(req.Body);
    if (!data.TryGetValue("cantidad", out int cant) || cant <= 0)
        return Results.BadRequest("Cantidad inválida.");

    var art = await db.Articulos.FindAsync(id);
    if (art == null)
        return Results.NotFound("Artículo no encontrado.");

    art.Stock += cant;
    await db.SaveChangesAsync();
    return Results.Ok(art);
});

app.MapPost("/articulos/{id}/reducir", async (int id, HttpRequest req, TiendaContexto db) => {
    var data = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(req.Body);
    if (!data.TryGetValue("cantidad", out int cant) || cant <= 0)
        return Results.BadRequest("Cantidad inválida.");

    var art = await db.Articulos.FindAsync(id);
    if (art == null)
        return Results.NotFound("Artículo no encontrado.");

    if (art.Stock < cant)
        return Results.BadRequest($"Stock insuficiente. Disponible: {art.Stock}");

    art.Stock -= cant;
    await db.SaveChangesAsync();
    return Results.Ok(art);
});

app.Run("http://localhost:5000");

class TiendaContexto : DbContext {
    public TiendaContexto(DbContextOptions<TiendaContexto> opt) : base(opt) { }
    public DbSet<Articulo> Articulos => Set<Articulo>();
}

class Articulo {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
