using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar SQLite
builder.Services.AddDbContext<TiendaContext>(options => {
    options.UseSqlite("Data Source=tienda.db");
});

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

app.UseDefaultFiles(); // opcional, sirve index.html por defecto
app.UseStaticFiles();  // habilita servir contenido de wwwroot


// Asegurar que la base de datos está creada y sembrada con datos iniciales
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TiendaContext>();
        ProductosSeed.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint para obtener productos con filtro opcional por nombre
app.MapGet("/api/productos", async (string? busqueda, TiendaContext db) =>
{
    var query = db.Productos.AsQueryable();// Crear una consulta base

    // Si se proporciona un término de búsqueda, filtrar por nombre

    if (!string.IsNullOrWhiteSpace(busqueda))// Verificar si la búsqueda no es nula o vacía
    {
        busqueda = busqueda.ToLower();// Convertir a minúsculas para comparación
        // Aplicar filtro de búsqueda
        query = query.Where(p => p.Nombre.ToLower().StartsWith(busqueda));
        // Filtrar por nombre que comience con el término de búsqueda

    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});


// Endpoint para registrar una nueva compra
app.MapPost("/api/compras", async (CompraDTO compraDTO, TiendaContext db) =>
{
    using var transaction = await db.Database.BeginTransactionAsync();
    try
    {
        // Validar datos del cliente
        if (string.IsNullOrEmpty(compraDTO.NombreCliente) ||
            string.IsNullOrEmpty(compraDTO.ApellidoCliente) ||
            string.IsNullOrEmpty(compraDTO.EmailCliente))
        {
            return Results.BadRequest("Los datos del cliente son obligatorios");
        }

        // Validar que haya items
        if (!compraDTO.Items.Any())
        {
            return Results.BadRequest("El carrito está vacío");
        }

        // Obtener y validar productos
        var productIds = compraDTO.Items.Select(i => i.ProductoId).ToList();
        var productos = await db.Productos
            .Where(p => productIds.Contains(p.Id))// Filtrar solo los productos que están en el carrito
            .ToDictionaryAsync(p => p.Id);

        // Validar que existan todos los productos
        if (productos.Count != productIds.Count)
        {
            return Results.BadRequest("Algunos productos no existen");
        }

        // Validar stock y calcular total
        decimal total = 0;
        foreach (var item in compraDTO.Items)
        {
            var producto = productos[item.ProductoId];
            if (producto.Stock < item.Cantidad)
            {
                return Results.BadRequest($"Stock insuficiente para {producto.Nombre}");
            }
            total += producto.Precio * item.Cantidad;
        }

        // Crear la compra
        var compra = new Compra
        {
            Fecha = DateTime.UtcNow,
            NombreCliente = compraDTO.NombreCliente,
            ApellidoCliente = compraDTO.ApellidoCliente,
            EmailCliente = compraDTO.EmailCliente,
            Total = total
        };

        // Agregar items y actualizar stock
        foreach (var item in compraDTO.Items)
        {
            var producto = productos[item.ProductoId];
            
            compra.Items.Add(new ItemCompra
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });

            // Actualizar stock
            producto.Stock -= item.Cantidad;
        }

        // Guardar cambios
        db.Compras.Add(compra);
        await db.SaveChangesAsync();

        // Confirmar transacción
        await transaction.CommitAsync();

        return Results.Ok(new { 
            Id = compra.Id,
            Total = compra.Total,
            Fecha = compra.Fecha
        });
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        return Results.StatusCode(500);
    }
});


app.Run();
