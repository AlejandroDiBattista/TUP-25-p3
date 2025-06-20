using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Auriculares Bluetooth", Descripcion = "Auriculares inalámbricos con cancelación de ruido", Imagen = "auriculares.jpg", Precio = 12000, Stock = 15 },
            new Producto { Nombre = "Mouse Gamer", Descripcion = "Mouse con luces RGB y alta precisión", Imagen = "mouse.jpg", Precio = 8500, Stock = 25 },
            new Producto { Nombre = "Teclado Mecánico", Descripcion = "Teclado mecánico retroiluminado", Imagen = "teclado.jpg", Precio = 18000, Stock = 10 }
        );
        db.SaveChanges();
    }
}

app.MapGet("/productos", async (AppDbContext db) =>
{
    return await db.Productos.ToListAsync();
});

app.Run();

app.MapPost("/compras", async (AppDbContext db, Compra compra) =>
{
    // Calcular total y asociar producto real
    decimal total = 0;

    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);

        if (producto == null || producto.Stock < item.Cantidad)
        {
            return Results.BadRequest("Producto no válido o sin stock.");
        }

        producto.Stock -= item.Cantidad;
        item.PrecioUnitario = producto.Precio;
        total += item.PrecioUnitario * item.Cantidad;
    }

    compra.Total = total;
    compra.Fecha = DateTime.Now;

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    return Results.Created($"/compras/{compra.Id}", compra);
});
