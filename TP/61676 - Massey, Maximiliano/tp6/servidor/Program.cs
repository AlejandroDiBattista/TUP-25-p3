using Microsoft.EntityFrameworkCore;
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

builder.Services.AddDbContext<ContenidoTiendaDb>(options => {
    options.UseSqlite("Data Source=tienda.db");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContenidoTiendaDb>();
    dbContext.Database.EnsureCreated(); // Asegurarse de que la base de datos esté actualizada
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (ContenidoTiendaDb db, string? busqueda) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(busqueda))
        query = query.Where(p => p.Nombre.Contains(busqueda));
    return await query.ToListAsync();
});


var carritos = new Dictionary<Guid, Dictionary<int, int>>();


app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid();
    carritos[id] = new Dictionary<int, int>();
    return Results.Ok(new { id });
});

app.MapGet("/carritos/{id}", async (Guid id, ContenidoTiendaDb db) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();
    var carrito = carritos[id];
    if (carrito.Count == 0)
        return Results.Ok(new List<object>());

    var productosIds = carrito.Keys.ToList();
    var productos = await db.Productos.Where(p => productosIds.Contains(p.Id)).ToListAsync();

    var detalle = productos.Select(p => new {
        id = p.Id,
        nombre = p.Nombre,
        precio = p.Precio,
        imagen = p.Imagen,
        cantidad = carrito[p.Id],
        subtotal = p.Precio * carrito[p.Id]
    }).ToList();

    return Results.Ok(detalle);
});

app.MapPut("/carritos/{id}/{producto}", async (Guid id, int producto, ContenidoTiendaDb db) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    var prod = await db.Productos.FindAsync(producto);
    if (prod == null)
        return Results.NotFound("Producto no encontrado");

    if (prod.Stock < 1)
        return Results.BadRequest("Sin stock");

    if (!carritos[id].ContainsKey(producto))
        carritos[id][producto] = 0;
    carritos[id][producto]++;

    return Results.Ok(carritos[id]);
});

app.MapDelete("/carritos/{id}/{producto}", (Guid id, int producto) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    if (!carritos[id].ContainsKey(producto))
        return Results.NotFound("Producto no está en el carrito");

    carritos[id][producto]--;
    if (carritos[id][producto] <= 0)
        carritos[id].Remove(producto);

    return Results.Ok(carritos[id]);
});

app.MapDelete("/carritos/{id}", (Guid id) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();
    carritos.Remove(id);
    return Results.Ok("Carrito eliminado");
});

app.MapPut("/carritos/{id}/confirmar", async (Guid id, ContenidoTiendaDb db, ConfirmacionDto datos) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    // Validación de datos del cliente
    if (string.IsNullOrWhiteSpace(datos.NombreCliente) ||
        string.IsNullOrWhiteSpace(datos.ApellidoCliente) ||
        string.IsNullOrWhiteSpace(datos.EmailCliente))
    {
        return Results.BadRequest("Nombre, apellido y email del cliente son obligatorios");
    }

    var carrito = carritos[id];
    if (carrito.Count == 0)
        return Results.BadRequest("El carrito está vacío");

    foreach (var item in carrito)
    {
        var prod = await db.Productos.FindAsync(item.Key);
        if (prod == null || prod.Stock < item.Value)
            return Results.BadRequest($"Sin stock suficiente para producto {item.Key}");
    }
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.NombreCliente,
        ApellidoCliente = datos.ApellidoCliente,
        EmailCliente = datos.EmailCliente,
        Items = new List<ItemCompra>(),
        Total = 0
    };
    foreach (var item in carrito)
    {
        var prod = await db.Productos.FindAsync(item.Key);
        prod.Stock -= item.Value;
        var itemCompra = new ItemCompra
        {
            ProductoId = prod.Id,
            Cantidad = item.Value,
            PrecioUnitario = (decimal)prod.Precio
        };
        compra.Items.Add(itemCompra);
        compra.Total += itemCompra.PrecioUnitario * itemCompra.Cantidad;
    }
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    carritos.Remove(id);
    return Results.Ok("Compra confirmada");
});

app.Run();
