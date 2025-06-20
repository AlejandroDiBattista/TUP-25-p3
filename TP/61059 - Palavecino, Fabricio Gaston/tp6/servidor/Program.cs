using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using servidor.Modelos;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5184").AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowClientApp");
app.UseStaticFiles();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
    CargaProductos.Seed(db);
}


app.MapGet("/", () => "Servidor API está en funcionamiento");


app.MapGet("/productos", async ([FromServices] TiendaContext db, [FromQuery] string q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    return await query.ToListAsync();
});


app.MapPost("/carritos", async ([FromServices] TiendaContext db) =>
{
    var carrito = new Carrito { Id = Guid.NewGuid().ToString(), Items = new List<ItemCarrito>() };
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    return Results.Ok(new { id = carrito.Id });
});


app.MapGet("/carritos/{carrito}", async ([FromServices] TiendaContext db, string carrito) =>
{
    var c = await db.Carritos.Include(x => x.Items).ThenInclude(i => i.Producto)
                             .FirstOrDefaultAsync(x => x.Id == carrito);
    if (c == null) return Results.NotFound();
    return Results.Ok(c.Items.Select(i => new {
        i.ProductoId,
        i.Producto.Nombre,
        i.Producto.Precio,
        i.Cantidad,
        i.Producto.ImagenUrl
    }));
});


app.MapDelete("/carritos/{carrito}", async ([FromServices] TiendaContext db, string carrito) =>
{
    var c = await db.Carritos.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == carrito);
    if (c == null) return Results.NotFound();

    // Devuelve el stock de todos los productos del carrito
    foreach (var item in c.Items)
    {
        var p = await db.Productos.FindAsync(item.ProductoId);
        if (p != null)
            p.Stock += item.Cantidad;
    }

    db.ItemCarritos.RemoveRange(c.Items);
    c.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok();
});


app.MapPut("/carritos/{carrito}/confirmar", async ([FromServices] TiendaContext db, string carrito, [FromBody] CompraConfirmacionDto dto) =>
{
    var c = await db.Carritos.Include(x => x.Items).ThenInclude(i => i.Producto)
                             .FirstOrDefaultAsync(x => x.Id == carrito);
    if (c == null) return Results.NotFound();

    // Validar stock antes de confirmar (opcional, ya está reservado)
    foreach (var item in c.Items)
    {
        if (item.Producto.Stock < 0)
            return Results.BadRequest($"Stock insuficiente para {item.Producto.Nombre}");
    }

    // NO descontar stock aquí

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = c.Items.Sum(i => i.Producto.Precio * i.Cantidad),
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        Items = c.Items.Select(i => new Item
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };
    db.Compras.Add(compra);
    db.ItemCarritos.RemoveRange(c.Items);
    c.Items.Clear();
    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Compra confirmada" });
});


app.MapPut("/carritos/{carrito}/{producto}", async ([FromServices] TiendaContext db, string carrito, int producto, [FromBody] int cantidad) =>
{
    var c = await db.Carritos.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == carrito);
    var p = await db.Productos.FindAsync(producto);
    if (c == null || p == null) return Results.NotFound();
    if (cantidad <= 0) return Results.BadRequest("La cantidad debe ser mayor a 0");

    var item = c.Items.FirstOrDefault(i => i.ProductoId == producto);
    int cantidadActual = item?.Cantidad ?? 0;
    int diferencia = cantidad - cantidadActual;

    if (diferencia > 0)
    {
        if (p.Stock < diferencia)
            return Results.BadRequest("Stock insuficiente");
        p.Stock -= diferencia;
    }
    else if (diferencia < 0)
    {
        p.Stock += -diferencia;
    }

    if (item == null)
    {
        item = new ItemCarrito { ProductoId = producto, Cantidad = cantidad, CarritoId = carrito };
        c.Items.Add(item);
        db.ItemCarritos.Add(item);
    }
    else
    {
        item.Cantidad = cantidad;
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/carritos/{carrito}/{producto}", async ([FromServices] TiendaContext db, string carrito, int producto) =>
{
    var c = await db.Carritos.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == carrito);
    if (c == null) return Results.NotFound();
    var item = c.Items.FirstOrDefault(i => i.ProductoId == producto);
    if (item == null) return Results.NotFound();

    // Devuelve el stock al producto
    var p = await db.Productos.FindAsync(producto);
    if (p != null)
        p.Stock += item.Cantidad;

    c.Items.Remove(item);
    db.ItemCarritos.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.Run();
