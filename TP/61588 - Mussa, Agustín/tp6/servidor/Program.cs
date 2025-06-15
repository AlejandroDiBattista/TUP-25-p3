#nullable enable

using Microsoft.EntityFrameworkCore;
using servidor;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = new { Message = "Ocurrió un error inesperado en el servidor." };
        await context.Response.WriteAsJsonAsync(error);
    });
});

app.UseCors("AllowClientApp");

TiendaContext.SeedDatabase(app);

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/productos", async (string? busqueda, TiendaContext db) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        var filtro = busqueda.Trim().ToLower();
        query = query.Where(p =>
            p.Nombre.ToLower().Contains(filtro) ||
            p.Descripcion.ToLower().Contains(filtro)
        );
    }

    return await query.ToListAsync();
});

app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{carrito.Id}", carrito);
});

app.MapGet("/carritos/{carritoId:int}", async (int carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos.FindAsync(carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");
    await db.Entry(carrito).Collection(c => c.Items).LoadAsync();
    return Results.Ok(carrito);
});

app.MapDelete("/carritos/{carritoId:int}", async (int carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos.FindAsync(carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{carritoId:int}/confirmar", async (int carritoId, CompraConfirmacionDto confirmacion, TiendaContext db) =>
{
    if (string.IsNullOrEmpty(confirmacion.Nombre) || string.IsNullOrEmpty(confirmacion.Apellido) || string.IsNullOrEmpty(confirmacion.Email))
    {
        return Results.BadRequest("Los datos del cliente son obligatorios.");
    }

    var carrito = await db.Carritos.FindAsync(carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");
    await db.Entry(carrito).Collection(c => c.Items).LoadAsync();
    if (!carrito.Items.Any())
        return Results.BadRequest("El carrito está vacío.");

    decimal total = 0;
    foreach (var item in carrito.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null)
            continue;
        if (producto.Stock < item.Cantidad)
            return Results.BadRequest($"Producto {producto.Nombre} no tiene stock suficiente.");

        total += producto.Precio * item.Cantidad;
        producto.Stock -= item.Cantidad;
    }

    var compra = new Compra
    {
        Fecha = DateTime.UtcNow,
        Total = total,
        NombreCliente = confirmacion.Nombre,
        ApellidoCliente = confirmacion.Apellido,
        EmailCliente = confirmacion.Email
    };

    db.Compras.Add(compra);
    carrito.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, int cantidad, TiendaContext db) =>
{
    if (cantidad <= 0)
    {
        return Results.BadRequest("La cantidad debe ser mayor a cero.");
    }

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
    {
        return Results.NotFound("Producto no encontrado.");
    }

    if (producto.Stock < cantidad)
    {
        return Results.BadRequest("Stock insuficiente.");
    }

    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null)
    {
        return Results.NotFound("Carrito no encontrado.");
    }

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
    {
        carrito.Items.Add(new ItemCarrito { ProductoId = productoId, Cantidad = cantidad, PrecioUnitario = producto.Precio });
    }
    else
    {
        item.Cantidad += cantidad;
    }

    producto.Stock -= cantidad;

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Carritos.FindAsync(carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");
    await db.Entry(carrito).Collection(c => c.Items).LoadAsync();
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("El producto no se encuentra en el carrito.");

    if (item.Cantidad > 1)
        item.Cantidad--;
    else
        carrito.Items.Remove(item);

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.Run();