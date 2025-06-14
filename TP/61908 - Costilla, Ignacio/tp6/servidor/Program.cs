using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tienda.db";
builder.Services.AddSqlite<TiendaContext>(connectionString);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowClientApp");

app.MapGet("/productos", async (TiendaContext db, string? search) => {
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(p => p.Nombre.ToLower().Contains(search.ToLower()));
    }
    return await query.ToListAsync();
});

app.MapPost("/carritos", async (TiendaContext db) => {
    var nuevaCompra = new Compra { NombreCliente = "PENDIENTE" };
    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();
    return Results.Ok(nuevaCompra.Id);
});

app.MapGet("/carritos/{id}", async (int id, TiendaContext db) => {
    var carrito = await db.Compras
                           .Include(c => c.Items)
                           .ThenInclude(i => i.Producto)
                           .FirstOrDefaultAsync(c => c.Id == id);

    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    return Results.Ok(carrito.Items);
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, TiendaContext db) => {
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (itemExistente != null)
    {
        if (producto.Stock > 0)
        {
            itemExistente.Cantidad++;
            producto.Stock--;
        }
        else
        {
            return Results.BadRequest("No hay suficiente stock");
        }
    }
    else
    {
        if (producto.Stock > 0)
        {
            carrito.Items.Add(new ItemCompra { ProductoId = productoId, Cantidad = 1, PrecioUnitario = producto.Precio });
            producto.Stock--;
        }
        else
        {
            return Results.BadRequest("No hay suficiente stock");
        }
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, TiendaContext db) => {
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("Item no encontrado en el carrito");

    item.Cantidad--;
    producto.Stock++;

    if (item.Cantidad == 0)
    {
        db.ItemsCompra.Remove(item);
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/carritos/{id}", async (int id, TiendaContext db) => {
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null) producto.Stock += item.Cantidad;
    }

    db.ItemsCompra.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPut("/carritos/{id}/confirmar", async (int id, Compra datosCliente, TiendaContext db) => {
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    if (string.IsNullOrWhiteSpace(datosCliente.NombreCliente) || 
        string.IsNullOrWhiteSpace(datosCliente.ApellidoCliente) || 
        string.IsNullOrWhiteSpace(datosCliente.EmailCliente))
    {
        return Results.BadRequest("Nombre, Apellido y Email son obligatorios.");
    }

    carrito.NombreCliente = datosCliente.NombreCliente;
    carrito.ApellidoCliente = datosCliente.ApellidoCliente;
    carrito.EmailCliente = datosCliente.EmailCliente;
    carrito.Fecha = DateTime.Now;
    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.Ok(carrito.Id);
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Productos.Any())
    {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "iPhone 15 Pro", Descripcion = "Titanio. Tan robusto. Tan ligero. Tan Pro.", Precio = 1999.99m, Stock = 10, ImagenUrl = "" },
            new Producto { Nombre = "Samsung Galaxy S24 Ultra", Descripcion = "El poder de la IA en tus manos.", Precio = 1899.99m, Stock = 15, ImagenUrl = "" },
            new Producto { Nombre = "Google Pixel 8", Descripcion = "La magia de Google en un teléfono.", Precio = 1500.00m, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Cargador USB-C 20W", Descripcion = "Carga rápida y eficiente.", Precio = 29.99m, Stock = 50, ImagenUrl = "" },
            new Producto { Nombre = "Funda de Silicona", Descripcion = "Protección suave y con estilo.", Precio = 39.99m, Stock = 40, ImagenUrl = "" },
            new Producto { Nombre = "Auriculares Inalámbricos", Descripcion = "Sonido inmersivo, sin cables.", Precio = 149.99m, Stock = 30, ImagenUrl = "" },
            new Producto { Nombre = "Smartwatch Pro", Descripcion = "Tu vida conectada en tu muñeca.", Precio = 299.99m, Stock = 25, ImagenUrl = "" },
            new Producto { Nombre = "Tablet Advance 11\"", Descripcion = "Potencia y portabilidad.", Precio = 799.99m, Stock = 12, ImagenUrl = "" },
            new Producto { Nombre = "Protector de Pantalla", Descripcion = "Resistencia contra golpes y arañazos.", Precio = 19.99m, Stock = 100, ImagenUrl = "" },
            new Producto { Nombre = "Batería Externa 10000mAh", Descripcion = "Energía extra para tus dispositivos.", Precio = 49.99m, Stock = 35, ImagenUrl = "" }
        };
        dbContext.Productos.AddRange(productos);
        dbContext.SaveChanges();
    }
}

app.Run();