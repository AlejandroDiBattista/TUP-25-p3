using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.DTOs;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Inicializar base de datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    context.Database.EnsureCreated();
    
    if (!context.Productos.Any())
    {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "NVIDIA RTX 4080 Super", Descripcion = "Tarjeta gráfica de alto rendimiento para gaming y diseño profesional. 16GB GDDR6X, Ray Tracing de 3ra generación, DLSS 3.0", Precio = 1199999.99M, Stock = 8, ImagenUrl = "/images/rtx4080.png" },
            new Producto { Nombre = "AMD Ryzen 7 7800X3D", Descripcion = "Procesador gaming de 8 núcleos y 16 hilos con tecnología 3D V-Cache. Ideal para gaming en alta resolución", Precio = 549999.99M, Stock = 12, ImagenUrl = "/images/ryzen7-9800x3d.png" },
            new Producto { Nombre = "Corsair Vengeance DDR5-6000", Descripcion = "Memoria RAM 32GB (2x16GB) DDR5-6000 MHz CL30. Optimizada para AMD EXPO y Intel XMP 3.0", Precio = 299999.99M, Stock = 15, ImagenUrl = "/images/gskill-ddr5.webp" },
            new Producto { Nombre = "Samsung 980 PRO NVMe SSD", Descripcion = "SSD NVMe M.2 1TB PCIe 4.0 con velocidades de hasta 7,000 MB/s. Incluye disipador térmico", Precio = 149999.99M, Stock = 20, ImagenUrl = "/images/samsung-980pro.png" },
            new Producto { Nombre = "MSI MAG B650 Tomahawk WiFi", Descripcion = "Motherboard ATX para AMD AM5. PCIe 5.0, WiFi 6E, DDR5-6000+, RGB Mystic Light", Precio = 219999.99M, Stock = 10, ImagenUrl = "/images/asus-z790.png" },
            new Producto { Nombre = "Corsair RM850x Gold", Descripcion = "Fuente de poder modular 850W 80+ Gold. Completamente modular, ventilador de 135mm con control de temperatura", Precio = 169999.99M, Stock = 18, ImagenUrl = "/images/corsair-rm1000x.png" },
            new Producto { Nombre = "NZXT H7 Flow RGB", Descripcion = "Gabinete ATX mid-tower con panel frontal de malla, 3 ventiladores RGB incluidos y panel lateral de vidrio templado", Precio = 129999.99M, Stock = 14, ImagenUrl = "/images/h7-flow.webp" },
            new Producto { Nombre = "Cooler Master Hyper 212 RGB", Descripcion = "Cooler de CPU con ventilador RGB de 120mm. Compatible con Intel LGA 1700 y AMD AM5", Precio = 45999.99M, Stock = 25, ImagenUrl = "/images/nzxt-kraken-x73.png" },
            new Producto { Nombre = "Logitech G Pro X Superlight", Descripcion = "Mouse gaming inalámbrico ultraliviano de 63g con sensor HERO 25K. Hasta 70 horas de batería", Precio = 109999.99M, Stock = 22, ImagenUrl = "/images/pro-x-superlight-black.png" },
            new Producto { Nombre = "Razer BlackWidow V4 Pro", Descripcion = "Teclado mecánico gaming con switches Green, RGB Chroma, dial de comandos y reposamuñecas magnético", Precio = 219999.99M, Stock = 16, ImagenUrl = "/images/razer-v4-pro.png" }
        };        context.Productos.AddRange(productos);
        context.SaveChanges();
        Console.WriteLine("✅ Base de datos inicializada con productos de gaming.");
    }
}

app.UseCors("AllowClientApp");
app.MapGet("/", () => "Servidor API de Tienda Online está en funcionamiento");

// ENDPOINTS DE PRODUCTOS

// GET /api/productos - Obtiene productos con búsqueda opcional
app.MapGet("/api/productos", async (TiendaContext context, string buscar = null) =>
{
    try
    {
        var query = context.Productos.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()));
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
            
        return Results.Ok(new 
        { 
            Productos = productos,
            Total = productos.Count,
            TerminoBusqueda = buscar ?? "todos"
        });
    }    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);
    }
});

