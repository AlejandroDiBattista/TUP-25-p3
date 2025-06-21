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
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

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

// Función auxiliar para obtener stock disponible
int ObtenerStockDisponible(int stockTotal, int productoId) {
    var reservado = GestorStockReserva.ObtenerTotalReservado(productoId);
    return stockTotal - reservado;
}

// Endpoints de Productos con stock actualizado
app.MapGet("/productos", async (TiendaDbContext db, string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    
    var productos = await query.ToListAsync();
    
    // Ajustar el stock visible según las reservas
    foreach (var producto in productos)
    {
        producto.Stock = ObtenerStockDisponible(producto.Stock, producto.Id);
    }
    
    return productos;
});

// Inicializa un carrito nuevo
app.MapPost("/carritos", () => {
    var carrito = new CarritoDto();
    carritos[carrito.Id] = carrito;
    return Results.Ok(carrito);
});

// Trae los ítems del carrito (GET)
app.MapGet("/carritos/{carritoId}", (Guid carritoId) => {
    if (carritos.TryGetValue(carritoId, out var carrito))
        return Results.Ok(carrito);
    return Results.NotFound();
});

// Agrega una cantidad al producto en el carrito (incremental, robusto)
app.MapPut("/carritos/{carritoId}/{productoId}", async (Guid carritoId, int productoId, [FromBody] int cantidad, TiendaDbContext db) => {
    if (!carritos.TryGetValue(carritoId, out var carrito))
        return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound();

    // Validar que la cantidad sea 1 (solo permitir agregar de a 1)
    if (cantidad != 1)
        return Results.BadRequest("Solo se puede agregar de a 1 producto por vez.");

    // Validar stock suficiente
    if (producto.Stock < 1)
        return Results.BadRequest($"Stock insuficiente. Stock disponible: {producto.Stock}");

    // Buscar el item en el carrito
    var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (itemExistente == null)
    {
        carrito.Items.Add(new ItemCarritoDto { ProductoId = productoId, Cantidad = 1 });
    }
    else
    {
        itemExistente.Cantidad += 1;
    }

    // Descontar del stock real
    producto.Stock -= 1;
    if (producto.Stock < 0) producto.Stock = 0;
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

// Elimina un producto del carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", (Guid carritoId, int productoId) => {
    if (!carritos.TryGetValue(carritoId, out var carrito))
        return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound();
    
    carrito.Items.Remove(item);
    return Results.Ok(carrito);
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

// Confirma la compra
app.MapPut("/carritos/{carritoId}/confirmar", async (Guid carritoId, [FromBody] CompraDto compraDto, TiendaDbContext db) => {
    if (!carritos.TryGetValue(carritoId, out var carrito) || !carrito.Items.Any())
        return Results.BadRequest("Carrito vacío o no encontrado");

    // Validar que los productos existan (ya no descontar stock)
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null)
            return Results.BadRequest($"Producto no encontrado");
    }

    // Crear la compra a partir del DTO
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = 0,
        NombreCliente = compraDto.NombreCliente,
        ApellidoCliente = compraDto.ApellidoCliente,
        EmailCliente = compraDto.EmailCliente,
        Items = new List<ItemCompra>()
    };

    // Procesar cada ítem
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null) continue;
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

    var responseDto = new CompraDto
    {
        Id = compra.Id,
        Fecha = compra.Fecha,
        Total = compra.Total,
        NombreCliente = compra.NombreCliente,
        ApellidoCliente = compra.ApellidoCliente,
        EmailCliente = compra.EmailCliente,
        Items = compra.Items.Select(i => new ItemCompraDto
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario,
            Producto = new ProductoDto
            {
                Id = i.Producto!.Id,
                Nombre = i.Producto.Nombre,
                Descripcion = i.Producto.Descripcion,
                Precio = i.Producto.Precio,
                Stock = i.Producto.Stock,
                ImagenUrl = i.Producto.ImagenUrl
            }
        }).ToList()
    };

    return Results.Ok(responseDto);
});

app.Run();
