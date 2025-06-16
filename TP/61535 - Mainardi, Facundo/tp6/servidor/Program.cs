using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Servidor.Modelos;
using Servidor.Stock;
using Servidor.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Tienda>(options =>
    options.UseInMemoryDatabase("Tienda"));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Tienda>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapGet("/productos", async (Tienda db, string? q) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
    {
        string filtro = q.ToLower();
        query = query.Where(p => p.Nombre.ToLower().Contains(filtro) || p.Descripcion.ToLower().Contains(filtro));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

var carritos = new Dictionary<Guid, List<ItemCompra>>();

app.MapPost("/carritos", () =>
{
    var nuevoId = Guid.NewGuid();
    carritos[nuevoId] = new List<ItemCompra>();
    return Results.Ok(nuevoId);
});
app.MapPut("/carritos/{carritoId}/{productoId}", async (
    Guid carritoId,
    int productoId,
    [FromBody] int cantidad,
    Tienda db) =>
{
    if (cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a 0.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null)
        return Results.NotFound("Producto no encontrado.");

    if (producto.Stock < cantidad)
        return Results.BadRequest("No hay suficiente stock.");

    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado.");

    var carrito = carritos[carritoId];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

    if (item == null)
    {
        item = new ItemCompra
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        };
        carrito.Add(item);
    }
    else
    {
        item.Cantidad += cantidad;
    }

    return Results.Ok("Producto agregado al carrito.");
});

app.MapPut("/carritos/{carritoId}/confirmar", async (Guid carritoId, ConfirmacionCompraDto dto, Tienda db) =>

{
    var errores = new List<string>();

    foreach (var item in dto.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            errores.Add($"Stock insuficiente para {item.Nombre}");
    }

    if (errores.Any())
        return Results.BadRequest(new { errores });

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = dto.Items.Sum(i => i.Precio * i.Cantidad),
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        ItemsCompra = new List<ItemCompra>()
    };

    foreach (var item in dto.Items)
    {
        compra.ItemsCompra.Add(new ItemCompra
        {
            ProductoId = item.ProductoId,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.Precio
        });

        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/", () => "Servidor API está en funcionamiento ✅");
app.UseAuthorization();
app.MapControllers();
app.Run();