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
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

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
            new Producto { Nombre = "Celular Galaxy A54", Descripcion = "Celular Samsung 128GB", Precio = 190000, Stock = 10, ImagenUrl = "https://imgs.search.brave.com/LaLRpDspuUpj9USNOkcKXx4Jgm99wY3O9zS3z8AIMCs/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9jZG4u/bGVzbnVtZXJpcXVl/cy5jb20vb3B0aW0v/cHJvZHVjdC83MS83/MTk3MS85YjU3YWQw/OC1nYWxheHktYTU0/X180MDBfNDAwLmpw/Zw" },
            new Producto { Nombre = "Celular iPhone 13", Descripcion = "Apple iPhone 13 256GB", Precio = 420000, Stock = 5, ImagenUrl = "https://imgs.search.brave.com/aoxrpNi0vTljtdxmNM4WmsYsq6vftQtByDl0chNHr7g/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly90aWVu/ZGEucGVyc29uYWwu/Y29tLmFyL2ltYWdl/cy83MjAvd2VicC9p/X1Bob25lXzEzX1By/b18yNTZfR0JfR3Jh/cGhpdGVfNzNmM2Uw/NmQxYy5wbmc" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido 20W", Precio = 7000, Stock = 30, ImagenUrl = "https://imgs.search.brave.com/hmXX10qrpBgPf9pdr2tS10AkXFlA7-PhUk7H5CB1hvs/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9odHRw/Mi5tbHN0YXRpYy5j/b20vRF9RX05QXzJY/Xzg0Njg3Ni1NTFU3/NTg1NTU4ODU1N18w/NDIwMjQtVi53ZWJw" },
            new Producto { Nombre = "Auriculares Bluetooth", Descripcion = "Auriculares inalámbricos", Precio = 15000, Stock = 25, ImagenUrl = "https://imgs.search.brave.com/NrkV8_8ym_q582Nb4t9CjbSkMRFulRNzV47T7zIBMLQ/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly90aWVu/ZGEucGVyc29uYWwu/Y29tLmFyL2ltYWdl/cy83MjAvd2VicC9B/dXJpY3VsYXJlc19C/bHVldG9vdGhfRm94/Ym94X09wZW5haXJf/Q2xpcF9jMTdlY2Ux/ZWJhLnBuZw" },
            new Producto { Nombre = "Smartwatch", Descripcion = "Reloj inteligente con GPS", Precio = 25000, Stock = 12, ImagenUrl = "https://imgs.search.brave.com/ICfA_LH4vnplJ6q30uPq2RSeFR6ro0m4XodiDM_baSw/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly93d3cu/dGl0YW4uY28uaW4v/ZHcvaW1hZ2UvdjIv/QktERF9QUkQvb24v/ZGVtYW5kd2FyZS5z/dGF0aWMvLS9TaXRl/cy10aXRhbi1tYXN0/ZXItY2F0YWxvZy9k/ZWZhdWx0L2R3YTZj/OGE5MWUvaW1hZ2Vz/L0Zhc3RyYWNrL0Nh/dGFsb2cvMzgxMjNR/TTAxXzQuanBnP3N3/PTM2MCZzaD0zNjA" },
            new Producto { Nombre = "Notebook Lenovo", Descripcion = "Laptop 15.6'' 8GB RAM", Precio = 320000, Stock = 7, ImagenUrl = "https://imgs.search.brave.com/442IklAxpbCqzYRhJ_mYwoHHJiPfp1Ou5abe1kJTHPE/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9odHRw/Mi5tbHN0YXRpYy5j/b20vRF9RX05QXzJY/Xzg2MTgxMy1NTFU3/NzEyNjkxNTYxOF8w/NjIwMjQtRS53ZWJw" },
            new Producto { Nombre = "Gaseosa Coca-Cola 2L", Descripcion = "Botella 2 litros", Precio = 1000, Stock = 50, ImagenUrl = "https://imgs.search.brave.com/cMOCtFIHhrcTIWd-pQMVnZihbOLYs-LQ2qa10p2-q8I/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9hbmRp/bmFjb2NhY29sYWFy/LnZ0ZXhhc3NldHMu/Y29tL2FycXVpdm9z/L2lkcy8xNTYyMjMt/ODAwLTgwMD92PTYz/ODU3MDE1NTkyNDM3/MDAwMCZ3aWR0aD04/MDAmaGVpZ2h0PTgw/MCZhc3BlY3Q9dHJ1/ZQ" },
            new Producto { Nombre = "Gaseosa Pepsi 2L", Descripcion = "Pepsi 2 litros", Precio = 950, Stock = 45, ImagenUrl = "https://imgs.search.brave.com/ZAjNsfIYrtqJy0RJdPHH8s4Omy0mSAXevb8c6LRD_Gc/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9tLm1l/ZGlhLWFtYXpvbi5j/b20vaW1hZ2VzL0kv/NTFZRTlqcFNjUkwu/anBn" },
            new Producto { Nombre = "Chocolate Milka", Descripcion = "Chocolate con leche 100g", Precio = 800, Stock = 100, ImagenUrl = "https://elnenearg.vtexassets.com/arquivos/ids/164041-800-auto?v=638023197840300000&width=800&height=auto&aspect=true" },
            new Producto { Nombre = "Papas Lays", Descripcion = "Bolsa de papas fritas 200g", Precio = 1200, Stock = 60, ImagenUrl = "https://imgs.search.brave.com/_2c-50Qz6ktaToRAjHUjhqBUMMh2iPiq7jyklAmDW84/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly90dXN1/cGVyLmNvbS5hci9p/bWFnZS9jYWNoZS9j/YXRhbG9nL1AyMDIz/L0RpY2llbWJyZSUy/MDE2L1BhcGFzLUZy/aXRhcy1DbGFzaWNh/cy1MYXlzLTg1LUdy/LTEtMTY5NzAlMjAo/MSklMjAoMSktNjAw/eDYwMC5qcGc" },
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
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound("Carrito no encontrado");

    db.ItemsCompra.RemoveRange(compra.Items);
    await db.SaveChangesAsync();

    return Results.Ok("Carrito vaciado exitosamente");
});

// Endpoint para confirmar una carrito de compras
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

    foreach (var item in compra.Items)
        {
            if (item.Cantidad > item.Producto.Stock) return Results.BadRequest($"No hay stock suficiente para el producto {item.Producto.Nombre}");
        }

    // Actualizar stock de productos y calcular total
    decimal total = 0;
    foreach (var item in compra.Items)
    {
        item.Producto.Stock -= item.Cantidad;
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
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    int cantidadTotal = (item?.Cantidad ?? 0) + data.Cantidad;
    if (cantidadTotal > producto.Stock) return Results.BadRequest("Stock insuficiente");

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