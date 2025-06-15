using Microsoft.EntityFrameworkCore;
using servidor.Modelos;
using System.Net.Http.Headers;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184/") });

var app = builder.Build();


if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowClientApp");
app.UseStaticFiles(); //.


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "../cliente/wwwroot")),
    RequestPath = ""
});

app.MapGet("/", () => "Servidor API está en funcionamiento");


app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });


app.MapGet("/productos", async (TiendaContext baseD, string? buscar) =>
{
    var query = baseD.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(buscar))
        query = query.Where(p =>
            p.Nombre.ToLower().Contains(buscar.ToLower()) ||
            p.Descripcion.ToLower().Contains(buscar.ToLower())
        );

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/carritos", async (TiendaContext baseD) =>
{
    var carrito = new Carrito();
    baseD.Carritos.Add(carrito);
    await baseD.SaveChangesAsync();
    return Results.Ok(carrito.Id);
});

app.MapGet("/carritos/{carritoId}", async (TiendaContext db, Guid carritoId) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null) return Results.NotFound();

    
    var resultado = carrito.Items.Select(i => new {
        i.Id,
        i.ProductoId,
        i.Cantidad,
        i.CarritoId
    });

    return Results.Ok(resultado);
});

app.MapDelete("/carritos/{carritoId}", async (TiendaContext baseD, Guid carritoId) =>
{
    var carrito = await baseD.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    foreach (var item in carrito.Items)
    {
        var producto = await baseD.Productos.FindAsync(item.ProductoId);
        if (producto != null)
            producto.Stock += item.Cantidad;
    }

    carrito.Items.Clear();
    await baseD.SaveChangesAsync();
    return Results.Ok();
});


app.MapPut("/carritos/{carritoId}/{productoId}", async (TiendaContext baseD, Guid carritoId, int productoId, int cantidad) =>
{
    var carrito = await baseD.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    var producto = await baseD.Productos.FindAsync(productoId);
    if (carrito == null || producto == null) return Results.NotFound();

    if (cantidad <= 0) return Results.BadRequest("Cantidad inválida");
    if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        carrito.Items.Add(new ItemCarrito { ProductoId = productoId, Cantidad = cantidad, CarritoId = carritoId });
    else
        item.Cantidad += cantidad;

    producto.Stock -= cantidad;






    await baseD.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/carritos/{carritoId}/{productoId}", async (TiendaContext baseD, Guid carritoId, int productoId, int cantidad) =>
{
    var carrito = await baseD.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    var producto = await baseD.Productos.FindAsync(productoId);
    if (carrito == null || producto == null) return Results.NotFound();

    if (cantidad <= 0) return Results.BadRequest("Cantidad inválida");
    if (producto.Stock < cantidad) return Results.BadRequest("Stock insuficiente");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        carrito.Items.Add(new ItemCarrito { ProductoId = productoId, Cantidad = cantidad, CarritoId = carritoId });
    else
        item.Cantidad += cantidad;

    producto.Stock -= cantidad;

    await baseD.SaveChangesAsync();
    return Results.Ok();
});


app.MapDelete("/carritos/{carritoId}/{productoId}", async (TiendaContext baseD, Guid carritoId, int productoId) =>
{
    var carrito = await baseD.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null) return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item != null)
    {
        var producto = await baseD.Productos.FindAsync(productoId);
        if (producto != null)
            producto.Stock += item.Cantidad; 

        carrito.Items.Remove(item);
        await baseD.SaveChangesAsync();
    }
    return Results.Ok();
});

app.MapPut("/carritos/{carritoId}/confirmar", async (TiendaContext baseD, Guid carritoId, ClienteDto cliente) =>
{
    var carrito = await baseD.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito == null || !carrito.Items.Any()) return Results.BadRequest("Carrito vacío o no existe");

    foreach (var item in carrito.Items)
    {
        var producto = await baseD.Productos.FindAsync(item.ProductoId);
        if (producto == null)
            return Results.BadRequest($"Producto no encontrado: {item.ProductoId}");
        
    }

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = cliente.Nombre,
        ApellidoCliente = cliente.Apellido,
        EmailCliente = cliente.Email,
        Total = carrito.Items.Sum(i => baseD.Productos.First(p => p.Id == i.ProductoId).Precio * i.Cantidad),
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = baseD.Productos.First(p => p.Id == i.ProductoId).Precio
        }).ToList()
    };
    baseD.Compras.Add(compra);

    carrito.Items.Clear();
    await baseD.SaveChangesAsync();
    return Results.Ok(compra.Id);
});


app.MapPut("/carritos/{carritoId}/{productoId}/cantidad", async (TiendaContext baseD, Guid carritoId, int productoId, int nuevaCantidad) =>
{
    var carrito = await baseD.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    var producto = await baseD.Productos.FindAsync(productoId);
    if (carrito == null || producto == null) return Results.NotFound();

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null) return Results.NotFound();

    int diferencia = nuevaCantidad - item.Cantidad;

   
    if (diferencia > 0 && producto.Stock < diferencia)
        return Results.BadRequest("Stock insuficiente");

   
    producto.Stock -= diferencia;
    item.Cantidad = nuevaCantidad;

    
    if (item.Cantidad <= 0)
        carrito.Items.Remove(item);

    await baseD.SaveChangesAsync();
    return Results.Ok();
});


using (var scope = app.Services.CreateScope())
{
    var baseD = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    baseD.Database.EnsureCreated();

    if (!baseD.Productos.Any())
    {
        baseD.Productos.AddRange(
            new Producto { Nombre = "Toyota Corolla", Descripcion = "Sedán compacto, motor 1.8L", Precio = 12000000, Stock = 5, ImagUrl = "toyotacorolla.jpg" },
            new Producto { Nombre = "Ford Fiesta", Descripcion = "Hatchback, motor 1.6L", Precio = 9500000, Stock = 7, ImagUrl = "forfiesta.jpg" },
            new Producto { Nombre = "Volkswagen Golf", Descripcion = "Hatchback, motor 1.4L TSI", Precio = 11000000, Stock = 4, ImagUrl = "VolkswagenGolf.jpg" },
            new Producto { Nombre = "Chevrolet Onix", Descripcion = "Hatchback, motor 1.2L", Precio = 8700000, Stock = 8, ImagUrl = "ChevroletOnix.jpg" },
            new Producto { Nombre = "Renault Sandero", Descripcion = "Hatchback, motor 1.6L", Precio = 8000000, Stock = 6, ImagUrl = "RenaultSandero.jpg" },
            new Producto { Nombre = "Peugeot 208", Descripcion = "Hatchback, motor 1.2L", Precio = 9200000, Stock = 9, ImagUrl = "Peugeot 208.jpg" },
            new Producto { Nombre = "Fiat Cronos", Descripcion = "Sedán, motor 1.3L", Precio = 8500000, Stock = 10, ImagUrl = "FiatCronos.jpg" },
            new Producto { Nombre = "Honda Civic", Descripcion = "Sedán, motor 2.0L", Precio = 15000000, Stock = 3, ImagUrl = "HondaCivic.jpg" },
            new Producto { Nombre = "Nissan Versa", Descripcion = "Sedán, motor 1.6L", Precio = 9800000, Stock = 6, ImagUrl = "NissanVersa.jpg" },
            new Producto { Nombre = "Toyota Hilux", Descripcion = "Pick-up, motor 2.8L", Precio = 18000000, Stock = 2, ImagUrl = "ToyotaHilux.jpg" }
        );
        baseD.SaveChanges();
    }
}

app.Run();
