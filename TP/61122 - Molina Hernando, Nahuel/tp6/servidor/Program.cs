using Microsoft.EntityFrameworkCore;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar el contexto de la base de datos SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

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

var app = builder.Build();

var carritos = new Dictionary<string, List<(int ProductoId, int Cantidad)>>();

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

app.MapGet("/productos", async (TiendaDbContext db, string? buscar) =>
{
    var consulta = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(buscar))
        consulta = consulta.Where(p => p.Nombre.Contains(buscar) || p.Descripcion.Contains(buscar));
    return await consulta.ToListAsync();
});

app.MapPost("/carritos", () =>
{
    var idCarrito = Guid.NewGuid().ToString();
    carritos[idCarrito] = new List<(int ProductoId, int Cantidad)>();
    return Results.Ok(idCarrito);
});

app.MapGet("/carritos/{carritoId}", async (string carritoId, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    var items = new List<object>();
    foreach (var x in carritos[carritoId])
    {
        var producto = await db.Productos.FindAsync(x.ProductoId);
        items.Add(new { Producto = producto, x.Cantidad });
    }
    return Results.Ok(items);
});

app.MapDelete("/carritos/{carritoId}", (string carritoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    carritos[carritoId].Clear();
    return Results.Ok();
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (string carritoId, int productoId, int cantidad, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");
    if (cantidad < 1 || cantidad > producto.Stock)
        return Results.BadRequest("Cantidad inválida o sin stock suficiente");

    var carrito = carritos[carritoId];
    var item = carrito.FirstOrDefault(x => x.ProductoId == productoId);
    if (item.ProductoId == 0)
        carrito.Add((productoId, cantidad));
    else
    {
        carrito.Remove(item);
        carrito.Add((productoId, cantidad));
    }
    return Results.Ok();
});

app.MapDelete("/carritos/{carritoId}/{productoId}", (string carritoId, int productoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    var carrito = carritos[carritoId];
    var item = carrito.FirstOrDefault(x => x.ProductoId == productoId);
    if (item.ProductoId == 0)
        return Results.NotFound("Producto no está en el carrito");
    carrito.Remove(item);
    return Results.Ok();
});

app.MapPut("/carritos/{carritoId}/confirmar", async (string carritoId, ClienteDatos datos, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");
    var carrito = carritos[carritoId];
    if (!carrito.Any())
        return Results.BadRequest("El carrito está vacío");

    foreach (var item in carrito)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {producto?.Nombre}");
    }

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.Nombre,
        ApellidoCliente = datos.Apellido,
        EmailCliente = datos.Email,
        Articulos = new List<ArticuloDeCompra>()
    };
    decimal total = 0;
    foreach (var item in carrito)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
        var articulo = new ArticuloDeCompra
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        };
        compra.Articulos.Add(articulo);
        total += producto.Precio * item.Cantidad;
    }
    compra.Total = total;
    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    carritos[carritoId].Clear();
    return Results.Ok(new { compra.Id, compra.Total });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new List<Producto>
        {
            new Producto { Nombre = "iPhone 11 Negro", Descripcion = "Color: Negro, 64GB, Cámara: 12MP", Precio = 300000, Stock = 10, ImagenUrl = "https://images-na.ssl-images-amazon.com/images/I/61gYe3YaoxL._AC_SL1500_.jpg" },
            new Producto { Nombre = "iPhone 11 Pro Verde", Descripcion = "Color: Verde, 256GB, Cámara: 12MP triple", Precio = 350000, Stock = 10, ImagenUrl = "https://img.pccomponentes.com/articles/23/232741/verdeeee.jpg" },
            new Producto { Nombre = "iPhone 12 Blanco", Descripcion = "Color: Blanco, 128GB, Cámara: 12MP dual", Precio = 400000, Stock = 10, ImagenUrl = "https://media3.allzone.es/595850-large_default/smartphones-iphone-12-128gb-blanco-705256.jpg" },
            new Producto { Nombre = "iPhone 12 Pro Azul", Descripcion = "Color: Azul, 256GB, Cámara: 12MP triple", Precio = 450000, Stock = 10, ImagenUrl = "https://img.pccomponentes.com/articles/32/328890/1391-apple-iphone-12-pro-max-256gb-azul-pacifico-libre.jpg" },
            new Producto { Nombre = "iPhone 13 Rosa", Descripcion = "Color: Rosa, 128GB, Cámara: 12MP dual", Precio = 500000, Stock = 10, ImagenUrl = "https://media3.allzone.es/994382-large_default/smartphones-iphone-13-128gb-rosa-iphone13128gbpink.jpg" },
            new Producto { Nombre = "iPhone 13 Pro Grafito", Descripcion = "Color: Grafito, 256GB, Cámara: 12MP triple", Precio = 550000, Stock = 10, ImagenUrl = "https://img.pccomponentes.com/articles/57/578933/1686-apple-iphone-13-pro-256gb-grafito-libre.jpg" },
            new Producto { Nombre = "iPhone 14 Morado", Descripcion = "Color: Morado, 128GB, Cámara: 12MP dual", Precio = 600000, Stock = 10, ImagenUrl = "https://m.media-amazon.com/images/I/619f09kK7tL._AC_SL1500_.jpg" },
            new Producto { Nombre = "iPhone 14 Pro Plata", Descripcion = "Color: Plata, 256GB, Cámara: 48MP triple", Precio = 650000, Stock = 10, ImagenUrl = "https://resources.claroshop.com/medios-plazavip/fotos/productos_sears1/original/3589788.jpg" },
            new Producto { Nombre = "iPhone 15 Amarillo", Descripcion = "Color: Amarillo, 256GB, Cámara: 48MP dual", Precio = 700000, Stock = 10, ImagenUrl = "https://www.zaraphone.com/wp-content/uploads/2023/10/iPhone-15-Amarillo.jpg" },
            new Producto { Nombre = "iPhone 16 Pro Max Titanio", Descripcion = "Color: Titanio, 512GB, Cámara: 48MP triple", Precio = 800000, Stock = 10, ImagenUrl = "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/9471f613-7d82-400e-97ed-7dca6c0101af.jpg" }
        });
        db.SaveChanges();
    }
}

app.Run();
