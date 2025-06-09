using servidor.Models;
using servidor.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors("AllowClientApp");

// Cargar stock persistente al iniciar
TiendaData.CargarStock();

// Ruta raíz
app.MapGet("/", () => "Servidor API en funcionamiento");

// GET /api/productos
app.MapGet("/api/productos", ([FromQuery] string? q) => {
    var productos = TiendaData.Productos
        .Where(p =>
            string.IsNullOrEmpty(q) ||
            p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            p.Descripcion.Contains(q, StringComparison.OrdinalIgnoreCase)
        )
        .ToList();
    return Results.Ok(productos);
});

// POST /api/carritos → crea un nuevo carrito
app.MapPost("/api/carritos", () => {
    var id = Guid.NewGuid();
    TiendaData.Carritos[id] = new List<ItemCarrito>();
    return Results.Ok(id);
});

// GET /api/carritos/{id}
app.MapGet("/api/carritos/{id}", (Guid id) => {
    if (!TiendaData.Carritos.ContainsKey(id))
        return Results.NotFound("Carrito no encontrado");

    return Results.Ok(TiendaData.Carritos[id]);
});

// PUT /api/carritos/{id}/{productoId}
app.MapPut("/api/carritos/{id}/{productoId}", (Guid id, int productoId) => {
    var producto = TiendaData.Productos.FirstOrDefault(p => p.Id == productoId);
    if (producto is null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock < 1)
        return Results.BadRequest("Sin stock");

    if (!TiendaData.Carritos.ContainsKey(id))
        return Results.NotFound("Carrito no encontrado");

    var carrito = TiendaData.Carritos[id];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null) {
        carrito.Add(new ItemCarrito {
            ProductoId = productoId,
            Cantidad = 1,
            PrecioUnitario = producto.Precio
        });
    } else {
        item.Cantidad++;
    }

    producto.Stock--;
    TiendaData.GuardarStock(); // <--- Guarda el stock
    return Results.Ok(carrito);
});

// DELETE /api/carritos/{id}/{productoId}
app.MapDelete("/api/carritos/{id}/{productoId}", (Guid id, int productoId) => {
    if (!TiendaData.Carritos.ContainsKey(id))
        return Results.NotFound("Carrito no encontrado");

    var carrito = TiendaData.Carritos[id];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null)
        return Results.NotFound("Producto no está en el carrito");

    if (item.Cantidad > 1) {
        item.Cantidad--;
    } else {
        carrito.Remove(item);
    }

    var producto = TiendaData.Productos.FirstOrDefault(p => p.Id == productoId);
    if (producto is not null)
        producto.Stock++;

    TiendaData.GuardarStock(); // <--- Guarda el stock
    return Results.Ok(carrito);
});

// DELETE /api/carritos/{id}
app.MapDelete("/api/carritos/{id}", (Guid id) => {
    if (!TiendaData.Carritos.ContainsKey(id))
        return Results.NotFound("Carrito no encontrado");

    var carrito = TiendaData.Carritos[id];
    foreach (var item in carrito)
    {
        var producto = TiendaData.Productos.FirstOrDefault(p => p.Id == item.ProductoId);
        if (producto is not null)
            producto.Stock += item.Cantidad;
    }

    carrito.Clear();
    TiendaData.GuardarStock(); // <--- Guarda el stock
    return Results.Ok();
});

// POST /api/compras → confirmar compra
app.MapPost("/api/compras", ([FromQuery] Guid carritoId, [FromBody] Cliente cliente) => {
    if (!TiendaData.Carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");

    var items = TiendaData.Carritos[carritoId];
    if (!items.Any())
        return Results.BadRequest("El carrito está vacío");

    var compra = new Compra {
        Id = Guid.NewGuid(),
        Cliente = cliente,
        Items = items.Select(i => new ItemCarrito {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList(),
        Fecha = DateTime.Now
    };

    TiendaData.Compras.Add(compra);

    // Vaciar el carrito
    TiendaData.Carritos[carritoId] = new List<ItemCarrito>();

    TiendaData.GuardarStock(); // <--- Guarda el stock por si acaso
    return Results.Ok(compra);
});

// GET /api/compras → ver todas las compras
app.MapGet("/api/compras", () => {
    return Results.Ok(TiendaData.Compras);
});

app.UseStaticFiles();
app.Run();