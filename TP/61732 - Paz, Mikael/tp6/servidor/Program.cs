using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor;
using servidor.Models;

// Simulación de almacenamiento de carritos en memoria
var carritos = new Dictionary<Guid, CarritoDto>();

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tiendaonline.db"));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Aplicar migraciones y crear la base de datos automáticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.Migrate();
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoints de Productos
app.MapGet("/productos", async (TiendaDbContext db, string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    return await query.ToListAsync();
});

// Carrito en memoria (simulación por sesión, para simplificar)

// Inicializa un carrito nuevo
app.MapPost("/carritos", () => {
    var carrito = new CarritoDto();
    carritos[carrito.Id] = carrito;
    return Results.Ok(carrito);
});

// Trae los ítems del carrito
app.MapGet("/carritos/{carritoId}", (Guid carritoId) => {
    if (carritos.TryGetValue(carritoId, out var carrito))
        return Results.Ok(carrito);
    return Results.NotFound();
});

// Vacía el carrito
app.MapDelete("/carritos/{carritoId}", (Guid carritoId) => {
    if (carritos.TryGetValue(carritoId, out var carrito))
    {
        carrito.Items.Clear();
        return Results.Ok(carrito);
    }
    return Results.NotFound();
});

// Agrega o actualiza un producto en el carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (Guid carritoId, int productoId, [FromBody] int cantidad, TiendaDbContext db) => {
    if (!carritos.TryGetValue(carritoId, out var carrito))
        return Results.NotFound();
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound();
    if (cantidad < 1 || cantidad > producto.Stock)
        return Results.BadRequest($"Stock insuficiente o cantidad inválida");
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        carrito.Items.Add(new ItemCarritoDto { ProductoId = productoId, Cantidad = cantidad });
    else
        item.Cantidad = cantidad;
    return Results.Ok(carrito);
});

// Elimina o reduce cantidad de un producto en el carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", (Guid carritoId, int productoId) => {
    if (!carritos.TryGetValue(carritoId, out var carrito))
        return Results.NotFound();
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound();
    carrito.Items.Remove(item);
    return Results.Ok(carrito);
});

// Confirma la compra
app.MapPut("/carritos/{carritoId}/confirmar", async (Guid carritoId, [FromBody] Compra compra, TiendaDbContext db) => {
    if (!carritos.TryGetValue(carritoId, out var carrito) || !carrito.Items.Any())
        return Results.BadRequest("Carrito vacío o no encontrado");
    // Validar stock
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {producto?.Nombre ?? "producto desconocido"}");
    }
    // Registrar compra
    compra.Fecha = DateTime.Now;
    compra.Total = 0;
    compra.Items = new List<ItemCompra>();
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null) continue;
        producto.Stock -= item.Cantidad;
        var itemCompra = new ItemCompra
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        };
        compra.Items.Add(itemCompra);
        compra.Total += producto.Precio * item.Cantidad;
    }
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    carrito.Items.Clear();
    return Results.Ok(compra);
});

app.Run();
