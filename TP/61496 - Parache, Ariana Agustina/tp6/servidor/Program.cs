using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);



// Agrego el DbContext
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));
    

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// Aquí hago el seeding:
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
app.MapPut("/carritos/{id:int}/{productoId:int}", async (int id, int productoId, int cantidad, TiendaContext db) =>
{
    if (cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a 0");

    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);

    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.Producto.Id == productoId);

    if (item != null)
    {
        // Si ya existe, sumamos la cantidad
        if (producto.Stock < item.Cantidad + cantidad)
            return Results.BadRequest("No hay suficiente stock");

        item.Cantidad += cantidad;
    }
    else
    {
        // Si no existe, validamos stock y lo agregamos
        if (producto.Stock < cantidad)
            return Results.BadRequest("No hay suficiente stock");

        item = new ItemCarrito
        {
            Producto = producto,
            Cantidad = cantidad
        };

        carrito.Items.Add(item);
    }

    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});
app.MapDelete("/carritos/{id:int}/{productoId:int}", async (int id, int productoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");

    carrito.Items.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});
app.MapGet("/carritos/{id:int}/total", async (int id, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var total = carrito.Items.Sum(item => item.Producto.Precio * item.Cantidad);

    return Results.Ok(new { Total = total });
});



app.MapGet("/", () => "🚀Tienda Online funcionando.");

app.Run();
