using Microsoft.EntityFrameworkCore;
using servidor.Modelos;
using servidor.Data;
#nullable enable

var builder = WebApplication.CreateBuilder(args);

// ✅ Configuración de CORS para Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177") // URL del cliente Blazor
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tiendaonline.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Desarrollo: Swagger y error dev
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Orden correcto de middlewares
app.UseCors("AllowClientApp");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// ✅ ENDPOINTS PERSONALIZADOS

// 1. Crear carrito vacío
app.MapPost("/carritos", async (TiendaDbContext db) =>
{
    var nuevoCarrito = new Carrito { Items = new List<CarritoItem>() };
    db.Carritos.Add(nuevoCarrito);
    await db.SaveChangesAsync();
    return Results.Ok(new { Id = nuevoCarrito.Id });
});

// 2. Obtener productos con búsqueda
app.MapGet("/productos", async (TiendaDbContext db, string? query) =>
{
    var productosQuery = db.Productos.AsQueryable();
    if (!string.IsNullOrEmpty(query))
    {
        productosQuery = productosQuery.Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query));
    }
    return await productosQuery.ToListAsync();
});

// 3. Obtener carrito por ID
app.MapGet("/carritos/{carritoId:int}", async (TiendaDbContext db, int carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    return carrito is not null ? Results.Ok(carrito) : Results.NotFound();
});

// 4. Vaciar carrito
app.MapDelete("/carritos/{carritoId:int}", async (TiendaDbContext db, int carritoId) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound();
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok();
});

// 5. Agregar o actualizar producto en carrito
app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (TiendaDbContext db, int carritoId, int productoId, HttpRequest request) =>
{
    var body = await request.ReadFromJsonAsync<Dictionary<string, int>>();
    if (body == null || !body.TryGetValue("cantidad", out var cantidad))
        return Results.BadRequest("Falta el parámetro 'cantidad'.");

    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound("Carrito no encontrado.");

    var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
    if (producto is null || producto.Stock < cantidad)
        return Results.BadRequest("Producto no encontrado o stock insuficiente.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    int cantidadTotal = (item?.Cantidad ?? 0) + cantidad;
    if (item is null)
    {
        carrito.Items.Add(new CarritoItem
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio,
        });
    }
    else
    {
        item.Cantidad += cantidad;
        if (cantidadTotal > producto.Stock)
            return Results.BadRequest("La cantidad supera el stock disponible.");
    }

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

// 6. Eliminar o reducir producto del carrito
app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (TiendaDbContext db, int carritoId, int productoId, int? cantidad) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound("Carrito no encontrado.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null) return Results.NotFound("Producto no encontrado en el carrito.");

    if (cantidad.HasValue && cantidad.Value < item.Cantidad)
        item.Cantidad -= cantidad.Value;
    else
        carrito.Items.Remove(item);

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

// 7. Confirmar compra
app.MapPut("/carritos/{carritoId:int}/confirmar", async (TiendaDbContext db, int carritoId, Compra compra) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(ci => ci.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound("Carrito no encontrado.");
    if (!carrito.Items.Any()) return Results.BadRequest("El carrito está vacío.");

    compra.Fecha = DateTime.Now;
    compra.Items = carrito.Items.Select(item => new ItemCompra
    {
        ProductoId = item.ProductoId,
        Cantidad = item.Cantidad,
        PrecioUnitario = item.PrecioUnitario,
        Productos = item.Producto
    }).ToList();
    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    foreach (var item in carrito.Items)
    {
        var prod = await db.Productos.FirstOrDefaultAsync(p => p.Id == item.ProductoId);
        if (prod is null || prod.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para el producto {item.ProductoId}");
        prod.Stock -= item.Cantidad;
    }

    try
{
    db.Compras.Add(compra);
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Compra realizada con éxito." });
}
catch (Exception ex)
{
    return Results.Problem("Error al guardar la compra: " + ex.Message);
}

});

app.Run();