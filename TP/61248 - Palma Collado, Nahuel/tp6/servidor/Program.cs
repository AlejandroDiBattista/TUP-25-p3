using Microsoft.EntityFrameworkCore;
using TiendaOnline.Datos;
using TiendaOnline.Modelos; 
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrar el DbContext con SQLite 
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tiendaonline.db"));
    
// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    DbInitializer.Inicializar(db);
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/api/productos", async (TiendaDbContext db, string? busqueda) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrEmpty(busqueda))
    {
        query = query.Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower()));
                                 
    }
    
    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/api/carritos", async (TiendaDbContext db) =>
{
    var nuevaCompra = new Compra
    {
        Total = 0,
    };

    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();

    return Results.Ok(new { CarritoId = nuevaCompra.Id });
});

app.MapGet("/api/carritos/{carritoId}", async (int carritoId, TiendaDbContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.ItemsDeCompra)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (compra is null)
        return Results.NotFound("Carrito no encontrado.");

    var resultado = compra.ItemsDeCompra.Select(item => new
    {
        item.ProductoId,
        Nombre = item.Producto!.Nombre,
        PrecioUnitario = item.PrecioUnitario,
        Cantidad = item.Cantidad,
        Importe = item.PrecioUnitario * item.Cantidad
    });

    return Results.Ok(resultado);
});


app.Run();
