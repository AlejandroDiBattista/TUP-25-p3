using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services
    .AddDbContext<TiendaDb>(opt => opt.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// 🔁 Reiniciar stock al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();

    var productos = db.Productos.ToList();

    foreach (var producto in productos)
    {
        producto.Stock = 15;
    }

    db.SaveChanges();
}

// Habilitar CORS
app.UseCors();

var carritos = new Dictionary<string, List<CarritoDto>>();

app.MapGet("api/carritos", () =>
{
    string carritoId = Guid.NewGuid().ToString();
    carritos[carritoId] = new List<CarritoDto>();
    return Results.Ok(carritoId);
});

app.MapGet("api/carritos/{carritoId}", (string carritoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("No encontramos un carrito con los datos enviados.");

    return Results.Ok(carritos[carritoId]);
});

app.MapDelete("api/carritos/{carritoId}", (string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("No se encontró el carrito a eliminar.");

    carritos.Remove(carritoId);
    return Results.Ok("El carrito fue eliminado con éxito.");
});

app.MapGet("api/carritos/{carritoId}/{productoId}", async (TiendaDb db, string carritoId, int productoId) =>
{
    if (!ValidarCarritoId(carritoId))
        carritos[carritoId] = new List<CarritoDto>();

    var producto = await db.Productos.FirstOrDefaultAsync(x => x.Id == productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado.");

    if (producto.Stock <= 0)
        return Results.BadRequest("Stock agotado.");

    var itemExistente = carritos[carritoId].FirstOrDefault(x => x.ProductoId == productoId);

    if (itemExistente != null)
    {
        itemExistente.Cantidad++;
    }
    else
    {
        carritos[carritoId].Add(new CarritoDto
        {
            ProductoId = productoId,
            Cantidad = 1,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio
        });
    }

    // Descontar stock en DB
    producto.Stock--;
    await db.SaveChangesAsync();

    return Results.Ok("Producto agregado al carrito.");
});

app.MapDelete("api/carritos/{carritoId}/eliminar/{productoId}", async (TiendaDb db, string carritoId, int productoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("Carrito no encontrado");

    var item = carritos[carritoId].FirstOrDefault(x => x.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");

    // Devolver stock completo del item al producto
    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += item.Cantidad;
        await db.SaveChangesAsync();
    }

    carritos[carritoId].Remove(item);
    return Results.Ok("Producto eliminado del carrito");
});

app.MapDelete("api/carritos/{carritoId}/vaciar", async (TiendaDb db, string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("Carrito no encontrado");

    foreach (var item in carritos[carritoId])
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            producto.Stock += item.Cantidad;
        }
    }

    await db.SaveChangesAsync();
    carritos[carritoId].Clear();
    return Results.Ok("Carrito vaciado con éxito");
});

app.MapGet("/productos/{carritoId}", (string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("No se encontró el carrito.");

    IList<CarritoDto> carritoDtos = carritos[carritoId];
    return Results.Ok(carritoDtos);
});

// Endpoint para ver todos los productos
// GET /productos (+ búsqueda por query).
app.MapGet("api/productos", async (TiendaDb db, string parametroBusqueda = "") =>
{
    var test = carritos;
    if (!string.IsNullOrEmpty(parametroBusqueda))
    {
        return Results.Ok(await db.Productos.Where(x => x.Nombre.ToLower().Contains(parametroBusqueda.ToLower()) ||
        x.Descripcion.ToLower().Contains(parametroBusqueda.ToLower()))
        .ToListAsync());
    }
    else
    {
        return Results.Ok(await db.Productos.ToListAsync());
    }
});

// POST Compras
app.MapPost("api/nuevaCompra", async (TiendaDb db, NuevaCompraDto nuevaCompraDto) =>
{
    if (!ValidarCarritoId(nuevaCompraDto.CarritoId))
        return Results.NotFound("Seleccione productos para su carrito.");

    IList<CarritoDto> items = carritos[nuevaCompraDto.CarritoId];
    if (items != null && items.Count > 0)
    {
        db.Compras.Add(new Compras
        {
            Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
            NombreCliente = nuevaCompraDto.Nombre,
            ApellidoCliente = nuevaCompraDto.Apellido,
            EmailCliente = nuevaCompraDto.Email,
            Total = items.Sum(x => x.Precio)
        });

        int idCompra = await db.SaveChangesAsync();

        foreach (CarritoDto carritoDto in items)
        {
            var producto = await db.Productos.FindAsync(carritoDto.ProductoId);
            if (producto == null)
                continue;

            db.ItemsCompra.Add(new ItemsCompra
            {
                ProductoId = carritoDto.ProductoId,
                CompraId = idCompra,
                Cantidad = carritoDto.Cantidad,
                PrecioUnitario = carritoDto.Precio
            });
        }

        await db.SaveChangesAsync();
        carritos[nuevaCompraDto.CarritoId].Clear();
        return Results.Ok("La compra fue grabada con éxito.");
    }

    return Results.NotFound("Su carrito no tiene productos.");

});

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapPost("api/nuevoProducto", async (TiendaDb db, [FromBody] Productos producto) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado con éxito.");
});

bool ValidarCarritoId(string carritoId)
{
    return carritos.ContainsKey(carritoId);
}

app.MapPut("api/carritos/{carritoId}/disminuir/{productoId}", async (TiendaDb db, string carritoId, int productoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("Carrito no encontrado");

    var item = carritos[carritoId].FirstOrDefault(x => x.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");

    item.Cantidad--;

    // Devolver stock al producto
    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock++;
        await db.SaveChangesAsync();
    }

    if (item.Cantidad <= 0)
        carritos[carritoId].Remove(item);

    return Results.Ok("Producto disminuido correctamente");
});

app.MapGet("api/carritos/{carritoId}/detalle", (string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.BadRequest("El carrito no existe.");

    var detalle = carritos[carritoId];
    return Results.Ok(detalle);
});

app.MapGet("/", () => "Hello World!");

app.Run();

public class TiendaDb : DbContext
{
    public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }
    public DbSet<Productos> Productos { get; set; }
    public DbSet<Compras> Compras { get; set; }
    public DbSet<ItemsCompra> ItemsCompra { get; set; }
}