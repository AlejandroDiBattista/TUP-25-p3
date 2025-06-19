using Microsoft.EntityFrameworkCore;
using Servidor.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<PaginaContext>(opt =>
    opt.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

var carritos = new Dictionary<string, Dictionary<int, int>>();

app.MapGet("/productos", async (PaginaContext db) =>
    await db.Productos.ToListAsync());

app.MapPost("/carritos/{carritoId}/agregar/{productoId}", async (string carritoId, int productoId, PaginaContext db) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null || producto.CantidadDisponible <= 0)
        return Results.BadRequest("Sin stock");

    if (!carritos.ContainsKey(carritoId))
        carritos[carritoId] = new Dictionary<int, int>();

    if (!carritos[carritoId].ContainsKey(productoId))
        carritos[carritoId][productoId] = 0;

    carritos[carritoId][productoId]++;
    producto.CantidadDisponible--;
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/carritos/{carritoId}", async (string carritoId, PaginaContext db) =>
{
    if (!carritos.TryGetValue(carritoId, out var items) || items.Count == 0)
        return Results.Ok(new List<object>());

    var productos = await db.Productos.Where(p => items.Keys.Contains(p.ProductoId)).ToListAsync();

    var resultado = productos.Select(p => new {
        producto = p,
        cantidad = items[p.ProductoId]
    });

    return Results.Ok(resultado);
});


app.MapDelete("/carritos/{carritoId}/quitar/{productoId}", async (string carritoId, int productoId, PaginaContext db) =>
{
    if (carritos.TryGetValue(carritoId, out var items) && items.ContainsKey(productoId))
    {
        items[productoId]--;
        var producto = await db.Productos.FindAsync(productoId);
        if (producto != null)
        {
            producto.CantidadDisponible++;
            await db.SaveChangesAsync();
        }
        if (items[productoId] <= 0)
            items.Remove(productoId);
    }
    return Results.Ok();
});

app.MapDelete("/carritos/{carritoId}", async (string carritoId, PaginaContext db) =>
{
    if (carritos.TryGetValue(carritoId, out var items))
    {
        foreach (var kv in items)
        {
            var producto = await db.Productos.FindAsync(kv.Key);
            if (producto != null)
            {
                producto.CantidadDisponible += kv.Value;
            }
        }
        await db.SaveChangesAsync();
        carritos.Remove(carritoId);
    }
    return Results.Ok();
});

app.Run();