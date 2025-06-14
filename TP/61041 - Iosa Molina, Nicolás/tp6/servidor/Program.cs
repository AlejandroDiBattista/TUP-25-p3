using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowClientApp");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
    
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "iPhone 15", Descripcion = "Celular Apple", Precio = 1200, Stock = 10, ImagenUrl = "https://maximstore.com/_next/image?url=https%3A%2F%2Fback.maximstore.com%2Fstatic%2Fimages%2F091e624b-46fa-4881-914e-527e7d3775c0.png&w=3840&q=75" },
            new Producto { Nombre = "Samsung Galaxy S24", Descripcion = "Celular Samsung", Precio = 1100, Stock = 8, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/hk_en/2401/gallery/hk-en-galaxy-s24-s928-489657-sm-s9280zogtgy-539359253?$624_624_PNG$" },
            new Producto { Nombre = "Xiaomi Redmi Note 13", Descripcion = "Celular Xiaomi", Precio = 400, Stock = 15, ImagenUrl = "https://authogar.vtexassets.com/arquivos/ids/214617/XIAOMI-REDMI-NOTE-13-GREEN-1.png?v=638715928359300000" },
            new Producto { Nombre = "Auriculares JBL", Descripcion = "Auriculares inalámbricos", Precio = 150, Stock = 20, ImagenUrl = "https://www.jbl.com/on/demandware.static/-/Sites-masterCatalog_Harman/default/dw31bae42a/JBL_TUNE_510BT_Product%20Image_Hero_Black.png" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido", Precio = 30, Stock = 50, ImagenUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MHXH3_AV1?wid=1144&hei=1144&fmt=jpeg&qlt=90&.v=1632955239000" },
            new Producto { Nombre = "Smartwatch Xiaomi Mi Band 7", Descripcion = "Reloj inteligente", Precio = 70, Stock = 25, ImagenUrl = "https://tienda.personal.com.ar/images/720/webp/Xiaomi_Smart_Band_7_Negro1_28c5fed47d.png" },
            new Producto { Nombre = "Altavoz Bluetooth Sony", Descripcion = "Altavoz portátil", Precio = 85, Stock = 15, ImagenUrl = "https://www.sony.com/image/5d02da5df552836db894cead8a68f5f3?fmt=pjpeg&wid=330&bgcolor=FFFFFF&bgc=FFFFFF" },
            new Producto { Nombre = "Funda iPhone 15", Descripcion = "Funda silicona", Precio = 25, Stock = 30, ImagenUrl = "https://store.storeimages.cdn-apple.com/4668/as-images.apple.com/is/MT4J3?wid=1144&hei=1144&fmt=jpeg&qlt=90&.v=1693594197616" },
            new Producto { Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico", Precio = 40, Stock = 25, ImagenUrl = "https://resource.logitechg.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/gaming/en/products/g502-lightspeed-gaming-mouse/g502-lightspeed-hero.png" },
            new Producto { Nombre = "Teclado Redragon", Descripcion = "Teclado mecánico", Precio = 60, Stock = 18, ImagenUrl = "https://redragon.es/content/uploads/2021/04/KUMARA.png" }
        );
        db.SaveChanges();
    }
}

var carritos = new ConcurrentDictionary<string, List<ItemCarrito>>();

app.MapGet("/", () => Results.Ok(new { Mensaje = "API de Tienda", Fecha = DateTime.Now }));

app.MapGet("/productos", async (TiendaContext db) =>
{
    return await db.Productos.ToListAsync();
});

app.MapGet("/productos/buscar/{texto}", async (TiendaContext db, string texto) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(texto))
    {
        var textoLower = texto.ToLower();
        query = query.Where(p => p.Nombre.ToLower().Contains(textoLower) || p.Descripcion.ToLower().Contains(textoLower));
    }
    return await query.ToListAsync();
});

app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid().ToString();
    carritos[id] = new List<ItemCarrito>();
    return Results.Ok(new { carrito = id });
});

app.MapGet("/carritos/{carrito}", async (string carrito, TiendaContext db) =>
{
    if (!carritos.TryGetValue(carrito, out var items))
        return Results.NotFound("Carrito no encontrado");

    var itemsConNombres = new List<object>();
    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        itemsConNombres.Add(new
        {
            ProductoId = item.ProductoId,
            NombreProducto = producto?.Nombre ?? "Producto no encontrado",
            Cantidad = item.Cantidad,
            PrecioUnitario = item.PrecioUnitario
        });
    }

    return Results.Ok(itemsConNombres);
});

app.MapDelete("/carritos/{carrito}", (string carrito) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    carritos[carrito] = new List<ItemCarrito>();
    return Results.Ok();
});

app.MapPut("/carritos/{carrito}/confirmar", async (
    HttpContext httpContext,
    string carrito,
    TiendaContext db
) =>
{    try
    {
        using var reader = new System.IO.StreamReader(httpContext.Request.Body);
        var body = await reader.ReadToEndAsync();
        
        ClienteDto datos;
        try {
            datos = System.Text.Json.JsonSerializer.Deserialize<ClienteDto>(body);
            
            if (string.IsNullOrWhiteSpace(datos?.Nombre) || 
                string.IsNullOrWhiteSpace(datos?.Apellido) || 
                string.IsNullOrWhiteSpace(datos?.Email))
            {
                return Results.BadRequest(new { error = "Todos los campos son requeridos" });
            }
        }
        catch {
            return Results.BadRequest(new { error = "Error en formato" });
        }
        
        if (!carritos.TryGetValue(carrito, out var items))
            return Results.BadRequest(new { error = "Carrito no encontrado" });
            
        if (items.Count == 0)
            return Results.BadRequest(new { error = "Carrito vacío" });

        foreach (var item in items)
        {
            var producto = await db.Productos.FindAsync(item.ProductoId);
            if (producto == null)
                return Results.BadRequest(new { error = "Producto no existe" });
                
            if (producto.Stock < item.Cantidad)
                return Results.BadRequest(new { error = "Stock insuficiente" });
        }

        double total = 0;
        var compra = new Compra
        {
            Fecha = DateTime.Now,
            NombreCliente = datos.Nombre,
            ApellidoCliente = datos.Apellido,
            EmailCliente = datos.Email,
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
    }    catch
    {
        return Results.BadRequest(new { error = "Error al procesar" });
    }
});

app.MapPut("/carritos/{carrito}/{producto}", async (string carrito, int producto, int cantidad, TiendaContext db) =>
{
    if (cantidad <= 0)
        return Results.BadRequest("Cantidad inválida");

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

app.MapDelete("/carritos/{carrito}/{producto}", (string carrito, int producto, int cantidad = 0) =>
{
    if (!carritos.TryGetValue(carrito, out var items))
        return Results.NotFound("Carrito no encontrado");

    var item = items.FirstOrDefault(i => i.ProductoId == producto);
    if (item == null)
        return Results.NotFound("Producto no encontrado");

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

public class ClienteDto
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
}
