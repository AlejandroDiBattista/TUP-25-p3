using Microsoft.EntityFrameworkCore;
using Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar contexto de base de datos con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Seed de productos de ejemplo
// Este bloque se ejecuta al iniciar la aplicación y carga o actualiza 10 productos de ejemplo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    var productosSeed = new List<Producto>
    {
        new Producto { Nombre = "Samsung Galaxy S23", Descripcion = "Smartphone de última generación con cámara de 108MP", Precio = 299999.99M, Stock = 10, ImagenUrl = "img/samsung-galaxy-s23-1.png" },
        new Producto { Nombre = "iPhone 15 Pro", Descripcion = "Apple iPhone 15 Pro 256GB", Precio = 399999.99M, Stock = 8, ImagenUrl = "img/65038654434d0-iPhone 15 Pro Natural titanium png.png" },
        new Producto { Nombre = "PlayStation 5", Descripcion = "Consola Sony PlayStation 5 Digital Edition", Precio = 499999.99M, Stock = 5, ImagenUrl = "img/ps5-pro-dualsense-image-block-01-en-16aug24.png" },
        new Producto { Nombre = "Smart TV Samsung 50''", Descripcion = "Televisor 4K UHD Smart TV 50 pulgadas", Precio = 350000.00M, Stock = 6, ImagenUrl = "img/smarttv.png" },
        new Producto { Nombre = "Notebook Lenovo IdeaPad 3", Descripcion = "Notebook 15.6'' Ryzen 5 8GB 512GB SSD", Precio = 420000.00M, Stock = 7, ImagenUrl = "img/IdeaPad_Slim_3_15IAH8_CT1_03.png" },
        new Producto { Nombre = "Auriculares Bluetooth JBL", Descripcion = "Auriculares inalámbricos JBL Tune 510BT", Precio = 29999.99M, Stock = 20, ImagenUrl = "img/jblbluetooth.png" },
        new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido 25W", Precio = 7999.99M, Stock = 30, ImagenUrl = "img/cargador-de-220v-a-usb-samsung-cable-15w-type-c-ep-t1510xbsgar-negro-2.png" },
        new Producto { Nombre = "Funda para iPhone 15", Descripcion = "Funda de silicona original Apple", Precio = 14999.99M, Stock = 25, ImagenUrl = "img/iphone15funda.png" },
        new Producto { Nombre = "Teclado Logitech K380", Descripcion = "Teclado Bluetooth multidispositivo", Precio = 24999.99M, Stock = 12, ImagenUrl = "img/k380-lavender-gallery-1-deu.png" },
        new Producto { Nombre = "Mouse Logitech M185", Descripcion = "Mouse inalámbrico compacto", Precio = 9999.99M, Stock = 18, ImagenUrl = "img/logim185.png" }
    };

    foreach (var prodSeed in productosSeed)
    {
        var existente = db.Productos.FirstOrDefault(p => p.Nombre == prodSeed.Nombre);
        if (existente != null)
        {
            // Actualiza los datos del producto existente
            existente.Descripcion = prodSeed.Descripcion;
            existente.Precio = prodSeed.Precio;
            existente.Stock = prodSeed.Stock;
            existente.ImagenUrl = prodSeed.ImagenUrl;
        }
        else
        {
            db.Productos.Add(prodSeed);
        }
    }
    db.SaveChanges();
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoint para obtener el listado de productos y permitir búsqueda por nombre o descripción
// Ejemplo de uso: GET /productos?query=notebook
app.MapGet("/productos", (TiendaContext db, string? query) =>
{
    // Si no se envía query, devuelve todos los productos
    if (string.IsNullOrWhiteSpace(query))
        return db.Productos.ToList();
    // Si se envía query, filtra por nombre o descripción (no sensible a mayúsculas)
    query = query.ToLower();
    return db.Productos
        .Where(p => p.Nombre.ToLower().Contains(query) || p.Descripcion.ToLower().Contains(query))
        .ToList();
});

// -----------------------------
// API de Tienda Online - Minimal API
// Todos los endpoints están documentados para facilitar la defensa en clase.
// -----------------------------

// Endpoint para crear un nuevo carrito
// POST /carritos
// Crea un carrito vacío y devuelve su Id para que el cliente lo use en las siguientes operaciones.
app.MapPost("/carritos", (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    db.SaveChanges();
    return Results.Ok(new { carrito.Id });
});

// Endpoint para obtener los ítems de un carrito
// GET /carritos/{carritoId}
// Devuelve los productos y cantidades que tiene el carrito indicado por Id.
app.MapGet("/carritos/{carritoId}", (TiendaContext db, int carritoId) =>
{
    var carrito = db.Carritos
        .Where(c => c.Id == carritoId)
        .Select(c => new {
            c.Id,
            Items = c.Items.Select(i => new {
                i.Id,
                i.ProductoId,
                i.Producto.Nombre,
                i.Producto.Precio,
                i.Cantidad,
                i.Producto.ImagenUrl
            })
        })
        .FirstOrDefault();
    return carrito is null ? Results.NotFound() : Results.Ok(carrito);
});

