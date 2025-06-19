using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Servidor.Data;
using Servidor.Models;
using Servidor.DTOs;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=tienda.db"));

// 2) CORS
builder.Services.AddCors(o => {
    o.AddPolicy("AllowClientApp", p => {
        p.WithOrigins("http://localhost:5177", "https://localhost:7221")
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});

// 3) Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mi API de Tienda",
        Version = "v1",
        Description = "API REST para la Tienda Online"
    });
});

var app = builder.Build();

// 4) Dev tools
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API de Tienda v1"));
}

// 🔹 Habilitar estáticos (wwwroot) para tus imágenes
app.UseStaticFiles();

app.UseCors("AllowClientApp");
// <<-- quitamos app.UseAuthorization() porque no estamos usando auth

// ENDPOINT DE PRUEBA
app.MapGet("/api/datos", () =>
    new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now }
);

// 5) Crear DB y semilla
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// 6.1 Salud
app.MapGet("/", () => "Servidor API está en funcionamiento");

// 6.2 Catálogo con búsqueda
app.MapGet("/productos", async (AppDbContext db, string? q) =>
{
    var consulta = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        var filtro = q.Trim().ToLower();
        consulta = consulta.Where(p =>
            p.Nombre.ToLower().Contains(filtro)
        );
    }
    return await consulta.ToListAsync();
});

// 6.3 Inicializar carrito
app.MapPost("/carritos", async (AppDbContext db) =>
{
    var carrito = new Compra {
        Fecha = DateTime.UtcNow,
        Total = 0,
        NombreCliente = "",
        ApellidoCliente = "",
        EmailCliente = "",
        Items = new List<ItemCompra>()
    };
    db.Compras.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{carrito.Id}", new { carritoId = carrito.Id });
});

// 6.4 Obtener carrito
app.MapGet("/carritos/{carritoId:int}", async (int carritoId, AppDbContext db) =>
{
    var carrito = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    var items = carrito.Items.Select(i => new {
        i.ProductoId,
        Nombre = i.Producto.Nombre,
        i.Cantidad,
        PrecioUnitario = i.PrecioUnitario,
        Subtotal = i.Cantidad * i.PrecioUnitario
    });

    return Results.Ok(new {
        carrito.Id,
        carrito.Total,
        Items = items
    });
});

// 6.5 Vaciar carrito
app.MapDelete("/carritos/{carritoId:int}", async (int carritoId, AppDbContext db) =>
{
    var items = db.ItemsCompra.Where(i => i.CompraId == carritoId);
    foreach (var itm in items)
    {
        var prod = await db.Productos.FindAsync(itm.ProductoId);
        if (prod != null) prod.Stock += itm.Cantidad;
    }
    db.ItemsCompra.RemoveRange(items);

    var carrito = await db.Compras.FindAsync(carritoId);
    if (carrito != null) carrito.Total = 0;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 6.6 Agregar/actualizar producto en carrito
app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (
    int carritoId,
    int productoId,
    QuantityUpdate data,
    AppDbContext db) =>
{
    if (data.Cantidad <= 0)
        return Results.BadRequest("Cantidad debe ser mayor a 0.");

    var prod = await db.Productos.FindAsync(productoId);
    if (prod == null) return Results.NotFound("Producto no encontrado.");
    if (prod.Stock < data.Cantidad)
        return Results.BadRequest("Stock insuficiente.");

    prod.Stock -= data.Cantidad;

    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);

    if (item == null)
    {
        item = new ItemCompra {
            CompraId = carritoId,
            ProductoId = productoId,
            Cantidad = data.Cantidad,
            PrecioUnitario = prod.Precio
        };
        db.ItemsCompra.Add(item);
    }
    else
    {
        item.Cantidad += data.Cantidad;
        item.PrecioUnitario = prod.Precio;
    }

    var carrito = await db.Compras
        .Include(c => c.Items)
        .FirstAsync(c => c.Id == carritoId);

    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.Ok();
});

// 6.7 Eliminar producto del carrito
app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (
    int carritoId,
    int productoId,
    AppDbContext db) =>
{
    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    var prod = await db.Productos.FindAsync(productoId);
    if (prod != null) prod.Stock += item.Cantidad;

    db.ItemsCompra.Remove(item);

    var carrito = await db.Compras
        .Include(c => c.Items)
        .FirstAsync(c => c.Id == carritoId);

    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 6.8 Confirmar compra
app.MapPut("/carritos/{carritoId:int}/confirmar", async (
    int carritoId,
    ClienteData cliente,
    AppDbContext db) =>
{
    var carrito = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado.");
    if (!carrito.Items.Any()) return Results.BadRequest("Carrito vacío.");

    carrito.NombreCliente = cliente.Nombre;
    carrito.ApellidoCliente = cliente.Apellido;
    carrito.EmailCliente = cliente.Email;
    carrito.Fecha = DateTime.UtcNow;
    carrito.Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.Ok(new {
        carrito.Id,
        carrito.Fecha,
        carrito.Total,
        cliente.Nombre,
        cliente.Apellido,
        cliente.Email
    });
});

app.Run();
