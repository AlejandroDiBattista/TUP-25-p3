using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

public record DataModel(string Name, int Age);

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class Carrito
{
    public int Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new();
}

public class ItemCarrito
{
    public int Id { get; set; }
    public int CarritoId { get; set; }
    public Carrito? Carrito { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
}

public class Cliente
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<ItemCarrito> ItemsCarrito { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Carrito>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Carrito)
            .HasForeignKey(i => i.CarritoId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ItemCarrito>()
            .HasOne(i => i.Producto)
            .WithMany()
            .HasForeignKey(i => i.ProductoId);
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Celular", Descripcion = "Celular de alta gama", Precio = 50000, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Id = 2, Nombre = "Auriculares", Descripcion = "Auriculares inalámbricos", Precio = 15000, Stock = 20, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Id = 3, Nombre = "Laptop", Descripcion = "Laptop para trabajo y juegos", Precio = 120000, Stock = 5, ImagenUrl = "https://via.placeholder.com/150" }
        );
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tiendaonline.db"));
var app = builder.Build();
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
app.MapGet("/productos", async (AppDbContext db, string? query) =>
{
    var productos = string.IsNullOrEmpty(query)
        ? await db.Productos.ToListAsync()
        : await db.Productos.Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query)).ToListAsync();
    return Results.Json(productos);
});
app.MapPost("/carritos", (AppDbContext db) =>
{
    var carrito = new Carrito();
    db.Add(carrito);
    db.SaveChanges();
    return Results.Json(carrito);
});
app.MapGet("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");
    return Results.Json(carrito);
});
app.MapDelete("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");
    db.Remove(carrito);
    await db.SaveChangesAsync();
    return Results.Ok("Carrito vaciado");
});
app.MapPut("/carritos/{carritoId}/confirmar", async (AppDbContext db, int carritoId, Cliente cliente) =>
{
    var carrito = await db.Set<Carrito>().FindAsync(carritoId);
    if (carrito == null || !carrito.Items.Any()) return Results.BadRequest("Carrito no encontrado o vacío");
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
app.Run();
