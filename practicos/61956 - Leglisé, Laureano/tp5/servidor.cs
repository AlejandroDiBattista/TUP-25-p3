using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// Configuración builder
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Asegurar DB creada y cargar datos si no hay
using(var scope = app.Services.CreateScope()){
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any()){
        for (int i = 1; i <= 10; i++){
            db.Productos.Add(new Producto {
                Nombre = $"Producto {i}",
                Precio = 10 + i,
                Stock = 10
            });
        }
        db.SaveChanges();
    }
}

// Endpoints

// Listar todos los productos
app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

// Listar productos a reponer (stock < 3)
app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync()
);

// Agrega stock al producto
app.MapPut("/productos/{id}/agregar", async (int id, StockUpdate update, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    producto.Stock += update.Cantidad;
    await db.SaveChangesAsync();

    return Results.Ok(producto);
});

//Saca el stock al producto (no puede ser negativo)
app.MapPut("/productos/{id}/quitar", async (int id, StockUpdate update, AppDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    if(producto.Stock - update.Cantidad < 0)
        return Results.BadRequest("No se puede dejar stock negativo");

    producto.Stock -= update.Cantidad;
    await db.SaveChangesAsync();

    return Results.Ok(producto);
});

app.Run("http://localhost:5000");

// Clases
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

class StockUpdate {
    public int Cantidad { get; set; }
}
