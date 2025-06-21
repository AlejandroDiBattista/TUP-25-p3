using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaDb>(opt =>
    opt.UseSqlite("Data Source=tienda.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("https://localhost:5284", "http://localhost:5284", "https://localhost:5184", "http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowClientApp");

var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "imagenes");
if (!Directory.Exists(imagePath))
    Directory.CreateDirectory(imagePath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagePath),
    RequestPath = "/imagenes"
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

app.MapGet("/productos", async (TiendaDb db) =>
    await db.Productos.Where(p => p.Stock > 0).ToListAsync());

app.MapPost("/carrito", async (TiendaDb db, Producto producto) =>
{
    if (producto.Stock <= 0)
        return Results.BadRequest("Producto sin stock");

    var carrito = new Carrito();

    var item = new ItemCarrito
    {
        ProductoId = producto.Id,
        Cantidad = 1,
        PrecioUnitario = producto.Precio,
        Carrito = carrito
    };

    carrito.Items.Add(item);
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

app.MapGet("/carrito/{carritoId:guid}", async (TiendaDb db, Guid carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    return carrito is null ? Results.NotFound() : Results.Ok(carrito);
});

app.MapDelete("/carrito/{carritoId:guid}", async (TiendaDb db, Guid carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound();

    db.Carritos.Remove(carrito);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/carrito/{carritoId:guid}/{productoId:int}", async (TiendaDb db, Guid carritoId, int productoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null || producto.Stock <= 0)
        return Results.BadRequest("Producto no disponible");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item != null)
        item.Cantidad++;
    else
        carrito.Items.Add(new ItemCarrito
        {
            ProductoId = productoId,
            Cantidad = 1,
            PrecioUnitario = producto.Precio,
            CarritoId = carritoId
        });

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.MapDelete("/carrito/{carritoId:guid}/{productoId:int}", async (TiendaDb db, Guid carritoId, int productoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null)
        return Results.BadRequest("Producto no está en el carrito");

    carrito.Items.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/carrito/{carritoId:guid}/confirmar", async (TiendaDb db, Guid carritoId, CompraDto dto) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
        return Results.NotFound("Carrito no encontrado.");

    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto is null)
            return Results.BadRequest($"Producto con ID {item.ProductoId} no existe.");

        if (producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay suficiente stock para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {item.Cantidad}");
    }

    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
    }

    var total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    var compra = new Compra
    {
        Fecha = DateTime.UtcNow,
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
app.MapPut("/api/productos/sumar-stock/{id:int}", async (int id, int cantidad, TiendaDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto == null) return Results.NotFound();

    producto.Stock += cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPut("/productos/{id:int}/restar-stock", async (TiendaDb db, int id) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock <= 0)
        return Results.BadRequest("Sin stock disponible");

    producto.Stock--;
    await db.SaveChangesAsync();

    return Results.Ok(producto.Stock);
});

app.MapGet("/", () => "API de Tienda Online funcionando correctamente.");

app.Run();

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
    public Guid CarritoId { get; set; }
    public Carrito Carrito { get; set; }
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
    public Producto Producto { get; set; }
    public int CompraId { get; set; }
    public Compra Compra { get; set; }
    public int Cantidad { get; set; }
    public int PrecioUnitario { get; set; }
}

public class CompraDto
{
    [Required] public string Nombre { get; set; }
    [Required] public string Apellido { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
}


