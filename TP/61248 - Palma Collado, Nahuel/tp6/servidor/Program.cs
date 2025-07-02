using Microsoft.EntityFrameworkCore;
using TiendaOnline.Datos;
using TiendaOnline.Modelos; 
using TiendaOnline.DTOs;
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrar el DbContext con SQLite 
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tiendaonline.db"));
    
// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    DbInitializer.Inicializar(db);
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoint para productos con búsqueda 
app.MapGet("/api/productos", async (TiendaDbContext db, string? busqueda) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrEmpty(busqueda))
    {
        query = query.Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower()));
                                 
    }
    
    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

// Endpoint para crear un nuevo carrito
app.MapPost("/api/carritos", async (TiendaDbContext db) =>
{
    var nuevaCompra = new Compra
    {
        Total = 0,
    };

    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();

    return Results.Ok(new { CarritoId = nuevaCompra.Id });
});

// Endpoint para obtener el contenido de un carrito
app.MapGet("/api/carritos/{carritoId}", async (int carritoId, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.ItemsDeCompra)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra is null)
        return Results.NotFound("Carrito no encontrado :(");

    var resultado = compra.ItemsDeCompra.Select(item => new
    {
        item.ProductoId,
        Nombre = item.Producto!.Nombre,
        PrecioUnitario = item.PrecioUnitario,
        Cantidad = item.Cantidad,
        Importe = item.PrecioUnitario * item.Cantidad
    });

    return Results.Ok(resultado);
});

// Endpoint para interactuar con los productos del carrito
app.MapPut("/api/carritos/{carritoId:int}/{productoId:int}", async (
    int carritoId,
    int productoId,
    TiendaDbContext db) =>
{
    var carrito = await db.Compras
        .Include(c => c.ItemsDeCompra)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound("Carrito no encontrado :(");

    var producto = await db.Productos.FindAsync(productoId);

    if (producto is null)
        return Results.NotFound("Producto no encontrado :(");

    if (producto.Stock < 1)
        return Results.BadRequest("No hay stock disponible :(");

    var itemExistente = carrito.ItemsDeCompra.FirstOrDefault(i => i.ProductoId == productoId);

    if (itemExistente is not null)
    {
        if (producto.Stock < itemExistente.Cantidad + 1)
            return Results.BadRequest("¡No hay más stock!");

        itemExistente.Cantidad += 1;
    }
    else
    {
        carrito.ItemsDeCompra.Add(new ItemDeCompra
        {
            ProductoId = productoId,
            Cantidad = 1,
            PrecioUnitario = producto.Precio
        });
    }

    producto.Stock -= 1;

    await db.SaveChangesAsync();

    return Results.Ok("Producto agregado al carrito :D");
});

// Endpoint para eliminar un producto del carrito o reducir su cantidad
app.MapDelete("/api/carritos/{carritoId:int}/{productoId:int}", async (
    int carritoId,
    int productoId,
    TiendaDbContext db) =>
{
    var carrito = await db.Compras
        .Include(c => c.ItemsDeCompra)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound("Carrito no encontrado :(");

    var item = carrito.ItemsDeCompra.FirstOrDefault(i => i.ProductoId == productoId);

    if (item is null)
        return Results.NotFound("El producto no está en el carrito :(");

    var producto = await db.Productos.FindAsync(productoId);

    if (producto is null)
        return Results.NotFound("Producto no encontrado en el inventario.");

    // Aumento en el stock porque vuelve al inventario
    producto.Stock += 1;

    if (item.Cantidad > 1)
    {
        item.Cantidad -= 1;
    }
    else
    {
        carrito.ItemsDeCompra.Remove(item);
    }

    await db.SaveChangesAsync();

    return Results.Ok("Producto eliminado :)");
});

// Endpoint para vaciar el carrito
app.MapDelete("/api/carritos/{carritoId:int}", async (int carritoId, TiendaDbContext db) =>
{
    var carrito = await db.Compras
        .Include(c => c.ItemsDeCompra)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound("Carrito no encontrado :(");

    foreach (var item in carrito.ItemsDeCompra)
    {
        if (item.Producto is not null)
        {
            item.Producto.Stock += item.Cantidad;
        }
    }

    carrito.ItemsDeCompra.Clear();
    await db.SaveChangesAsync();

    return Results.Ok("¡Carrito vaciado correctamente! :D");
});

// Endpoint para confirmar la compra
app.MapPut("/api/carritos/{carritoId:int}/confirmar", async (
    int carritoId,
    ConfirmacionDTO datosCliente,
    TiendaDbContext db) =>
{
    var carrito = await db.Compras
        .Include(c => c.ItemsDeCompra)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound("Carrito no encontrado :(");

    if (!carrito.ItemsDeCompra.Any())
        return Results.BadRequest("El carrito está vacío :(");

    // Acá se completan los datos del cliente
    carrito.NombreCliente = datosCliente.Nombre;
    carrito.ApellidoCliente = datosCliente.Apellido;
    carrito.EmailCliente = datosCliente.Email;
    carrito.Fecha = DateTime.Now;
    carrito.Total = carrito.ItemsDeCompra.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();

    return Results.Ok("Compra confirmada correctamente. ¡Muchas gracias! :D");
});




app.Run();
