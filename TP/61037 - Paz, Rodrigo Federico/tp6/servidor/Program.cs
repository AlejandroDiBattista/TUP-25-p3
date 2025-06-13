using servidor.Data;
using servidor.Endpoints;
using Microsoft.EntityFrameworkCore;
using servidor.Endpoints.ModelosRequest;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapPut("/carritos/{carritoId}/confirmar", async (Guid carritoId, CompraRequest datos, TiendaContext db) =>
{
    if (string.IsNullOrWhiteSpace(datos.NombreCliente) ||
        string.IsNullOrWhiteSpace(datos.ApellidoCliente) ||
        string.IsNullOrWhiteSpace(datos.EmailCliente))
    {
        return Results.BadRequest("Nombre, Apellido y Email son obligatorios.");
    }

    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null) return Results.NotFound();

    if (!carrito.Items.Any())
        return Results.BadRequest("El carrito está vacío.");

    // Crear la compra
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.NombreCliente,
        ApellidoCliente = datos.ApellidoCliente,
        EmailCliente = datos.EmailCliente,
        Total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio),
        Items = new List<ItemCompra>()
    };

    foreach (var item in carrito.Items)
    {
        if (item.Producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay stock suficiente para el producto {item.Producto.Nombre}.");

        item.Producto.Stock -= item.Cantidad;

        compra.Items.Add(new ItemCompra
        {
            ProductoId = item.ProductoId,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.Producto.Precio
        });
    }

    db.Compras.Add(compra);
    db.ItemsCarrito.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        compra.Id,
        compra.Fecha,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Total = compra.Total,
        Items = compra.Items.Select(i => new
        {
            i.ProductoId,
            i.Cantidad,
            i.PrecioUnitario
        })
    });
});

app.MapCarritoEndpoints();
app.MapCompraEndpoints();
app.MapProductoEndpoints();
app.Run();
