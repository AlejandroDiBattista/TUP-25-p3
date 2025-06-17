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

app.MapGet("/carritos/{carritoId}/productos/{productoId}", async (int carritoId, int productoId, AppDbContext db) =>
{
    try
    {
        var cart = await db.Compras
                            .Include(c => c.Items)
                            .ThenInclude(ic => ic.Producto)
                            .FirstOrDefaultAsync(c => c.Id == carritoId && c.Status == "Pending");

        if (cart == null)
        {
            return Results.NotFound("Carrito no encontrado o ya confirmado.");
        }

        var item = cart.Items.FirstOrDefault(i => i.ProductoId == productoId);

        if (item == null)
        {
            return Results.NotFound($"Producto con ID {productoId} no encontrado en el carrito {carritoId}.");
        }

        return Results.Ok(new
        {
            item.Id,
            item.ProductoId,
            ProductoNombre = item.Producto?.Nombre,
            item.Cantidad,
            item.PrecioUnitario,
            ProductoImagenUrl = item.Producto?.ImagenUrl,
            ProductoStock = item.Producto?.Stock
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--- ERROR EN GET /carritos/{carritoId}/productos/{productoId} ---");
        Console.WriteLine($"Mensaje: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
        }
        Console.WriteLine("-------------------------------------------------");
        return Results.StatusCode(500); 
    }
});


app.MapPut("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, [FromBody] int cantidad, AppDbContext db) =>
{
    try
    {
        Console.WriteLine($"[PUT] Recibida solicitud para carritoId: {carritoId}, productoId: {productoId}, cantidad solicitada: {cantidad}");

        if (cantidad <= 0)
        {
            return Results.BadRequest("La cantidad debe ser mayor que cero.");
        }

        var cart = await db.Compras
                            .Include(c => c.Items)
                            .ThenInclude(ic => ic.Producto) 
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

        
        Console.WriteLine($"[PUT] Producto encontrado: {producto.Nombre}, Stock actual: {producto.Stock}");
        

        var cartItem = cart.Items.FirstOrDefault(item => item.ProductoId == productoId);

        if (cartItem == null)
        {
            Console.WriteLine($"[PUT] Añadiendo nuevo ítem. Cantidad solicitada: {cantidad}, Stock disponible: {producto.Stock}");
            if (producto.Stock < cantidad)
            {
                return Results.BadRequest($"No hay suficiente stock de {producto.Nombre}.");
            }

            var newItem = new ItemCompra
            {
                ProductoId = productoId,
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            };
            cart.Items.Add(newItem);
            producto.Stock -= cantidad;

            Console.WriteLine($"[PUT] Nuevo ítem añadido. Cantidad en carrito: {newItem.Cantidad}, Nuevo stock de producto: {producto.Stock}");
            
        }
        else
        {
            int oldQuantity = cartItem.Cantidad;
            int quantityDifference = cantidad - oldQuantity; 

            Console.WriteLine($"[PUT] Actualizando ítem existente. Cantidad vieja: {oldQuantity}, Cantidad solicitada: {cantidad}, Diferencia: {quantityDifference}, Stock disponible: {producto.Stock}");
            

            if (producto.Stock < quantityDifference)
            {
                return Results.BadRequest($"No hay suficiente stock de {producto.Nombre}");
            }
            if (cantidad > (oldQuantity + producto.Stock))
            {
                return Results.BadRequest($"No puedes tener más de {oldQuantity + producto.Stock} unidades de {producto.Nombre}.");
            }


            cartItem.Cantidad = cantidad; 
            producto.Stock -= quantityDifference; 
            cartItem.PrecioUnitario = producto.Precio; 
            Console.WriteLine($"[PUT] Ítem actualizado. Cantidad en carrito: {cartItem.Cantidad}, Nuevo stock de producto: {producto.Stock}");
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
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--- ERROR EN PUT /carritos/{carritoId}/{productoId} ---");
        Console.WriteLine($"Mensaje: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
        }
        Console.WriteLine("-------------------------------------------------");
        
        return Results.StatusCode(500); 
    }
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, AppDbContext db) =>
{
    var cart = await db.Compras
                        .Include(c => c.Items)
                        .ThenInclude(ic => ic.Producto) 
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

app.MapDelete("/carritos/{carritoId}/vaciar", async (int carritoId, AppDbContext db) =>
{
    var cart = await db.Compras
                        .Include(c => c.Items)
                        .ThenInclude(ic => ic.Producto) 
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
