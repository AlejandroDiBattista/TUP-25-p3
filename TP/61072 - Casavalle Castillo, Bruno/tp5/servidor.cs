using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TiendaContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    context.Database.EnsureCreated();

    if (!context.Productos.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            context.Productos.Add(new global::Producto { Nombre = $"Producto {i}", Stock = 10 });
        }
        context.SaveChanges();
    }
}

app.MapGet("/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (TiendaContext db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/agregar/{id}/{cantidad}", async (int id, int cantidad, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/quitar/{id}/{cantidad}", async (int id, int cantidad, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock < cantidad) return Results.BadRequest("No hay suficiente stock.");
    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.Run();

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
}

class TiendaContext : DbContext
{
    public DbSet<global::Producto> Productos => Set<global::Producto>();
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=tienda.db");
}