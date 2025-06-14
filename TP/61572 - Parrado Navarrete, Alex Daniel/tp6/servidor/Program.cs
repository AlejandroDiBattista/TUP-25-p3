using Microsoft.EntityFrameworkCore;
using TiendaOnline.Servidor.Data;
using TiendaOnline.Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}); // ✅ BLOQUE cerrado correctamente

// ✅ AHORA ESTÁ FUERA del bloque AddCors
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbInitializer.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Endpoints
app.MapGet("/productos", async (string? search, AppDbContext db) =>
    await db.Productos
        .Where(p => string.IsNullOrEmpty(search) ||
                    p.Nombre.Contains(search) ||
                    p.Descripcion.Contains(search))
        .ToListAsync());

app.MapPost("/carritos", async (AppDbContext db) =>
{
    var carrito = new Compra { Fecha = DateTime.UtcNow, Total = 0 };
    db.Compras.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{carrito.Id}", carrito.Id);
});

app.MapGet("/carritos/{carritoId:int}", async (int carritoId, AppDbContext db) =>
{
    var items = await db.ItemsCompra.Where(i => i.CompraId == carritoId).ToListAsync();
    return items.Any() ? Results.Ok(items) : Results.NotFound();
});

app.MapDelete("/carritos/{carritoId:int}", async (int carritoId, AppDbContext db) =>
{
    var items = db.ItemsCompra.Where(i => i.CompraId == carritoId);
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null || producto.Stock <= 0) return Results.BadRequest("Sin stock");

    var item = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);
    if (item == null)
    {
        item = new ItemCompra { CompraId = carritoId, ProductoId = productoId, Cantidad = 1, PrecioUnitario = producto.Precio };
        db.ItemsCompra.Add(item);
    }
    else
    {
        if (producto.Stock < item.Cantidad + 1) return Results.BadRequest("Sin stock");
        item.Cantidad++;
    }

    producto.Stock--;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, AppDbContext db) =>
{
    var item = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    producto.Stock += item.Cantidad;

    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{carritoId:int}/confirmar", async (int carritoId, Compra clienteData, AppDbContext db) =>
{
    var compra = await db.Compras.FindAsync(carritoId);
    if (compra == null) return Results.NotFound();

    var items = await db.ItemsCompra.Where(i => i.CompraId == carritoId).ToListAsync();
    compra.NombreCliente = clienteData.NombreCliente;
    compra.ApellidoCliente = clienteData.ApellidoCliente;
    compra.EmailCliente = clienteData.EmailCliente;
    compra.Total = items.Sum(i => i.Cantidad * i.PrecioUnitario);

    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();