using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.Common;
using servidor.Modelos;
using Microsoft.Extensions.DependencyInjection;
using servidor.Data;
#nullable enable

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

// configuro el efcore con sqlite
builder.Services.AddDbContext<TiendaDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tiendaonline.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

//endpoins

//1. GET de productos con busqueda
app.MapGet("/productos", async (TiendaDbContext db, string? query) =>
{
    var productosQuery = db.Productos.AsQueryable();
    if (!string.IsNullOrEmpty(query))
    {
        productosQuery = productosQuery.Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query));
    }
    return await productosQuery.ToListAsync();
});

//POST: inicializar el carrito y retorna su ID
app.MapGet("/carritos/{carritoId:int}", async (TiendaDbContext db, int carritoId) =>
{
    var carrito = await db.Carritos
    .Include(c => c.Items)
    .ThenInclude(i => i.Producto)
    .FirstOrDefaultAsync(c => c.Id == carritoId);
    return carrito is not null ? Results.Ok(carrito) : Results.NotFound();
});

//3. DELETE vaciar carrito
app.MapDelete("/carritos/{carritoId:int}", async (TiendaDbContext db, int carritoId) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound();
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok();
});

//4. PUT agrega/actualiza el producto en carrito
app.MapPut("carritos/{carritoId:int}/{productoId:int}", async (TiendaDbContext db, int carritoId, int productoId, int cantidad) =>
{
    //buscar carrito y producto
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound("Carrito no encontrado.");

    var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
    if (producto is null || producto.Stock < cantidad)
        return Results.BadRequest("Producto no encontrado o stock insuficiente.");

    //buscar producto si ya existe en el carrito
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
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
        if (item.Cantidad > producto.Stock)
            return Results.BadRequest("La cantidad supera el stock disponible.");
    }
    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

//5. DELETE elima/ reduce 
app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (TiendaDbContext db, int carritoId, int productoId, int? cantidad) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null) return Results.NotFound("Producto en el carrito no encontrado.");
    
    // se especifica cantidad y es menor al total, se reduce, sino se remueve el item.
    if (cantidad.HasValue && cantidad.Value < item.Cantidad)
        item.Cantidad -= cantidad.Value;
    else
        carrito.Items.Remove(item);

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

//6. PUT confrimmar compra
app.MapPut("/carritos/{carritoId:int}/confirmar", async (TiendaDbContext db, int carritoId, Compra compra) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(ci => ci.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null) return Results.NotFound("Carrito no encontrado.");

    if (!carrito.Items.Any())
        return Results.BadRequest("El carrito está vacío.");

    // Preparar la compra usando los ítems del carrito
    compra.Fecha = DateTime.Now;
    compra.Items = carrito.Items.Select(item => new ItemCompra
    {
        ProductoId = item.ProductoId,
        Cantidad = item.Cantidad,
        PrecioUnitario = item.PrecioUnitario,
        Productos = item.Producto // Asegúrate de que 'Producto' está incluido en el Include del carrito
    }).ToList();
    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    // Descontar stock de cada producto
    foreach (var item in carrito.Items)
    {
        var prod = await db.Productos.FirstOrDefaultAsync(p => p.Id == item.ProductoId);
        if (prod is null || prod.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para el producto {item.ProductoId}");
        prod.Stock -= item.Cantidad;
    }

    db.Compras.Add(compra);
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

app.Run();