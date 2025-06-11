using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (TiendaContext db, string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
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
                i.Producto?.ImagenUrl
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (TiendaContext db, int carritoId, int productoId, int cantidad) =>
{
    if (cantidad < 1) return Results.BadRequest("La cantidad debe ser mayor a 0");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    var compra = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado");

    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);

    int stockDisponible = producto.Stock + (item?.Cantidad ?? 0);

    if (cantidad > stockDisponible)
        return Results.BadRequest("No hay suficiente stock");

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
                i.Producto?.ImagenUrl
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (TiendaContext db, int carritoId, int productoId, int? cantidad) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado");

    var item = compra.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("Producto no está en el carrito");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");


    if (cantidad == null || cantidad >= item.Cantidad)
    {
        compra.Items.Remove(item);
    }
    else
    {
        item.Cantidad -= cantidad.Value;
    }

    await db.SaveChangesAsync();

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
                i.Producto?.ImagenUrl
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapDelete("/carritos/{carritoId}", async (TiendaContext db, int carritoId) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (compra == null) return Results.NotFound("Carrito no encontrado");

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
                i.Producto?.ImagenUrl
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});


app.MapPut("/carritos/{carritoId}/confirmar", async (TiendaContext db, int carritoId, ConfirmarCompraDto datos) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound("Carrito no encontrado");
    if (compra.Items.Count == 0) return Results.BadRequest("El carrito está vacío");

    foreach (var item in compra.Items)
    {
        if (item.Producto != null)
        {
            if (item.Producto.Stock < item.Cantidad)
                return Results.BadRequest($"No hay suficiente stock para {item.Producto.Nombre}");
            item.Producto.Stock -= item.Cantidad;
        }
    }

    compra.NombreCliente = datos.Nombre;
    compra.ApellidoCliente = datos.Apellido;
    compra.EmailCliente = datos.Email;
    compra.Fecha = DateTime.Now;
    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();

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
                i.Producto?.ImagenUrl
            },
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.Run();

public record ConfirmarCompraDto(string Nombre, string Apellido, string Email);
