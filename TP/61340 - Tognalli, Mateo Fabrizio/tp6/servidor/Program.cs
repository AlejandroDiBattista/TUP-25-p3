using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Services;
using Servidor.DTOs;
using Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tienda.db"));

// Registrar servicios - CarritoService como Singleton para mantener estado entre requests
builder.Services.AddSingleton<CarritoService>();

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar servicios OpenAPI/Swagger para documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializar la base de datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    context.Database.EnsureCreated();
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// === ENDPOINTS DE LA API ===

// GET /productos - Obtener todos los productos (con búsqueda opcional)
app.MapGet("/productos", async (TiendaDbContext context, string busqueda = "") =>
{
    var query = context.Productos.AsQueryable();
    
    if (!string.IsNullOrEmpty(busqueda))
    {
        var busquedaLower = busqueda.ToLower();
        query = query.Where(p => p.Nombre.ToLower().Contains(busquedaLower) || p.Descripcion.ToLower().Contains(busquedaLower));
    }
    
    var productos = await query.Select(p => new ProductoDto
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Precio = p.Precio,
        Stock = p.Stock,
        ImagenUrl = p.ImagenUrl
    }).ToListAsync();
    
    return Results.Ok(productos);
});

// POST /carritos - Inicializar un nuevo carrito
app.MapPost("/carritos", (CarritoService carritoService) =>
{
    var carritoId = carritoService.CrearCarrito();
    return Results.Ok(new { CarritoId = carritoId });
});

// GET /carritos/{carrito} - Obtener los items del carrito
app.MapGet("/carritos/{carrito}", (string carrito, CarritoService carritoService) =>
{
    var carritoDto = carritoService.ObtenerCarrito(carrito);
    
    if (carritoDto == null)
        return Results.NotFound("Carrito no encontrado");
    
    return Results.Ok(carritoDto);
});

// DELETE /carritos/{carrito} - Vaciar el carrito
app.MapDelete("/carritos/{carrito}", (string carrito, CarritoService carritoService) =>
{
    var resultado = carritoService.VaciarCarrito(carrito);
    
    if (!resultado)
        return Results.NotFound("Carrito no encontrado");
    
    return Results.Ok(new { Mensaje = "Carrito vaciado correctamente" });
});

// PUT /carritos/{carrito}/confirmar - Confirmar compra
app.MapPut("/carritos/{carrito}/confirmar", async (string carrito, ConfirmarCompraDto datosCliente, CarritoService carritoService) =>
{
    var resultado = await carritoService.ConfirmarCompra(carrito, datosCliente);
    
    if (!resultado)
        return Results.BadRequest("No se pudo confirmar la compra. Verifique el stock disponible.");
    
    return Results.Ok(new { Mensaje = "Compra confirmada exitosamente" });
});

// PUT /carritos/{carrito}/{producto} - Agregar producto al carrito
app.MapPut("/carritos/{carrito}/{producto}", async (string carrito, int producto, CarritoService carritoService, int cantidad = 1) =>
{
    var resultado = await carritoService.AgregarProducto(carrito, producto, cantidad);
    
    if (!resultado)
        return Results.BadRequest("No se pudo agregar el producto. Verifique el stock disponible.");
    
    return Results.Ok(new { Mensaje = "Producto agregado al carrito" });
});

// DELETE /carritos/{carrito}/{producto} - Eliminar producto del carrito
app.MapDelete("/carritos/{carrito}/{producto}", (string carrito, int producto, CarritoService carritoService, int cantidad = 1) =>
{
    var resultado = carritoService.EliminarProducto(carrito, producto, cantidad);
    
    if (!resultado)
        return Results.NotFound("Producto no encontrado en el carrito");
    
    return Results.Ok(new { Mensaje = "Producto eliminado del carrito" });
});

// PUT /admin/productos/{id}/stock - Endpoint administrativo para restaurar stock
app.MapPut("/admin/productos/{id}/stock", async (int id, int nuevoStock, TiendaDbContext context) =>
{
    var producto = await context.Productos.FindAsync(id);
    
    if (producto == null)
        return Results.NotFound("Producto no encontrado");
    
    producto.Stock = nuevoStock;
    await context.SaveChangesAsync();
    
    return Results.Ok(new { 
        Mensaje = $"Stock actualizado para {producto.Nombre}",
        ProductoId = producto.Id,
        NombreProducto = producto.Nombre,
        NuevoStock = producto.Stock
    });
});

// Endpoint adicional para obtener un producto específico
app.MapGet("/productos/{id}", async (int id, TiendaDbContext context) =>
{
    var producto = await context.Productos.FindAsync(id);
    
    if (producto == null)
        return Results.NotFound("Producto no encontrado");
    
    var productoDto = new ProductoDto
    {
        Id = producto.Id,
        Nombre = producto.Nombre,
        Descripcion = producto.Descripcion,
        Precio = producto.Precio,
        Stock = producto.Stock,
        ImagenUrl = producto.ImagenUrl
    };
    
    return Results.Ok(productoDto);
});

// Endpoint de estado de la API
app.MapGet("/", () => "TechStore API está funcionando correctamente");

app.Run();
