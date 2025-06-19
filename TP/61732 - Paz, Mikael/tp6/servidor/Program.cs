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

// Diccionario para llevar el control del stock reservado por producto
var stockReservado = new Dictionary<int, int>();

// Función auxiliar para obtener stock disponible
int ObtenerStockDisponible(int stockTotal, int productoId) {
    stockReservado.TryGetValue(productoId, out var reservado);
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

// Función auxiliar para actualizar reservas
void ActualizarStockReservado(int productoId, int cantidadAnterior, int cantidadNueva)
{
    // Eliminar la cantidad anterior de la reserva
    if (cantidadAnterior > 0)
    {
        if (stockReservado.ContainsKey(productoId))
        {
            stockReservado[productoId] -= cantidadAnterior;
            if (stockReservado[productoId] <= 0)
                stockReservado.Remove(productoId);
        }
    }

    // Agregar la nueva cantidad a la reserva
    if (cantidadNueva > 0)
    {
        if (!stockReservado.ContainsKey(productoId))
            stockReservado[productoId] = 0;
        stockReservado[productoId] += cantidadNueva;
    }
}

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

    // Obtener la cantidad anterior en el carrito
    var cantidadAnterior = 0;
    var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (itemExistente != null)
        cantidadAnterior = itemExistente.Cantidad;

    // Verificar stock disponible
    var stockDisponible = ObtenerStockDisponible(producto.Stock, productoId);
    if (cantidad < 1 || cantidad > stockDisponible + cantidadAnterior)
        return Results.BadRequest($"Stock insuficiente o cantidad inválida. Stock disponible: {stockDisponible}");

    // Actualizar el carrito
    if (itemExistente == null)
        carrito.Items.Add(new ItemCarritoDto { ProductoId = productoId, Cantidad = cantidad });
    else
        itemExistente.Cantidad = cantidad;

    // Actualizar las reservas de stock
    ActualizarStockReservado(productoId, cantidadAnterior, cantidad);

    return Results.Ok(carrito);
});

// Elimina o reduce cantidad de un producto en el carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", (Guid carritoId, int productoId) => {
    if (!carritos.TryGetValue(carritoId, out var carrito))
        return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound();

    // Liberar el stock reservado
    ActualizarStockReservado(productoId, item.Cantidad, 0);
    
    carrito.Items.Remove(item);
    return Results.Ok(carrito);
});

// Confirma la compra
app.MapPut("/carritos/{carritoId}/confirmar", async (Guid carritoId, [FromBody] CompraDto compraDto, TiendaDbContext db) => {
    if (!carritos.TryGetValue(carritoId, out var carrito) || !carrito.Items.Any())
        return Results.BadRequest("Carrito vacío o no encontrado");

    // Validar stock una última vez
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        var stockDisponible = ObtenerStockDisponible(producto?.Stock ?? 0, item.ProductoId);
        if (producto == null || stockDisponible < 0)
            return Results.BadRequest($"Stock insuficiente para {producto?.Nombre ?? "producto desconocido"}");
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
        
        // Actualizar stock y liberar reserva
        producto.Stock -= item.Cantidad;
        ActualizarStockReservado(item.ProductoId, item.Cantidad, 0);
        
        // Crear ítem de compra
        var itemCompra = new ItemCompra
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        };
        
        compra.Items.Add(itemCompra);
        compra.Total += producto.Precio * item.Cantidad;
    }

    // Guardar en la base de datos
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    
    // Vaciar el carrito
    carrito.Items.Clear();

    // Crear DTO de respuesta
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
