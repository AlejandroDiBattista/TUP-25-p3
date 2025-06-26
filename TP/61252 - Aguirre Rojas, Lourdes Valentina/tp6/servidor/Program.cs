using Microsoft.EntityFrameworkCore;
using servidor.Data;
using shared.Models;
using shared.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http.Json;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Tienda Online API",
            Version = "v1"
        });
    });


var app = builder.Build();


app.UseCors("AllowBlazorClient");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TiendaOnline API v1");
    });
}


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    context.Database.EnsureCreated();

if (!context.Productos.Any())
{
    var productos = new List<Producto>
    {
        new() { Nombre = "Zapatillas Urbanas Rosé", Descripcion = "Zapatillas cómodas y modernas con detalles en rosa pastel. Perfectas para un look casual con estilo.", Precio = 18999m, Stock = 20, ImagenUrl = "https://calzalindo.com.ar/1614-large_default/zapatillas-urbanas-jaguar-709-rosa-m.jpg" },
        new() { Nombre = "Botas Clásicas Cuero Negro", Descripcion = "Elegancia y abrigo para el invierno. Estas botas de cuero negro combinan con cualquier outfit.", Precio = 24999m, Stock = 15, ImagenUrl = "https://calzalindo.com.ar/1614-large_default/zapatillas-urbanas-jaguar-709-rosa-m.jpg" },
        new() { Nombre = "Sandalias Nude con Plataforma", Descripcion = "Ideales para días soleados, estas sandalias nude con plataforma realzan tu figura con confort.", Precio = 14999m, Stock = 25, ImagenUrl = "https://calzalindo.com.ar/1614-large_default/zapatillas-urbanas-jaguar-709-rosa-m.jpg" },
        new() { Nombre = "Stilettos Vino Intenso", Descripcion = "Para eventos o noches especiales, estos stilettos vino tinto aportan sofisticación a tu estilo.", Precio = 20999m, Stock = 10, ImagenUrl = "https://calzalindo.com.ar/1614-large_default/zapatillas-urbanas-jaguar-709-rosa-m.jpg" },
        new() { Nombre = "Mocasines Livianos Beige", Descripcion = "Estilo europeo para tu día a día. Estos mocasines beige ofrecen comodidad sin perder elegancia.", Precio = 13999m, Stock = 30, ImagenUrl = "https://calzalindo.com.ar/1614-large_default/zapatillas-urbanas-jaguar-709-rosa-m.jpg" }
    };

    context.Productos.AddRange(productos);
    context.SaveChanges();
}
}


app.MapGet("/productos", async (TiendaContext context, string? q) =>
{
    var query = context.Productos.AsQueryable();

    if (!string.IsNullOrEmpty(q))
    {
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    }

    return await query.ToListAsync();
});

app.MapPost("/carritos", async (TiendaContext context) =>
{
    var carritoId = Guid.NewGuid().ToString();
    var carrito = new Carrito
    {
        Id = carritoId,
        FechaCreacion = DateTime.UtcNow
    };

    context.Carritos.Add(carrito);
    await context.SaveChangesAsync();

    return new CarritoResponse { CarritoId = carritoId };
});

app.MapGet("/carritos/{carrito}", async (TiendaContext context, string carrito) =>
{
    var items = await (from ic in context.ItemsCarrito
                       join p in context.Productos on ic.ProductoId equals p.Id
                       where ic.CarritoId == carrito
                       select new ItemCarrito
                       {
                           Id = ic.Id,
                           CarritoId = ic.CarritoId,
                           ProductoId = ic.ProductoId,
                           Cantidad = ic.Cantidad,
                           Nombre = p.Nombre,
                           Precio = p.Precio,
                           ImagenUrl = p.ImagenUrl,
                           Stock = p.Stock
                       }).ToListAsync();

    return items;
});

app.MapDelete("/carritos/{carrito}", async (TiendaContext context, string carrito) =>
{
    var items = context.ItemsCarrito.Where(ic => ic.CarritoId == carrito);
    context.ItemsCarrito.RemoveRange(items);
    await context.SaveChangesAsync();

    return Results.Ok(new { success = true });
});

