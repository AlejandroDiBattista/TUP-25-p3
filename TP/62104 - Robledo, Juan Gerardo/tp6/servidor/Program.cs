using Microsoft.EntityFrameworkCore;
using servidor.data;
using servidor.models;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Crear la base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    context.Database.EnsureCreated();
}

app.UseCors("AllowBlazorClient");


// Almacenamiento en memoria para carritos (en producción sería una cache como Redis)
var carritos = new Dictionary<string, List<CarritoItem>>();

// API Endpoints

// Endpoint para traer todos los productos sin filtro
app.MapGet("/productos", async (TiendaDbContext db) =>
{
    var productos = await db.Productos.ToListAsync();
    return Results.Ok(productos);
});

// Endpoint para buscar productos por nombre o descripción
app.MapGet("/productos/buscar", async (TiendaDbContext db, string termino) =>
{
    if (string.IsNullOrWhiteSpace(termino))
    {
        return Results.BadRequest("Debe especificar un término de búsqueda.");
    }

    var terminoLower = termino.ToLower();

    var productosFiltrados = await db.Productos
        .Where(p => p.Nombre.ToLower().Contains(terminoLower))
        .ToListAsync();

    return Results.Ok(productosFiltrados);
});

// POST /carritos - Inicializar un carrito
app.MapPost("/carritos", () =>
{
    var carritoId = Guid.NewGuid().ToString();
    carritos[carritoId] = new List<CarritoItem>();
    return Results.Ok(new { CarritoId = carritoId });
});

// GET /carritos/{carritoId} - Obtener items del carrito
app.MapGet("/carritos/{carritoId}", (string carritoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    
    return Results.Ok(carritos[carritoId]);
});

// DELETE /carritos/{carritoId} - Vaciar carrito
app.MapDelete("/carritos/{carritoId}", async (string carritoId, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    
    // Restaurar stock de productos en el carrito
    foreach (var item in carritos[carritoId])
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            producto.Stock += item.Cantidad;
        }
    }
    await db.SaveChangesAsync();
    
    carritos[carritoId].Clear();
    return Results.Ok("Carrito vaciado");
});

// PUT /carritos/{carritoId}/{productoId} - Agregar/actualizar producto en carrito
app.MapPut("/carritos/{carritoId}/{productoId:int}", async (string carritoId, int productoId, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");
    
    if (producto.Stock <= 0)
        return Results.BadRequest("Producto sin stock");
    
    var carrito = carritos[carritoId];
    var itemExistente = carrito.FirstOrDefault(i => i.ProductoId == productoId);
    
    if (itemExistente != null)
    {
        // Actualizar cantidad
        itemExistente.Cantidad++;
    }
    else
    {
        // Agregar nuevo item
        carrito.Add(new CarritoItem
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Cantidad = 1,
            ImagenUrl = producto.ImagenUrl
        });
    }
    
    // Reducir stock
    producto.Stock--;
    await db.SaveChangesAsync();
    
    return Results.Ok("Producto agregado al carrito");
});

// DELETE /carritos/{carritoId}/{productoId} - Eliminar/reducir producto del carrito
app.MapDelete("/carritos/{carritoId}/{productoId:int}", async (string carritoId, int productoId, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    
    var carrito = carritos[carritoId];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
    
    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");
    
    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock++; // Restaurar stock
    }
    
    if (item.Cantidad > 1)
    {
        item.Cantidad--;
    }
    else
    {
        carrito.Remove(item);
    }
    
    await db.SaveChangesAsync();
    return Results.Ok("Producto actualizado en carrito");
});

// PUT /carritos/{carritoId}/confirmar - Confirmar compra
app.MapPut("/carritos/{carritoId}/confirmar", async (string carritoId, ConfirmarCompraRequest request, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId) || carritos[carritoId].Count == 0)
        return Results.BadRequest("Carrito vacío o no encontrado");
    
    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        return Results.BadRequest(validationResults.Select(r => r.ErrorMessage));
    }
    
    var carrito = carritos[carritoId];
    var total = carrito.Sum(i => i.Subtotal);
    
    var compra = new Compra
    {
        Fecha = DateTime.UtcNow,
        Total = total,
        NombreCliente = request.NombreCliente,
        ApellidoCliente = request.ApellidoCliente,
        EmailCliente = request.EmailCliente
    };
    
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    
    // Agregar items de compra
    foreach (var item in carrito)
    {
        db.ItemsCompra.Add(new ItemCompra
        {
            ProductoId = item.ProductoId,
            CompraId = compra.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.Precio
        });
    }
    
    await db.SaveChangesAsync();
    
    // Limpiar carrito
    carritos[carritoId].Clear();
    
    return Results.Ok(new { CompraId = compra.Id, Total = total });
});

app.Run();