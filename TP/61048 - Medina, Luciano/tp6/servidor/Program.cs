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


// --- **NUEVO CÓDIGO PARA CREAR LA BD AUTOMÁTICAMENTE** ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    // Esta línea asegura que la base de datos se cree si no existe.
    // Ya no se necesitan migraciones.
    context.Database.EnsureCreated();
}
// --- FIN DEL CÓDIGO NUEVO ---


// --- 2. CONFIGURACIÓN DEL PIPELINE DE SOLICITUDES HTTP ---

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowClientApp");
app.UseStaticFiles();

// --- 3. DEFINICIÓN DE LOS ENDPOINTS DE LA API ---
// (El resto de tus endpoints de la API va aquí, sin cambios)
#region Endpoints de la API

app.MapGet("/", () => "Servidor de la Tienda de Perfumes está en funcionamiento.");

app.MapGet("/api/productos", async (ApplicationDbContext db, string? busqueda) =>
{
    if (string.IsNullOrWhiteSpace(busqueda))
    {
        return Results.Ok(await db.Productos.AsNoTracking().ToListAsync());
    }
    return Results.Ok(await db.Productos.AsNoTracking().Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower())).ToListAsync());
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
            i.Id, i.ProductoId, i.Cantidad, i.PrecioUnitario,
            new ProductoResumenDto(i.Producto.Id, i.Producto.Nombre, i.Producto.Precio, i.Producto.Descripcion, i.Producto.Stock, i.Producto.ImagenUrl)
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
    int cantidadPrevia = itemExistente?.Cantidad ?? 0;
    
    int stockTotalDisponible = producto.Stock + cantidadPrevia;

    if (stockTotalDisponible < cantidad)
    {
        return Results.BadRequest($"Stock insuficiente. Solo quedan {stockTotalDisponible} unidades en total.");
    }

    int cambioEnCantidad = cantidad - cantidadPrevia;
    producto.Stock -= cambioEnCantidad;

    if (itemExistente != null)
    {
        itemExistente.Cantidad = cantidad;
    }
    else
    {
        compra.Items.Add(new ItemCompra { ProductoId = productoId, CompraId = carritoId, Cantidad = cantidad, PrecioUnitario = producto.Precio });
    }
    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado/actualizado y stock modificado.");
});

app.MapPost("/api/carritos/{carritoId:int}/incrementar/{productoId:int}", async (ApplicationDbContext db, int carritoId, int productoId) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado.");
    if (producto.Stock < 1) return Results.BadRequest("Stock insuficiente.");
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado.");
    var itemExistente = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    producto.Stock -= 1;
    if (itemExistente != null)
    {
        itemExistente.Cantidad += 1;
    }
    else
    {
        compra.Items.Add(new ItemCompra { ProductoId = productoId, CompraId = carritoId, Cantidad = 1, PrecioUnitario = producto.Precio });
    }
    await db.SaveChangesAsync();
    return Results.Ok("Producto incrementado en el carrito.");
});

app.MapDelete("/api/carritos/{carritoId:int}/remover/{productoId:int}", async (ApplicationDbContext db, int carritoId, int productoId) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado.");
    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("El producto no está en el carrito.");
    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null) producto.Stock += item.Cantidad;
    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok("Producto eliminado y stock restaurado.");
});

app.MapPut("/api/carritos/{carritoId:int}/confirmar", async (ApplicationDbContext db, int carritoId, ClienteDto cliente) =>
{
    var compra = await db.Compras.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado.");
    if (!compra.Items.Any()) return Results.BadRequest("El carrito está vacío.");
    decimal totalCompra = 0;
    foreach (var item in compra.Items) totalCompra += item.Cantidad * item.PrecioUnitario;
    compra.NombreCliente = cliente.Nombre;
    compra.ApellidoCliente = cliente.Apellido;
    compra.EmailCliente = cliente.Email;
    compra.Total = totalCompra;
    compra.Fecha = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok("¡Compra confirmada con éxito!");
});

#endregion

app.Run();

// --- DEFINICIÓN DE TIPOS (DTOs) ---
public record ClienteDto(string Nombre, string Apellido, string Email);
public record ProductoResumenDto(int Id, string Nombre, decimal Precio, string Descripcion, int Stock, string ImagenUrl);
public record ItemCompraResumenDto(int Id, int ProductoId, int Cantidad, decimal PrecioUnitario, ProductoResumenDto Producto);
public record CompraResumenDto(int Id, List<ItemCompraResumenDto> Items);
