using Microsoft.EntityFrameworkCore;
using Compartido; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tienda.db";
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/api/productos", async (TiendaDbContext dbContext, string? busqueda) =>
{
    var query = dbContext.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        query = query.Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower()) ||
                                 p.Descripcion.ToLower().Contains(busqueda.ToLower()));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/api/carritos", async (TiendaDbContext dbContext) =>
{
    var nuevoCarrito = new Carrito
    {
        Id = Guid.NewGuid(), 
        FechaCreacion = DateTime.UtcNow
    };

    dbContext.Carritos.Add(nuevoCarrito);
    await dbContext.SaveChangesAsync();

    return Results.Ok(new { CarritoId = nuevoCarrito.Id });
});

app.MapGet("/api/carritos/{carritoId:guid}", async (Guid carritoId, TiendaDbContext dbContext) =>
{
    var carrito = await dbContext.Carritos
                                 .Include(c => c.Items)
                                 .ThenInclude(ic => ic.Producto) 
                                 .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
    {
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
    }

    var itemsDto = carrito.Items.Select(ic => new 
    {
        ic.ProductoId,
        NombreProducto = ic.Producto?.Nombre,
        ic.Cantidad,
        PrecioUnitario = ic.Producto?.Precio,
        ImagenUrl = ic.Producto?.ImagenUrl
    }).ToList();

    return Results.Ok(itemsDto);
});

app.Run();
