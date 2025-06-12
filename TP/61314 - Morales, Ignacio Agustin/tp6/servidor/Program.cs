using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddDbContext<TiendaDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("https://localhost:7295", "http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Servir archivos estáticos desde la carpeta "imagenes"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "imagenes")),
    RequestPath = "/imagenes"
});

// Inicializar la base de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseCors("AllowClientApp");

// Endpoints

app.MapGet("/productos", async (TiendaDb db) =>
{
    return await db.Productos
        .Where(p => p.Stock > 0)
        .ToListAsync();
});

app.MapPost("/Carrito", async (TiendaDb db, Producto producto) =>
{
    if (producto.Stock <= 0)
    {
        return Results.BadRequest("Producto sin stock");
    }

    var item = new ItemCarrito
    {
        ProductoId = producto.Id,
        Cantidad = 1,
        PrecioUnitario = producto.Precio
    };

    var carrito = new Carrito
    {
        Items = new List<ItemCarrito> { item }
    };

    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

app.MapGet("/Carrito/{CarritoID}", async (TiendaDb db, Guid CarritoID) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == CarritoID);

    if (carrito == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(carrito);
});

app.MapDelete("/Carrito/{CarritoID}", async (TiendaDb db, Guid CarritoID) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == CarritoID);

    if (carrito == null)
    {
        return Results.NotFound();
    }

    db.Carritos.Remove(carrito);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/Carrito/{CarritoID}/{ProductoID}", async (TiendaDb db, Guid CarritoID, int ProductoID) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == CarritoID);

    if (carrito == null)
    {
        return Results.NotFound();
    }

    var producto = await db.Productos.FindAsync(ProductoID);
    if (producto == null || producto.Stock <= 0)
    {
        return Results.BadRequest("Producto no disponible");
    }

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == ProductoID);
    if (item != null)
    {
        item.Cantidad++;
    }
    else
    {
        carrito.Items.Add(new ItemCarrito
        {
            ProductoId = ProductoID,
            Cantidad = 1,
            PrecioUnitario = producto.Precio
        });
    }

    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

app.MapDelete("/Carrito/{CarritoID}/{ProductoID}", async (TiendaDb db, Guid CarritoID, int ProductoID) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == CarritoID);

    if (carrito == null)
    {
        return Results.NotFound();
    }

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == ProductoID);
    if (item == null)
    {
        return Results.BadRequest("El producto no está en el carrito");
    }

    carrito.Items.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/Carrito/{CarritoID}/Confirmar", async (TiendaDb db, Guid CarritoID, CompraDto dto) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == CarritoID);

    if (carrito == null)
    {
        return Results.NotFound();
    }

    var total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };
    db.Compras.Add(compra);
    db.Carritos.Remove(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

app.Run();

// --- CLASES Y MODELOS ---

class TiendaDb : DbContext
{
    public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCarrito> ItemsCarrito => Set<ItemCarrito>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto
            {
                Id = 1,
                Nombre = "Iphone 15",
                Descripcion = "Celular con la mejor autonomia",
                Stock = 10,
                Precio = 780000,
                ImagenUrl = "imagenes/iphone15.jpg"
            },
            new Producto
            {
                Id = 2,
                Nombre = "Iphone 15 Plus",
                Descripcion = "Celular con bateria resistente",
                Stock = 5,
                Precio = 820000,
                ImagenUrl = "imagenes/iphone15plus.jpg"
            },
            new Producto
            {
                Id = 3,
                Nombre = "Iphone 16",
                Descripcion = "Celular de ultima generacion",
                Stock = 13,
                Precio = 980000,
                ImagenUrl = "imagenes/iphone16.jpg"
            },
            new Producto
            {
                Id = 4,
                Nombre = "Iphone 16 Pro MAX",
                Descripcion = "Celular mas demandado del mercado",
                Stock = 6,
                Precio = 120000000,
                ImagenUrl = "imagenes/iphone16promax.jpg"
            },
            new Producto
            {
                Id = 5,
                Nombre = "AirPods Pro",
                Descripcion = "Auriculares bluetooth",
                Stock = 20,
                Precio = 200000,
                ImagenUrl = "imagenes/airpodspro.jpg"
            },
            new Producto
            {
                Id = 6,
                Nombre = "MacBook Air",
                Descripcion = "Notebook eficaz",
                Stock = 12,
                Precio = 220000000,
                ImagenUrl = "imagenes/macbookair.jpg"
            },
            new Producto
            {
                Id = 7,
                Nombre = "MacBook Pro",
                Descripcion = "Notebook mas potente",
                Stock = 8,
                Precio = 390000000,
                ImagenUrl = "imagenes/macbookpro.jpg"
            },
            new Producto
            {
                Id = 8,
                Nombre = "Apple Watch",
                Descripcion = "Reloj inteligente",
                Stock = 15,
                Precio = 340000,
                ImagenUrl = "imagenes/applewatch.jpg"
            },
            new Producto
            {
                Id = 9,
                Nombre = "Ipad Air",
                Descripcion = "Pantalla inteligente",
                Stock = 11,
                Precio = 720000,
                ImagenUrl = "imagenes/ipadair.jpg"
            },
            new Producto
            {
                Id = 10,
                Nombre = "Ipad Pro",
                Descripcion = "Ipad mas potente y avanzado",
                Stock = 9,
                Precio = 130000000,
                ImagenUrl = "imagenes/ipadpro.jpg"
            }
        );
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Stock { get; set; }
    public int Precio { get; set; }
    public string ImagenUrl { get; set; }
    public List<ItemCarrito> ItemsCarrito { get; set; } = new();
}

public class ItemCarrito
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
    public int PrecioUnitario { get; set; }
}

public class Carrito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ItemCarrito> Items { get; set; } = new();
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int CompraId { get; set; }
    public int Cantidad { get; set; }
    public int PrecioUnitario { get; set; }
}

public class CompraDto
{
    [Required] public string Nombre { get; set; }
    [Required] public string Apellido { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
}