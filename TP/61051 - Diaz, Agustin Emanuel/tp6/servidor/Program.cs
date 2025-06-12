using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
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
}

public class Carrito
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public List<CarritoItem> Items { get; set; } = new();
}

var carritos = new Dictionary<Guid, Carrito>();

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (AppDbContext db) =>
    await db.Productos.ToListAsync());

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

app.MapPost("/carrito/agregar", async ([FromQuery] Guid id, [FromBody] CarritoItem item, ApplicationDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(item.ProductoId);
    if (producto == null || producto.Stock < item.Cantidad)
        return Results.BadRequest("Producto no válido o sin stock");

    var existente = carrito.Items.FirstOrDefault(i => i.ProductoId == item.ProductoId);
    if (existente != null)
        existente.Cantidad += item.Cantidad;
    else
        carrito.Items.Add(item);

    return Results.Ok(carrito);
});

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.EnsureCreated();

  if (!db.Productos.Any())
  {
    db.Productos.AddRange(new[]
    {
            new Producto { Nombre = "Celular Samsung", Descripcion = "Galaxy A52", Precio = 600, Stock = 5, ImagenUrl = "images/samsung.jpg" },
            new Producto { Nombre = "Celular iPhone", Descripcion = "iPhone 12", Precio = 450, Stock = 3, ImagenUrl = "images/iphone.jpg" },
            new Producto { Nombre = "Notebook HP", Descripcion = "Intel i5 10° gen", Precio = 900, Stock = 4, ImagenUrl = "images/hp.jpg" },
            new Producto { Nombre = "Mouse Logitech", Descripcion = "Inalámbrico", Precio = 100, Stock = 10, ImagenUrl = "images/mouse.jpg" },
            new Producto { Nombre = "Auriculares JBL", Descripcion = "Bluetooth", Precio = 150, Stock = 8, ImagenUrl = "images/jbl.jpg" },
            new Producto { Nombre = "Monitor LG", Descripcion = "24 pulgadas Full HD", Precio = 160, Stock = 2, ImagenUrl = "images/monitor.jpg" },
            new Producto { Nombre = "Teclado Redragon", Descripcion = "Mecánico RGB", Precio = 100, Stock = 6, ImagenUrl = "images/teclado.jpg" },
            new Producto { Nombre = "Cargador portátil", Descripcion = "10000 mAh", Precio = 80, Stock = 9, ImagenUrl = "images/cargador.jpg" },
            new Producto { Nombre = "MacBook Pro M1", Descripcion = "Macbook M1 Pro 16 pulgadas", Precio = 1500, Stock = 6, ImagenUrl = "images/macbook.jpg" },
            new Producto { Nombre = "Silla gamer", Descripcion = "Silla gamer Redragon", Precio = 700, Stock = 15, ImagenUrl = "images/silla.jpg" }
        });

    db.SaveChanges();
  }
}

app.Run();