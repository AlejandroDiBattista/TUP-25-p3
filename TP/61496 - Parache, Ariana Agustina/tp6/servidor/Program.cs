using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    context.Database.Migrate();

    if (!context.Productos.Any())
    {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "iPhone 15", Descripcion = "Smartphone Apple", Precio = 1200, Stock = 10, ImagenUrl = "iphone.jpg" },
            new Producto { Nombre = "Samsung Galaxy S23", Descripcion = "Smartphone Samsung", Precio = 1100, Stock = 15, ImagenUrl = "samsung.jpg" },
            new Producto { Nombre = "Xiaomi Redmi Note 12", Descripcion = "Smartphone Xiaomi", Precio = 500, Stock = 20, ImagenUrl = "xiaomi.jpg" },
            new Producto { Nombre = "Apple AirPods Pro", Descripcion = "Auriculares inalámbricos", Precio = 250, Stock = 30, ImagenUrl = "airpods.jpg" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido", Precio = 30, Stock = 50, ImagenUrl = "cargador.jpg" },
            new Producto { Nombre = "Macbook Pro", Descripcion = "Laptop Apple", Precio = 2200, Stock = 5, ImagenUrl = "macbook.jpg" },
            new Producto { Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico", Precio = 45, Stock = 25, ImagenUrl = "mouse.jpg" },
            new Producto { Nombre = "Teclado mecánico", Descripcion = "Teclado para gamers", Precio = 80, Stock = 15, ImagenUrl = "teclado.jpg" },
            new Producto { Nombre = "Monitor 27\"", Descripcion = "Monitor full HD", Precio = 300, Stock = 10, ImagenUrl = "monitor.jpg" },
            new Producto { Nombre = "Disco SSD 1TB", Descripcion = "Almacenamiento rápido", Precio = 120, Stock = 40, ImagenUrl = "ssd.jpg" }
        };

        context.Productos.AddRange(productos);
        context.SaveChanges();
    }
}


app.MapGet("/productos", async (TiendaContext db, string? query) =>
{
    var productos = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(query))
    {
        productos = productos.Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query));
    }

    return Results.Ok(await productos.ToListAsync());
});

app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});
app.MapGet("/carritos/{id:int}", async (int id, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(item => item.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (carrito == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(carrito);
});


app.MapGet("/", () => "🚀Tienda Online funcionando.");

app.Run();
