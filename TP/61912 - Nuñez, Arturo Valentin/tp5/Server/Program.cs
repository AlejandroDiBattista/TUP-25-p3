using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar Entity Framework Core con SQLite
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlite("Data Source=store.db"));

// Registrar el servicio de CORS
builder.Services.AddCors();

var app = builder.Build();

// Configurar CORS para permitir conexiones desde el cliente
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Endpoints de la API
app.MapGet("/products", async (StoreContext context) =>
{
    var products = await context.Products.ToListAsync();
    return Results.Ok(products);
});

app.MapGet("/products/low-stock", async (StoreContext context) =>
{
    var lowStockProducts = await context.Products
        .Where(p => p.Stock < 3)
        .ToListAsync();
    return Results.Ok(lowStockProducts);
});

app.MapPost("/products/{id}/add-stock", async (int id, int quantity, StoreContext context) =>
{
    var product = await context.Products.FindAsync(id);
    if (product == null)
        return Results.NotFound("Producto no encontrado");

    product.Stock += quantity;
    await context.SaveChangesAsync();
    return Results.Ok(product);
});

app.MapPost("/products/{id}/remove-stock", async (int id, int quantity, StoreContext context) =>
{
    var product = await context.Products.FindAsync(id);
    if (product == null)
        return Results.NotFound("Producto no encontrado");

    if (product.Stock - quantity < 0)
        return Results.BadRequest("No hay suficiente stock disponible");

    product.Stock -= quantity;
    await context.SaveChangesAsync();
    return Results.Ok(product);
});

// Inicializar la base de datos con productos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    context.Database.EnsureCreated();
    
    if (!context.Products.Any())
    {
        var products = new List<Product>
        {
            new Product { Name = "Laptop", Price = 999.99m, Stock = 10 },
            new Product { Name = "Mouse", Price = 25.50m, Stock = 10 },
            new Product { Name = "Teclado", Price = 45.00m, Stock = 10 },
            new Product { Name = "Monitor", Price = 299.99m, Stock = 10 },
            new Product { Name = "Auriculares", Price = 79.99m, Stock = 10 },
            new Product { Name = "Webcam", Price = 89.99m, Stock = 10 },
            new Product { Name = "Impresora", Price = 199.99m, Stock = 10 },
            new Product { Name = "Tablet", Price = 399.99m, Stock = 10 },
            new Product { Name = "Smartphone", Price = 599.99m, Stock = 10 },
            new Product { Name = "Cargador USB", Price = 15.99m, Stock = 10 }
        };
        
        context.Products.AddRange(products);
        context.SaveChanges();
    }
}

app.Run("http://localhost:5000"); 