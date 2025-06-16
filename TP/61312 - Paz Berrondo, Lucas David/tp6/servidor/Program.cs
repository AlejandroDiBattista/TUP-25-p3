using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Services;
using servidor.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios de la aplicación
builder.Services.AddScoped<CarritoService>();

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

// Crear, migrar y poblar la base de datos al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    
    // Crear la base de datos si no existe
    context.Database.EnsureCreated();
    
    // Poblar la base de datos con datos iniciales
    DatabaseSeeder.SeedDatabase(context);
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API de Tienda Online está en funcionamiento");

// === ENDPOINTS DE PRODUCTOS ===

// GET /api/productos - Obtiene todos los productos o busca por nombre usando el parámetro 'buscar'
app.MapGet("/api/productos", async (TiendaContext context, string? buscar) =>
{
    try
    {
        var query = context.Productos.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()));
        }
        
        var productos = await query
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Stock = p.Stock,
                ImagenUrl = p.ImagenUrl
            })
            .ToListAsync();
            
        return Results.Ok(new 
        { 
            Productos = productos,
            Total = productos.Count,
            TerminoBusqueda = buscar ?? "todos"
        });    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
        return Results.Problem(
            title: "Error al obtener productos",
            detail: "Ocurrió un error interno del servidor al procesar la solicitud.",
            statusCode: 500
        );
    }
})
.WithName("ObtenerProductos");

// GET /api/productos/{id} - Obtiene un producto específico por ID
app.MapGet("/api/productos/{id:int}", async (TiendaContext context, int id) =>
{
    try
    {
        var producto = await context.Productos
            .Where(p => p.Id == id)
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Stock = p.Stock,
                ImagenUrl = p.ImagenUrl
            })
            .FirstOrDefaultAsync();
            
        if (producto == null)
        {
            return Results.NotFound(new { 
                Mensaje = $"Producto con ID {id} no encontrado",
                ProductoId = id 
            });
        }
        
        return Results.Ok(producto);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener producto {id}: {ex.Message}");
        return Results.Problem(
            title: "Error al obtener producto",
            detail: $"Ocurrió un error al buscar el producto con ID {id}.",
            statusCode: 500
        );    }
})
.WithName("ObtenerProductoPorId");

// === ENDPOINTS DE CARRITO ===

