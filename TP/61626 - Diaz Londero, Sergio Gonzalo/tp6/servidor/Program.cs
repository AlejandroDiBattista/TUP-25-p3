using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS para permitir al cliente Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));


var app = builder.Build();

app.UseCors("AllowClientApp");

// Crear base de datos y seed inicial
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Botín de Fútbol", Descripcion = "Botín profesional con tapones de goma", Precio = 199999m, Stock = 15, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/botin2.jpg" },
            new Producto { Nombre = "Buzo Deportivo", Descripcion = "Buzo de algodón con capucha", Precio = 80000m, Stock = 25, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/buzo.jpg" },
            new Producto { Nombre = "Campera de Abrigo", Descripcion = "Campera impermeable con interior polar", Precio = 130000m, Stock = 18, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/campera.jpg" },
            new Producto { Nombre = "Pantalón Jogger", Descripcion = "Pantalón cómodo estilo urbano", Precio = 60000m, Stock = 30, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/pantalon.jpg" },
            new Producto { Nombre = "Remera Estampada", Descripcion = "Remera de algodón con diseño gráfico", Precio = 35000m, Stock = 40, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/remera2.jpg" },
            new Producto { Nombre = "Remera Básica", Descripcion = "Remera lisa color blanco", Precio = 30000m, Stock = 50, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/remera.jpg" },
            new Producto { Nombre = "Short Deportivo", Descripcion = "Short de secado rápido para entrenamiento", Precio = 30000m, Stock = 35, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/short.jpg" },
            new Producto { Nombre = "Zapatilla Urbana", Descripcion = "Zapatilla moderna y cómoda para el día a día", Precio = 180000m, Stock = 20, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/zapatilla2.jpg" },
            new Producto { Nombre = "Zapatilla Running", Descripcion = "Zapatilla ideal para correr largas distancias", Precio = 190000m, Stock = 22, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/zapatilla.jpg" },
            new Producto { Nombre = "Zapatilla Clásica", Descripcion = "Zapatilla estilo retro de lona", Precio = 70000m, Stock = 28, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/zapatilla.jpg" }
        );
        db.SaveChanges();
    }
}

// --- ENDPOINTS API ---

// GET /productos
app.MapGet("/productos", async ([FromQuery] string? q, AppDbContext db) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q));
    return await query.ToListAsync();
});

// POST /carritos
app.MapPost("/carritos", async (AppDbContext db) =>
{
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = "",
        ApellidoCliente = "",
        EmailCliente = "",
        Total = 0
    };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok(compra.Id);
});

// GET /carritos/{carritoId}
app.MapGet("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var items = await db.ItemsCompra
        .Include(i => i.Producto)
        .Where(i => i.CompraId == carritoId)
        .ToListAsync();
    return Results.Ok(items);
});

// DELETE /carritos/{carritoId}
app.MapDelete("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var items = db.ItemsCompra.Where(i => i.CompraId == carritoId);
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carritoId}/{productoId}?cantidad=3
app.MapPut("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, [FromQuery] int cantidad, AppDbContext db) =>
{
    if (cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a cero.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado.");

    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);

    if (item == null)
    {
        if (producto.Stock < cantidad)
            return Results.BadRequest("Stock insuficiente");

        item = new ItemCompra
        {
            CompraId = carritoId,
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        };
        db.ItemsCompra.Add(item);
        producto.Stock -= cantidad;
    }
    else
    {
        if (producto.Stock < cantidad)
            return Results.BadRequest("Stock insuficiente");

        item.Cantidad += cantidad;
        producto.Stock -= cantidad;
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});

// DELETE /carritos/{carritoId}/{productoId}
app.MapDelete("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, AppDbContext db) =>
{
    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);

    if (item == null) return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += item.Cantidad;
    }

    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carritoId}/confirmar
app.MapPut("/carritos/{carritoId}/confirmar", async (int carritoId, ClienteDto cliente, AppDbContext db) =>
{
    var compra = await db.Compras.FindAsync(carritoId);
    if (compra == null) return Results.NotFound();

    var items = await db.ItemsCompra.Where(i => i.CompraId == carritoId).ToListAsync();
    if (!items.Any()) return Results.BadRequest("Carrito vacío");

    if (string.IsNullOrWhiteSpace(cliente.Email))
        return Results.BadRequest("Email requerido");

    compra.NombreCliente = cliente.Nombre;
    compra.ApellidoCliente = cliente.Apellido;
    compra.EmailCliente = cliente.Email;
    compra.Total = items.Sum(i => i.Cantidad * i.PrecioUnitario);
    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

// GET /compras/{id}
app.MapGet("/compras/{id}", async (int id, AppDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (compra == null) return Results.NotFound();

    return Results.Ok(compra);
});

app.Run();

// --- MODELOS ---

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
    public string NombreCliente { get; set; } = "";
    public string ApellidoCliente { get; set; } = "";
    public string EmailCliente { get; set; } = "";
    public decimal Total { get; set; }
    public List<ItemCompra> Items { get; set; } = new List<ItemCompra>();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class ClienteDto
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
}

// --- CONTEXTO EF ---

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Compra> Compras { get; set; } = null!;
    public DbSet<ItemCompra> ItemsCompra { get; set; } = null!;
}
