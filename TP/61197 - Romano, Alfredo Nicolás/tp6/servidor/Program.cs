using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);



// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de serialización camelCase
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Configuración de EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Poblar la base de datos con productos de ejemplo si está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Coca-Cola", Descripcion = "Bebida gaseosa 1.5L", Precio = 1200, Stock = 10, ImagenUrl = "https://www.myccba.africa/media/catalog/product/cache/5479647258cfabec4d973a924b24e3d0/1/7/1759-ZA_10.png" },
            new Producto { Nombre = "Pepsi", Descripcion = "Bebida gaseosa 1.5L", Precio = 1100, Stock = 8, ImagenUrl = "https://boozy.ph/cdn/shop/files/2024-2ndPlatforms-ProductImageTemplate_12_c0e1851b-cdca-4e5a-82b1-c73fc488fda1_grande.png?v=1727744884" },
            new Producto { Nombre = "Fanta", Descripcion = "Bebida naranja 1.5L", Precio = 1150, Stock = 7, ImagenUrl = "https://www.coca-cola.com/content/dam/onexp/py/es/brands/fanta/nuevos_renders/7840058000392.png" },
            new Producto { Nombre = "Sprite", Descripcion = "Bebida lima-limón 1.5L", Precio = 1150, Stock = 5, ImagenUrl = "https://www.myccba.africa/media/catalog/product/cache/5479647258cfabec4d973a924b24e3d0/6/1/6121-ZA_5_2.png" },
            new Producto { Nombre = "Fernet Branca", Descripcion = "Bebida alcoholica 1L", Precio = 10000, Stock = 3, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/211/660/products/branca-7501-f2d07ee453f8e36afd16050362910743-480-0.png" },
            new Producto { Nombre = "Heineken", Descripcion = "Bebida alcoholica 1L", Precio = 4000, Stock = 6, ImagenUrl = "https://png.pngtree.com/png-clipart/20240131/original/pngtree-heineken-bottle-beer-cut-photo-png-image_14191815.png" },
            new Producto { Nombre = "Mirinda", Descripcion = "Bebida naranja 480ml", Precio = 1000, Stock = 12, ImagenUrl = "https://www.shopping-d.com/cdn/shop/products/10MRO490P-AXL-12P-CT490ml.png?v=1733886847&width=1445" },
            new Producto { Nombre = "Agua mineral", Descripcion = "Bebida hidratante 500ml", Precio = 1000, Stock = 4, ImagenUrl = "https://png.pngtree.com/png-vector/20240913/ourmid/pngtree-mineral-water-bottle-png-image_13600124.png" },
            new Producto { Nombre = "Gaseosa Manaos", Descripcion = "Cola 2.25L", Precio = 800, Stock = 15, ImagenUrl = "https://www.gaitasdistribuidora.com.ar/datos/uploads/mod_catalogo/31047/manaos-cola-sin-az-6-x-2-25l-67925ace20cb7.png" },
            new Producto { Nombre = "Cigarrillos Malboro", Descripcion = "Tabaco 12 unidades", Precio = 3500, Stock = 20, ImagenUrl = "https://www.verdistribuciones.com/wp-content/uploads/2020/04/Marlboro-Rojo.png" }
        });
        db.SaveChanges();
    }
}

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Carritos en memoria (por usuario/carritoId)
var carritos = new Dictionary<string, List<(int productoId, int cantidad)>>();

// Endpoint para obtener productos
app.MapGet("/productos", async ([FromServices] TiendaContext db, [FromQuery] string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.ToLower().Contains(q.ToLower()));
    return await query.ToListAsync();
});

// Endpoint para confirmar compra
app.MapPut("/carritos/{carrito}/confirmar", async ([FromRoute] string carrito, [FromBody] CompraConfirmacionDto dto, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");

    var carritoItems = carritos[carrito];
    if (carritoItems.Count == 0)
        return Results.BadRequest("El carrito está vacío");

    var itemsCompra = new List<Item>();
    decimal total = 0;

    foreach (var item in carritoItems)
    {
        var prod = await db.Productos.FindAsync(item.productoId);
        if (prod == null)
            return Results.BadRequest($"Producto con ID {item.productoId} no encontrado");

        var precioTotal = prod.Precio * item.cantidad;
        total += precioTotal;

        itemsCompra.Add(new Item
        {
            ProductoId = prod.Id,
            Cantidad = item.cantidad,
            PrecioUnitario = prod.Precio,
            PrecioTotal = precioTotal
        });
    }

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = dto?.Nombre ?? "",
        ApellidoCliente = dto?.Apellido ?? "",
        EmailCliente = dto?.Email ?? "",
        Items = itemsCompra
    };

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carritoItems.Clear();
    return Results.Ok();
});

// POST /carritos (inicializa un carrito)
app.MapPost("/carritos", () =>
{
    var carritoId = Guid.NewGuid().ToString();
    carritos[carritoId] = new List<(int, int)>();
    return Results.Ok(new { carritoId });
});