// GET /api/productos/{id} - Obtiene un producto por ID
app.MapGet("/api/productos/{id:int}", async (TiendaContext context, int id) =>
{
    try
    {
        var producto = await context.Productos.FindAsync(id);
        if (producto == null)
        {
            return Results.NotFound(new { Mensaje = "Producto no encontrado" });
        }

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
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener producto {id}: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// GET /api/productos/{id}/stock-disponible/{carritoId} - Stock disponible considerando carrito
app.MapGet("/api/productos/{id:int}/stock-disponible/{carritoId:int}", async (TiendaContext context, int id, int carritoId) =>
{
    try
    {
        var producto = await context.Productos.FindAsync(id);
        if (producto == null)
        {
            return Results.NotFound(new { Mensaje = "Producto no encontrado" });
        }        var cantidadEnCarrito = await context.ItemsCarrito
            .Where(i => i.CarritoId == carritoId && i.ProductoId == id)
            .Select(i => i.Cantidad)
            .FirstOrDefaultAsync();

        var stockDisponible = producto.Stock - cantidadEnCarrito;

        return Results.Ok(new { 
            ProductoId = id,
            NombreProducto = producto.Nombre,
            StockTotal = producto.Stock,
            CantidadEnCarrito = cantidadEnCarrito,
            StockDisponible = Math.Max(0, stockDisponible)
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener stock disponible: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// GET /api/productos/stock/{productoId} - Stock de un producto
app.MapGet("/api/productos/stock/{productoId:int}", async (TiendaContext context, int productoId) =>
{
    try
    {
        var producto = await context.Productos.FindAsync(productoId);
        if (producto == null)
        {
            return Results.NotFound(new { Mensaje = "Producto no encontrado" });
        }

        return Results.Ok(new { Stock = producto.Stock });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener stock: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// ENDPOINTS DE CARRITO

// POST /api/carritos - Crear carrito
app.MapPost("/api/carritos", async (TiendaContext context) =>
{
    try
    {
        var carrito = new Carrito();
        context.Carritos.Add(carrito);
        await context.SaveChangesAsync();
        
        return Results.Created($"/api/carritos/{carrito.Id}", new 
        {
            CarritoId = carrito.Id.ToString(),
            Mensaje = "Carrito creado exitosamente"
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al crear carrito: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// GET /api/carritos/{carritoId} - Obtener carrito
app.MapGet("/api/carritos/{carritoId:int}", async (TiendaContext context, int carritoId) =>
{
    try
    {
        var carrito = await context.Carritos
            .Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(c => c.Id == carritoId);
        
        if (carrito == null)
        {
            return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
        }

        var carritoDto = new CarritoDto
        {
            Id = carrito.Id.ToString(),
            Items = carrito.Items.Select(i => new ItemCarritoDto
            {
                ProductoId = i.ProductoId,
                NombreProducto = i.Producto.Nombre,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.Producto.Precio,
                Subtotal = i.Cantidad * i.Producto.Precio,
                ImagenUrl = i.Producto.ImagenUrl
            }).ToList(),
            Total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio),
            TotalItems = carrito.Items.Sum(i => i.Cantidad)
        };

        return Results.Ok(carritoDto);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener carrito: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// PUT /api/carritos/{carritoId}/productos/{productoId} - Agregar producto al carrito
app.MapPut("/api/carritos/{carritoId:int}/productos/{productoId:int}", async (TiendaContext context, int carritoId, int productoId, int cantidad) =>
{
    try
    {
        var carrito = await context.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == carritoId);
        
        if (carrito == null)
        {
            return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
        }

        var producto = await context.Productos.FindAsync(productoId);
        if (producto == null)
        {
            return Results.NotFound(new { Mensaje = "Producto no encontrado" });
        }        if (cantidad <= 0)
        {
            return Results.BadRequest(new { Mensaje = "La cantidad debe ser mayor a 0" });
        }        var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        var cantidadActualEnCarrito = itemExistente?.Cantidad ?? 0;
        var nuevaCantidadTotal = cantidadActualEnCarrito + cantidad;

        if (producto.Stock < nuevaCantidadTotal)
        {
            var stockDisponible = producto.Stock - cantidadActualEnCarrito;
            return Results.BadRequest(new { 
                Mensaje = $"Stock insuficiente. Disponible para agregar: {stockDisponible}, solicitado: {cantidad}" 
            });
        }

        if (itemExistente != null)
        {
            itemExistente.Cantidad = nuevaCantidadTotal; // Lógica acumulativa
        }
        else
        {
            var nuevoItem = new ItemCarrito
            {
                CarritoId = carritoId,
                ProductoId = productoId,
                Cantidad = cantidad
            };
            context.ItemsCarrito.Add(nuevoItem);
        }

        await context.SaveChangesAsync();
        return Results.Ok(new { Mensaje = "Producto agregado exitosamente" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al agregar producto: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// DELETE /api/carritos/{carritoId}/productos/{productoId} - Eliminar producto del carrito
app.MapDelete("/api/carritos/{carritoId:int}/productos/{productoId:int}", async (TiendaContext context, int carritoId, int productoId) =>
{
    try
    {
        var item = await context.ItemsCarrito
            .FirstOrDefaultAsync(i => i.CarritoId == carritoId && i.ProductoId == productoId);
        
        if (item == null)
        {
            return Results.NotFound(new { Mensaje = "Producto no encontrado en el carrito" });
        }

        context.ItemsCarrito.Remove(item);
        await context.SaveChangesAsync();
        
        return Results.Ok(new { Mensaje = "Producto eliminado del carrito" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al eliminar producto: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// DELETE /api/carritos/{carritoId} - Vaciar carrito
app.MapDelete("/api/carritos/{carritoId:int}", async (TiendaContext context, int carritoId) =>
{
    try
    {
        var items = await context.ItemsCarrito
            .Where(i => i.CarritoId == carritoId)
            .ToListAsync();
        
        context.ItemsCarrito.RemoveRange(items);
        await context.SaveChangesAsync();
        
        return Results.Ok(new { Mensaje = "Carrito vaciado exitosamente" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al vaciar carrito: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);    }
});

// POST /api/carritos/{carritoId}/confirmar - Confirmar compra
app.MapPost("/api/carritos/{carritoId:int}/confirmar", async (TiendaContext context, int carritoId, ConfirmarCompraDto datosCliente) =>
{
    try
    {
        var carrito = await context.Carritos
            .Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(c => c.Id == carritoId);
        
        if (carrito == null || !carrito.Items.Any())
        {            return Results.BadRequest(new { Mensaje = "Carrito vacío o no encontrado" });
        }

        if (string.IsNullOrWhiteSpace(datosCliente.NombreCliente) ||
            string.IsNullOrWhiteSpace(datosCliente.ApellidoCliente) ||
            string.IsNullOrWhiteSpace(datosCliente.EmailCliente))
        {
            return Results.BadRequest(new { Mensaje = "Todos los datos del cliente son obligatorios" });
        }

        foreach (var item in carrito.Items)
        {
            if (item.Producto.Stock < item.Cantidad)
            {
                return Results.BadRequest(new { Mensaje = $"Stock insuficiente para {item.Producto.Nombre}" });
            }
        }        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var compra = new Compra
            {
                Fecha = DateTime.Now,
                NombreCliente = datosCliente.NombreCliente,
                ApellidoCliente = datosCliente.ApellidoCliente,
                EmailCliente = datosCliente.EmailCliente,
                Items = carrito.Items.Select(i => new ItemCompra
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.Producto.Precio
                }).ToList()
            };

            compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
            context.Compras.Add(compra);

            foreach (var item in carrito.Items)
            {
                item.Producto.Stock -= item.Cantidad;
            }

            context.ItemsCarrito.RemoveRange(carrito.Items);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Results.Ok(new 
            { 
                CompraId = compra.Id,
                Total = compra.Total,
                Mensaje = "Compra confirmada exitosamente"
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al confirmar compra: {ex.Message}");
        return Results.Problem("Error interno del servidor", statusCode: 500);
    }
});

app.Run();
