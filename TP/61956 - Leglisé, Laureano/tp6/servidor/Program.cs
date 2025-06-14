using servidor.Data;
using servidor.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔒 CORS para permitir acceso desde Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 🧠 EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// ⚙️ Dev mode
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 🔄 Aplicar CORS
app.UseCors("AllowClientApp");

// 🧱 Crear la base de datos y cargar productos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated(); // ¡MUY importante!
}

// 🧪 Rutas mínimas de prueba
app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// 📦 Aquí más adelante agregamos los endpoints REST (productos, carritos, etc.)
// GET /productos (con búsqueda opcional)
app.MapGet("/productos", async (string? buscar, TiendaContext db) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(buscar))
        query = query.Where(p => p.Nombre.Contains(buscar) || p.Descripcion.Contains(buscar));

    return await query.ToListAsync();
});

//POST /carritos → Inicializa (no necesita lógica por ahora)
app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid().ToString();
    return Results.Ok(new { carritoId = id }); 

});

// GET /carritos/{carrito} → Lista de ítems
var carritos = new Dictionary<string, List<ItemCompra>>(); // memoria

app.MapGet("/carritos/{carritoId}", (string carritoId) =>
{
    if (carritos.TryGetValue(carritoId, out var items))
        return Results.Ok(items);

    return Results.NotFound("Carrito no encontrado.");
});

// DELETE /carritos/{carritoId}
app.MapDelete("/carritos/{carritoId}", (string carritoId) =>
{
    carritos.Remove(carritoId);
    return Results.Ok("Carrito vaciado.");
});

// PUT /carritos/{carritoId}/{productoId} → Agrega producto o actualiza cantidad
app.MapPut("/carritos/{carritoId}/{productoId}", async (string carritoId, int productoId, int cantidad, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado.");
    if (producto.Stock < cantidad) return Results.BadRequest("No hay stock suficiente.");

    if (!carritos.ContainsKey(carritoId))
        carritos[carritoId] = new();

    var carrito = carritos[carritoId];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

    if (item == null)
    {
        carrito.Add(new ItemCompra
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        });
    }
    else
    {
        item.Cantidad += cantidad;
    }

    producto.Stock -= cantidad;
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

// DELETE /carritos/{carritoId}/{productoId} → Quita producto o reduce cantidad
app.MapDelete("/carritos/{carritoId}/{productoId}", async (string carritoId, int productoId, int cantidad, TiendaContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no existe.");

    var carrito = carritos[carritoId];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("Producto no está en el carrito.");

    if (cantidad >= item.Cantidad)
        carrito.Remove(item);
    else
        item.Cantidad -= cantidad;

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += cantidad;
        await db.SaveChangesAsync();
    }

    return Results.Ok(carrito);
});

//  PUT /carritos/{carritoId}/confirmar → Confirma compra
app.MapPut("/carritos/{carritoId}/confirmar", async (string carritoId, Compra datosCliente, TiendaContext db) =>
{
    if (!carritos.TryGetValue(carritoId, out var items) || items.Count == 0)
        return Results.BadRequest("El carrito está vacío o no existe.");

    var total = items.Sum(i => i.Cantidad * i.PrecioUnitario);

    var compra = new Compra
    {
        NombreCliente = datosCliente.NombreCliente,
        ApellidoCliente = datosCliente.ApellidoCliente,
        EmailCliente = datosCliente.EmailCliente,
        Total = total,
        Items = items
    };

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carritos.Remove(carritoId);

    return Results.Ok(new { Mensaje = "Compra confirmada", CompraId = compra.Id });
});

app.Run();
