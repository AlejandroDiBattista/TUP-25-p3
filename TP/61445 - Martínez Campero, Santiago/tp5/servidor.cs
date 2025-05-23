#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.0-rc.2.24472.11"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.0-rc.2.24472.11"
#r "nuget: SQLitePCLRaw.bundle_e_sqlite3, 2.1.10" 
#r "nuget: Microsoft.Extensions.Hosting, 9.0.0-rc.2.24472.3"
#r "nuget: Microsoft.Extensions.DependencyInjection, 9.0.0-rc.2.24472.3"

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

app.MapGet("/productos/reponer", async (AppDb db) =>
    await db.Productos.Where(p => p.Stock < 3).ToListAsync());

app.MapPost("/productos/{id}/agregar-stock", async (int id, StockDto stockDto, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    producto.Stock += stockDto.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapPost("/productos/{id}/quitar-stock", async (int id, StockDto stockDto, AppDb db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();
    if (producto.Stock < stockDto.Cantidad) return Results.BadRequest("Stock insuficiente.");
    producto.Stock -= stockDto.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            db.Productos.Add(new Producto { Nombre = $"Producto {i}", Precio = 10 * i, Stock = 10 });
        }
        db.SaveChanges();
    }
}
