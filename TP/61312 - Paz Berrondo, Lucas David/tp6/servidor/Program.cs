using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Services;
using servidor.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

/// <summary>
/// Endpoint para obtener todos los productos o buscar por nombre.
/// GET /api/productos - Obtiene todos los productos
/// GET /api/productos?buscar=iphone - Busca productos que contengan "iphone" en el nombre
/// </summary>
app.MapGet("/api/productos", async (TiendaContext context, string? buscar) =>
{
    try
    {
        // Consulta base de productos
        var query = context.Productos.AsQueryable();
        
        // Si se proporciona un término de búsqueda, filtrar por nombre
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()));
        }
        
        // Ejecutar consulta y mapear a DTOs
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
        });
    }
    catch (Exception ex)
    {
        // Log del error para debugging
        Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
        
        return Results.Problem(
            title: "Error al obtener productos",
            detail: "Ocurrió un error interno del servidor al procesar la solicitud.",
            statusCode: 500
        );
    }
})
.WithName("ObtenerProductos")
.WithSummary("Obtiene todos los productos o busca por nombre")
.WithDescription("Endpoint para listar productos. Permite búsqueda opcional por nombre usando el parámetro 'buscar'.");

/// <summary>
/// Endpoint para obtener un producto específico por ID.
/// GET /api/productos/1 - Obtiene el producto con ID 1
/// </summary>
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
        );
    }
})
.WithName("ObtenerProductoPorId")
.WithSummary("Obtiene un producto específico por ID")
.WithDescription("Endpoint para obtener los detalles completos de un producto usando su ID único.");

// Ejemplo de endpoint de API (se reemplazará con endpoints reales)
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();
