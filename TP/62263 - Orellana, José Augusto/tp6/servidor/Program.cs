using Microsoft.EntityFrameworkCore;

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
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=./tienda.db"));

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Para habilitar archivos estáticos
app.UseStaticFiles();

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Se crea la base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    // Si no hay productos, se agregan 10 productos de ejemplo
    if (!db.Productos.Any())
    {
        var productosIniciales = new List<Producto>
        {
            new Producto { Nombre = "Celular Galaxy A54", Descripcion = "Celular Samsung 128GB", Precio = 190000, Stock = 10, ImagenUrl = "/images/galaxy.webp" },
            new Producto { Nombre = "Celular iPhone 13", Descripcion = "Apple iPhone 13 256GB", Precio = 420000, Stock = 5, ImagenUrl = "/images/iPhone.webp" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido 20W", Precio = 7000, Stock = 30, ImagenUrl = "/images/cargador.webp" },
            new Producto { Nombre = "Auriculares Bluetooth", Descripcion = "Auriculares inalámbricos", Precio = 15000, Stock = 25, ImagenUrl = "/images/auriculares.webp" },
            new Producto { Nombre = "Smartwatch", Descripcion = "Reloj inteligente con GPS", Precio = 25000, Stock = 12, ImagenUrl = "/images/smartwatch.webp" },
            new Producto { Nombre = "Notebook Lenovo", Descripcion = "Laptop 15.6'' 8GB RAM", Precio = 320000, Stock = 7, ImagenUrl = "/images/notebook.webp" },
            new Producto { Nombre = "Gaseosa Coca-Cola 2L", Descripcion = "Botella 2 litros", Precio = 1000, Stock = 50, ImagenUrl = "/images/coca.webp" },
            new Producto { Nombre = "Gaseosa Pepsi 2L", Descripcion = "Pepsi 2 litros", Precio = 950, Stock = 45, ImagenUrl = "/images/pepsi.webp" },
            new Producto { Nombre = "Chocolate Milka", Descripcion = "Chocolate con leche 100g", Precio = 800, Stock = 100, ImagenUrl = "/images/milka.webp" },
            new Producto { Nombre = "Papas Lays", Descripcion = "Bolsa de papas fritas 200g", Precio = 1200, Stock = 60, ImagenUrl = "/images/lays.webp" },
        };

        db.Productos.AddRange(productosIniciales);
        db.SaveChanges();
    }
}

