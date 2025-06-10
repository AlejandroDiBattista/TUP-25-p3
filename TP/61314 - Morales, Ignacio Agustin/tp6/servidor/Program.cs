using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TiendaDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseCors("AllowClientApp")

app.MapGet("/productos", async (TiendaDb db) ) =>
{
     return await db.Producotos
      .Where(p => p.Stock > 0)
      .ToListAsync();
}

app.MapPost("/Carrito", async (TiendaDb db, Producto producto) =>
{
    if (producto.Stock <= 0)
    {
        return Results.BadRequest("Producto sin stock");
    }

    var carrito = new Carrito
    {
        ProductoId = producto.Id,
        Cantidad = 1
    };

    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

app.MapGet("/Carrito/{CarritoID}", async (TiendaDb db, int CarritoID) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Producto)
        .FirstOrDefaultAsync(c => c.Id == CarritoID);

    if (carrito == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(carrito);
});

app.MapDelete("/Carrito/{CarritoID}", async (TiendaDb db, int CarritoID) =>
{
    var carrito = await db.Carritos.FindAsync(CarritoID);
    if (carrito == null)
    {
        return Results.NotFound();
    }

    foreach (var item in db.Carritos)
    {
        if (item.ProductoId == carrito.ProductoId)
        {
            item.Cantidad--;
            if (item.Cantidad <= 0)
            {
                db.Carritos.Remove(item);
            }
        }
    }

    db.Carritos.Remove(carrito);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/Carrito/{CarritoID}/{ProductoID}", async (TiendaDb db, int CarritoID, int ProductoID) =>
{
    var carrito = await db.Carritos.FindAsync(CarritoID);
    if (carrito == null)
    {
        return Results.NotFound();
    }

    var producto = await db.Producotos.FindAsync(ProductoID);
    if (producto == null || producto.Stock <= 0)
    {
        return Results.BadRequest("Producto no disponible");
    }

    carrito.ProductoId = ProductoID;
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

app.MapDelete("/Carrito/{CarritoID}/{ProductoID}", async (TiendaDb db, int CarritoID, int ProductoID) =>
{
    var carrito = await db.Carritos.FindAsync(CarritoID);
    if (carrito == null)
    {
        return Results.NotFound();
    }

    var producto = await db.Producotos.FindAsync(ProductoID);
    if (producto == null)
    {
        return Results.BadRequest("Producto no encontrado");
    }

    if (carrito.ProductoId != ProductoID)
    {
        return Results.BadRequest("El producto no está en el carrito");
    }

    db.Carritos.Remove(carrito);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("Carrito/{CarritoID}/Confirmar", async (TiendaDb db, int CarritoID) =>
{
    var carrito = await db.Carritos.FindAsync(CarritoID);
    if (carrito == null)
    {
        return Results.NotFound();
    }

    var total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };
    db.Compras.Add(compra);
    db.Carritos.Remove(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(compra);
});