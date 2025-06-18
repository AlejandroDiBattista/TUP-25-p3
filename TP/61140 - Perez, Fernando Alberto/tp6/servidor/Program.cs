using Microsoft.EntityFrameworkCore;
using TiendaOnline.Api.Data;
using TiendaOnline.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints de la API
app.MapGet("/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/search", async (string? query, TiendaContext db) =>
    string.IsNullOrWhiteSpace(query)
        ? await db.Productos.ToListAsync()
        : await db.Productos.Where(p => p.Nombre.Contains(query)).ToListAsync());

app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Compra();
    db.Compras.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{carrito.Id}", carrito);
});

app.MapGet("/carritos/{carritoId}", async (int carritoId, TiendaContext db) =>
    await db.Compras.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId));

app.MapDelete("/carritos/{carritoId}", async (int carritoId, TiendaContext db) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    db.Compras.Remove(carrito);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{carritoId}/confirmar", async (int carritoId, Compra datosCliente, TiendaContext db) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    carrito.NombreCliente = datosCliente.NombreCliente;
    carrito.ApellidoCliente = datosCliente.ApellidoCliente;
    carrito.EmailCliente = datosCliente.EmailCliente;
    carrito.Fecha = DateTime.Now;
    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, int cantidad, TiendaContext db) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
    if (producto == null || producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        item = new ItemCompra { ProductoId = productoId, Cantidad = cantidad, PrecioUnitario = producto.Precio };
        carrito.Items.Add(item);
    }
    else
    {
        item.Cantidad += cantidad;
    }

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
    if (producto != null)
    {
        producto.Stock += item.Cantidad;
    }

    carrito.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();