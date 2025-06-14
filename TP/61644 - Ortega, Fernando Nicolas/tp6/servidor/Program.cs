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
        productos = productos.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
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

    if (compra == null) return Results.NotFound();

    db.ArticulosCompra.RemoveRange(compra.Articulos);
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
    }
    else
    {
        if (producto.Stock + articulo.Cantidad < cantidadDto.Cantidad)
            return Results.BadRequest("No hay stock suficiente");

        articulo.Cantidad = cantidadDto.Cantidad;
    }

    await db.SaveChangesAsync();
    var articulosDto = compra.Articulos.Select(a => new {
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
    });
    return Results.Ok(articulosDto);
});
app.MapDelete("/carritos/{carritoId:int}/{productoId:int}", async (int carritoId, int productoId, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Articulos)
        .ThenInclude(a => a.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra == null) return Results.NotFound();

    var articulo = compra.Articulos.FirstOrDefault(a => a.ProductoId == productoId);

    if (articulo == null) return Results.NotFound();

    compra.Articulos.Remove(articulo);
    db.ArticulosCompra.Remove(articulo);

    await db.SaveChangesAsync();

    // Devuelve solo los artículos restantes como DTO
    var articulosDto = compra.Articulos.Select(a => new {
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
    });

    return Results.Ok(articulosDto);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();
}
app.Run();
public record CantidadDto(int Cantidad);
