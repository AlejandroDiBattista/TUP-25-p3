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
// Este bloque se ejecuta al iniciar la aplicación y carga 10 productos de ejemplo si la base está vacía.
// Cada producto tiene nombre, descripción, precio, stock e imagen representativa.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            // Producto 1: Celular Samsung
            new Producto { Nombre = "Samsung Galaxy S23", Descripcion = "Smartphone de última generación con cámara de 108MP", Precio = 299999.99M, Stock = 10, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/sm-s911bzklaro/gallery/ar-galaxy-s23-s911-sm-s911bzklaro-thumb-534863401" },
            // Producto 2: iPhone
            new Producto { Nombre = "iPhone 15 Pro", Descripcion = "Apple iPhone 15 Pro 256GB", Precio = 399999.99M, Stock = 8, ImagenUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-15-pro-model-unselect-gallery-1-202309?wid=512&hei=512&fmt=jpeg&qlt=95&.v=1692923778665" },
            // Producto 3: Consola PlayStation
            new Producto { Nombre = "PlayStation 5", Descripcion = "Consola Sony PlayStation 5 Digital Edition", Precio = 499999.99M, Stock = 5, ImagenUrl = "https://www.sony.com/image/ps5-digital-edition.png" },
            // Producto 4: Smart TV
            new Producto { Nombre = "Smart TV Samsung 50''", Descripcion = "Televisor 4K UHD Smart TV 50 pulgadas", Precio = 350000.00M, Stock = 6, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/ua50au7000gxcz/gallery/ar-uhd-au7000-ua50au7000gxcz-530183401" },
            // Producto 5: Notebook
            new Producto { Nombre = "Notebook Lenovo IdeaPad 3", Descripcion = "Notebook 15.6'' Ryzen 5 8GB 512GB SSD", Precio = 420000.00M, Stock = 7, ImagenUrl = "https://www.lenovo.com/medias/ideapad-3-15alc6-hero.png" },
            // Producto 6: Auriculares
            new Producto { Nombre = "Auriculares Bluetooth JBL", Descripcion = "Auriculares inalámbricos JBL Tune 510BT", Precio = 29999.99M, Stock = 20, ImagenUrl = "https://www.jbl.com.ar/on/demandware.static/-/Sites-masterCatalog_Harman/default/dw7e2e2e2e/JBL_TUNE510BT_Product%20Image_Hero_Black.png" },
            // Producto 7: Cargador
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido 25W", Precio = 7999.99M, Stock = 30, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/ep-ta800nwegaro/gallery/ar-ep-ta800nwegaro-ep-ta800nwegaro-530183401" },
            // Producto 8: Funda
            new Producto { Nombre = "Funda para iPhone 15", Descripcion = "Funda de silicona original Apple", Precio = 14999.99M, Stock = 25, ImagenUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MQ0E3?wid=2000&hei=2000&fmt=jpeg&qlt=95&.v=1660803972361" },
            // Producto 9: Teclado
            new Producto { Nombre = "Teclado Logitech K380", Descripcion = "Teclado Bluetooth multidispositivo", Precio = 24999.99M, Stock = 12, ImagenUrl = "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/keyboards/k380/gallery/k380-rose-gallery-1-us.png?v=1" },
            // Producto 10: Mouse
            new Producto { Nombre = "Mouse Logitech M185", Descripcion = "Mouse inalámbrico compacto", Precio = 9999.99M, Stock = 18, ImagenUrl = "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/m185/gallery/m185-grey-gallery-1.png?v=1" }
        );
        db.SaveChanges();
    }
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

// Endpoint para crear un nuevo carrito
// POST /carritos
app.MapPost("/carritos", (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    db.SaveChanges();
    return Results.Ok(new { carrito.Id });
});

// Endpoint para obtener los ítems de un carrito
// GET /carritos/{carritoId}
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
app.MapPut("/carritos/{carritoId}/{productoId}", (TiendaContext db, int carritoId, int productoId, int cantidad) =>
{
    if (cantidad < 1) return Results.BadRequest("La cantidad debe ser mayor a cero.");
    var carrito = db.Carritos.Include(c => c.Items).FirstOrDefault(c => c.Id == carritoId);
    var producto = db.Productos.FirstOrDefault(p => p.Id == productoId);
    if (carrito is null || producto is null) return Results.NotFound();
    if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente.");
    var item = db.ItemsCarrito.FirstOrDefault(i => i.CarritoId == carritoId && i.ProductoId == productoId);
    if (item is null)
    {
        item = new ItemCarrito { CarritoId = carritoId, ProductoId = productoId, Cantidad = cantidad };
        db.ItemsCarrito.Add(item);
    }
    else
    {
        item.Cantidad = cantidad;
    }
    db.SaveChanges();
    return Results.Ok();
});

// Endpoint para quitar un producto del carrito
// DELETE /carritos/{carritoId}/{productoId}
app.MapDelete("/carritos/{carritoId}/{productoId}", (TiendaContext db, int carritoId, int productoId) =>
{
    var item = db.ItemsCarrito.FirstOrDefault(i => i.CarritoId == carritoId && i.ProductoId == productoId);
    if (item is null) return Results.NotFound();
    db.ItemsCarrito.Remove(item);
    db.SaveChanges();
    return Results.Ok();
});

app.Run();
