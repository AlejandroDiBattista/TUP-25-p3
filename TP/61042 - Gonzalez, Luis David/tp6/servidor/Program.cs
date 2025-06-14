using servidor.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
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
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// ENDPOINTS API TIENDA ONLINE

app.MapGet("/api/productos", async (TiendaContext db, string? busqueda) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(busqueda))
        query = query.Where(p => p.Nombre.Contains(busqueda) || p.Descripcion.Contains(busqueda));
    return await query.ToListAsync();
});

app.MapPost("/api/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(carrito.Id);
});

app.MapGet("/api/carritos/{carritoId}", async (TiendaContext db, Guid carritoId) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    var items = from item in carrito.Items
                join prod in db.Productos on item.ProductoId equals prod.Id
                select new {
                    Id = item.Id,
                    ProductoId = prod.Id,
                    Nombre = prod.Nombre,
                    Precio = prod.Precio,
                    ImagenUrl = prod.ImagenUrl,
                    Cantidad = item.Cantidad
                };
    return Results.Ok(items);
});

app.MapDelete("/api/carritos/{carritoId}", async (TiendaContext db, Guid carritoId) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    db.CarritoItems.RemoveRange(carrito.Items);
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPut("/api/carritos/{carritoId}/confirmar", async (TiendaContext db, Guid carritoId, ConfirmacionDto datos) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null || !carrito.Items.Any()) return Results.BadRequest();
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.Nombre,
        ApellidoCliente = datos.Apellido,
        EmailCliente = datos.Email,
        Total = 0
    };
    foreach (var item in carrito.Items)
    {
        var prod = await db.Productos.FindAsync(item.ProductoId);
        if (prod == null || prod.Stock < item.Cantidad) return Results.BadRequest($"Sin stock para {prod?.Nombre}");
        prod.Stock -= item.Cantidad;
        compra.Items.Add(new ItemCompra { ProductoId = prod.Id, Cantidad = item.Cantidad, PrecioUnitario = prod.Precio });
        compra.Total += prod.Precio * item.Cantidad;
    }
    db.Compras.Add(compra);
    db.CarritoItems.RemoveRange(carrito.Items);
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

app.MapPut("/api/carritos/{carritoId}/{productoId}", async (TiendaContext db, Guid carritoId, int productoId, int cantidad) =>
{
    if (cantidad < 1) return Results.BadRequest();
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    var prod = await db.Productos.FindAsync(productoId);
    if (carrito == null || prod == null) return Results.NotFound();
    if (prod.Stock < cantidad) return Results.BadRequest("Sin stock suficiente");
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        carrito.Items.Add(new CarritoItem { ProductoId = productoId, Cantidad = cantidad });
    }
    else
    {
        if (prod.Stock < item.Cantidad + cantidad) return Results.BadRequest("Sin stock suficiente");
        item.Cantidad += cantidad;
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/api/carritos/{carritoId}/{productoId}", async (TiendaContext db, Guid carritoId, int productoId) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();
    db.CarritoItems.Remove(item);
    carrito.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
});

void InicializarBaseDeDatos(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Zapatilla Nike Air Max", Descripcion = "Running, Talle 46", Precio = 120000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Adidas Ultraboost", Descripcion = "Training, Talle 46", Precio = 135000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Puma RS-X", Descripcion = "Casual, Talle 46", Precio = 110000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Converse Chuck Taylor", Descripcion = "Clásica, Talle 46", Precio = 95000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Reebok Classic", Descripcion = "Urbanas, Talle 46", Precio = 98000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Vans Old Skool", Descripcion = "Skate, Talle 46", Precio = 90000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Fila Disruptor", Descripcion = "Moda, Talle 46", Precio = 85000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla New Balance 574", Descripcion = "Running, Talle 46", Precio = 105000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Asics Gel", Descripcion = "Deportiva, Talle 46", Precio = 115000, Stock = 20, ImagenUrl = "" },
            new Producto { Nombre = "Zapatilla Topper Urbana", Descripcion = "Diaria, Talle 46", Precio = 70000, Stock = 20, ImagenUrl = "" }
        });
        db.SaveChanges();
    }
}

InicializarBaseDeDatos(app);

app.Run();
