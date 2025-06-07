using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);



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

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

app.UseCors("AllowClientApp");
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Diccionario temporal de carritos (en memoria)
var carritos = new Dictionary<string, List<CarritoItem>>();

// ENDPOINTS

// GET /productos
app.MapGet("/productos", async (AppDbContext db) =>
    await db.Productos.ToListAsync());

// POST /carritos
app.MapPost("/carritos", () => {
    var id = Guid.NewGuid().ToString();
    carritos[id] = new List<CarritoItem>();
    return Results.Ok(id);
});

// GET /carritos/{id}
app.MapGet("/carritos/{id}", (string id) =>
    carritos.ContainsKey(id) ? Results.Ok(carritos[id]) : Results.NotFound());

// DELETE /carritos/{id}
app.MapDelete("/carritos/{id}", (string id) => {
    if (carritos.Remove(id))
        return Results.Ok();
    return Results.NotFound();
});

// PUT /carritos/{id}/confirmar
app.MapPut("/carritos/{id}/confirmar", async (string id, ClienteDTO cliente, AppDbContext db) => {
    if (!carritos.TryGetValue(id, out var items)) return Results.NotFound();

    var compra = new Compra {
        NombreCliente = cliente.Nombre,
        ApellidoCliente = cliente.Apellido,
        EmailCliente = cliente.Email,
        Total = items.Sum(i => i.Cantidad * i.PrecioUnitario),
        Items = items.Select(i => new ItemCompra {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };

    foreach (var item in items) {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest("Stock insuficiente");
        producto.Stock -= item.Cantidad;
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    carritos.Remove(id);
    return Results.Ok();
});

// PUT /carritos/{id}/{producto}
app.MapPut("/carritos/{id}/{productoId}", async (string id, int productoId, int cantidad, AppDbContext db) => {
    if (!carritos.TryGetValue(id, out var items)) return Results.NotFound();
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no existe");
    if (cantidad > producto.Stock) return Results.BadRequest("Stock insuficiente");

    var item = items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) {
        items.Add(new CarritoItem {
            ProductoId = productoId,
            Nombre = producto.Nombre,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        });
    } else {
        item.Cantidad = cantidad;
    }
    return Results.Ok();
});

// DELETE /carritos/{id}/{producto}
app.MapDelete("/carritos/{id}/{productoId}", (string id, int productoId) => {
    if (!carritos.TryGetValue(id, out var items)) return Results.NotFound();
    var item = items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item != null) items.Remove(item);
    return Results.Ok();
});

app.Run();

// DTO para confirmación de compra
public record ClienteDTO(string Nombre, string Apellido, string Email);
