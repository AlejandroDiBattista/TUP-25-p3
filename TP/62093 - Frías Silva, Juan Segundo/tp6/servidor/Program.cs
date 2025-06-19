using Microsoft.EntityFrameworkCore;
using servidor.Data;
using System.IO;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurar EF Core con SQLite
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "tienda.db");
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}")
);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(app); 
}

app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// GET /productos + búsqueda opcional
app.MapGet("/productos", async (TiendaDbContext db, string? nombre) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(nombre))
    {
        nombre = nombre.ToLower();
        query = query.Where(p => p.Nombre.ToLower().Contains(nombre));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

// POST /compras (crear nueva compra)
app.MapPost("/compras", async (TiendaDbContext db, Compra compra) =>
{
    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto is null || producto.Stock < item.Cantidad)
        {
            return Results.BadRequest($"No hay suficiente stock de {producto?.Nombre ?? "producto desconocido"}.");
        }

        producto.Stock -= item.Cantidad;
        item.PrecioUnitario = producto.Precio;
    }

    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
    compra.Fecha = DateTime.Now;

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    return Results.Ok(new { compra.Id, compra.Total });
});

// GET /compras (historial con items anidados correctamente)
app.MapGet("/compras", async (TiendaDbContext db) =>
{
    var compras = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .ToListAsync();

    return Results.Ok(compras.Select(c => new
    {
        c.Id,
        c.NombreCliente,
        c.ApellidoCliente,
        c.EmailCliente,
        c.Total,
        c.Fecha,
        Items = c.Items.Select(i => new
        {
            Producto = new { i.Producto!.Nombre },
            i.Cantidad,
            i.PrecioUnitario
        })
    }));
});

app.Run();