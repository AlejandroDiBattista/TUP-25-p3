using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy
          .WithOrigins("http://localhost:5184", "https://localhost:7221", "http://localhost:5177")
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowClientApp");

app.MapGet("/", () => "API De mi Tienda Fender - Funciona correctamente.");

app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos
            .AsNoTracking()
            .ToListAsync()
);

app.MapGet("/api/productos/buscar", async (string term, TiendaContext db) =>
    await db.Productos
            .AsNoTracking()
            .Where(p => p.Nombre.Contains(term) || p.Descripcion.Contains(term))
            .ToListAsync()
);

app.MapPost("/api/carritos", async (TiendaContext db) =>
{
    var carritoId = Guid.NewGuid().ToString();
    var nuevoCarrito = new Carrito { Id = carritoId };

    db.Carritos.Add(nuevoCarrito);
    await db.SaveChangesAsync();

    return Results.Created($"/api/carritos/{carritoId}", nuevoCarrito);
});

app.MapGet("/api/carritos", async (TiendaContext db) =>
    await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .ToListAsync()
);

app.MapGet("/api/carritos/{carritoId}", async (string carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    return carrito is null 
        ? Results.NotFound() 
        : Results.Ok(carrito);
});

app.MapPut("/api/carritos/{carritoId}/{productoId}", 
    async (string carritoId, int productoId, int cantidad, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null) return Results.NotFound("Producto no encontrado.");

    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (item is not null)
    {
        item.Cantidad += cantidad;
        if (item.Cantidad <= 0)
        {
            carrito.Items.Remove(item);
        }
        else if (item.Cantidad > producto.Stock)
        {
            return Results.BadRequest("Stock insuficiente.");
        }
    }
    else if (cantidad > 0)
    {
        if (producto.Stock < cantidad)
            return Results.BadRequest("Stock insuficiente.");
        carrito.Items.Add(new ItemCarrito
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        });
    }
    else
    {
        return Results.BadRequest("No se puede restar productos que no están en el carrito.");
    }

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.MapDelete("/api/carritos/{carritoId}", async (string carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound();

    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/carritos/{carritoId}/{productoId}", async (string carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null)
        return Results.NotFound();

    carrito.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/api/carritos/{carritoId}/confirmar", async (string carritoId, ClienteDto cliente, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null || !carrito.Items.Any())
        return Results.BadRequest("El carrito está vacío o no existe.");

    foreach (var item in carrito.Items)
    {
        if (item.Producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {item.Producto.Nombre}.");
    }

    var compra = new Compra
    {
        NombreCliente = cliente.Nombre,
        ApellidoCliente = cliente.Apellido,
        EmailCliente = cliente.Email,
        Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario),
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };

    foreach (var item in carrito.Items)
    {
        item.Producto.Stock -= item.Cantidad;
    }

    db.Compras.Add(compra);
    carrito.Items.Clear();
    await db.SaveChangesAsync();

    return Results.Ok(new { mensaje = "Compra confirmada" });
});

app.Run();

public record ClienteDto(string Nombre, string Apellido, string Email);

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options)
        : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto {
                Id = 1,
                Nombre = "American Luxe Telecaster",
                Descripcion = "Guitarra Telecaster de lujo con acabado premium.",
                Precio = 2500,
                Stock = 5,
                ImagenUrl = "/imagenes/american-luxe-telecaster.png"
            },
            new Producto {
                Id = 2,
                Nombre = "American Telecaster Blanca",
                Descripcion = "Telecaster con cuerpo blanco y sonido potente.",
                Precio = 2200,
                Stock = 4,
                ImagenUrl = "/imagenes/american-telecaster-blanca.png"
            },
            new Producto {
                Id = 3,
                Nombre = "Stratocaster Professional II",
                Descripcion = "Stratocaster ideal para músicos profesionales.",
                Precio = 2300,
                Stock = 6,
                ImagenUrl = "/imagenes/stratocaster-professional-ii.png"
            },
            new Producto {
                Id = 4,
                Nombre = "Vintage Telecaster",
                Descripcion = "Modelo Telecaster con estética y tono vintage.",
                Precio = 2100,
                Stock = 3,
                ImagenUrl = "/imagenes/vintage-telecaster.png"
            },
            new Producto {
                Id = 5,
                Nombre = "Fender Acoustasonic Telecaster",
                Descripcion = "Guitarra acústica y eléctrica en un solo instrumento.",
                Precio = 1800,
                Stock = 10,
                ImagenUrl = "/imagenes/acoustasonic-tele.png"
            },
            new Producto {
                Id = 6,
                Nombre = "Fender Acoustasonic Stratocaster",
                Descripcion = "Stratocaster de la serie Acoustasonic con gran versatilidad.",
                Precio = 1500,
                Stock = 8,
                ImagenUrl = "/imagenes/acoustasonic-strato.png"
            },
            new Producto {
                Id = 7,
                Nombre = "Fender Standard Stratocaster",
                Descripcion = "Stratocaster de la serie Standard con características clásicas.",
                Precio = 1200,
                Stock = 12,
                ImagenUrl = "/imagenes/stratocaster-standard.png"
            },
            new Producto
            {
                Id = 8,
                Nombre = "Fender Telecaster Verde Modificada",
                Descripcion = "Telecaster con cuerpo verde y modificaciones personalizadas.",
                Precio = 1300,
                Stock = 15,
                ImagenUrl = "/imagenes/telecaster-verde-modificada.png"
            },
            new Producto
            {
                Id = 9,
                Nombre = "Mike Campbell Telecaster",
                Descripcion = "Telecaster inspirada y modificada con el estilo de Mike Campbell.",
                Precio = 1400,
                Stock = 7,
                ImagenUrl = "/imagenes/mike-campbell-telecaster.png"
            },
            new Producto
            {
                Id = 10,
                Nombre = "Fender Vintage Telecaster",
                Descripcion = "Telecaster de la serie Vintage con un sonido al pasado.",
                Precio = 1600,
                Stock = 9,
                ImagenUrl = "/imagenes/edicion-limitada-vintage-telecaster.png"
            }
        );
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }
}

public class Carrito
{
    public string Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new();
}

public class ItemCarrito
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int CompraId { get; set; }
    public Compra Compra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}