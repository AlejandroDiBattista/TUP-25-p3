using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
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


app.MapGet("/", () => "Servidor API está en funcionamiento");


app.MapGet("/productos", async ([FromServices] TiendaContext db, [FromQuery] string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.ToLower().Contains(q.ToLower()));
    return await query.ToListAsync();
});


app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    
    return Results.Ok(new { carritoId = carrito.Id.ToString() });
});


app.MapGet("/carritos/{carritoId:int}", async (int carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var items = carrito.Items.Select(i => new {
        Id = i.ProductoId,
        Nombre = i.Producto.Nombre,
        Precio = i.Producto.Precio,
        ImagenUrl = i.Producto.ImagenUrl,
        Stock = i.Producto.Stock + i.Cantidad,
        Cantidad = i.Cantidad
    }).ToList();

    return Results.Ok(items);
});


app.MapDelete("/carritos/{carritoId:int}", async (int carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    foreach (var item in carrito.Items)
    {
        var prod = await db.Productos.FindAsync(item.ProductoId);
        if (prod != null)
            prod.Stock += item.Cantidad;
    }
    db.itemsCarrito.RemoveRange(carrito.Items);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPut("/carritos/{carritoId:int}/confirmar", async (int carritoId, CompraConfirmacionDto dto, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");

    if (!carrito.Items.Any())
        return Results.BadRequest("El carrito está vacío.");


    if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido) || string.IsNullOrWhiteSpace(dto.Email))
{
        return Results.BadRequest("Datos del cliente incompletos.");
    }
    if (!dto.Email.Contains("@"))
    {
        return Results.BadRequest("Email del cliente inválido.");
    }


    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio),
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        Items = carrito.Items.Select(i => new Item
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio,
            PrecioTotal = i.Cantidad * i.Producto.Precio
        }).ToList()
    };

    db.Compras.Add(compra);


    db.itemsCarrito.RemoveRange(carrito.Items);
    db.Carritos.Remove(carrito);

    await db.SaveChangesAsync();

    return Results.Ok(new {
        message = "Compra confirmada.",
        compra = new {
            compra.Id,
            compra.Fecha,
            compra.Total,
            compra.NombreCliente,
            compra.ApellidoCliente,
            compra.EmailCliente,
            Items = compra.Items.Select(i => new {
                i.Id,
                i.ProductoId,
                i.Cantidad,
                i.PrecioUnitario,
                i.PrecioTotal
            }).ToList()
        }
    });
});


app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, [FromQuery] string? accion, TiendaContext db) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    var producto = await db.Productos.FindAsync(productoId);

    if (accion == "restar")
    {
        if (item == null)
            return Results.NotFound("Producto no encontrado en el carrito.");

        if (item.Cantidad > 1)
        {
            item.Cantidad -= 1;
            if (producto != null)
                producto.Stock += 1;
            await db.SaveChangesAsync();
        }
    }
    else
    {
        if (producto == null || producto.Stock == 0)
            return Results.BadRequest("Producto no disponible o stock insuficiente.");

        if (item != null)
        {
            item.Cantidad += 1;
        }
        else
        {
            item = new itemsCarrito
            {
                Producto = producto,
                ProductoId = producto.Id,
                Cantidad = 1,
                Carrito = carrito,
            };
            carrito.Items.Add(item);
        }
        producto.Stock -= 1;
        await db.SaveChangesAsync();
    }

    return Results.Ok();
});


app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null)
        return Results.NotFound("Carrito no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
        producto.Stock += item.Cantidad;

    db.itemsCarrito.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok();
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "REMERA", Descripcion = "Remera negra simple", Precio = 5600, Stock = 12, ImagenUrl = "https://acdn.mitiendanube.com/stores/002/215/740/products/mockup-basica-negra-b989fa8f1daf238e2f17123227639037-1024-1024.jpg" },
            new Producto { Nombre = "BUZO", Descripcion = "Buzo azul", Precio = 7600, Stock = 4, ImagenUrl = "https://lafabricaderemeras.com.ar/wp-content/uploads/2023/02/5-BUZO-CANGURO.jpg" },
            new Producto { Nombre = "CAMPERA", Descripcion = "Campera verde polar", Precio = 8750, Stock = 9, ImagenUrl = "https://th.bing.com/th/id/OIP.ev4BHAuDBqQLTcvm8ewFiQHaHa?r=0&rs=1&pid=ImgDetMain" },
            new Producto { Nombre = "GORRA", Descripcion = "Gorra roja simple", Precio = 2250, Stock = 12, ImagenUrl = "https://th.bing.com/th/id/OIP.6ojJEC8EiiIYHWhbqK_0agHaHa?r=0&rs=1&pid=ImgDetMain" },
            new Producto { Nombre = "GORRO", Descripcion = "Gorro negro estampado", Precio = 2200, Stock = 45, ImagenUrl = "https://calaveras.cl/wp-content/uploads/2022/03/gor00302_1.jpg" },
            new Producto { Nombre = "ZAPATILLAS", Descripcion = "Zapatillas adidas blancas", Precio = 6700, Stock = 8, ImagenUrl = "https://th.bing.com/th/id/OIP.r6OA5dLFWKPQTSjV7C3zwwHaHa?r=0&rs=1&pid=ImgDetMain" },
            new Producto { Nombre = "BUFANDA", Descripcion = "bufanda amarilla a rallas", Precio = 5600, Stock = 12, ImagenUrl = "https://img.freepik.com/fotos-premium/bufanda-lana-rayas-rojas-amarillas-colgantes-aislada-sobre-fondo-blanco-foto-alta-calidad_153912-16157.jpg" },
            new Producto { Nombre = "CASCO", Descripcion = "Caso de moto negro", Precio =60000, Stock = 9, ImagenUrl = "https://th.bing.com/th/id/OIP.rPj89v4gaBmvIdDygDXVjQHaHa?r=0&rs=1&pid=ImgDetMain" },
            new Producto { Nombre = "CINTO", Descripcion = "Cinto de cuero marron", Precio = 8100, Stock = 15, ImagenUrl = "https://images-na.ssl-images-amazon.com/images/I/612LxBZeWGL._AC_UY879_.jpg" },
            new Producto { Nombre = "PANTALON", Descripcion = "Cargo negro", Precio = 3000, Stock = 20, ImagenUrl = "https://th.bing.com/th/id/OIP.cRiRNMYpbcpRucOEqfZjGwAAAA?r=0&rs=1&pid=ImgDetMain" }
        });
        db.SaveChanges();
    }
}

app.Run();


public class CompraConfirmacionDto
{
    public string Cliente { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public List<ItemCompraDto> Items { get; set; }
}

public class ItemCompraDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class Carrito
{
    public int Id { get; set; }
    public List<itemsCarrito> Items { get; set; } = new();
}

public class itemsCarrito
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
    public int CarritoId { get; set; }
    public Carrito Carrito { get; set; }
}