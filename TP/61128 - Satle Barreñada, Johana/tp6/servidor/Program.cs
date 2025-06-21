using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//cambios importantes

builder.Services.AddControllers();
var app = builder.Build();



using (var scope = app.Services.CreateScope()) // Crea un nuevo ámbito de servicio
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TiendaContext>();

        // Verificar si estamos en desarrollo para evitar esto en producción accidentalmente
        if (app.Environment.IsDevelopment())
        {
            // CUIDADO: Estas dos líneas BORRARÁN y RECREARÁN la base de datos
            // EN CADA INICIO del servidor. Esto es lo que necesitas para resetear el stock.
            dbContext.Database.EnsureDeleted(); // Elimina la base de datos si existe
            dbContext.Database.EnsureCreated(); // Crea la base de datos (y ejecutará HasData)
            Console.WriteLine("DEBUG: Base de datos recreada y productos sembrados con stock inicial.");
        }
        else
        {
            // En otros entornos (Staging/Production), simplemente aplica migraciones
            dbContext.Database.Migrate();
            Console.WriteLine("DEBUG: Migraciones aplicadas en entorno de producción/staging.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating or migrating the DB.");
    }
}



app.UseCors("AllowClientApp");
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Diccionario temporal de carritos en memoria
var carritos = new Dictionary<string, List<CarritoItem>>();

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