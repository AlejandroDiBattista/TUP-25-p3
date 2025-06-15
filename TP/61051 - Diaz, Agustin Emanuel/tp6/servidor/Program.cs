using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

var carritos = new Dictionary<Guid, Carrito>();

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async ([FromQuery] string? query, ApplicationDbContext db) =>
{
    var productos = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(query))
    {
        query = query.ToLower();
        productos = productos.Where(p =>
            p.Nombre.ToLower().Contains(query) ||
            p.Descripcion.ToLower().Contains(query));
    }

    return Results.Ok(await productos.ToListAsync());
});

app.MapGet("/carrito/{carritoId}", (Guid carritoId) =>
{
    if (carritos.TryGetValue(carritoId, out var carrito))
        return Results.Ok(carrito);

    return Results.NotFound(new { mensaje = "Carrito no encontrado" });
});

app.MapPost("/carrito", () =>
{
  var nuevoCarrito = new Carrito();
  carritos[nuevoCarrito.Id] = nuevoCarrito;
  return Results.Ok(new { carritoId = nuevoCarrito.Id });
});

app.MapPut("/carritos/{id}/{productoId}", async (
    [FromRoute] Guid id,
    [FromRoute] int productoId,
    [FromBody] int cantidad,
    ApplicationDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null || producto.Stock < cantidad)
        return Results.BadRequest("Producto no válido o sin stock");

    var existente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (existente != null)
    {
      existente.Cantidad += cantidad;
      existente.PrecioUnitario = producto.Precio;
    }
    else
    carrito.Items.Add(new CarritoItem
    {
      ProductoId = productoId,
      Cantidad = cantidad,
      PrecioUnitario = producto.Precio
    });

    return Results.Ok(carrito);
});

app.MapPut("/carritos/{id}/confirmar", async ([FromRoute] Guid id, [FromBody] Compra datosCompra, ApplicationDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito) || !carrito.Items.Any())
        return Results.BadRequest("Carrito inválido o vacío");

    var compra = new Compra
    {
        NombreCliente = datosCompra.NombreCliente,
        ApellidoCliente = datosCompra.ApellidoCliente,
        EmailCliente = datosCompra.EmailCliente
    };

    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest("Stock insuficiente o producto no encontrado");

        producto.Stock -= item.Cantidad;

        compra.Items.Add(new ItemCompra
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        });
    }

    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carritos.Remove(id);

    return Results.Ok(new { compra.Id, compra.Total });
});

app.MapGet("/compras", async (ApplicationDbContext db) =>
{
  var compras = await db.Compras
      .Include(c => c.Items)
      .ThenInclude(i => i.Producto)
      .ToListAsync();

  var resultado = compras.Select(c => new
  {
    c.Id,
    c.Fecha,
    c.Total,
    Cliente = new
    {
      c.NombreCliente,
      c.ApellidoCliente,
      c.EmailCliente
    },
    Productos = c.Items.Select(i => new
    {
      Producto = i.Producto?.Nombre ?? "Producto no disponible",
      i.Cantidad,
      i.PrecioUnitario,
      Subtotal = i.Cantidad * i.PrecioUnitario
    }).ToList()
  });

  return Results.Ok(resultado);
});

app.MapDelete("/carritos/{id}/{productoId}", ([FromRoute] Guid id, [FromRoute] int productoId) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.BadRequest("Producto no encontrado en el carrito");

    if (item.Cantidad > 1)
        item.Cantidad--;
    else
        carrito.Items.Remove(item);

    return Results.Ok(carrito);
});

app.MapPost("/carrito/vaciar", ([FromQuery] Guid id) =>
{
  if (!carritos.TryGetValue(id, out var carrito))
    return Results.NotFound("Carrito no encontrado");

  carrito.Items.Clear();
  return Results.Ok("Carrito vaciado correctamente");
});

app.MapDelete("/carrito", ([FromQuery] Guid id) =>
{
  if (!carritos.Remove(id))
    return Results.NotFound("Carrito no encontrado");

  return Results.Ok("Carrito eliminado");
});

app.MapGet("/compras/{id}", async (int id, ApplicationDbContext db) =>
{
  var compra = await db.Compras
      .Include(c => c.Items)
      .ThenInclude(i => i.Producto)
      .FirstOrDefaultAsync(c => c.Id == id);

  if (compra == null)
    return Results.NotFound("Compra no encontrada");

  var resultado = new
  {
    compra.Id,
    compra.Fecha,
    compra.Total,
    Cliente = new
    {
      compra.NombreCliente,
      compra.ApellidoCliente,
      compra.EmailCliente
    },
    Productos = compra.Items.Select(i => new
    {
      Producto = i.Producto?.Nombre ?? "Producto no disponible",
      i.Cantidad,
      i.PrecioUnitario,
      Subtotal = i.Cantidad * i.PrecioUnitario
    }).ToList()
  };

  return Results.Ok(resultado);
});

app.MapDelete("/carritos/{id}", ([FromRoute] Guid id) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    carrito.Items.Clear();
    return Results.Ok("Carrito vaciado correctamente");
});


using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  db.Database.EnsureCreated();

  if (!db.Productos.Any())
  {
    db.Productos.AddRange(new[]
    {
            new Producto { Nombre = "Samsung Galaxy A52", Descripcion = "Galaxy A52, 128GB", Precio = 600, Stock = 5, ImagenUrl = "images/samsung.jpg" },
            new Producto { Nombre = "iPhone 12", Descripcion = "iPhone 12, 256GB", Precio = 450, Stock = 3, ImagenUrl = "images/iphone.jpg" },
            new Producto { Nombre = "Notebook HP", Descripcion = "Intel i5 10° gen", Precio = 900, Stock = 4, ImagenUrl = "images/hp.jpg" },
            new Producto { Nombre = "Mouse Logitech G502", Descripcion = "Inalámbrico", Precio = 100, Stock = 10, ImagenUrl = "images/mouse.jpg" },
            new Producto { Nombre = "Auriculares JBL", Descripcion = "Bluetooth", Precio = 150, Stock = 8, ImagenUrl = "images/jbl.jpg" },
            new Producto { Nombre = "Monitor LG", Descripcion = "24 pulgadas Full HD", Precio = 160, Stock = 2, ImagenUrl = "images/monitor.jpg" },
            new Producto { Nombre = "Teclado Redragon", Descripcion = "Mecánico RGB", Precio = 100, Stock = 10, ImagenUrl = "images/teclado.jpg" },
            new Producto { Nombre = "Cargador portátil", Descripcion = "10000 mAh", Precio = 80, Stock = 9, ImagenUrl = "images/cargador.jpg" },
            new Producto { Nombre = "MacBook Pro M1", Descripcion = "Macbook M1 Pro 16 pulgadas", Precio = 1500, Stock = 6, ImagenUrl = "images/macbook.jpg" },
            new Producto { Nombre = "Silla gamer", Descripcion = "Silla gamer Redragon", Precio = 700, Stock = 15, ImagenUrl = "images/silla.jpg" }
        });

    db.SaveChanges();
  }
}

app.Run();

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
  public int Id { get; set; }
  public int ProductoId { get; set; }
  public int CompraId { get; set; }
  public int Cantidad { get; set; }
  public decimal PrecioUnitario { get; set; }

  public Producto? Producto { get; set; }
  public Compra? Compra { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
}

public class CarritoItem
{
  public int ProductoId { get; set; }
  public int Cantidad { get; set; }
  public decimal PrecioUnitario { get; set; } 
}

public class Carrito
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public List<CarritoItem> Items { get; set; } = new();
}