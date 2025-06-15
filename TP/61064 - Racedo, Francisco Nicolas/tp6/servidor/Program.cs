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
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db")
);

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Habilitar archivos estáticos
app.UseStaticFiles();

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint: Obtener productos (con búsqueda opcional por query ?q=texto)
app.MapGet("/api/productos", async (TiendaDbContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
    {
        productos = productos.Where(p =>
            p.Nombre.Contains(q) ||
            p.Descripcion.Contains(q)
        );
    }

    return await productos.ToListAsync();
});

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoint: Crear un nuevo carrito
app.MapPost("/api/carritos", () =>
{
    var carritoId = Guid.NewGuid();
    CarritoStore.Carritos[carritoId] = new Dictionary<int, int>();
    return Results.Ok(new { carritoId });
});

// Endpoint: Obtener los ítems(Productos) de un carrito
app.MapGet("/api/carritos/{carritoId}", async (Guid carritoId, TiendaDbContext db) =>
{
    if (!CarritoStore.Carritos.ContainsKey(carritoId))
        return Results.NotFound(new { error = "Carrito no encontrado" });

    var items = CarritoStore.Carritos[carritoId];
    var productos = await db.Productos
        .Where(p => items.Keys.Contains(p.Id))
        .ToListAsync();

    var resultado = productos.Select(p => new {
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.ImagenUrl,
        Cantidad = items[p.Id],
        Subtotal = items[p.Id] * p.Precio,
        Stock = p.Stock // Se agrega el stock para que el cliente pueda ver el stock disponible de cada producto
    });

    return Results.Ok(resultado);
});


// Endpoint: Agregar o actualizar producto en el carrito
app.MapPut("/api/carritos/{carritoId}/{productoId}", async (Guid carritoId, int productoId, CantidadDto dto, TiendaDbContext db) =>
{
    if (!CarritoStore.Carritos.ContainsKey(carritoId))
        return Results.NotFound(new { error = "Carrito no encontrado" });

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound(new { error = "Producto no encontrado" });

    if (dto.Cantidad < 1)
        return Results.BadRequest(new { error = "La cantidad debe ser mayor a cero" });

    // Validar stock disponible
    int enCarrito = CarritoStore.Carritos[carritoId].ContainsKey(productoId) ? CarritoStore.Carritos[carritoId][productoId] : 0;
    int nuevaCantidadTotal = dto.Cantidad; // Cambiado: ahora la cantidad es absoluta, no incremental
    if (nuevaCantidadTotal > producto.Stock)
        return Results.BadRequest(new { error = "No hay suficiente stock disponible" });

    CarritoStore.Carritos[carritoId][productoId] = nuevaCantidadTotal;
    return Results.Ok(new { mensaje = "Producto agregado/actualizado en el carrito" });
});

// Endpoint: Eliminar o reducir cantidad de un producto del carrito
app.MapDelete("/api/carritos/{carritoId}/{productoId}", (Guid carritoId, int productoId) =>
{
    if (!CarritoStore.Carritos.ContainsKey(carritoId))
        return Results.NotFound(new { error = "Carrito no encontrado" });

    if (!CarritoStore.Carritos[carritoId].ContainsKey(productoId))
        return Results.NotFound(new { error = "Producto no está en el carrito" });

    CarritoStore.Carritos[carritoId].Remove(productoId);
    return Results.Ok(new { mensaje = "Producto eliminado del carrito" });
});

// Endpoint: Vaciar el carrito
app.MapDelete("/api/carritos/{carritoId}", (Guid carritoId) =>
{
    if (!CarritoStore.Carritos.ContainsKey(carritoId))
        return Results.NotFound(new { error = "Carrito no encontrado" });

    CarritoStore.Carritos[carritoId].Clear();
    return Results.Ok(new { mensaje = "Carrito vaciado" });
});

