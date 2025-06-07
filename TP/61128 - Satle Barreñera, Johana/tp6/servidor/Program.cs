using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseCors("AllowClientApp");
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Diccionario temporal de carritos en memoria
var carritos = new Dictionary<string, List<CarritoItem>>();

// Cargar productos iniciales si no existen
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[] {
            new Producto { Nombre = "Celular Samsung A10", Descripcion = "Celular económico", Precio = 100000, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Notebook Lenovo", Descripcion = "Portátil para estudiantes", Precio = 250000, Stock = 5, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Auriculares Bluetooth", Descripcion = "Auriculares inalámbricos", Precio = 15000, Stock = 20, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Teclado Mecánico", Descripcion = "RGB con switches rojos", Precio = 30000, Stock = 7, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Mouse Gamer", Descripcion = "Sensor óptico 16000 DPI", Precio = 18000, Stock = 15, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Monitor 24 pulgadas", Descripcion = "Full HD 1080p", Precio = 120000, Stock = 3, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Silla Gamer", Descripcion = "Ergonómica con soporte lumbar", Precio = 80000, Stock = 4, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Disco SSD 500GB", Descripcion = "Alta velocidad de lectura", Precio = 40000, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Router WiFi 6", Descripcion = "Cobertura mejorada", Precio = 35000, Stock = 6, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Webcam HD", Descripcion = "Ideal para videollamadas", Precio = 10000, Stock = 12, ImagenUrl = "https://via.placeholder.com/150" }
        });
        db.SaveChanges();
    }
}

// GET /productos
app.MapGet("/productos", async (TiendaContext db, string? q) =>
{
    var productos = db.Productos
        .Where(p => string.IsNullOrEmpty(q) || p.Nombre.Contains(q))
        .ToList();

    return Results.Ok(productos);
});

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
app.MapPut("/carritos/{id}/confirmar", async (string id, ClienteDTO cliente, TiendaContext db) => {
    if (!carritos.TryGetValue(id, out var items)) return Results.NotFound();

    var compra = new Compra {
        NombreCliente = cliente.Nombre,
        ApellidoCliente = cliente.Apellido,
        EmailCliente = cliente.Email,
        Total = items.Sum(i => i.Cantidad * i.PrecioUnitario),
        Fecha = DateTime.Now,
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
app.MapPut("/carritos/{id}/{productoId}", async (string id, int productoId, int cantidad, TiendaContext db) => {
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

// DTO
public record ClienteDTO(string Nombre, string Apellido, string Email);