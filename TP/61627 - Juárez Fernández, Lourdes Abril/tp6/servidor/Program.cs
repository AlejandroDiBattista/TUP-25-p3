using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using servidor.Models;
using servidor.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseStaticFiles();

// Inicialización de la base de datos con productos
async Task InitializeDatabase()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.EnsureCreatedAsync();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Cuaderno A4", Descripcion = "96 hojas rayadas", Precio = 6000M, Stock = 50, ImagenUrl = "http://localhost:5184/imagenes/cuaderno.webp" },
            new Producto { Nombre = "Lápiz Negro", Descripcion = "Grafito HB2", Precio = 1000M, Stock = 100, ImagenUrl = "http://localhost:5184/imagenes/lapiz.jpg" },
            new Producto { Nombre = "Goma de borrar", Descripcion = "Blanca suave", Precio = 500M, Stock = 80 , ImagenUrl = "http://localhost:5184/imagenes/goma.webp" },
            new Producto { Nombre = "Cartuchera", Descripcion = "Estuche de tela con cierre", Precio = 10000M , Stock = 30, ImagenUrl = "http://localhost:5184/imagenes/cartu.jpg" },
            new Producto { Nombre = "Mochila Escolar", Descripcion = "Con varios compartimentos", Precio = 50000M , Stock = 18, ImagenUrl = "http://localhost:5184/imagenes/moch.webp" },
            new Producto { Nombre = "Resaltadores x6", Descripcion = "Colores surtidos", Precio = 8000M, Stock = 40 , ImagenUrl = "http://localhost:5184/imagenes/resalt.jpg" },
            new Producto { Nombre = "Regla de 30cm", Descripcion = "Plástica transparente", Precio = 1000M, Stock = 60 , ImagenUrl = "http://localhost:5184/imagenes/regla.jpg" },
            new Producto { Nombre = "Compás Escolar", Descripcion = "Con mina de repuesto", Precio = 2000M, Stock = 25, ImagenUrl = "http://localhost:5184/imagenes/compas.jpg" },
            new Producto { Nombre = "Tijera Escolar", Descripcion = "Punta redonda", Precio = 10000M, Stock = 35, ImagenUrl = "http://localhost:5184/imagenes/tijera.jpg" },
            new Producto { Nombre = "Pegamento en barra", Descripcion = "No tóxico, 20g", Precio = 2000M, Stock = 45, ImagenUrl = "http://localhost:5184/imagenes/pegamento.jpg" },
            new Producto { Nombre = "Lapicera azul", Descripcion = "Tinta seca, punta fina", Precio = 1000M, Stock = 90, ImagenUrl = "http://localhost:5184/imagenes/lapicera.jpg" },
            new Producto { Nombre = "Set de lápices de colores", Descripcion = "12 colores largos", Precio = 10000M, Stock = 40, ImagenUrl = "http://localhost:5184/imagenes/lapices.jpg" }
        );

        await db.SaveChangesAsync();
    }
}

await InitializeDatabase();

// Endpoints
app.MapGet("/productos", async (AppDbContext db, string? search) =>
    await db.Productos
        .Where(p => search == null || p.Nombre.ToLower().Contains(search.ToLower()))
        .ToListAsync());

app.MapPost("/carritos", async (AppDbContext db) =>
{
    var carrito = new Compra { Total = 0 };
    db.Compras.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{carrito.Id}", carrito);
});

app.MapGet("/carritos", async (AppDbContext db) =>
    await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .ToListAsync());

app.MapGet("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
    await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId));

app.MapPut("/carritos/{carritoId:int}/{productoId:int}/{cantidad:int}", async (AppDbContext db, int carritoId, int productoId, int cantidad) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    int cantidadActualEnCarrito = item?.Cantidad ?? 0;
    int cantidadTotalNueva = cantidadActualEnCarrito + cantidad;

    // Stock disponible = stock actual + lo que ya estaba en el carrito
    int stockDisponible = producto.Stock + cantidadActualEnCarrito;

    if (cantidadTotalNueva > stockDisponible)
        return Results.BadRequest("Stock insuficiente");

    if (item == null)
    {
        carrito.Items.Add(new ItemCompra
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        });
    }
    else
    {
        item.Cantidad += cantidad;
        if (item.Cantidad <= 0)
            carrito.Items.Remove(item);
    }

    // DESCONTAR STOCK EN TIEMPO REAL
    producto.Stock = stockDisponible - cantidadTotalNueva;

    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.MapDelete("/carritos/{carritoId}", async (AppDbContext db, int carritoId) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            producto.Stock += item.Cantidad;
        }
    }

    db.Compras.Remove(carrito);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPut("/carritos/{carritoId}/confirmar", async (AppDbContext db, int carritoId, Compra compra) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    carrito.NombreCliente = compra.NombreCliente;
    carrito.ApellidoCliente = compra.ApellidoCliente;
    carrito.EmailCliente = compra.EmailCliente;

    // NO descontar stock aquí

    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/", () => "¡Servidor funcionando correctamente! Accede a:\n- /productos para ver los productos\n- /carritos para gestionar compras");

app.Run();