// Endpoint para vaciar un carrito
// DELETE /carritos/{carritoId}
// Elimina todos los ítems del carrito, dejándolo vacío.
app.MapDelete("/carritos/{carritoId}", (TiendaContext db, int carritoId) =>
{
    var carrito = db.Carritos.Include(c => c.Items).FirstOrDefault(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound();
    db.ItemsCarrito.RemoveRange(carrito.Items);
    db.SaveChanges();
    return Results.Ok();
});

// Endpoint para agregar o actualizar un producto en el carrito
// PUT /carritos/{carritoId}/{productoId}
// Agrega un producto al carrito o actualiza la cantidad si ya existe. Valida stock disponible.
app.MapPut("/carritos/{carritoId}/{productoId}", (TiendaContext db, int carritoId, int productoId, int cantidad) =>
{
    var carrito = db.Carritos.Include(c => c.Items).FirstOrDefault(c => c.Id == carritoId);
    var producto = db.Productos.FirstOrDefault(p => p.Id == productoId);
    if (carrito is null || producto is null) return Results.NotFound();
    var item = db.ItemsCarrito.FirstOrDefault(i => i.CarritoId == carritoId && i.ProductoId == productoId);
    int cantidadFinal = cantidad;
    int cantidadOriginal = item?.Cantidad ?? 0;
    if (item != null)
    {
        cantidadFinal = item.Cantidad + cantidad;
    }
    // Si la cantidad final es menor a 1, eliminar el item y devolver stock
    if (cantidadFinal < 1)
    {
        if (item != null)
        {
            producto.Stock += item.Cantidad; // Devolver stock
            db.ItemsCarrito.Remove(item);
            db.SaveChanges();
        }
        return Results.Ok();
    }
    int diferencia = cantidadFinal - cantidadOriginal;
    if (diferencia > 0)
    {
        if (producto.Stock < diferencia) return Results.BadRequest("Stock insuficiente.");
        producto.Stock -= diferencia;
    }
    else if (diferencia < 0)
    {
        producto.Stock += -diferencia;
    }
    if (item is null)
    {
        item = new ItemCarrito { CarritoId = carritoId, ProductoId = productoId, Cantidad = cantidad };
        db.ItemsCarrito.Add(item);
    }
    else
    {
        item.Cantidad = cantidadFinal;
    }
    db.SaveChanges();
    return Results.Ok();
});

// Endpoint para quitar un producto del carrito
// DELETE /carritos/{carritoId}/{productoId}
// Elimina un producto específico del carrito.
app.MapDelete("/carritos/{carritoId}/{productoId}", (TiendaContext db, int carritoId, int productoId) =>
{
    var item = db.ItemsCarrito.FirstOrDefault(i => i.CarritoId == carritoId && i.ProductoId == productoId);
    var producto = db.Productos.FirstOrDefault(p => p.Id == productoId);
    if (item is null || producto is null) return Results.NotFound();
    producto.Stock += item.Cantidad; // Devolver stock
    db.ItemsCarrito.Remove(item);
    db.SaveChanges();
    return Results.Ok();
});

// Endpoint para confirmar la compra y limpiar el carrito
// POST /carritos/{carritoId}/confirmar
// Registra la compra, descuenta stock y limpia el carrito. Recibe datos del cliente.
app.MapPost("/carritos/{carritoId}/confirmar", (TiendaContext db, int carritoId, DatosClienteDto datos) =>
{
    // Validar que nombre y apellido no contengan números
    if (string.IsNullOrWhiteSpace(datos.Nombre) || string.IsNullOrWhiteSpace(datos.Apellido))
        return Results.BadRequest("Nombre y apellido son obligatorios.");
    if (datos.Nombre.Any(char.IsDigit) || datos.Apellido.Any(char.IsDigit))
        return Results.BadRequest("Nombre y apellido no pueden contener números.");

    // Buscar el carrito con sus ítems y productos
    var carrito = db.Carritos
        .Where(c => c.Id == carritoId)
        .Select(c => new {
            c.Id,
            Items = c.Items.Select(i => new {
                i.ProductoId,
                i.Cantidad,
                Producto = i.Producto
            }).ToList()
        })
        .FirstOrDefault();
    if (carrito is null || !carrito.Items.Any())
        return Results.BadRequest("Carrito vacío o no encontrado");

    // Ya no se valida stock aquí, porque el stock ya fue descontado al agregar al carrito

    // Calcular total
    decimal total = carrito.Items.Sum(i => i.Producto.Precio * i.Cantidad);

    // Registrar la compra
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = datos.Nombre,
        ApellidoCliente = datos.Apellido,
        EmailCliente = datos.Email,
        ItemsCompra = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };
    db.Compras.Add(compra);

    // Ya no se descuenta stock aquí, porque se descuenta al agregar al carrito

    // Limpiar el carrito
    var itemsCarrito = db.ItemsCarrito.Where(i => i.CarritoId == carritoId);
    db.ItemsCarrito.RemoveRange(itemsCarrito);
    db.SaveChanges();

    return Results.Ok(new { compra.Id, compra.Total });
});

app.Run();