// PUT /carritos/{carrito}/{producto}
app.MapPut("/carritos/{carrito}/{producto}", async ([FromRoute] string carrito, [FromRoute] int producto, [FromBody] int cantidad, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var prod = await db.Productos.FindAsync(producto);
    if (prod == null)
        return Results.NotFound("Producto no encontrado");

    var items = carritos[carrito];
    var idx = items.FindIndex(x => x.productoId == producto);
    int cantidadAnterior = idx >= 0 ? items[idx].cantidad : 0;
    int diferencia = cantidad - cantidadAnterior;

    if (diferencia > 0 && diferencia > prod.Stock)
        return Results.BadRequest("Cantidad inválida o sin stock suficiente");

    // Actualiza stock según diferencia
    prod.Stock -= diferencia;
    await db.SaveChangesAsync();

    if (cantidad <= 0)
    {
        if (idx >= 0) items.RemoveAt(idx);
    }
    else
    {
        if (idx >= 0)
            items[idx] = (producto, cantidad);
        else
            items.Add((producto, cantidad));
    }
    return Results.Ok();
});

// GET /carritos/{carrito}
app.MapGet("/carritos/{carrito}", ([FromRoute] string carrito, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var items = carritos[carrito]
        .Select(ci =>
        {
            var prod = db.Productos.Find(ci.productoId);
            return prod == null ? null : new
            {
                Id = prod.Id,
                Nombre = prod.Nombre,
                Precio = prod.Precio,
                ImagenUrl = prod.ImagenUrl,
                // Stock real disponible para este carrito: stock en base + lo reservado en este carrito
                Stock = prod.Stock + ci.cantidad,
                Cantidad = ci.cantidad
            };
        })
        .Where(x => x != null)
        .ToList();
    return Results.Ok(items);
});

// DELETE /carritos/{carrito}
app.MapDelete("/carritos/{carrito}", async ([FromRoute] string carrito, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var items = carritos[carrito];
    foreach (var (productoId, cantidad) in items)
    {
        var prod = await db.Productos.FindAsync(productoId);
        if (prod != null)
            prod.Stock += cantidad;
    }
    await db.SaveChangesAsync();
    items.Clear();
    return Results.Ok();
});

// DELETE /carritos/{carrito}/{producto}
app.MapDelete("/carritos/{carrito}/{producto}", async ([FromRoute] string carrito, [FromRoute] int producto, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var items = carritos[carrito];
    var idx = items.FindIndex(x => x.productoId == producto);
    if (idx >= 0)
    {
        int cantidad = items[idx].cantidad;
        var prod = await db.Productos.FindAsync(producto);
        if (prod != null)
        {
            prod.Stock += cantidad;
            await db.SaveChangesAsync();
        }
        items.RemoveAt(idx);
    }
    return Results.Ok();
});

// Carritos en memoria (por usuario/carritoId)
var carritos = new Dictionary<string, List<(int productoId, int cantidad)>>();

// POST /carritos (inicializa un carrito)
app.MapPost("/carritos", () =>
{
    var carritoId = Guid.NewGuid().ToString();
    carritos[carritoId] = new List<(int, int)>();
    return Results.Ok(new { carritoId });
});

// GET /carritos/{carrito}
app.MapGet("/carritos/{carrito}", ([FromRoute] string carrito, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var items = carritos[carrito]
        .Select(ci =>
        {
            var prod = db.Productos.Find(ci.productoId);
            return prod == null ? null : new
            {
                prod.Id,
                prod.Nombre,
                prod.Precio,
                prod.ImagenUrl,
                prod.Stock,
                Cantidad = ci.cantidad
            };
        })
        .Where(x => x != null)
        .ToList();
    return Results.Ok(items);
});

// DELETE /carritos/{carrito}
app.MapDelete("/carritos/{carrito}", ([FromRoute] string carrito) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    carritos[carrito].Clear();
    return Results.Ok();
});

// PUT /carritos/{carrito}/{producto}
app.MapPut("/carritos/{carrito}/{producto}", ([FromRoute] string carrito, [FromRoute] int producto, [FromBody] int cantidad, [FromServices] TiendaContext db) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var prod = db.Productos.Find(producto);
    if (prod == null)
        return Results.NotFound("Producto no encontrado");
    if (cantidad < 1 || cantidad > prod.Stock)
        return Results.BadRequest("Cantidad inválida o sin stock suficiente");

    var items = carritos[carrito];
    var idx = items.FindIndex(x => x.productoId == producto);
    if (idx >= 0)
        items[idx] = (producto, cantidad);
    else
        items.Add((producto, cantidad));
    return Results.Ok();
});

// DELETE /carritos/{carrito}/{producto}
app.MapDelete("/carritos/{carrito}/{producto}", ([FromRoute] string carrito, [FromRoute] int producto) =>
{
    if (!carritos.ContainsKey(carrito))
        return Results.NotFound("Carrito no encontrado");
    var items = carritos[carrito];
    var idx = items.FindIndex(x => x.productoId == producto);
    if (idx >= 0)
        items.RemoveAt(idx);
    return Results.Ok();
});

app.Run();

// DTOs para la confirmación de compra
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
