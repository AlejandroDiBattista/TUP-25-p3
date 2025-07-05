using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

// Inicialización de SQLite
Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos y JSON
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlite("Data Source=./store.db"));
builder.Services.Configure<JsonOptions>(opts =>
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Asegurarse de que la base de datos existe y cargue datos iniciales
SeedData(app);

Console.WriteLine("Servidor REST corriendo en http://localhost:5000");
Console.WriteLine("Rutas disponibles:");
Console.WriteLine("- GET  /items");
Console.WriteLine("- GET  /items/restock");
Console.WriteLine("- POST /items/{id}/add");
Console.WriteLine("- POST /items/{id}/remove\n");

// Endpoints
app.MapGet("/items", async (StoreContext db) =>
    await db.Items.ToListAsync());

app.MapGet("/items/restock", async (StoreContext db) =>
    await db.Items.Where(i => i.Quantity < 3).ToListAsync());

app.MapPost("/items/{id}/add", async (int id, HttpRequest req, StoreContext db) =>
{
    var data = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(req.Body);
    if (data == null || !data.TryGetValue("amount", out var amount) || amount <= 0)
        return Results.BadRequest("Se requiere 'amount' mayor a 0");

    var item = await db.Items.FindAsync(id);
    if (item == null)
        return Results.NotFound("Producto no existe");

    item.Quantity += amount;
    await db.SaveChangesAsync();
    Console.WriteLine($"Stock aumentado: {item.Name}, ahora {item.Quantity}");
    return Results.Ok(item);
});

app.MapPost("/items/{id}/remove", async (int id, HttpRequest req, StoreContext db) =>
{
    var data = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(req.Body);
    if (data == null || !data.TryGetValue("amount", out var amount) || amount <= 0)
        return Results.BadRequest("Se requiere 'amount' mayor a 0");

    var item = await db.Items.FindAsync(id);
    if (item == null)
        return Results.NotFound("Producto no existe");

    if (item.Quantity < amount)
        return Results.BadRequest($"Stock insuficiente: {item.Quantity}");

    item.Quantity -= amount;
    await db.SaveChangesAsync();
    Console.WriteLine($"Stock reducido: {item.Name}, ahora {item.Quantity}");
    return Results.Ok(item);
});

app.Run("http://localhost:5000");

// Métodos auxiliares
void SeedData(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<StoreContext>();
    db.Database.EnsureCreated();

    if (!db.Items.Any())
    {
        var initialItems = new[]
        {
            new Item { Name = "MacBook Pro", Price = 15000m, Quantity = 10 },
            new Item { Name = "Mouse logitech", Price = 800m, Quantity = 10 },
            new Item { Name = "AirPods 4", Price = 1200m, Quantity = 10 },
            new Item { Name = "Auriculares Inalambricos", Price = 1000m, Quantity = 10 },
            new Item { Name = "Auriculares", Price = 600m, Quantity = 10 },
            new Item { Name = "Cable USB tipo C", Price = 500m, Quantity = 10 },
            new Item { Name = "Cable USB tipo lightning", Price = 450m, Quantity = 10 },
            new Item { Name = "Funda Iphone X", Price = 800m, Quantity = 10 },
            new Item { Name = "Iphone X", Price = 12000m, Quantity = 10 },
            new Item { Name = "Iphone 7", Price = 7000m, Quantity = 10 }
        };

        db.Items.AddRange(initialItems);
        db.SaveChanges();
        Console.WriteLine("Datos iniciales cargados en la base de datos.");
    }
}

// Contexto EF Core
class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }
    public DbSet<Item> Items => Set<Item>();
}

// Entidad del catálogo
class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}