// POST /api/carritos - Crea un nuevo carrito vacío y retorna su ID único
app.MapPost("/api/carritos", (CarritoService carritoService) =>
{
    try
    {
        var carritoId = carritoService.CrearCarrito();
        return Results.Created($"/api/carritos/{carritoId}", new 
        {
            CarritoId = carritoId,
            Mensaje = "Carrito creado exitosamente",
            FechaCreacion = DateTime.Now
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al crear carrito: {ex.Message}");
        return Results.Problem(
            title: "Error al crear carrito",
            detail: "Ocurrió un error interno al crear el carrito de compras.",
            statusCode: 500
        );
    }
})
.WithName("CrearCarrito");

// GET /api/carritos/{carritoId} - Obtiene todos los items del carrito específico
app.MapGet("/api/carritos/{carritoId}", async (CarritoService carritoService, string carritoId) =>
{
    try
    {
        var carrito = await carritoService.ObtenerCarritoAsync(carritoId);
        
        if (carrito == null)
        {
            return Results.NotFound(new 
            {
                Mensaje = $"Carrito con ID {carritoId} no encontrado",
                CarritoId = carritoId
            });
        }
          var carritoDto = carritoService.ConvertirADto(carrito);
        return Results.Ok(carritoDto);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener carrito {carritoId}: {ex.Message}");
        return Results.Problem(
            title: "Error al obtener carrito",
            detail: $"Ocurrió un error al buscar el carrito con ID {carritoId}.",
            statusCode: 500
        );
    }
})
.WithName("ObtenerCarrito");

// DELETE /api/carritos/{carritoId} - Elimina todos los items del carrito
app.MapDelete("/api/carritos/{carritoId}", (CarritoService carritoService, string carritoId) =>
{
    try
    {
        var vaciado = carritoService.VaciarCarrito(carritoId);
        
        if (!vaciado)
        {
            return Results.NotFound(new 
            {
                Mensaje = $"Carrito con ID {carritoId} no encontrado",
                CarritoId = carritoId
            });
        }
        
        return Results.Ok(new 
        {
            Mensaje = "Carrito vaciado exitosamente",
            CarritoId = carritoId,
            FechaVaciado = DateTime.Now        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al vaciar carrito {carritoId}: {ex.Message}");
        return Results.Problem(
            title: "Error al vaciar carrito",
            detail: $"Ocurrió un error al vaciar el carrito con ID {carritoId}.",
            statusCode: 500
        );
    }
})
.WithName("VaciarCarrito");

// PUT /api/carritos/{carritoId}/{productoId} - Agrega/actualiza producto en carrito (Body: { "cantidad": 2 })
app.MapPut("/api/carritos/{carritoId}/{productoId:int}", async (
    CarritoService carritoService, 
    string carritoId, 
    int productoId, 
    ActualizarItemCarritoDto request) =>
{
    try
    {
        if (request.Cantidad <= 0)
        {
            return Results.BadRequest(new 
            {
                Mensaje = "La cantidad debe ser mayor a cero",
                Cantidad = request.Cantidad
            });
        }

        var resultado = await carritoService.AgregarProductoAsync(carritoId, productoId, request.Cantidad);
        
        if (!resultado.Exito)
        {
            return resultado.Mensaje.Contains("no encontrado") ? 
                Results.NotFound(new { Mensaje = resultado.Mensaje, CarritoId = carritoId, ProductoId = productoId }) :
                Results.BadRequest(new { Mensaje = resultado.Mensaje, CarritoId = carritoId, ProductoId = productoId });
        }
        
        return Results.Ok(new 
        {
            Mensaje = "Producto agregado/actualizado exitosamente en el carrito",
            CarritoId = carritoId,
            ProductoId = productoId,
            CantidadFinal = request.Cantidad,
            FechaActualizacion = DateTime.Now        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al agregar producto {productoId} al carrito {carritoId}: {ex.Message}");
        return Results.Problem(
            title: "Error al actualizar carrito",
            detail: $"Ocurrió un error al agregar el producto {productoId} al carrito {carritoId}.",
            statusCode: 500
        );
    }
})
.WithName("AgregarProductoAlCarrito");

// DELETE /api/carritos/{carritoId}/{productoId} - Elimina un producto específico del carrito
app.MapDelete("/api/carritos/{carritoId}/{productoId:int}", async (
    CarritoService carritoService, 
    string carritoId, 
    int productoId) =>
{
    try
    {
        var resultado = await carritoService.EliminarProductoCompletoAsync(carritoId, productoId);
        
        if (!resultado.Exito)
        {
            return Results.NotFound(new 
            { 
                Mensaje = resultado.Mensaje, 
                CarritoId = carritoId, 
                ProductoId = productoId 
            });
        }
        
        return Results.Ok(new 
        {
            Mensaje = "Producto eliminado exitosamente del carrito",
            CarritoId = carritoId,
            ProductoId = productoId,
            FechaEliminacion = DateTime.Now
        });    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al eliminar producto {productoId} del carrito {carritoId}: {ex.Message}");
        return Results.Problem(
            title: "Error al eliminar producto del carrito",
            detail: $"Ocurrió un error al eliminar el producto {productoId} del carrito {carritoId}.",
            statusCode: 500
        );
    }
})
.WithName("EliminarProductoDelCarrito");

// GET /api/carritos/estadisticas - Información general del sistema de carritos (debugging)
app.MapGet("/api/carritos/estadisticas", (CarritoService carritoService) =>
{
    try
    {
        var estadisticas = carritoService.ObtenerEstadisticas();
        return Results.Ok(new 
        {
            Mensaje = "Estadísticas de carritos activos",
            Fecha = DateTime.Now,
            Estadisticas = estadisticas
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener estadísticas: {ex.Message}");
        return Results.Problem(
            title: "Error al obtener estadísticas",
            detail: "Ocurrió un error al generar las estadísticas de carritos.",
            statusCode: 500
        );
    }
})
.WithName("EstadisticasCarritos");

// PUT /api/carritos/{carritoId}/confirmar - Confirma la compra con datos del cliente
/// Cuerpo de la solicitud: { "nombreCliente": "Juan", "apellidoCliente": "Pérez", "emailCliente": "juan@email.com" }
/// </summary>
app.MapPut("/api/carritos/{carritoId}/confirmar", async (
    CarritoService carritoService,
    string carritoId,
    ConfirmarCompraDto datosCliente) =>
{
    try
    {
        var resultado = await carritoService.ConfirmarCompraAsync(carritoId, datosCliente);
          if (!resultado.Exito)
        {
            if (resultado.Mensaje.Contains("no encontrado") || 
                resultado.Mensaje.Contains("carrito vacío"))
            {
                return Results.NotFound(new 
                { 
                    Mensaje = resultado.Mensaje, 
                    CarritoId = carritoId 
                });
            }
            
            return Results.BadRequest(new 
            { 
                Mensaje = resultado.Mensaje, 
                CarritoId = carritoId 
            });
        }
        
        return Results.Ok(resultado.Compra);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al confirmar compra del carrito {carritoId}: {ex.Message}");
        return Results.Problem(
            title: "Error al confirmar compra",
            detail: $"Ocurrió un error interno al procesar la compra del carrito {carritoId}.",
            statusCode: 500
        );
    }
})
.WithName("ConfirmarCompra");

// Endpoint de ejemplo (mantenido para referencia)
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();
