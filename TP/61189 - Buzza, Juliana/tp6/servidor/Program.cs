using Microsoft.EntityFrameworkCore; 
using servidor.Data; 
using servidor.Models; 
using System.Linq; 


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DatabaseInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

app.UseCors(policy =>
    policy.WithOrigins("http://localhost:5177", "https://localhost:7221") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/productos", async (string? query, AppDbContext db) =>
{
    IQueryable<Producto> productos = db.Productos;
    if (!string.IsNullOrEmpty(query))
    {
        productos = productos.Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query));
    }
    return Results.Ok(await productos.ToListAsync());
});

app.MapPost("/carritos", async (AppDbContext db) =>
{
    var newCart = new Compra { Status = "Pending", Fecha = DateTime.Now, Total = 0m };
    db.Compras.Add(newCart);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{newCart.Id}", newCart);
});

app.MapGet("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var cart = await db.Compras
                    .Include(c => c.Items)
                    .ThenInclude(ic => ic.Producto)
                    .FirstOrDefaultAsync(c => c.Id == carritoId && c.Status == "Pending");

    if (cart == null)
    {
        return Results.NotFound("Carrito no encontrado o ya confirmado.");
    }

    return Results.Ok(cart.Items.Select(item => new
    {
        item.Id,
        item.ProductoId,
        ProductoNombre = item.Producto?.Nombre,
        item.Cantidad,
        item.PrecioUnitario,
        ProductoImagenUrl = item.Producto?.ImagenUrl,
        ProductoStock = item.Producto?.Stock 
    }));
});

app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();
