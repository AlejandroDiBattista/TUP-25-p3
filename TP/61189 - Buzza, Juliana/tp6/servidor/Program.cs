using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; 


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221", "http://localhost:5000", "https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DatabaseInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowClientApp");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (string? query, AppDbContext db) =>
{
    IQueryable<Producto> productos = db.Productos;
    if (!string.IsNullOrEmpty(query))
    {
        string lowerQuery = query.ToLower();
        productos = productos.Where(p => p.Nombre.ToLower().Contains(lowerQuery) || p.Descripcion.ToLower().Contains(lowerQuery));
    }
    return Results.Ok(await productos.ToListAsync());
});

app.MapPost("/carritos", async (AppDbContext db) =>
{
    var newCart = new Compra { Status = "Pending", Fecha = DateTime.Now, Total = 0m };
    db.Compras.Add(newCart);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{newCart.Id}", newCart);
});

app.MapGet("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var cart = await db.Compras
                       .Include(c => c.Items)
                       .ThenInclude(ic => ic.Producto)
                       .FirstOrDefaultAsync(c => c.Id == carritoId && c.Status == "Pending");

    if (cart == null)
    {
        return Results.NotFound("Carrito no encontrado o ya confirmado.");
    }

    return Results.Ok(cart.Items.Select(item => new
    {
        item.Id,
        item.ProductoId,
        ProductoNombre = item.Producto?.Nombre,
        item.Cantidad,
        item.PrecioUnitario,
        ProductoImagenUrl = item.Producto?.ImagenUrl,
        ProductoStock = item.Producto?.Stock 
    }));
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, [FromBody] int cantidad, AppDbContext db) =>
{
    if (cantidad <= 0)
    {
        return Results.BadRequest("La cantidad debe ser mayor que cero.");
    }

    var cart = await db.Compras
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.Id == carritoId && c.Status == "Pending");
    if (cart == null)
    {
        return Results.NotFound("Carrito no encontrado o ya confirmado.");
    }

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
    {
        return Results.NotFound("Producto no encontrado.");
    }

    var cartItem = cart.Items.FirstOrDefault(item => item.ProductoId == productoId);

    if (cartItem == null)
    {
        if (producto.Stock < cantidad)
        {
            return Results.BadRequest($"No hay suficiente stock de {producto.Nombre}. Stock disponible: {producto.Stock}");
        }
        cart.Items.Add(new ItemCompra
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        });
        producto.Stock -= cantidad; 
    }
    else
    {
        int oldQuantity = cartItem.Cantidad;
        int quantityDifference = cantidad - oldQuantity;

        if (producto.Stock < quantityDifference)
        {
            return Results.BadRequest($"No hay suficiente stock de {producto.Nombre}. Stock disponible: {producto.Stock}");
        }
        cartItem.Cantidad = cantidad;
        producto.Stock -= quantityDifference; 
        cartItem.PrecioUnitario = producto.Precio; 
    }

    cart.Total = cart.Items.Sum(item => item.Cantidad * item.PrecioUnitario);
    await db.SaveChangesAsync();
    return Results.Ok(cart.Items.Select(item => new
    {
        item.Id,
        item.ProductoId,
        ProductoNombre = item.Producto?.Nombre, 
        item.Cantidad,
        item.PrecioUnitario,
        ProductoImagenUrl = item.Producto?.ImagenUrl,
        ProductoStock = item.Producto?.Stock
    }));
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, AppDbContext db) =>
{
    var cart = await db.Compras
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.Id == carritoId && c.Status == "Pending");
    if (cart == null)
    {
        return Results.NotFound("Carrito no encontrado o ya confirmado.");
    }

    var cartItem = cart.Items.FirstOrDefault(item => item.ProductoId == productoId);
    if (cartItem == null)
    {
        return Results.NotFound("Producto no encontrado en el carrito.");
    }

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += cartItem.Cantidad; 
    }

    cart.Items.Remove(cartItem);
    cart.Total = cart.Items.Sum(item => item.Cantidad * item.PrecioUnitario);
    await db.SaveChangesAsync();
    return Results.Ok(cart.Items);
});

app.MapDelete("/carritos/{carritoId}/vaciar", async (int carritoId, AppDbContext db) =>
{
    var cart = await db.Compras
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.Id == carritoId && c.Status == "Pending");
    if (cart == null)
    {
        return Results.NotFound("Carrito no encontrado o ya confirmado.");
    }

    foreach (var item in cart.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            producto.Stock += item.Cantidad;
        }
    }

    cart.Items.Clear();
    cart.Total = 0m;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{carritoId}/confirmar", async (int carritoId, CompraConfirmationDto confirmationData, AppDbContext db) =>
{
    var cart = await db.Compras
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.Id == carritoId && c.Status == "Pending");
    if (cart == null)
    {
        return Results.NotFound("Carrito no encontrado o ya confirmado.");
    }

    if (!cart.Items.Any())
    {
        return Results.BadRequest("El carrito está vacío, no se puede confirmar la compra.");
    }

    cart.NombreCliente = confirmationData.NombreCliente;
    cart.ApellidoCliente = confirmationData.ApellidoCliente;
    cart.EmailCliente = confirmationData.EmailCliente;
    cart.Status = "Confirmed";
    cart.Fecha = DateTime.Now; 
    cart.Total = cart.Items.Sum(item => item.Cantidad * item.PrecioUnitario); 

    await db.SaveChangesAsync();
    return Results.Ok(new
    {
        cart.Id,
        cart.Fecha,
        cart.Total,
        cart.NombreCliente,
        cart.ApellidoCliente,
        cart.EmailCliente,
        cart.Status,
        ItemsCount = cart.Items.Count 
    });
});


app.Run();
