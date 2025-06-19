using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using servidor.Modelos;

var CarritosActivos = new ConcurrentDictionary<Guid, List<CarritoItem>>();
var builder = WebApplication.CreateBuilder(args);

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TiendaDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CONFIGURACIÓN CLAVE - UseStaticFiles debe ir antes que UseCors
app.UseStaticFiles();

app.UseCors();

// Asegurar que la base de datos existe
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    context.Database.EnsureCreated();
}

var api = app.MapGroup("/api");

// Endpoint para probar que las imágenes estén funcionando
app.MapGet("/test-images", () =>
{
    var webRootPath = app.Environment.WebRootPath;
    var imagesPath = Path.Combine(webRootPath, "images");
    
    if (!Directory.Exists(imagesPath))
    {
        return Results.Json(new { 
            error = "La carpeta images no existe", 
            expectedPath = imagesPath,
            webRootExists = Directory.Exists(webRootPath)
        });
    }
    
    var imageFiles = Directory.GetFiles(imagesPath, "*.jpg")
        .Concat(Directory.GetFiles(imagesPath, "*.png"))
        .Select(f => Path.GetFileName(f))
        .ToArray();
    
    return Results.Json(new { 
        imagesPath = imagesPath,
        imageFiles = imageFiles,
        count = imageFiles.Length
    });
});

// ENDPOINT DE PRODUCTOS CORREGIDO - Búsqueda case-insensitive
api.MapGet("/productos", async (string? q, TiendaDbContext db) =>
{
    var query = db.Productos.AsQueryable();
    
    if (!string.IsNullOrWhiteSpace(q))
    {
        // CORRECCIÓN: Hacer la búsqueda case-insensitive usando ToLower()
        var searchTerm = q.ToLower();
        query = query.Where(p => 
            p.Nombre.ToLower().Contains(searchTerm) || 
            p.Descripcion.ToLower().Contains(searchTerm));
    }
    
    var productos = await query.ToListAsync();
    
    
    return Results.Ok(productos);
});

api.MapPost("/carritos", () =>
{
    var carritoId = Guid.NewGuid();
    CarritosActivos[carritoId] = new List<CarritoItem>();
    return Results.Ok(new { CarritoId = carritoId });
});

api.MapGet("/carritos/{carritoId:guid}", (Guid carritoId) =>
{
    if (CarritosActivos.TryGetValue(carritoId, out var items)) return Results.Ok(items);
    return Results.NotFound("Carrito no encontrado.");
});

api.MapDelete("/carritos/{carritoId:guid}", (Guid carritoId) =>
{
    if (CarritosActivos.TryGetValue(carritoId, out var carrito))
    {
        carrito.Clear();
        return Results.NoContent();
    }
    return Results.NotFound("Carrito no encontrado.");
});

api.MapPut("/carritos/{carritoId:guid}/{productoId:int}", async (Guid carritoId, int productoId, TiendaDbContext db) =>
{
    if (!CarritosActivos.TryGetValue(carritoId, out var carrito)) return Results.NotFound("Carrito no encontrado.");
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado.");
    var itemExistente = carrito.FirstOrDefault(item => item.ProductoId == productoId);
    int cantidadEnCarrito = itemExistente?.Cantidad ?? 0;
    if (producto.Stock <= cantidadEnCarrito) return Results.Conflict("No hay suficiente stock.");

    if (itemExistente != null)
    {
        carrito.Remove(itemExistente);
        carrito.Add(itemExistente with { Cantidad = itemExistente.Cantidad + 1 });
    }
    else
    {
        carrito.Add(new CarritoItem(productoId, 1, producto.Precio));
    }
    return Results.Ok(carrito);
});

api.MapDelete("/carritos/{carritoId:guid}/{productoId:int}", (Guid carritoId, int productoId) =>
{
    if (!CarritosActivos.TryGetValue(carritoId, out var carrito)) return Results.NotFound("Carrito no encontrado.");
    var itemExistente = carrito.FirstOrDefault(item => item.ProductoId == productoId);
    if (itemExistente == null) return Results.NotFound("Producto no encontrado en el carrito.");

    if (itemExistente.Cantidad > 1)
    {
        carrito.Remove(itemExistente);
        carrito.Add(itemExistente with { Cantidad = itemExistente.Cantidad - 1 });
    }
    else
    {
        carrito.Remove(itemExistente);
    }
    return Results.Ok(carrito);
});

api.MapPut("/carritos/{carritoId:guid}/confirmar", async (Guid carritoId, DatosClienteDto datosCliente, TiendaDbContext db) =>
{
    if (!CarritosActivos.TryGetValue(carritoId, out var carrito) || !carrito.Any()) return Results.BadRequest("El carrito no existe o está vacío.");
    using var transaction = await db.Database.BeginTransactionAsync();
    try
    {
        var nuevaCompra = new Compra
        {
            Fecha = DateTime.UtcNow,
            NombreCliente = datosCliente.Nombre,
            ApellidoCliente = datosCliente.Apellido,
            EmailCliente = datosCliente.Email,
            Total = carrito.Sum(i => i.Cantidad * i.PrecioUnitario)
        };
        foreach (var item in carrito)
        {
            var productoEnDb = await db.Productos.FindAsync(item.ProductoId);
            if (productoEnDb == null || productoEnDb.Stock < item.Cantidad)
            {
                await transaction.RollbackAsync();
                return Results.Conflict($"Stock insuficiente para el producto ID {item.ProductoId}.");
            }
            productoEnDb.Stock -= item.Cantidad;
            nuevaCompra.Items.Add(new ItemCompra { ProductoId = item.ProductoId, Cantidad = item.Cantidad, PrecioUnitario = item.PrecioUnitario });
        }
        db.Compras.Add(nuevaCompra);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        CarritosActivos.TryRemove(carritoId, out _);
        return Results.Created($"/compras/{nuevaCompra.Id}", nuevaCompra);
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        return Results.Problem("Ocurrió un error al procesar la compra.");
    }
});

app.Run();