using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tiendaonline.db"));

var app = builder.Build();

// Modelo de datos
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

// Modelo de datos para el carrito
public class Carrito
{
    public int Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new();
}

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Datos iniciales
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Celular", Descripcion = "Celular de alta gama", Precio = 50000, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Id = 2, Nombre = "Auriculares", Descripcion = "Auriculares inalámbricos", Precio = 15000, Stock = 20, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Id = 3, Nombre = "Laptop", Descripcion = "Laptop para trabajo y juegos", Precio = 120000, Stock = 5, ImagenUrl = "https://via.placeholder.com/150" }
        );
    }
}

// Middleware para inicializar la base de datos
app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
});

app.MapGet("/", () => "Servidor funcionando correctamente");

app.MapPost("/data", (DataModel data) => {
    return Results.Json(new { message = "Datos recibidos", data });
});

// Endpoint para obtener todos los productos o buscar por query
app.MapGet("/productos", async (AppDbContext db, string? query) =>
{
    var productos = string.IsNullOrEmpty(query)
        ? await db.Productos.ToListAsync()
        : await db.Productos.Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query)).ToListAsync();

    return Results.Json(productos);
});

// Endpoint para inicializar un carrito
app.MapPost("/carritos", (AppDbContext db) =>
{
    var carrito = new Carrito();
    db.Add(carrito);
    db.SaveChanges();
    return Results.Json(carrito);
});

// Endpoint para obtener los ítems de un carrito
app.MapGet("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");
    return Results.Json(carrito);
});

// Endpoint para vaciar un carrito
app.MapDelete("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");
    db.Remove(carrito);
    await db.SaveChangesAsync();
    return Results.Ok("Carrito vaciado");
});

// Endpoint para confirmar una compra
app.MapPut("/carritos/{carritoId}/confirmar", async (AppDbContext db, int carritoId, Cliente cliente) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null || !carrito.Items.Any()) return Results.BadRequest("Carrito no encontrado o vacío");

    // Crear una nueva compra
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = carrito.Items.Sum(i => i.Cantidad * db.Productos.Find(i.ProductoId)!.Precio),
        NombreCliente = cliente.Nombre,
        ApellidoCliente = cliente.Apellido,
        EmailCliente = cliente.Email
    };

    db.Add(compra);
    db.Remove(carrito);
    await db.SaveChangesAsync();

    return Results.Ok("Compra confirmada");
});

// Endpoint para agregar o actualizar un producto en el carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (AppDbContext db, int carritoId, int productoId, int cantidad) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null || producto.Stock < cantidad) return Results.BadRequest("Producto no disponible o stock insuficiente");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        carrito.Items.Add(new ItemCarrito { ProductoId = productoId, Cantidad = cantidad });
    }
    else
    {
        item.Cantidad = cantidad;
    }

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();

    return Results.Ok("Producto agregado/actualizado en el carrito");
});

// Endpoint para eliminar o reducir un producto del carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", async (AppDbContext db, int carritoId, int productoId) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("Producto no encontrado en el carrito");

    carrito.Items.Remove(item);
    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null) producto.Stock += item.Cantidad;

    await db.SaveChangesAsync();

    return Results.Ok("Producto eliminado del carrito");
});

// Modelo de datos para el cliente
public class Cliente
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Modelo de datos para la compra
public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
}

record DataModel(string Name, int Age);

app.Run();

// Inicialización de la base de datos
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}