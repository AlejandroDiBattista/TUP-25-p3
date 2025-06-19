using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

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

builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

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

app.MapGet("/productos", async (TiendaDbContext db, string? q) =>
{
    var productos = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        var consulta = q.Trim().ToLower().Replace(" ", "");
        productos = productos.Where(p =>
            p.Nombre.ToLower().Replace(" ", "").Contains(consulta) ||
            p.Descripcion.ToLower().Replace(" ", "").Contains(consulta)
        );
    }

    return await productos.ToListAsync();
});
app.MapPost("/carritos", async (TiendaDbContext db) =>
{
    var compra = new servidor.Models.Compra
    {
        Fecha = DateTime.Now,
        Total = 0,
        NombreCliente = "",
        ApellidoCliente = "",
        EmailCliente = "",
        Articulos = new List<servidor.Models.ArticuloCompra>()
    };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok(new { carritoId = compra.Id });
});
app.MapGet("/carritos/{carritoId:int}", async (int carritoId, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Articulos)
        .ThenInclude(a => a.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound();

    var dto = new
    {
        compra.Id,
        compra.Fecha,
        compra.Total,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Articulos = compra.Articulos.Select(a => new {
            a.Id,
            a.ProductoId,
            Producto = new {
                a.Producto.Id,
                a.Producto.Nombre,
                a.Producto.Precio,
                a.Producto.ImagenUrl
            },
            a.Cantidad,
            a.PrecioUnitario
        })
    };

    return Results.Ok(dto);
});
app.MapDelete("/carritos/{carritoId:int}", async (int carritoId, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Articulos)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null)
        return Results.NotFound();

    foreach (var articulo in compra.Articulos)
    {
        var producto = await db.Productos.FindAsync(articulo.ProductoId);
        if (producto != null)
        {
            producto.Stock += articulo.Cantidad;
        }
    }
    compra.Articulos.Clear();
    await db.SaveChangesAsync();

    return Results.Ok();
});
app.MapPut("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, CantidadDto cantidadDto, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Articulos)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    var producto = await db.Productos.FindAsync(productoId);

    if (compra == null || producto == null)
        return Results.NotFound();

    if (cantidadDto.Cantidad < 1)
        return Results.BadRequest("Cantidad inválida");

    var articulo = compra.Articulos.FirstOrDefault(a => a.ProductoId == productoId);

    if (articulo == null)
    {
        if (producto.Stock < cantidadDto.Cantidad)
            return Results.BadRequest("No hay stock suficiente");

        articulo = new servidor.Models.ArticuloCompra
        {
            ProductoId = productoId,
            Cantidad = cantidadDto.Cantidad,
            PrecioUnitario = producto.Precio
        };
        compra.Articulos.Add(articulo);

        producto.Stock -= cantidadDto.Cantidad;
    }
    else
    {
        int diferencia = cantidadDto.Cantidad - articulo.Cantidad;
        if (diferencia > 0)
        {
            if (producto.Stock < diferencia)
                return Results.BadRequest("No hay stock suficiente");

            producto.Stock -= diferencia;
        }
        else if (diferencia < 0)
        {
            producto.Stock += (-diferencia);
        }
        articulo.Cantidad = cantidadDto.Cantidad;
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});
app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Articulos)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null)
        return Results.NotFound();

    var articulo = compra.Articulos.FirstOrDefault(a => a.ProductoId == productoId);
    if (articulo == null)
        return Results.NotFound();

    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += articulo.Cantidad;
    }

    compra.Articulos.Remove(articulo);
    await db.SaveChangesAsync();

    return Results.Ok();
});
app.MapPut("/carritos/{carritoId:int}/confirmar", async (int carritoId, ClienteDto cliente, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Articulos)
        .ThenInclude(a => a.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(cliente.Nombre) ||
        string.IsNullOrWhiteSpace(cliente.Apellido) ||
        string.IsNullOrWhiteSpace(cliente.Email))
    {
        return Results.BadRequest("Todos los datos del cliente son obligatorios.");
    }

    if (compra.Articulos == null || !compra.Articulos.Any())
    {
        return Results.BadRequest("El carrito está vacío.");
    }

    compra.NombreCliente = cliente.Nombre;
    compra.ApellidoCliente = cliente.Apellido;
    compra.EmailCliente = cliente.Email;
    compra.Total = compra.Articulos.Sum(a => a.Cantidad * a.PrecioUnitario);

    foreach (var articulo in compra.Articulos)
    {
        if (articulo.Producto.Stock < 0)
            return Results.BadRequest($"Stock inconsistente para {articulo.Producto.Nombre}");
    }

    await db.SaveChangesAsync();

    var resumen = new
    {
        compra.Id,
        compra.Total,
        compra.NombreCliente,
        compra.ApellidoCliente,
        compra.EmailCliente,
        Articulos = compra.Articulos.Select(a => new {
            a.ProductoId,
            a.Cantidad,
            a.PrecioUnitario
        })
    };

    return Results.Ok(resumen);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();
}
app.Run();
public record CantidadDto(int Cantidad);
public record ClienteDto(string Nombre, string Apellido, string Email);