// Endpoint busqueda con query
app.MapGet("/productos", async (TiendaContext db, string? buscar) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(buscar))
    {
        var textoBusqueda = buscar.Trim().ToLower();

        if (!string.IsNullOrEmpty(textoBusqueda))
        {
            query = query.Where(p =>
                p.Nombre.ToLower().Contains(textoBusqueda));
        }
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

// Endpoint para crear un nuevo carrito de compras
app.MapPost("/carritos", async (TiendaContext db) =>
{
    var nuevaCompra = new Compra
    {
        Fecha = DateTime.Now,
        Total = 0,
        NombreCliente = "",
        ApellidoCliente = "",
        EmailCliente = "",
        Items = new List<ItemCompra>()
    };

    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();

    return Results.Created($"/carritos/{nuevaCompra.Id}", new { nuevaCompra.Id });
});

// Endpoint para obtener los ítems de un carrito específico
app.MapGet("/carritos/{carritoId:int}", async (int carritoId, TiendaContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(item => item.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound("Carrito no encontrado.");

    var respuesta = compra.Items.Select(item => new
    {
        ProductoId = item.ProductoId,
        NombreProducto = item.Producto.Nombre,
        PrecioUnitario = item.PrecioUnitario,
        Cantidad = item.Cantidad,
        ImagenUrl = item.Producto.ImagenUrl,
        Subtotal = item.Cantidad * item.PrecioUnitario
    });

    return Results.Ok(respuesta);
});

// Endpoint para vaciar un carrito de compras, por ID
app.MapDelete("/carritos/{carritoId:int}", async (int carritoId, TiendaContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound("Carrito no encontrado");

    // Se devuelve la cantidad al stock de cada producto
    foreach (var item in compra.Items)
    {
        item.Producto.Stock += item.Cantidad;
    }

    // Se eliminan los ítems del carrito
    db.ItemsCompra.RemoveRange(compra.Items);
    await db.SaveChangesAsync();

    return Results.Ok("Carrito vaciado exitosamente");
});

// Endpoint para confirmar un carrito de compras
app.MapPut("/carritos/{carritoId}/confirmar", async (int carritoId, ClienteDTO cliente, TiendaContext db) =>
{
    // Validar campos del cliente
    if (string.IsNullOrWhiteSpace(cliente.Nombre) ||
        string.IsNullOrWhiteSpace(cliente.Apellido) ||
        string.IsNullOrWhiteSpace(cliente.Email))
    {
        return Results.BadRequest("Todos los campos del cliente son obligatorios");    
    }

    // Validar formato de email
    if (!cliente.Email.Contains("@")) return Results.BadRequest("El email debe ser válido, es decir, contener @");

    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(ic => ic.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null || !compra.Items.Any()) return Results.BadRequest("Carrito no encontrado o vacío");

    // Validación de carrito ya confirmado
    if (!string.IsNullOrWhiteSpace(compra.NombreCliente) &&
        !string.IsNullOrWhiteSpace(compra.ApellidoCliente) &&
        !string.IsNullOrWhiteSpace(compra.EmailCliente) &&
        compra.Total > 0)
    {
        return Results.BadRequest("Este carrito ya fue confirmado anteriormente");
    }

    // Se calcula el total sin modificar el stock
    decimal total = 0;
    foreach (var item in compra.Items)
    {
        total += item.Cantidad * item.PrecioUnitario;
    }

    // Actualizar datos del cliente
    compra.NombreCliente = cliente.Nombre;
    compra.ApellidoCliente = cliente.Apellido;
    compra.EmailCliente = cliente.Email;
    compra.Fecha = DateTime.Now;
    compra.Total = total;

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        compra.Id,
        compra.Total,
        Fecha = compra.Fecha.ToString("dd/MM/yyyy HH:mm"),
        Cliente = new {compra.NombreCliente, compra.ApellidoCliente, compra.EmailCliente}
    });
});

// Endpoint para agregar un producto al carrito (o actualizar la cantidad).
app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, ItemCantidadDTO data, TiendaContext db) =>
{
    if (data.Cantidad <= 0) return Results.BadRequest("La cantidad debe ser mayor a cero");

    var carrito = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (data.Cantidad > producto.Stock) return Results.BadRequest("Stock insuficiente");

    // Se descuenta del stock
    producto.Stock -= data.Cantidad;

    if (item != null)
    {
        item.Cantidad += data.Cantidad;
        item.PrecioUnitario = producto.Precio; // Actualizar precio unitario
    }
    else
    {
        carrito.Items.Add(new ItemCompra
        {
            ProductoId = productoId,
            Cantidad = data.Cantidad,
            PrecioUnitario = producto.Precio
        });
    }

    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado o actualizado en el carrito");
});

// Endpoint para eliminar un producto del carrito (o actualizar la cantidad)
app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, TiendaContext db) =>
{
    var carrito = await db.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound("Carrito no encontrado.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("Producto no encontrado en el carrito.");

    // Se carga el producto para actualizar stock
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado.");

    // Se incrementa el stock
    producto.Stock += 1;

    if (item.Cantidad > 1) item.Cantidad--;
    else db.ItemsCompra.Remove(item);

    await db.SaveChangesAsync();
    return Results.Ok("Producto eliminado o cantidad actualizada en el carrito.");
});

app.Run();

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = null!;

    public List<ItemCompra> ItemsCompra { get; set; } = new (); // Relación uno a muchos
}

class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    public string NombreCliente { get; set; } = null!;
    public string ApellidoCliente { get; set; } = null!;
    public string EmailCliente { get; set; } = null!;

    public List<ItemCompra> Items { get; set; } = new (); // Relación uno a muchos
}

class ItemCompra
{
    public int Id { get; set; }

    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;

    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
}

record ClienteDTO(string Nombre, string Apellido, string Email);

record ItemCantidadDTO(int Cantidad);