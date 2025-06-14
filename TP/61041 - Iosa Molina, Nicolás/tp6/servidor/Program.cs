using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Crear la base de datos y cargar datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "iPhone 15", Descripcion = "Celular Apple", Precio = 1200, Stock = 10, ImagenUrl = "https://store.storeimages.cdn-apple.com/iphone15.jpg" },
            new Producto { Nombre = "Samsung Galaxy S24", Descripcion = "Celular Samsung", Precio = 1100, Stock = 8, ImagenUrl = "https://images.samsung.com/galaxy-s24.jpg" },
            new Producto { Nombre = "Xiaomi Redmi Note 13", Descripcion = "Celular Xiaomi", Precio = 400, Stock = 15, ImagenUrl = "https://xiaomi.com/redmi-note-13.jpg" },
            new Producto { Nombre = "Auriculares JBL", Descripcion = "Auriculares inalámbricos", Precio = 150, Stock = 20, ImagenUrl = "https://jbl.com/auriculares.jpg" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido", Precio = 30, Stock = 50, ImagenUrl = "https://images.cargadorusb-c.jpg" },
            new Producto { Nombre = "Coca Cola 2L", Descripcion = "Gaseosa 2 litros", Precio = 3, Stock = 100, ImagenUrl = "https://cocacola.com/2l.jpg" },
            new Producto { Nombre = "Pepsi 2L", Descripcion = "Gaseosa 2 litros", Precio = 2.8, Stock = 90, ImagenUrl = "https://pepsi.com/2l.jpg" },
            new Producto { Nombre = "Funda iPhone 15", Descripcion = "Funda silicona", Precio = 25, Stock = 30, ImagenUrl = "https://fundas.com/iphone15.jpg" },
            new Producto { Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico", Precio = 40, Stock = 25, ImagenUrl = "https://logitech.com/mouse.jpg" },
            new Producto { Nombre = "Teclado Redragon", Descripcion = "Teclado mecánico", Precio = 60, Stock = 18, ImagenUrl = "https://redragon.com/teclado.jpg" }
        );
        db.SaveChanges();
    }
}

// Variable global para múltiples carritos (uno por id)
var carritos = new ConcurrentDictionary<string, List<ItemCarrito>>();

app.MapGet("/productos", async (TiendaContext db) =>
{
    return await db.Productos.ToListAsync();
});

app.MapGet("/productos/buscar/{texto}", async (TiendaContext db, string texto) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(texto))
    {
        query = query.Where(p => p.Nombre.Contains(texto) || p.Descripcion.Contains(texto));
    }
    return await query.ToListAsync();
});

// ENDPOINTS PARA CARRITOS

// POST /carritos - Inicializa un carrito y devuelve el id
app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid().ToString();
    carritos[id] = new List<ItemCarrito>();
    return Results.Ok(new { carrito = id });
});

// GET /carritos/{carrito} - Trae los ítems del carrito
app.MapGet("/carritos/{carrito}", (string carrito) =>
{
    if (!carritos.TryGetValue(carrito, out var items))
        return Results.NotFound("Carrito no encontrado");
    return Results.Ok(items);
});

// DELETE /carritos/{carrito} - Vacía el carrito
app.MapDelete("/carritos/{carrito}", (string carrito) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    carritos[carrito] = new List<ItemCarrito>();
    return Results.Ok();
});

// PUT /carritos/{carrito}/confirmar - Confirma la compra
app.MapPut("/carritos/{carrito}/confirmar", async (
    string carrito,
    TiendaContext db,
    Dictionary<string, string> datos
) =>
{
    if (!carritos.TryGetValue(carrito, out var items) || items.Count == 0)
        return Results.BadRequest("Carrito vacío o no encontrado");

    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para producto {item.ProductoId}");
    }

    double total = 0;
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.ContainsKey("nombre") ? datos["nombre"] : "",
        ApellidoCliente = datos.ContainsKey("apellido") ? datos["apellido"] : "",
        EmailCliente = datos.ContainsKey("email") ? datos["email"] : "",
        Items = new List<ItemCarrito>()
    };

    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
        total += producto.Precio * item.Cantidad;
        compra.Items.Add(new ItemCarrito
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        });
    }
    compra.Total = total;
    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carritos[carrito] = new List<ItemCarrito>();
    return Results.Ok(new { compra.Id, compra.Total });
});

// PUT /carritos/{carrito}/{producto} - Agrega o actualiza cantidad de un producto en el carrito
app.MapPut("/carritos/{carrito}/{producto}", async (string carrito, int producto, int cantidad, TiendaContext db) =>
{
    if (cantidad <= 0)
        return Results.BadRequest("Cantidad debe ser mayor a cero");

    var prod = await db.Productos.FindAsync(producto);
    if (prod == null)
        return Results.NotFound("Producto no encontrado");

    if (prod.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente");

    var items = carritos.GetOrAdd(carrito, _ => new List<ItemCarrito>());
    var item = items.FirstOrDefault(i => i.ProductoId == producto);
    if (item == null)
        items.Add(new ItemCarrito { ProductoId = producto, Cantidad = cantidad, PrecioUnitario = prod.Precio });
    else
        item.Cantidad = cantidad;

    return Results.Ok(items);
});

// DELETE /carritos/{carrito}/{producto} - Elimina o reduce cantidad de un producto en el carrito
app.MapDelete("/carritos/{carrito}/{producto}", (string carrito, int producto, int cantidad = 0) =>
{
    if (!carritos.TryGetValue(carrito, out var items))
        return Results.NotFound("Carrito no encontrado");

    var item = items.FirstOrDefault(i => i.ProductoId == producto);
    if (item == null)
        return Results.NotFound("Producto no está en el carrito");

    if (cantidad <= 0 || cantidad >= item.Cantidad)
        items.Remove(item);
    else
        item.Cantidad -= cantidad;

    return Results.Ok(items);
});

app.Run();

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public double Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public double Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCarrito> Items { get; set; }
}

public class ItemCarrito
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int CompraId { get; set; }
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
}

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCarrito> ItemsCarrito { get; set; }
}