app.MapPut("/carritos/{carrito}/confirmar", async (TiendaContext context, string carrito, DatosCliente datosCliente) =>
{
    if (string.IsNullOrEmpty(datosCliente.NombreCliente) ||
        string.IsNullOrEmpty(datosCliente.ApellidoCliente) ||
        string.IsNullOrEmpty(datosCliente.EmailCliente))
    {
        return Results.BadRequest(new { error = "Todos los campos son obligatorios" });
    }

    var items = await (from ic in context.ItemsCarrito
                       join p in context.Productos on ic.ProductoId equals p.Id
                       where ic.CarritoId == carrito
                       select new { ic, p }).ToListAsync();

    if (!items.Any())
    {
        return Results.BadRequest(new { error = "El carrito está vacío" });
    }

    var total = items.Sum(item => item.ic.Cantidad * item.p.Precio);
    var compraId = Guid.NewGuid().ToString();

    using var transaction = await context.Database.BeginTransactionAsync();
    try
    {
        var compra = new Compra
        {
            Id = compraId,
            Fecha = DateTime.UtcNow,
            Total = total,
            NombreCliente = datosCliente.NombreCliente,
            ApellidoCliente = datosCliente.ApellidoCliente,
            EmailCliente = datosCliente.EmailCliente
        };

        context.Compras.Add(compra);

        foreach (var item in items)
        {
            var itemCompra = new ItemCompra
            {
                Id = Guid.NewGuid().ToString(),
                ProductoId = item.ic.ProductoId,
                CompraId = compraId,
                Cantidad = item.ic.Cantidad,
                PrecioUnitario = item.p.Precio
            };

            context.ItemsCompra.Add(itemCompra);

            item.p.Stock -= item.ic.Cantidad;
        }

        context.ItemsCarrito.RemoveRange(items.Select(i => i.ic));

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Results.Ok(new CompraResponse { CompraId = compraId, Total = total });
    }
    catch
    {
        await transaction.RollbackAsync();
        return Results.Problem("Error al procesar la compra");
    }
});

app.MapPut("/carritos/{carrito}/{producto}", async (TiendaContext context, string carrito, int producto, int cantidad = 1) =>
{
    var productInfo = await context.Productos.FindAsync(producto);
    if (productInfo == null)
    {
        return Results.NotFound(new { error = "Producto no encontrado" });
    }

    var existingItem = await context.ItemsCarrito
        .FirstOrDefaultAsync(ic => ic.CarritoId == carrito && ic.ProductoId == producto);

    if (existingItem != null)
    {
        var newQuantity = existingItem.Cantidad + cantidad;
        if (newQuantity > productInfo.Stock)
        {
            return Results.BadRequest(new { error = "Stock insuficiente" });
        }

        existingItem.Cantidad = newQuantity;
    }
    else
    {
        if (cantidad > productInfo.Stock)
        {
            return Results.BadRequest(new { error = "Stock insuficiente" });
        }

        var newItem = new ItemCarritoEntity
        {
            Id = Guid.NewGuid().ToString(),
            CarritoId = carrito,
            ProductoId = producto,
            Cantidad = cantidad
        };

        context.ItemsCarrito.Add(newItem);
    }

    await context.SaveChangesAsync();
    return Results.Ok(new { success = true });
});

app.MapDelete("/carritos/{carrito}/{producto}", async (TiendaContext context, string carrito, int producto, int cantidad = 1) =>
{
    var existingItem = await context.ItemsCarrito
        .FirstOrDefaultAsync(ic => ic.CarritoId == carrito && ic.ProductoId == producto);

    if (existingItem == null)
    {
        return Results.NotFound(new { error = "Item no encontrado en el carrito" });
    }

    if (existingItem.Cantidad <= cantidad)
    {
        context.ItemsCarrito.Remove(existingItem);
    }
    else
    {
        existingItem.Cantidad -= cantidad;
    }

    await context.SaveChangesAsync();
    return Results.Ok(new { success = true });
});

app.Run();
