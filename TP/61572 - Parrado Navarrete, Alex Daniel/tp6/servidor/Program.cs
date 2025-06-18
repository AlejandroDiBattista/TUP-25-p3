using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiendaOnline.Servidor.Data;
using TiendaOnline.Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Servicios
var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataFolder);
var conn = $"Data Source={Path.Combine(dataFolder, "tienda.db")}";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(conn));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Seed
using var scope = app.Services.CreateScope();
var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
ctx.Database.EnsureCreated();
DbInitializer.Initialize(ctx);

// Middleware
app.UseCors();
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TiendaOnline API V1");
        c.RoutePrefix = string.Empty;
    });
}

// Endpoints de productos (igual que antes)
app.MapGet("/productos", async (string? q, AppDbContext db) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p =>
            p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            p.Descripcion.Contains(q, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(await query.ToListAsync());
});

app.MapPut("/productos/{id:int}", async (int id, Producto dto, AppDbContext db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound();
    prod.Nombre      = dto.Nombre;
    prod.Descripcion = dto.Descripcion;
    prod.Precio      = dto.Precio;
    prod.Stock       = dto.Stock;
    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

// Endpoints de carritos, modificado GET para no usar .ThenInclude
app.MapPost("/carritos", async (AppDbContext db) =>
{
    var c = new Compra { Fecha = DateTime.UtcNow, Total = 0 };
    db.Compras.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{c.Id}", c.Id);
});

app.MapGet("/carritos/{cid:int}", async (int cid, AppDbContext db) =>
{
    // Sólo cargamos los items; sin navegación a Producto
    var items = await db.ItemsCompra
                        .Where(i => i.CompraId == cid)
                        .ToListAsync();
    return items.Any() ? Results.Ok(items) : Results.NotFound();
});

app.MapDelete("/carritos/{cid:int}", async (int cid, AppDbContext db) =>
{
    var items = await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync();
    foreach (var item in items)
    {
        var p = await db.Productos.FindAsync(item.ProductoId);
        if (p != null) p.Stock += item.Cantidad;
    }
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{cid:int}/{pid:int}", async (int cid, int pid, AppDbContext db) =>
{
    var p = await db.Productos.FindAsync(pid);
    if (p == null || p.Stock < 1) return Results.BadRequest("Sin stock");
    var itm = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == cid && i.ProductoId == pid);
    if (itm is null)
    {
        itm = new ItemCompra { CompraId = cid, ProductoId = pid, Cantidad = 1, PrecioUnitario = p.Precio };
        db.ItemsCompra.Add(itm);
    }
    else
    {
        if (p.Stock < itm.Cantidad + 1) return Results.BadRequest("Sin stock");
        itm.Cantidad++;
    }
    p.Stock--;
    await db.SaveChangesAsync();
    return Results.Ok(itm);
});

app.MapDelete("/carritos/{cid:int}/{pid:int}", async (int cid, int pid, AppDbContext db) =>
{
    var itm = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == cid && i.ProductoId == pid);
    if (itm is null) return Results.NotFound();
    var p = await db.Productos.FindAsync(pid);
    if (p != null) p.Stock += itm.Cantidad;
    db.ItemsCompra.Remove(itm);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{cid:int}/confirmar", async (int cid, Compra cli, AppDbContext db) =>
{
    var c = await db.Compras.FindAsync(cid);
    if (c is null) return Results.NotFound();
    var items = await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync();
    c.NombreCliente   = cli.NombreCliente;
    c.ApellidoCliente = cli.ApellidoCliente;
    c.EmailCliente    = cli.EmailCliente;
    c.Total           = items.Sum(i => i.Cantidad * i.PrecioUnitario);
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();