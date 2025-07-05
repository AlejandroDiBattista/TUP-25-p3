using Microsoft.EntityFrameworkCore;
using Shared.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();


app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}



app.MapGet("/api/productos", async (AppDbContext db, string? q) =>
    string.IsNullOrWhiteSpace(q)
        ? await db.Productos.ToListAsync()
        : await db.Productos.Where(p => p.Nombre.Contains(q)).ToListAsync());

app.MapPost("/api/carrito", async (AppDbContext db, List<CarritoItemDto> items) =>
{
    foreach (var item in items)
    {
        var prod = await db.Productos.FindAsync(item.ProductoId);
        if (prod is null || prod.Stock < item.Cantidad) return Results.BadRequest("Sin stock");
        prod.Stock -= item.Cantidad;
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/api/carritos", () =>
{
    var id = Guid.NewGuid().ToString();
  
    return Results.Ok(id);
});

app.MapGet("/api/carritos/{carrito}", (string carrito) =>
{
    return Results.Ok(new List<CarritoItemDto>());
});

app.MapDelete("/api/carritos/{carrito}", (string carrito) =>
{
    
    return Results.NoContent();
});

app.MapPut("/api/carritos/{carrito}/confirmar",(string carrito, ConfirmacionDto confirmacion) =>
{
    
    return Results.Ok();
});

app.MapPut("/api/carritos/{carrito}/{producto}", (string carrito, int producto, int cantidad) =>
{
    
    return Results.Ok();
});

app.MapDelete("/api/carritos/{carrito}/{producto}", (string carrito, int producto) =>
{
    
    return Results.NoContent();
});

app.Run();



class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductoDto> Productos => Set<ProductoDto>();
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<ProductoDto>().HasData(
            new ProductoDto { Id = 1, Nombre = "Heladera", Descripcion = "Heladera No Frost", Stock = 30, Precio = 120000, ImagenUrl = "Heladera.png" },
            new ProductoDto { Id = 2, Nombre = "Lavarropas", Descripcion = "Lavarropas automático", Stock = 40, Precio = 95000, ImagenUrl = "Lavarropas.png" },
            new ProductoDto { Id = 3, Nombre = "Cocina", Descripcion = "Cocina a gas", Stock = 35, Precio = 85000, ImagenUrl = "Cocina.png" },
            new ProductoDto { Id = 4, Nombre = "Microondas", Descripcion = "Microondas con grill", Stock = 25, Precio = 45000, ImagenUrl = "Microondas.webp" },
            new ProductoDto { Id = 5, Nombre = "Aire Acondicionado", Descripcion = "Aire acondicionado frío/calor", Stock = 15, Precio = 150000, ImagenUrl = "AireAcondicionado.png" }, 
            new ProductoDto { Id = 6, Nombre = "Televisor", Descripcion = "Televisor LED 4K", Stock = 50, Precio = 80000, ImagenUrl = "Televisor.png" },
            new ProductoDto { Id = 7, Nombre = "Aspiradora", Descripcion = "Aspiradora sin bolsa", Stock = 40, Precio = 30000, ImagenUrl = "Aspiradora.png" },
            new ProductoDto { Id = 8, Nombre = "Plancha", Descripcion = "Plancha a vapor", Stock = 25, Precio = 15000, ImagenUrl = "Plancha.png" },
            new ProductoDto { Id = 9, Nombre = "Batidora", Descripcion = "Batidora de mano", Stock = 25, Precio = 10000, ImagenUrl = "Batidora.webp" },
            new ProductoDto { Id = 10, Nombre = "Tostadora", Descripcion = "Tostadora eléctrica", Stock = 35, Precio = 5000, ImagenUrl = "Tostadora.png" },
            new ProductoDto { Id = 11, Nombre = "Cafetera", Descripcion = "Cafetera eléctrica", Stock = 45, Precio = 7000, ImagenUrl = "Cafetera.png" },
            new ProductoDto { Id = 12, Nombre = "Freidora", Descripcion = "Freidora eléctrica", Stock = 40, Precio = 8000, ImagenUrl = "Freidora.png" }
        );
    }
}

public record ConfirmacionDto(List<CarritoItemDto> Items, string Nombre, string Apellido, string Email);