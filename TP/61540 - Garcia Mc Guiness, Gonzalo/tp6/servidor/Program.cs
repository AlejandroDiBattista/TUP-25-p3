using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using servidor.Hubs;
using System;
using servidor.Models;
#nullable enable


var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar controladores si es necesario
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

builder.Services.AddSignalR();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}

    // Configurar el pipeline de solicitudes HTTP
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

app.UseStaticFiles();

app.MapHub<servidor.Hubs.StockHub>("/stockhub");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (TiendaContext db, string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        var qNorm = q.ToLower().Replace(" ", "");
        query = query.Where(p =>
            p.Nombre.ToLower().Contains(q.ToLower()) ||
            p.Descripcion.ToLower().Contains(q.ToLower()) ||
            p.Nombre.ToLower().Replace(" ", "").Contains(qNorm) ||
            p.Descripcion.ToLower().Replace(" ", "").Contains(qNorm)
        );
    }
    return await query.ToListAsync();
});

app.MapPost("/carritos", async (TiendaContext db) =>
{
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = 0,
        NombreCliente = "",
        ApellidoCliente = "",
        EmailCliente = "",
        Items = new List<ItemCompra>()
    };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok(new { carritoId = compra.Id });
});

app.MapGet("/carritos/{carritoId}", async (TiendaContext db, int carritoId) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null)
        return Results.NotFound();

    return Results.Ok(new {
        compra.Id,
        compra.Fecha,
        compra.Total,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Items = compra.Items.Select(i => new {
            i.Id,
            i.ProductoId,
            Producto = new {
                i.Producto?.Id,
                i.Producto?.Nombre,
                i.Producto?.Descripcion,
                i.Producto?.Precio,
                i.Producto?.Stock,
                i.Producto?.ImagenUrl,
                StockTotal = (i.Producto != null ? i.Producto.Stock + i.Cantidad : i.Cantidad)
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (TiendaContext db, IHubContext<StockHub> hubContext, int carritoId, int productoId, int cantidad) =>
{
    if (cantidad < 1) return Results.BadRequest("La cantidad debe ser mayor a 0");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    var compra = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado");

    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);

    int cantidadAnterior = item?.Cantidad ?? 0;
    int stockDisponible = producto.Stock + cantidadAnterior;

    if (cantidad > stockDisponible)
        return Results.BadRequest("No hay suficiente stock");

    int diferencia = cantidad - cantidadAnterior;
    producto.Stock -= diferencia;

    if (item == null)
    {
        item = new ItemCompra
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        };
        compra.Items.Add(item);
    }
    else
    {
        item.Cantidad = cantidad;
    }

    await db.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("StockActualizado", productoId, producto.Stock);

    return Results.Ok(new
    {
        compra.Id,
        compra.Fecha,
        compra.Total,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Items = compra.Items.Select(i => new
        {
            i.Id,
            i.ProductoId,
            Producto = new
            {
                i.Producto?.Id,
                i.Producto?.Nombre,
                i.Producto?.Descripcion,
                i.Producto?.Precio,
                i.Producto?.Stock,
                i.Producto?.ImagenUrl,
                StockTotal = (i.Producto != null ? i.Producto.Stock + i.Cantidad : i.Cantidad)
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (TiendaContext db, IHubContext<StockHub> hubContext, int carritoId, int productoId, int? cantidad) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado");

    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("Producto no está en el carrito");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    int cantidadLiberada = 0;

    if (cantidad == null || cantidad >= item.Cantidad)
    {
        cantidadLiberada = item.Cantidad;
        compra.Items.Remove(item);
    }
    else
    {
        cantidadLiberada = cantidad.Value;
        item.Cantidad -= cantidad.Value;
    }

    producto.Stock += cantidadLiberada;

    await db.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("StockActualizado", productoId, producto.Stock);

    return Results.Ok(new
    {
        compra.Id,
        compra.Fecha,
        compra.Total,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Items = compra.Items.Select(i => new
        {
            i.Id,
            i.ProductoId,
            Producto = new
            {
                i.Producto?.Id,
                i.Producto?.Nombre,
                i.Producto?.Descripcion,
                i.Producto?.Precio,
                i.Producto?.Stock,
                i.Producto?.ImagenUrl,
                StockTotal = (i.Producto != null ? i.Producto.Stock + i.Cantidad : i.Cantidad)
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapDelete("/carritos/{carritoId}", async (TiendaContext db, IHubContext<StockHub> hubContext, int carritoId) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado");

    foreach (var item in compra.Items.ToList())
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            producto.Stock += item.Cantidad;
            await hubContext.Clients.All.SendAsync("StockActualizado", producto.Id, producto.Stock);
        }
    }

    compra.Items.Clear();
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        compra.Id,
        compra.Fecha,
        compra.Total,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Items = compra.Items.Select(i => new
        {
            i.Id,
            i.ProductoId,
            Producto = new
            {
                i.Producto?.Id,
                i.Producto?.Nombre,
                i.Producto?.Descripcion,
                i.Producto?.Precio,
                i.Producto?.Stock,
                i.Producto?.ImagenUrl,
                StockTotal = (i.Producto != null ? i.Producto.Stock + i.Cantidad : i.Cantidad)
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapPut("/carritos/{carritoId}/confirmar", async (TiendaContext db, IHubContext<StockHub> hubContext, int carritoId, ConfirmarCompraDto datos) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound("Carrito no encontrado");
    if (compra.Items.Count == 0) return Results.BadRequest("El carrito está vacío");

    compra.NombreCliente = datos.Nombre;
    compra.ApellidoCliente = datos.Apellido;
    compra.EmailCliente = datos.Email;
    compra.Fecha = DateTime.Now;
    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        compra.Id,
        compra.Fecha,
        compra.Total,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Items = compra.Items.Select(i => new
        {
            i.Id,
            i.ProductoId,
            Producto = new
            {
                i.Producto?.Id,
                i.Producto?.Nombre,
                i.Producto?.Descripcion,
                i.Producto?.Precio,
                i.Producto?.Stock,
                i.Producto?.ImagenUrl,
                StockTotal = (i.Producto != null ? i.Producto.Stock + i.Cantidad : i.Cantidad)
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.Run();

public record ConfirmarCompraDto(string Nombre, string Apellido, string Email);
