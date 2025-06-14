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
    new Producto { Nombre = "iPhone 16 Pro", Descripcion = " Tan robusto. Tan ligero. Tan Pro.", Precio = 1200.99m, Stock = 10, ImagenUrl = "https://www.forbes.com.au/wp-content/uploads/2024/09/Apple-iPhone-16-Pro-finish-lineup-240909.jpg?w=1024" },
    new Producto { Nombre = "Samsung Galaxy S24 Ultra", Descripcion = "El poder de la IA en tus manos.", Precio = 900.99m, Stock = 15, ImagenUrl = "https://www.cordobadigital.net/wp-content/uploads/2024/08/S24-Ultra-black.webp" },
    new Producto { Nombre = "Google Pixel 8", Descripcion = "La magia de Google en un teléfono.", Precio = 500.00m, Stock = 20, ImagenUrl = "https://images.tcdn.com.br/img/img_prod/625110/google_pixel_8_pro_512gb_porcelain_desbloqueado_5859_1_246782dbd96a858c180ca4edfc527fde.jpg" },
    new Producto { Nombre = "Cargador USB-C 20W", Descripcion = "Carga rápida y eficiente.", Precio = 29.99m, Stock = 50, ImagenUrl = "https://m.media-amazon.com/images/I/51qIsW21sHL._SL1500_.jpg" },
    new Producto { Nombre = "Funda de Silicona", Descripcion = "Protección suave y con estilo.", Precio = 20.99m, Stock = 40, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/643/020/products/silicone-case-con-logo-iphone-16-pro-roja-22f5de7476a561e7de17260731914622-1024-1024.png" },
    new Producto { Nombre = "Auriculares Inalámbricos", Descripcion = "Sonido inmersivo, sin cables.", Precio = 149.99m, Stock = 30, ImagenUrl = "https://kanji.com.ar/wp-content/uploads/2024/03/KJ-AUBT001-negro.jpg" },
    new Producto { Nombre = "Smartwatch Pro", Descripcion = "Tu vida conectada en tu muñeca.", Precio = 299.99m, Stock = 25, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/145/546/products/i8-pro-max-final-f8c3a4279b5cd327ce17010981120048-1024-1024.jpg" },
    new Producto { Nombre = "Ipad pro 11\"", Descripcion = "Potencia y portabilidad.", Precio = 799.99m, Stock = 12, ImagenUrl = "https://www.apple.com/v/ipad-air/s/images/overview/design/colors__bmip5mc8xueu_large.jpg" },
    new Producto { Nombre = "Apple TV 4K", Descripcion = "Mejor definicion y rapidez.", Precio = 300.99m, Stock = 100, ImagenUrl = "https://www.apple.com/v/apple-tv-4k/ak/images/meta/apple-tv-4k__efpszaiqoh2e_og.png" },
    new Producto { Nombre = "Batería Externa 10000mAh", Descripcion = "Energía extra para tus dispositivos.", Precio = 49.99m, Stock = 35, ImagenUrl = "https://ipoint.pe/wp-content/uploads/2024/09/10-2.jpg" }
};

        dbContext.Productos.AddRange(productos);
        dbContext.SaveChanges();
    }
}

app.Run();