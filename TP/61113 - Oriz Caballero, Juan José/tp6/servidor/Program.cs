#nullable enable
using Microsoft.EntityFrameworkCore;
using Servidor.Modelo;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

//clase 19.1
//intento de conexion eliminado (innecesario en el servidor)

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

//si explota o no funciona, no existe la bd ni el traslado de datos [CREO] (comprobar luego con documentacion y borrador) 
//(pregunta para mi: si explota es culpa de la incorporación de estas lineas o soy tonto y no lo apliqué bien??????)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseRouting();
app.UseCors("AllowClientApp");
app.UseAuthorization();

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapPost("/carritos", async (AppDbContext db) =>
{
    var carrito = new Carrito
    {
        Items = new List<ItemCarrito>()
    };

    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();

    return Results.Json(new Dictionary<string, int> { { "CarritoId", carrito.Id } });
});

//ESTO ES GET Y LA BUSQUEDA (mejorada para ignorar tildes y ñ)
app.MapGet("/productos", async (AppDbContext db, string? search) =>
{
    var productos = await db.Productos.ToListAsync();

    if (!string.IsNullOrWhiteSpace(search))
    {
        // normaliza texto a minúscula y sin acentos/tildes
        string Normalizar(string texto)
        {
            return string.Concat(
                texto.Normalize(NormalizationForm.FormD)
                     .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            ).ToLower();
        }

        var textoBusqueda = Normalizar(search);

        productos = productos.Where(p =>
            Normalizar(p.Nombre).Contains(textoBusqueda) ||
            Normalizar(p.Descripcion).Contains(textoBusqueda)
        ).ToList();
    }

    return Results.Ok(productos);
});

//ESTO ES GET CON CONEXION AL CARRITO
app.MapGet("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var carrito = await db.Carritos
                          .Include(c => c.Items)
                          .ThenInclude(i => i.Producto) 
                          .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound("Carrito no encontrado.");

    var carritoDto = new
    {
        Id = carrito.Id,
        Items = carrito.Items.Select(item => new {
            ProductoId = item.ProductoId,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.PrecioUnitario,
            Producto = new {
                Id = item.Producto.Id,
                Nombre = item.Producto.Nombre,
                Precio = item.Producto.Precio,
                Stock = item.Producto.Stock,
                ImagenUrl = item.Producto.ImagenUrl,
                Descripcion = item.Producto.Descripcion
            }
        })
    };

    return Results.Ok(carritoDto);
});

//ESTO ES DELETE
app.MapDelete("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var carrito = await db.Carritos.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound("Carrito no encontrado.");

    foreach (var item in carrito.Items)
    {
        item.Producto.Stock += item.Cantidad;
    }

    db.Carritos.Remove(carrito);
    await db.SaveChangesAsync();
    return Results.Ok("Carrito eliminado.");
});

//ESTO ES PUT Y LA CONFIRMACION
app.MapPut("/carritos/{carritoId}/confirmar", async (int carritoId, AppDbContext db, ConfirmacionCompraDto datos) =>
{
    var carrito = await db.Carritos
                          .Include(c => c.Items)
                          .ThenInclude(i => i.Producto)
                          .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound("Carrito no encontrado.");
    if (carrito.Items.Count == 0) return Results.BadRequest("El carrito está vacío.");

    // VALIDACION
    foreach (var item in carrito.Items)
    {
        if (item.Producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para el producto {item.Producto.Nombre}.");
        item.Producto.Stock -= item.Cantidad;
    }

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = datos.Nombre,
        ApellidoCliente = datos.Apellido,
        EmailCliente = datos.Email,
        Total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario)
    };

    db.Compras.Add(compra);
    carrito.Items.Clear(); 
    await db.SaveChangesAsync();

    return Results.Ok("Compra confirmada.");
});

// PUT: Agregar o actualizar producto en carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, AppDbContext db, [FromBody] int cantidad) =>
{
    if (cantidad <= 0) return Results.BadRequest("La cantidad debe ser mayor a cero.");

    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound("Carrito no encontrado.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null) return Results.NotFound("Producto no encontrado.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (item == null)
    {
        // Si el producto no está en el carrito, lo agregamos si hay stock suficiente
        if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente.");

        carrito.Items.Add(new ItemCarrito
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        });

        producto.Stock -= cantidad;
    }
    else
    {
        // ajusta cantidad
        int diferencia = cantidad - item.Cantidad;
        if (producto.Stock < diferencia) return Results.BadRequest("Stock insuficiente.");

        producto.Stock -= diferencia;
        item.Cantidad = cantidad;
    }

    await db.SaveChangesAsync();

    // formato
    var carritoDto = new
    {
        Id = carrito.Id,
        Items = carrito.Items.Select(item => new {
            ProductoId = item.ProductoId,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.PrecioUnitario,
            Producto = new {
                Id = item.Producto.Id,
                Nombre = item.Producto.Nombre,
                Precio = item.Producto.Precio,
                Stock = item.Producto.Stock,
                ImagenUrl = item.Producto.ImagenUrl,
                Descripcion = item.Producto.Descripcion
            }
        })
    };

    return Results.Ok(carritoDto);
});

// DELETE: Eliminar o reducir cantidad de producto en carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, AppDbContext db, [FromQuery] int cantidad) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound("Carrito no encontrado.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound("Producto no está en el carrito.");

    var producto = item.Producto;

    if (cantidad <= 0 || cantidad >= item.Cantidad)
    {
        producto.Stock += item.Cantidad; // DEVUELVE TODO
        carrito.Items.Remove(item);
    }
    else
    {
        item.Cantidad -= cantidad;
        producto.Stock += cantidad; //  DEVUELVE PARTE
    }

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

app.Run();
