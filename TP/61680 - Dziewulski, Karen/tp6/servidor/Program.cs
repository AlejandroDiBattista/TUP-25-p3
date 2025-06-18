using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servidor.Datos;
using Servidor.Modelos;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

var carritos = new ConcurrentDictionary<string, List<ItemCarrito>>();

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/productos", async (TiendaDbContext db, string? q) => {
    var query = db.Productos.AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    }
    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/carritos", () => {
    var id = Guid.NewGuid().ToString();
    carritos[id] = new List<ItemCarrito>();
    return Results.Ok(id);
});

app.MapGet("/carritos/{carrito}", (string carrito) => {
    if (carritos.TryGetValue(carrito, out var items))
        return Results.Ok(items);
    return Results.NotFound();
});

app.MapPut("/carritos/{carrito}/{productoId}", async (string carrito, int productoId, TiendaDbContext db) => {
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null || producto.Stock <= 0) return Results.BadRequest();

    var item = carritos.GetOrAdd(carrito, _ => new List<ItemCarrito>())
                       .FirstOrDefault(i => i.ProductoId == productoId);

    if (item != null)
        item.Cantidad++;
    else
        carritos[carrito].Add(new ItemCarrito { ProductoId = productoId, Cantidad = 1, PrecioUnitario = producto.Precio });

    return Results.Ok();
});

app.MapDelete("/carritos/{carrito}/{productoId}", (string carrito, int productoId) => {
    if (!carritos.TryGetValue(carrito, out var items)) return Results.NotFound();

    var item = items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    item.Cantidad--;
    if (item.Cantidad <= 0)
        items.Remove(item);

    return Results.Ok();
});

app.MapDelete("/carritos/{carrito}", (string carrito) => {
    carritos.TryRemove(carrito, out _);
    return Results.Ok();
});

app.MapPut("/carritos/{carrito}/confirmar", async (string carrito, TiendaDbContext db, [FromBody] CompraDTO datos) => {
    
    if (!carritos.TryGetValue(carrito, out var items) || items.Count == 0)
        return Results.BadRequest("El carrito está vacío.");

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.Cliente.Nombre,
        ApellidoCliente = datos.Cliente.Apellido,
        EmailCliente = datos.Cliente.Email,
        Total = items.Sum(i => i.Cantidad * i.PrecioUnitario),
        Items = items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };

    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para el producto {item.ProductoId}");

        producto.Stock -= item.Cantidad;
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carritos.TryRemove(carrito, out _);
    return Results.Ok();
});

app.Run();

public class CompraDTO
{
    public ClienteDto Cliente { get; set; } = new();
    public List<ItemDTO> Items { get; set; } = new();
}

public class ClienteDto
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
}

public class ItemDTO
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
