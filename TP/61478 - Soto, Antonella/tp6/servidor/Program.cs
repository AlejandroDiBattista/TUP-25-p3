using Microsoft.EntityFrameworkCore;
using Servidor.Models;
using Servidor.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Crear la base de datos y cargar productos iniciales
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();

    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Base de maquillaje", Descripcion = "Base líquida mate", Precio = 4500, Stock = 15, ImagenUrl = "http://localhost:5184/basemaquillaje.webp" },
            new Producto { Nombre = "Labial rojo", Descripcion = "Color intenso", Precio = 2500, Stock = 30, ImagenUrl = "http://localhost:5184/imagenes/labiosrojos.jpg" },
            new Producto { Nombre = "Delineador líquido", Descripcion = "Punta fina, resistente al agua", Precio = 1800, Stock = 20, ImagenUrl = "http://localhost:5184/imagenes/delineador.webp" },
            new Producto { Nombre = "Rubor coral", Descripcion = "Rubor en polvo tono coral", Precio = 3200, Stock = 25, ImagenUrl = "http://localhost:5184/imagenes/rubor.webp" },
            new Producto { Nombre = "Máscara de pestañas", Descripcion = "Volumen extremo", Precio = 2800, Stock = 18, ImagenUrl = "http://localhost:5184/imagenes/mascara.jpg" },
            new Producto { Nombre = "Paleta de sombras", Descripcion = "12 tonos nude", Precio = 6500, Stock = 10, ImagenUrl = "http://localhost:5184/imagenes/paleta.webp" },
            new Producto { Nombre = "Corrector", Descripcion = "Alta cobertura", Precio = 2200, Stock = 22, ImagenUrl = "http://localhost:5184/imagenes/corrector.webp" },
            new Producto { Nombre = "Iluminador", Descripcion = "Brillo natural", Precio = 2700, Stock = 16, ImagenUrl = "http://localhost:5184/imagenes/iluminador.webp" },
            new Producto { Nombre = "Esmalte nude", Descripcion = "Tono duradero", Precio = 1500, Stock = 40, ImagenUrl = "http://localhost:5184/imagenes/esmalte.jpeg" },
            new Producto { Nombre = "Brocha", Descripcion = "Cerdas suaves", Precio = 3500, Stock = 12, ImagenUrl = "http://localhost:5184/imagenes/brochas.jpg" }
        );
        db.SaveChanges();
    }
}

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoints
app.MapGet("/productos", async (TiendaContext db, string? nombre) =>
{
    if (string.IsNullOrWhiteSpace(nombre))
    {
        return await db.Productos.ToListAsync();
    }

    return await db.Productos
        .Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()))
        .ToListAsync();
});

app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(new { carrito.Id });
});

app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (
    TiendaContext db,
    int carritoId,
    int productoId,
    int cantidad) =>
{
    if (cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor que cero.");

    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado.");

    if (producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        item = new ItemCarrito
        {
            ProductoId = productoId,
            Cantidad = cantidad
        };
        carrito.Items.Add(item);
    }
    else
    {
        item.Cantidad += cantidad;
    }

    producto.Stock -= cantidad;

    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

app.MapGet("/carritos/{carritoId:int}", async (TiendaContext db, int carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");

    return Results.Ok(carrito);
});

app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (
    TiendaContext db,
    int carritoId,
    int productoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no está en el carrito.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += item.Cantidad;
    }

    carrito.Items.Remove(item);
    db.ItemsCarrito.Remove(item);

    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

app.MapPut("/carritos/{carritoId:int}/confirmar", async (
    TiendaContext db,
    int carritoId,
    string nombre,
    string apellido,
    string email) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null || carrito.Items.Count == 0)
        return Results.BadRequest("El carrito no existe o está vacío.");

    var total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio);

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = nombre,
        ApellidoCliente = apellido,
        EmailCliente = email,
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };

    db.Compras.Add(compra);

    db.ItemsCarrito.RemoveRange(carrito.Items);
    carrito.Items.Clear();

    await db.SaveChangesAsync();

    return Results.Ok(new { compra.Id, compra.Total, compra.Fecha });
});

app.MapDelete("/carritos/{carritoId:int}", async (TiendaContext db, int carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");

    foreach (var item in carrito.Items)
    {
        item.Producto.Stock += item.Cantidad;
    }

    db.ItemsCarrito.RemoveRange(carrito.Items);
    carrito.Items.Clear();

    await db.SaveChangesAsync();

    return Results.Ok(new { mensaje = "Carrito vaciado correctamente." });
});

app.MapGet("/compras", async (TiendaContext db) =>
{
    var compras = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .ToListAsync();

    return Results.Ok(compras);
});

app.MapGet("/compras/{id:int}", async (TiendaContext db, int id) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (compra == null)
        return Results.NotFound("Compra no encontrada.");

    return Results.Ok(compra);
});

// Para servir archivos estáticos (imágenes)
app.UseStaticFiles();

app.Run();
