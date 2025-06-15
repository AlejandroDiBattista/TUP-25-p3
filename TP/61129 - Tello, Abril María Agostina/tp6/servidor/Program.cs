using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Configura EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configura CORS para permitir el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5177",
            "https://localhost:7221",
            "http://localhost:5180"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

Dictionary<string, List<CarritoItemDTO>> carritos = new();

// Crea la base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}

// Usa CORS
app.UseCors("AllowClientApp");

// Endpoints Minimal API

// GET /api/productos
app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

// POST /api/compras
app.MapPost("/api/compras", async (TiendaContext db, List<CarritoItemDTO> items) =>
{
    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest("Stock insuficiente o producto no encontrado.");

        producto.Stock -= item.Cantidad;
    }
    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Compra confirmada y stock actualizado" });
});

// --- ENDPOINTS DE CARRITO ---

// POST /api/carritos (inicializa un carrito)
app.MapPost("/api/carritos", () =>
{
    var id = Guid.NewGuid().ToString();
    carritos[id] = new List<CarritoItemDTO>();
    return Results.Ok(new { carrito = id });
});

// GET /api/carritos/{carrito}
app.MapGet("/api/carritos/{carrito}", (string carrito) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    return Results.Ok(carritos[carrito]);
});

// DELETE /api/carritos/{carrito}
app.MapDelete("/api/carritos/{carrito}", (string carrito) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    carritos[carrito].Clear();
    return Results.Ok();
});

// PUT /api/carritos/{carrito}/confirmar
app.MapPut("/api/carritos/{carrito}/confirmar", async (TiendaContext db, string carrito, CompraConfirmacionDTO confirmacion) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");

    foreach (var item in carritos[carrito])
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest("Stock insuficiente o producto no encontrado.");
        producto.Stock -= item.Cantidad;
    }
    await db.SaveChangesAsync();
    carritos[carrito].Clear();
    return Results.Ok(new { mensaje = "Compra confirmada y stock actualizado" });
});

// PUT /api/carritos/{carrito}/{producto}
app.MapPut("/api/carritos/{carrito}/{producto}", (string carrito, int producto, CarritoItemDTO item) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var lista = carritos[carrito];
    var existente = lista.FirstOrDefault(i => i.ProductoId == producto);
    if (existente == null)
        lista.Add(item);
    else
        existente.Cantidad = item.Cantidad;
    return Results.Ok();
});

// DELETE /api/carritos/{carrito}/{producto}
app.MapDelete("/api/carritos/{carrito}/{producto}", (string carrito, int producto) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var lista = carritos[carrito];
    var existente = lista.FirstOrDefault(i => i.ProductoId == producto);
    if (existente != null)
        lista.Remove(existente);
    return Results.Ok();
});

app.Run();