// Endpoint: Confirmar compra
app.MapPut("/api/carritos/{carritoId}/confirmar", async (Guid carritoId, ConfirmarCompraDto datos, TiendaDbContext db) =>
{
    if (!CarritoStore.Carritos.ContainsKey(carritoId))
        return Results.NotFound(new { error = "Carrito no encontrado" });

    var items = CarritoStore.Carritos[carritoId];
    if (items.Count == 0)
        return Results.BadRequest(new { error = "El carrito está vacío" });

    // Validar stock y calcular total
    var productos = await db.Productos.Where(p => items.Keys.Contains(p.Id)).ToListAsync();
    foreach (var p in productos)
    {
        if (items[p.Id] > p.Stock)
            return Results.BadRequest(new { error = $"No hay suficiente stock para {p.Nombre}" });
    }

    decimal total = productos.Sum(p => p.Precio * items[p.Id]);

    // Registrar la compra
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = datos.Nombre,
        ApellidoCliente = datos.Apellido,
        EmailCliente = datos.Email,
        Items = productos.Select(p => new ItemCompra
        {
            ProductoId = p.Id,
            Cantidad = items[p.Id],
            PrecioUnitario = p.Precio
        }).ToList()
    };
    db.Compras.Add(compra);

    // Descontar stock
    foreach (var p in productos)
    {
        p.Stock -= items[p.Id];
    }

    await db.SaveChangesAsync();

    // Limpiar carrito
    CarritoStore.Carritos[carritoId].Clear();

    return Results.Ok(new { mensaje = "Compra confirmada", compraId = compra.Id });
});


// Se inicializa la base de datos y se cargan los productos.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        string baseUrl = "http://localhost:5184/images/";
        db.Productos.AddRange(
            new Producto { Nombre = "Aceite Natura 900cc", Descripcion = "Aceite vegetal comestible", Precio = 1800, Stock = 15, ImagenUrl = baseUrl + "Aceite Natura 900cc.jpg" },
            new Producto { Nombre = "Arroz Lucchetti 500grs", Descripcion = "Arroz blanco premium", Precio = 950, Stock = 20, ImagenUrl = baseUrl + "Arroz Lucchetti 500grs.jpg" },
            new Producto { Nombre = "Azucar Ledesma 1kg", Descripcion = "Azúcar refinada", Precio = 1300, Stock = 10, ImagenUrl = baseUrl + "Azucar Ledesma 1kg.jpg" },
            new Producto { Nombre = "Coca-Cola 1.5 ltrs", Descripcion = "Bebida gaseosa cola", Precio = 2800, Stock = 25, ImagenUrl = baseUrl + "Coca-Cola 1.5 ltrs.jpg" },
            new Producto { Nombre = "Fernet Branca 750cc", Descripcion = "Fernet Branca", Precio = 11000, Stock = 18, ImagenUrl = baseUrl + "Fernet Branca 750cc.png" },
            new Producto { Nombre = "Galleta TerrabuSi 400grs", Descripcion = "Galletas surtidas", Precio = 2700, Stock = 30, ImagenUrl = baseUrl + "Galleta TerrabuSi 400grs.jpg" },
            new Producto { Nombre = "Harina 000 Cañuelas 1kg", Descripcion = "Harina 000 Cañuelas", Precio = 800, Stock = 22, ImagenUrl = baseUrl + "Harina 000 Cañuelas 1kg.jpg" },
            new Producto { Nombre = "Pure de Tomate Mora 520grs", Descripcion = "Pure de tomate", Precio = 600, Stock = 12, ImagenUrl = baseUrl + "Pure de Tomate Mora 520grs.jpg" },
            new Producto { Nombre = "Spaghetti La Providencia 500grs", Descripcion = "Fideo tipo Spaghetti", Precio = 750, Stock = 16, ImagenUrl = baseUrl + "Spaghetti La Providencia 500grs.jpg" },
            new Producto { Nombre = "Té La Virginia x 25 unidades", Descripcion = "Té La Virginia x 25 unidades", Precio = 650, Stock = 8, ImagenUrl = baseUrl + "Té La Virginia x 25 unidades.jpg" }
        );
        db.SaveChanges();
    }
}

app.Run();

public partial class Program
{
    // ...no es necesario agregar nada aquí...
}

public class CantidadDto
{
    public int Cantidad { get; set; }
}

public class ConfirmarCompraDto
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = null!;
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = null!;
    public string ApellidoCliente { get; set; } = null!;
    public string EmailCliente { get; set; } = null!;
    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int CompraId { get; set; }
    public Compra? Compra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class TiendaDbContext : DbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
}

public static class CarritoStore
{
    public static Dictionary<Guid, Dictionary<int, int>> Carritos { get; } = new();
}
