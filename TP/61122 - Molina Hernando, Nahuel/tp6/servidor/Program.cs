using Microsoft.EntityFrameworkCore;
using servidor.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configura el contexto de la base de datos SQLite (sin migraciones)
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configura CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

var carritos = new List<Carrito>();

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

app.MapGet("/productos", async (TiendaDbContext db, string? filtro) =>
{
    var consulta = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(filtro))
    {
        var filtroLower = filtro.ToLower();
        consulta = consulta.Where(p =>
            p.Nombre.ToLower().Contains(filtroLower) ||
            p.Descripcion.ToLower().Contains(filtroLower)
        );
    }
    return await consulta.ToListAsync();
});

app.MapGet("/compras", async (TiendaDbContext db) =>
{
    var compras = await db.Compras
        .Include(c => c.Articulos)
        .ToListAsync();
    return Results.Ok(compras);
});

app.MapGet("/carritos", () =>
{
    return Results.Ok(carritos);
});

app.MapPost("/carritos", () =>
{
    var idCarrito = Guid.NewGuid().ToString();
    carritos.Add(new Carrito(){
        Id = idCarrito,
    });
    return Results.Ok(idCarrito);
});

app.MapGet("/carritos/{carritoId}", async (string carritoId, TiendaDbContext db) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == carritoId);
    if (carrito == null)
    {
        return Results.NotFound("Carrito no encontrado");
    }

    var items = new List<object>();
    foreach (var p in carrito.Productos)
    {
        var producto = await db.Productos.FindAsync(p.Id);
        items.Add(new { Producto = producto, p.Cantidad });
    }
    return Results.Ok(items);
});

app.MapDelete("/carritos/{carritoId}", async (string carritoId, TiendaDbContext db) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == carritoId);
    if (carrito == null)
    {
        return Results.NotFound("Carrito no encontrado");
    }

    foreach(var productoCarrito in carrito.Productos)
    {
        var prodDb = await db.Productos.FindAsync(productoCarrito.Id);
        prodDb.Stock += productoCarrito.Cantidad;
    }
    await db.SaveChangesAsync();

    carrito.Productos.Clear();
    return Results.Ok();
});

app.MapPut("/carritos/{carritoId}/{productoId}", async (string carritoId, int productoId, int cantidad, TiendaDbContext db) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == carritoId);
    if (carrito == null)
    {
        return Results.NotFound("Carrito no encontrado");
    }

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
    {
        return Results.NotFound("Producto no encontrado");
    }

    var item = carrito.Productos.FirstOrDefault(x => x.Id == productoId);
    int cantidadEnCarrito = item?.Cantidad ?? 0;
    int stockDisponible = producto.Stock + cantidadEnCarrito;

    if (cantidad < 1 || (cantidadEnCarrito + cantidad) > stockDisponible)
    {
        return Results.BadRequest("Cantidad inválida o sin stock suficiente");
    }

    if (item == null)
    {
        carrito.Productos.Add(new Carrito.ProductoCarrito(){
            Id = productoId, 
            Cantidad = cantidad
        });
    }
    else
    {
        item.Cantidad += cantidad;
    }

    // Actualiza el stock real
    producto.Stock = stockDisponible - (cantidadEnCarrito + cantidad);

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapDelete("/carritos/{carritoId}/{productoId}", async (string carritoId, int productoId, int cantidad, TiendaDbContext db) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == carritoId);
    if (carrito == null)
    {
        return Results.NotFound("Carrito no encontrado");
    }

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
    {
        return Results.NotFound("Producto no encontrado");
    }

    var item = carrito.Productos.FirstOrDefault(x => x.Id == productoId);
    if (item == null)
    {
        return Results.NotFound("Producto no está en el carrito");
    }
    else
    {
        if(item.Cantidad - cantidad < 0)
        {
            cantidad = item.Cantidad;
        }
        item.Cantidad -= cantidad;
        if(item.Cantidad == 0)
        {
            carrito.Productos.Remove(item);
        }
        producto.Stock += cantidad;
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/carritos/{carritoId}/confirmar", async (string carritoId, [FromBody]ClienteDatos datos, TiendaDbContext db) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == carritoId);
    if (carrito == null)
    {
        return Results.NotFound("Carrito no encontrado");
    }

    if (!carrito.Productos.Any())
    {
        return Results.BadRequest("El carrito está vacío");
    }

    List<Producto> productos = new List<Producto>();
    foreach (var item in carrito.Productos)
    {
        var producto = await db.Productos.FindAsync(item.Id);
        productos.Add(producto);
       
    }

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.Nombre,
        ApellidoCliente = datos.Apellido,
        EmailCliente = datos.Email
    };

    decimal total = 0;
    foreach (var item in carrito.Productos)
    {
        var producto = await db.Productos.FindAsync(item.Id);
        //producto.Stock -= item.Cantidad;
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

    carritos.Remove(carrito);
    return Results.Ok(new { compra.Id, compra.Total });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated(); // <-- Agrega esta línea para crear la base y las tablas si no existen

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
