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

app.Run();
