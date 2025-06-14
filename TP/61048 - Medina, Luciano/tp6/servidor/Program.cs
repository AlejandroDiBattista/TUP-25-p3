using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using System.Text.Json.Serialization; // <-- ASEGÚRATE DE TENER ESTE USING

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---

// Agregar servicios CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- ** CORRECCIÓN AQUÍ ** ---
// Configurar el serializador JSON para ignorar ciclos.
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


// Registrar el DbContext para Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// --- 2. CONFIGURACIÓN DEL PIPELINE DE SOLICITUDES HTTP ---

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

// --- 3. DEFINICIÓN DE LOS ENDPOINTS DE LA API ---

app.MapGet("/", () => "Servidor de la Tienda de Perfumes está en funcionamiento.");

// GET /api/productos (+ búsqueda opcional por nombre)
app.MapGet("/api/productos", async (ApplicationDbContext db, string? busqueda) =>
{
    if (string.IsNullOrWhiteSpace(busqueda))
    {
        return Results.Ok(await db.Productos.ToListAsync());
    }

    var productosFiltrados = await db.Productos
        .Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower()))
        .ToListAsync();

    return Results.Ok(productosFiltrados);
});

// POST /api/carritos (Crea una nueva compra en estado "pendiente" que actúa como carrito)
app.MapPost("/api/carritos", async (ApplicationDbContext db) =>
{
    var nuevaCompra = new Compra { Fecha = DateTime.UtcNow };
    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();
    return Results.Ok(nuevaCompra.Id);
});

// GET /api/carritos/{carritoId} (Obtiene el contenido de un carrito)
app.MapGet("/api/carritos/{carritoId:int}", async (ApplicationDbContext db, int carritoId) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound("Carrito no encontrado.");

    return Results.Ok(compra);
});

// PUT /api/carritos/{carritoId}/agregar/{productoId} (Agrega o actualiza un producto en el carrito)
app.MapPut("/api/carritos/{carritoId:int}/agregar/{productoId:int}", async (ApplicationDbContext db, int carritoId, int productoId, int cantidad) =>
{
    if (cantidad <= 0) return Results.BadRequest("La cantidad debe ser mayor a cero.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado.");

    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado.");

    var itemExistente = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    
    int cantidadAValidar = itemExistente != null ? cantidad : (itemExistente?.Cantidad ?? 0) + cantidad;

    if (producto.Stock < cantidadAValidar && itemExistente == null) {
        return Results.BadRequest($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {cantidadAValidar}");
    }
     if (itemExistente != null && producto.Stock < cantidad) {
        return Results.BadRequest($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {cantidad}");
    }

    if (itemExistente != null)
    {
        itemExistente.Cantidad = cantidad;
    }
    else
    {
        var nuevoItem = new ItemCompra
        {
            ProductoId = productoId,
            CompraId = carritoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        };
        compra.Items.Add(nuevoItem);
    }

    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado/actualizado en el carrito.");
});

// DELETE /api/carritos/{carritoId}/remover/{productoId} (Elimina un producto del carrito)
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

// PUT /api/carritos/{carritoId}/confirmar (Confirma la compra)
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
        if (item.Producto.Stock < item.Cantidad)
        {
            return Results.BadRequest($"No hay suficiente stock para '{item.Producto.Nombre}'. Disponible: {item.Producto.Stock}, en carrito: {item.Cantidad}.");
        }
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

// --- 4. EJECUTAR LA APLICACIÓN ---
app.Run();

// --- DEFINICIÓN DE TIPOS (DTOs) ---
public record ClienteDto(string Nombre, string Apellido, string Email);
