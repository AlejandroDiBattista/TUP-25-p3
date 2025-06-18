using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---


builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();
app.UseStaticFiles();


if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");



app.MapGet("/", () => "Servidor de la Tienda de Perfumes está en funcionamiento.");

app.MapGet("/api/productos", async (ApplicationDbContext db, string? busqueda) =>
{
    if (string.IsNullOrWhiteSpace(busqueda))
    {
        return Results.Ok(await db.Productos.ToListAsync());
    }
    return Results.Ok(await db.Productos
        .Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower()))
        .ToListAsync());
});

app.MapPost("/api/carritos", async (ApplicationDbContext db) =>
{
    var nuevaCompra = new Compra { Fecha = DateTime.UtcNow };
    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();
    return Results.Ok(nuevaCompra.Id);
});


app.MapGet("/api/carritos/{carritoId:int}", async (ApplicationDbContext db, int carritoId) =>
{
    var compra = await db.Compras
        .AsNoTracking()
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound("Carrito no encontrado.");

    
    var compraDto = new CompraResumenDto(
        compra.Id,
        compra.Items.Select(i => new ItemCompraResumenDto(
            i.Id,
            i.ProductoId,
            i.Cantidad,
            i.PrecioUnitario,
            new ProductoResumenDto(
                i.Producto.Id,
                i.Producto.Nombre,
                i.Producto.Precio,
                i.Producto.Descripcion,
                i.Producto.Stock,
                i.Producto.ImagenUrl
            )
        )).ToList()
    );

    return Results.Ok(compraDto);
});

app.MapPut("/api/carritos/{carritoId:int}/agregar/{productoId:int}", async (ApplicationDbContext db, int carritoId, int productoId, int cantidad) =>
{
    if (cantidad <= 0) return Results.BadRequest("La cantidad debe ser mayor a cero.");
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado.");
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado.");
    var itemExistente = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    int cantidadAValidar = itemExistente != null ? cantidad : (itemExistente?.Cantidad ?? 0) + cantidad;
    if (producto.Stock < cantidadAValidar) return Results.BadRequest($"Stock insuficiente para '{producto.Nombre}'.");
    if (itemExistente != null) itemExistente.Cantidad = cantidad;
    else
    {
        compra.Items.Add(new ItemCompra
        {
            ProductoId = productoId,
            CompraId = carritoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        });
    }
    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado/actualizado en el carrito.");
});

app.MapDelete("/api/carritos/{carritoId:int}/remover/{productoId:int}", async (ApplicationDbContext db, int carritoId, int productoId) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado.");
    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("El producto no está en el carrito.");
    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok("Producto eliminado del carrito.");
});

app.MapPut("/api/carritos/{carritoId:int}/confirmar", async (ApplicationDbContext db, int carritoId, ClienteDto cliente) =>
{
    if (string.IsNullOrWhiteSpace(cliente.Nombre) || string.IsNullOrWhiteSpace(cliente.Apellido) || string.IsNullOrWhiteSpace(cliente.Email))
    {
        return Results.BadRequest("Nombre, Apellido y Email son obligatorios.");
    }
    var compra = await db.Compras.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado.");
    if (!compra.Items.Any()) return Results.BadRequest("El carrito está vacío, no se puede confirmar la compra.");
    decimal totalCompra = 0;
    foreach (var item in compra.Items)
    {
        if (item.Producto == null) return Results.BadRequest("Ocurrió un error con un producto del carrito.");
        if (item.Producto.Stock < item.Cantidad) return Results.BadRequest($"No hay suficiente stock para '{item.Producto.Nombre}'.");
        item.Producto.Stock -= item.Cantidad;
        totalCompra += item.Cantidad * item.PrecioUnitario;
    }
    compra.NombreCliente = cliente.Nombre;
    compra.ApellidoCliente = cliente.Apellido;
    compra.EmailCliente = cliente.Email;
    compra.Total = totalCompra;
    compra.Fecha = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok("¡Compra confirmada con éxito!");
});


app.Run();


public record ClienteDto(string Nombre, string Apellido, string Email);

// DTOs para evitar problemas de serialización en respuestas complejas.
public record ProductoResumenDto(int Id, string Nombre, decimal Precio, string Descripcion, int Stock, string ImagenUrl);
public record ItemCompraResumenDto(int Id, int ProductoId, int Cantidad, decimal PrecioUnitario, ProductoResumenDto Producto);
public record CompraResumenDto(int Id, List<ItemCompraResumenDto> Items);