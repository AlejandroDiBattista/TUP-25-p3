#nullable enable
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using servidor.Dto;


var builder = WebApplication.CreateBuilder(args);

// habilito CORS para que el cliente Blazor pueda acceder
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// configuro EF con SQLite
builder.Services.AddDbContext<TiendaDb>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();
app.UseCors();

// ───── endpoints de productos ─────

app.MapGet("/productos", async (string? buscar, TiendaDb db) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(buscar))
        query = query.Where(p => p.Nombre.Contains(buscar));

    return await query.ToListAsync();
});

// ───── carritos en memoria ─────

var carritos = new Dictionary<Guid, List<ItemCompra>>();

app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid();
    carritos[id] = new List<ItemCompra>();
    return Results.Ok(id);
});

app.MapGet("/carritos/{id}", (Guid id) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    return Results.Ok(carritos[id]);
});

app.MapDelete("/carritos/{id}", (Guid id) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    carritos[id].Clear();
    return Results.NoContent();
});

app.MapPut("/carritos/{id}/{productoId}", async (Guid id, int productoId, TiendaDb db) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound("carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null)
        return Results.NotFound("producto no encontrado");

    if (producto.Stock <= 0)
        return Results.BadRequest("sin stock");

    var carrito = carritos[id];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

    if (item is null)
    {
        carrito.Add(new ItemCompra
        {
            ProductoId = productoId,
            Producto = producto,
            Cantidad = 1,
            PrecioUnitario = producto.Precio
        });
    }
    else
    {
        if (item.Cantidad >= producto.Stock)
            return Results.BadRequest("no hay mas stock");

        item.Cantidad++;
    }

    return Results.Ok(carrito);
});

app.MapDelete("/carritos/{id}/{productoId}", (Guid id, int productoId) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    var carrito = carritos[id];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

    if (item is null)
        return Results.NotFound();

    item.Cantidad--;

    if (item.Cantidad <= 0)
        carrito.Remove(item);

    return Results.Ok(carrito);
});

app.MapPut("/carritos/{id}/confirmar", async (Guid id, ClienteDto cliente, TiendaDb db) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    var carrito = carritos[id];
    if (!carrito.Any())
        return Results.BadRequest("carrito vacio");

    foreach (var item in carrito)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"stock insuficiente para {producto?.Nombre}");
    }

    var total = carrito.Sum(i => i.Cantidad * i.PrecioUnitario);

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = cliente.Nombre,
        ApellidoCliente = cliente.Apellido,
        EmailCliente = cliente.Email,
        Items = new List<ItemCompra>()
    };

    foreach (var item in carrito)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            producto.Stock -= item.Cantidad;

            compra.Items.Add(new ItemCompra
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario
            });
        }
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carrito.Clear();

    return Results.Ok(compra);
});

// ───── inicializar bd con productos por defecto ─────

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Camisa Casual", Descripcion = "Algodon, manga larga", Precio = 2500, Stock = 20, ImagenUrl = "/img/camisa_casual.jpg" },
            new Producto { Nombre = "Pantalon Jeans", Descripcion = "Denim azul oscuro", Precio = 3500, Stock = 15, ImagenUrl = "/img/jeans.jpg" },
            new Producto { Nombre = "Vestido Veraniego", Descripcion = "Fresco y ligero", Precio = 4200, Stock = 10, ImagenUrl = "/img/vestido_verano.jpg" },
            new Producto { Nombre = "Chaqueta de Cuero", Descripcion = "Estilo biker sintetico", Precio = 7800, Stock = 5, ImagenUrl = "/img/chaqueta_cuero.jpg" },
            new Producto { Nombre = "Sudadera con Capucha", Descripcion = "Felpa suave unisex", Precio = 3200, Stock = 12, ImagenUrl = "/img/sudadera.jpg" },
            new Producto { Nombre = "Falda Plisada", Descripcion = "Midi elegante", Precio = 2900, Stock = 8, ImagenUrl = "/img/falda_plisada.jpg" },
            new Producto { Nombre = "Camisa de Vestir", Descripcion = "Formal blanca", Precio = 2700, Stock = 25, ImagenUrl = "/img/camisa_vestir.jpg" },
            new Producto { Nombre = "Chaleco Deportivo", Descripcion = "Acolchado para el frio", Precio = 5000, Stock = 7, ImagenUrl = "/img/chaleco.jpg" },
            new Producto { Nombre = "Leggings Deportivos", Descripcion = "Compresion y confort", Precio = 2300, Stock = 18, ImagenUrl = "/img/leggings.jpg" },
            new Producto { Nombre = "Abrigo Largo", Descripcion = "Lana oversize", Precio = 9500, Stock = 4, ImagenUrl = "/img/abrigo.jpg" }
        );
        db.SaveChanges();
    }
}

app.Run();
