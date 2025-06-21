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
new() { Nombre = "Éclat de Luna - Fragancia Única", Descripcion = "Éclat de Lune es una fragancia exclusiva de nuestra tienda, creada para quienes buscan un toque de lujo en su vida diaria. Con notas frescas de cítricos y un toque suave de jazmín, este perfume es perfecto para cualquier ocasión, desde una salida casual hasta una cena elegante. ¡Haz de cada momento algo especial!", Precio = 12900m, Stock = 15, ImagenUrl = "https://s.alicdn.com/@sc04/kf/Hb665a6fa6e0649e9b065439d4841b03dK.jpg" },
new() { Nombre = "Jardín Secreto - Fragancia Floral y Dulce", Descripcion = "Jardín Secreto es una fragancia floral y dulce que captura la esencia de un jardín en plena primavera. Con notas de rosa, peonía y un toque de miel, este perfume es perfecto para quienes buscan un aroma suave y romántico. Lleva la frescura de la naturaleza contigo todo el día.", Precio = 10900m, Stock = 20, ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQQoBftJvJ-tpEbz0GPGdeUL6DsL3NHMoulX8Uo1-FRNxjjPr1IzI6BTgRCtw-EqYLs0U4&usqp=CAU" },
new() { Nombre = "Misterio Nocturno - Fragancia Amaderada y Cálida", Descripcion = "Misterio Nocturno es una fragancia amaderada y cálida que te envolverá con sus notas de ámbar, pachulí y sándalo. Perfecto para la noche, este perfume tiene un toque sensual y misterioso que dejará una estela inconfundible allá por donde pases. Ideal para eventos especiales y citas nocturnas.", Precio = 13900m, Stock = 18, ImagenUrl = "https://www.zonafarma.com.ar/wp-content/uploads/2023/05/cardon-perfume-de-mujer-viajera-eau-de-parfum-x-100-ml.jpg" },
new() { Nombre = "Esencia de Mar - Fragancia Fresca y Acuática", Descripcion = "Esencia de Mar es una fragancia fresca y acuática, ideal para quienes aman los aromas suaves y revitalizantes. Con notas de agua de mar, limón y pepino, este perfume te transportará a la brisa marina. Perfecto para el día a día o para aquellos días calurosos en los que necesitas una fragancia ligera.", Precio = 11900m, Stock = 25, ImagenUrl = "https://farmaciadardationline.com.ar/wp-content/uploads/2022/07/7798336944114.jpeg" },
new() { Nombre = "Refugio Zen - Difusor de Fragancia", Descripcion = "Refugio Zen es el difusor perfecto para crear un ambiente relajante en tu hogar u oficina. Con esencias naturales de lavanda, eucalipto y sándalo, este difusor transformará tu espacio en un refugio de tranquilidad. Ideal para usar durante el trabajo, meditación o para relajarte después de un día ajetreado.", Precio = 1999.99m, Stock = 30, ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQEje0725URYiGX_mw6bHauqc-swgctseQ0yVgvM1TDXk7E7M2AzFB8DGdDULKWlSuiOJo&usqp=CAU" },
new() { Nombre = "Amanecer Refrescante - Difusor de Fragancia", Descripcion = "Empieza el día con energía con el Difusor 'Amanecer Refrescante'. Con una mezcla revitalizante de menta, cítricos y albahaca, este difusor te ayudará a llenar tu espacio de frescura y vitalidad. Perfecto para la mañana o para crear un ambiente energizante mientras trabajas o estudias.", Precio = 1799.99m, Stock = 20, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_256601-MLA20361799562_072015-O.webp" },
new() { Nombre = "Mi Esencia Única - Perfume Personalizado", Descripcion = "Crea tu propio perfume con 'Mi Esencia Única'. Personaliza tu fragancia seleccionando las notas que más te gustan, desde las florales y frutales hasta las amaderadas y especiadas. Nuestro proceso de creación de perfumes te permitirá tener una fragancia única que refleje tu personalidad. ¡Haz de tu aroma algo tan individual como tú!", Precio = 19900m, Stock = 10, ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSb9-gNJgGxhSdzX31LNQtcxF61HK7FT9OClA&s" },
new() { Nombre = "Verde Sereno - Perfume Ecológico", Descripcion = "Verde Sereno es un perfume ecológico formulado con ingredientes naturales y libres de crueldad animal. Su fragancia fresca y amaderada te transportará a un bosque tranquilo, donde las notas de lavanda, romero y vetiver se combinan para ofrecerte una sensación de paz y equilibrio. Elige una fragancia consciente para ti y el planeta.", Precio = 14900m, Stock = 15, ImagenUrl = "https://www.perfumeriasavenida.com/pub/media/catalog/product/cache/4fbcd446712066eccea2fe003f1c49fb/2/8/28767_2-angel-star-recargable-50ml.jpg"  }
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
