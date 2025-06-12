#nullable enable
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Compartido;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tienda.db";
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(connectionString));


var app = builder.Build();

app.UseStaticFiles(); 

app.UseCors("AllowClientApp");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TiendaDbContext>();
        SeedData.Initialize(context);     
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Un error ocurrió al cargar datos de ejemplo (seeding the DB).");
    }
}

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/api/productos", async (TiendaDbContext dbContext, string? busqueda) =>
{
    var query = dbContext.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        query = query.Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower()) ||
                                 p.Descripcion.ToLower().Contains(busqueda.ToLower()));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

app.MapGet("/api/productos/{id:int}", async (int id, TiendaDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    return producto == null ? Results.NotFound() : Results.Ok(producto);
});

app.MapGet("/api/carritos/{carritoId:guid}", async (Guid carritoId, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Compras
                                 .Include(c => c.Items)
                                 .ThenInclude(ic => ic.Producto) 
                                 .FirstOrDefaultAsync(c => c.Id == carritoId && c.FechaCompra == null);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
    }

    var itemsDto = carrito.Items.Select(ic => new 
    {
        ic.ProductoId,
        NombreProducto = ic.Producto?.Nombre,
        ic.Cantidad,
        PrecioUnitario = ic.PrecioUnitario,
        ImagenUrl = ic.Producto?.ImagenUrl
    }).ToList();

    return Results.Ok(itemsDto);
});

app.MapPut("/api/carritos/{carritoId:guid}/{productoId:int}", 
    async (Guid carritoId, int productoId, CantidadRequest request, TiendaDbContext dbContext) =>
{
    if (request.Cantidad <= 0)
    {
        return Results.BadRequest(new { Mensaje = "La cantidad debe ser mayor que cero." });
    }

    var carrito = await dbContext.Compras
                                 .Include(c => c.Items)
                                 .FirstOrDefaultAsync(c => c.Id == carritoId && c.FechaCompra == null);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado." });
    }

    var producto = await dbContext.Productos.FindAsync(productoId);

    if (producto == null)
    {
        return Results.NotFound(new { Mensaje = "Producto no encontrado." });
    }
    
    var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (itemEnCarrito == null) 
    {
        if (producto.Stock < request.Cantidad)
        {
            return Results.BadRequest(new { Mensaje = $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {request.Cantidad}." });
        }
        itemEnCarrito = new ItemCompra
        {
            CompraId = carritoId,
            ProductoId = productoId,
            Cantidad = request.Cantidad,
            PrecioUnitario = producto.Precio 
        };
        dbContext.ItemsCompra.Add(itemEnCarrito);
    }
    else 
    {
        if (producto.Stock < request.Cantidad && request.Cantidad > itemEnCarrito.Cantidad) 
        {
             return Results.BadRequest(new { Mensaje = $"Stock insuficiente para aumentar la cantidad de '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado en total: {request.Cantidad}." });
        }

        itemEnCarrito.Cantidad = request.Cantidad;
        itemEnCarrito.PrecioUnitario = producto.Precio; 
    }
    
    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await dbContext.SaveChangesAsync();

    var itemDto = new 
    {
        itemEnCarrito.ProductoId,
        NombreProducto = producto?.Nombre,
        itemEnCarrito.Cantidad,
        PrecioUnitario = producto?.Precio,
        ImagenUrl = producto?.ImagenUrl,
        Subtotal = itemEnCarrito.Cantidad * (producto?.Precio ?? 0)
    };

    return Results.Ok(itemDto);
});

app.MapDelete("/api/carritos/{carritoId:guid}/{productoId:int}", 
    async (Guid carritoId, int productoId, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Compras
                                 .Include(c => c.Items)
                                 .FirstOrDefaultAsync(c => c.Id == carritoId && c.FechaCompra == null);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado." });
    }

    var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (itemEnCarrito == null)
    {
        return Results.NotFound(new { Mensaje = "Producto no encontrado en el carrito." });
    }

    dbContext.ItemsCompra.Remove(itemEnCarrito);
        
    carrito.Total = carrito.Items.Where(i => i.ProductoId != productoId).Sum(i => i.Cantidad * i.PrecioUnitario);
    
    await dbContext.SaveChangesAsync();

    return Results.NoContent(); 
});

app.MapDelete("/api/carritos/{carritoId:guid}", 
    async (Guid carritoId, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Compras
                                 .Include(c => c.Items)
                                 .FirstOrDefaultAsync(c => c.Id == carritoId && c.FechaCompra == null);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado." });
    }

    if (carrito.Items.Any())
    {
        dbContext.ItemsCompra.RemoveRange(carrito.Items);
    }
    carrito.Total = 0; 
    
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/api/carritos/{carritoId:guid}/confirmar", 
    async (Guid carritoId, ConfirmarCompraRequest request, TiendaDbContext dbContext) => 
{
    if (string.IsNullOrWhiteSpace(request.Nombre) || 
        string.IsNullOrWhiteSpace(request.Apellido) || 
        string.IsNullOrWhiteSpace(request.Email))
    {
        return Results.BadRequest(new { Mensaje = "Nombre, Apellido y Email del cliente son obligatorios." });
    }

    var carritoPendiente = await dbContext.Compras
                                 .Include(c => c.Items)
                                 .ThenInclude(ic => ic.Producto) 
                                 .FirstOrDefaultAsync(c => c.Id == carritoId && c.FechaCompra == null); 

    if (carritoPendiente == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito pendiente no encontrado o ya confirmado." });
    }

    if (!carritoPendiente.Items.Any())
    {
        return Results.BadRequest(new { Mensaje = "El carrito está vacío." });
    }

    var erroresStock = new List<string>();
    foreach (var item in carritoPendiente.Items)
    {
        if (item.Producto == null) 
        {
            return Results.Problem($"Error interno crítico: Producto con ID {item.ProductoId} no encontrado. Por favor, contacte a soporte.");
        }
        if (item.Producto.Stock < item.Cantidad)
        {
            erroresStock.Add($"Stock insuficiente para '{item.Producto.Nombre}'. Disponible: {item.Producto.Stock}, Solicitado: {item.Cantidad}.");
        }
    }

    if (erroresStock.Any())
    {
        return Results.BadRequest(new { Mensaje = "Error de stock.", Errores = erroresStock });
    }

    foreach (var item in carritoPendiente.Items)
    {
        item.Producto!.Stock -= item.Cantidad;
    }
    
    carritoPendiente.FechaCompra = DateTime.UtcNow;
    carritoPendiente.NombreCliente = request.Nombre;
    carritoPendiente.ApellidoCliente = request.Apellido;
    carritoPendiente.EmailCliente = request.Email;
    
    carritoPendiente.Total = carritoPendiente.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await dbContext.SaveChangesAsync();

    return Results.Ok(new 
    {
        Mensaje = "Compra confirmada exitosamente.",
        CompraId = carritoPendiente.Id,
        carritoPendiente.Total,
        ItemsComprados = carritoPendiente.Items.Count
    });
});

app.Run();
