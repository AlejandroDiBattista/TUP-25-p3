#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.10"

using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

SQLitePCL.Batteries_V2.Init();

// configuracion del servidor
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); 
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// endpoints principales
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());
app.MapGet("/productos/reponer", async (AppDb db) => await db.Productos.Where(p => p.Stock < 3).ToListAsync());
app.MapPost("/productos/{id}/agregar", async (int id, AppDb db, StockDto dto) => {
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound("Producto no encontrado");
    prod.Stock += dto.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});
app.MapPost("/productos/{id}/quitar", async (int id, AppDb db, StockDto dto) => {
    var prod = await db.Productos.FindAsync(id);
    if (prod == null) return Results.NotFound("Producto no encontrado");
    if (prod.Stock < dto.Cantidad) return Results.BadRequest("No hay suficiente stock");
    prod.Stock -= dto.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    db.Productos.RemoveRange(db.Productos);
    var nombresBebidas = new[] {
        "Coca-Cola", "Pepsi", "Sprite", "Fanta", "Agua Mineral", "Jugo de Naranja", "Jugo de Manzana", "Tónica", "Energizante", "Cerveza"
    };
    for (int i = 0; i < nombresBebidas.Length; i++)
        db.Productos.Add(new Producto { Nombre = nombresBebidas[i], Stock = 10 });
    db.SaveChanges();
}

app.Run("http://localhost:5000"); 





class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int Stock { get; set; }
}

class StockDto { public int Cantidad { get; set; } }