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

// Endpoints

//GET /productos (+ búsqueda por query).
app.MapGet("/productos", async ([FromServices] TiendaContext db, [FromQuery] string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.ToLower().Contains(q.ToLower()));
    return await query.ToListAsync();
});

// POST /carritos (inicializa el carrito).
app.MapPost("/carritos", async (TiendaContext db) =>
{
    var carrito = new Carrito();
    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();
    
    return Results.Ok(new { carritoId = carrito.Id.ToString() });
});

// GET /carritos/{carrito} → Trae los ítems del carrito.
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

// DELETE /carritos/{carrito} → Vacía el carrito.
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

// PUT /carritos/{carrito}/confirmar (detalle + datos cliente).
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

    // Validar datos del cliente
    if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido) || string.IsNullOrWhiteSpace(dto.Email))
{
        return Results.BadRequest("Datos del cliente incompletos.");
    }
    if (!dto.Email.Contains("@"))
    {
        return Results.BadRequest("Email del cliente inválido.");
    }

    // Crear la compra definitiva
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

    // Eliminar los ítems y el carrito
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

// PUT /carritos/{carrito}/{producto} → Agrega un producto al carrito (o actualiza cantidad).
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

// DELETE /carritos/{carrito}/{producto} → Elimina un producto del carrito (o reduce cantidad).
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