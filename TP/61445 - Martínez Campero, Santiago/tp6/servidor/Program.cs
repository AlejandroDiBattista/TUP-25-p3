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

// Seed data (cargar productos de ejemplo)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TiendaDbContext>();
        // Asegurarse que la base de datos esté creada
        context.Database.EnsureCreated(); 
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

app.UseCors("AllowClientApp");

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

app.MapPost("/api/carritos", async (TiendaDbContext dbContext) =>
{
    var nuevoCarrito = new Carrito
    {
        Id = Guid.NewGuid(), 
        FechaCreacion = DateTime.UtcNow
    };

    dbContext.Carritos.Add(nuevoCarrito);
    await dbContext.SaveChangesAsync();

    return Results.Ok(new { CarritoId = nuevoCarrito.Id });
});

app.MapGet("/api/carritos/{carritoId:guid}", async (Guid carritoId, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Carritos
                                 .Include(c => c.Items)
                                 .ThenInclude(ic => ic.Producto) 
                                 .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
    }

    var itemsDto = carrito.Items.Select(ic => new 
    {
        ic.ProductoId,
        NombreProducto = ic.Producto?.Nombre,
        ic.Cantidad,
        PrecioUnitario = ic.Producto?.Precio,
        ImagenUrl = ic.Producto?.ImagenUrl
    }).ToList();

    return Results.Ok(itemsDto);
});

app.MapPost("/api/carritos/{carritoId:guid}/items", async (Guid carritoId, ItemCarrito item, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Carritos.FindAsync(carritoId);
    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
    }

    var producto = await dbContext.Productos.FindAsync(item.ProductoId);
    if (producto == null)
    {
        return Results.NotFound(new { Mensaje = "Producto no encontrado" });
    }

    item.CarritoId = carritoId;
    item.Carrito = carrito;
    item.Producto = producto;

    dbContext.ItemsCarrito.Add(item);
    await dbContext.SaveChangesAsync();

    return Results.Ok(item);
});

app.MapPut("/api/carritos/{carritoId:guid}/{productoId:int}", 
    async (Guid carritoId, int productoId, AgregarActualizarItemCarritoRequest request, TiendaDbContext dbContext) =>
{
    if (request.Cantidad <= 0)
    {
        return Results.BadRequest(new { Mensaje = "La cantidad debe ser mayor que cero." });
    }

    var carrito = await dbContext.Carritos
                                 .Include(c => c.Items)
                                 .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado." });
    }

    var producto = await dbContext.Productos.FindAsync(productoId);

    if (producto == null)
    {
        return Results.NotFound(new { Mensaje = "Producto no encontrado." });
    }
    
   
    if (producto.Stock < request.Cantidad)
    {
        return Results.BadRequest(new { Mensaje = $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {request.Cantidad}." });
    }

    var itemCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (itemCarrito == null) 
    {
        itemCarrito = new ItemCarrito
        {
            CarritoId = carritoId,
            ProductoId = productoId,
            Cantidad = request.Cantidad,
        };
        dbContext.ItemsCarrito.Add(itemCarrito); 
    }
    else 
    {
        itemCarrito.Cantidad = request.Cantidad;
    }

    await dbContext.SaveChangesAsync();

    if (itemCarrito.Producto == null)
    {
        itemCarrito.Producto = producto; 
    }
    
    var itemDto = new 
    {
        ItemCarritoId = itemCarrito.Id,
        itemCarrito.ProductoId,
        NombreProducto = itemCarrito.Producto?.Nombre,
        itemCarrito.Cantidad,
        PrecioUnitario = itemCarrito.Producto?.Precio,
        ImagenUrl = itemCarrito.Producto?.ImagenUrl
    };

    return Results.Ok(itemDto);
});

app.MapDelete("/api/carritos/{carritoId:guid}/{productoId:int}", 
    async (Guid carritoId, int productoId, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Carritos
                                 .Include(c => c.Items) 
                                 .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado." });
    }

    var itemCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (itemCarrito == null)
    {
        return Results.NotFound(new { Mensaje = "Producto no encontrado en el carrito." });
    }

    dbContext.ItemsCarrito.Remove(itemCarrito);
    await dbContext.SaveChangesAsync();

    return Results.NoContent(); 
});

app.MapDelete("/api/carritos/{carritoId:guid}", 
    async (Guid carritoId, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Carritos
                                 .Include(c => c.Items)
                                 .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado." });
    }

    if (carrito.Items.Any())
    {
        dbContext.ItemsCarrito.RemoveRange(carrito.Items);
    }
    
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/api/carritos/{carritoId:guid}/confirmar", 
    async (Guid carritoId, ConfirmarCompraRequest request, TiendaDbContext dbContext) => 
{
    if (string.IsNullOrWhiteSpace(request.NombreCliente) || 
        string.IsNullOrWhiteSpace(request.ApellidoCliente) || 
        string.IsNullOrWhiteSpace(request.EmailCliente))
    {
        return Results.BadRequest(new { Mensaje = "Nombre, Apellido y Email del cliente son obligatorios." });
    }

    var carrito = await dbContext.Carritos
                                 .Include(c => c.Items)
                                 .ThenInclude(ic => ic.Producto)
                                 .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado." });
    }

    if (!carrito.Items.Any())
    {
        return Results.BadRequest(new { Mensaje = "El carrito está vacío." });
    }

    var erroresStock = new List<string>();
    foreach (var item in carrito.Items)
    {
        if (item.Producto == null)
        {
            return Results.Problem($"Error interno: Producto con ID {item.ProductoId} no encontrado para el item del carrito.");
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

    var nuevaCompra = new Compra
    {
        Fecha = DateTime.UtcNow,
        NombreCliente = request.NombreCliente,
        ApellidoCliente = request.ApellidoCliente,
        EmailCliente = request.EmailCliente,
        Items = new List<ItemCompra>(),
        Total = 0
    };

    decimal totalCompra = 0;

    foreach (var itemCarrito in carrito.Items)
    {
        var producto = itemCarrito.Producto;
        var itemCompra = new ItemCompra
        {
            ProductoId = producto.Id,
            Compra = nuevaCompra,
            Cantidad = itemCarrito.Cantidad,
            PrecioUnitario = producto.Precio
        };
        nuevaCompra.Items.Add(itemCompra);
        totalCompra += itemCompra.Cantidad * itemCompra.PrecioUnitario;

        producto.Stock -= itemCarrito.Cantidad;
    }

    nuevaCompra.Total = totalCompra;
    dbContext.Compras.Add(nuevaCompra);

    dbContext.ItemsCarrito.RemoveRange(carrito.Items);

    await dbContext.SaveChangesAsync();

    return Results.Ok(new 
    {
        Mensaje = "Compra confirmada exitosamente.",
        CompraId = nuevaCompra.Id,
        nuevaCompra.Total,
        ItemsComprados = nuevaCompra.Items.Count
    });
});

app.Run();

public record AgregarActualizarItemCarritoRequest(int Cantidad);
public record ConfirmarCompraRequest(string NombreCliente, string ApellidoCliente, string EmailCliente);
