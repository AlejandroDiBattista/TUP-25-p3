#nullable enable

using Microsoft.EntityFrameworkCore;
using servidor.Data;
using modelos_compartidos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Añadir servicios para controladores
builder.Services.AddControllers();
builder.Services.AddRazorPages();


var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiTienda API V1"));
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection(); // Solo usar HTTPS en producción
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseCors("AllowClientApp");

app.UseRouting();

// INICIALIZACIÓN DE LA BASE DE DATOS 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        var dbPath = context.Database.GetDbConnection().DataSource;
        Console.WriteLine($"--> DEBUG: Ruta de la base de datos SQLite: {dbPath}");


        context.Database.EnsureCreated(); 
        Console.WriteLine("--> DEBUG: Base de datos y esquema asegurados.");

      
        if (!context.Productos.Any()) 
        {
            Console.WriteLine("--> DEBUG: La tabla 'Productos' está vacía. Sembrando datos iniciales...");
            context.Productos.AddRange(
                new Producto { Id = 1, Nombre = "Smart TV 4K", Descripcion = "Televisor de 50 pulgadas con resolución 4K y HDR.", Precio = 450000m, Stock = 10, ImagenUrl = "/images/tv.jpg" },
                new Producto { Id = 2, Nombre = "Iphone 16 Pro Max", Descripcion = "Teléfono de última generación.", Precio = 300000m, Stock = 25, ImagenUrl = "/images/telefono.jpg" },
                new Producto { Id = 3, Nombre = "Auriculares JBL", Descripcion = "Auriculares inalámbricos con cancelación de ruido activa.", Precio = 50000m, Stock = 50, ImagenUrl = "/images/auriculares.jpg" },
                new Producto { Id = 4, Nombre = "Teclado Mecánico RGB", Descripcion = "Teclado gaming con switches Cherry MX y retroiluminación RGB personalizable.", Precio = 75000m, Stock = 15, ImagenUrl = "/images/teclado.jpg" },
                new Producto { Id = 5, Nombre = "Mouse Gamer Logitech", Descripcion = "Mouse con sensor óptico de alta precisión y batería de larga duración.", Precio = 30000m, Stock = 20, ImagenUrl = "/images/mouse.jpg" },
                new Producto { Id = 6, Nombre = "Monitor Curvo 27''", Descripcion = "Monitor QHD con tasa de refresco de 144Hz y panel VA curvo.", Precio = 200000m, Stock = 8, ImagenUrl = "/images/monitor.jpg" },
                new Producto { Id = 7, Nombre = "Camara Full HD", Descripcion = "Cámara web con micrófono integrado, ideal para videollamadas y streaming.", Precio = 25000m, Stock = 30, ImagenUrl = "/images/camara.jpg" },
                new Producto { Id = 8, Nombre = "Disco SSD 1TB", Descripcion = "Unidad de estado sólido NVMe PCIe Gen4 para almacenamiento ultrarrápido.", Precio = 90000m, Stock = 12, ImagenUrl = "/images/ssd.jpg" },
                new Producto { Id = 9, Nombre = "Router Wi-Fi", Descripcion = "Router de doble banda con tecnología Wi-Fi 6 para conexiones ultrarrápidas y estables.", Precio = 60000m, Stock = 18, ImagenUrl = "/images/router.jpg" },
                new Producto { Id = 10, Nombre = "Impresora Multifunción", Descripcion = "Impresora, escáner y copiadora con conectividad Wi-Fi y impresión a doble cara.", Precio = 110000m, Stock = 7, ImagenUrl = "/images/impresora.jpg" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("--> DEBUG: Datos iniciales sembrados en la tabla 'Productos'.");
        }
        else
        {
            Console.WriteLine("--> DEBUG: La tabla 'Productos' ya contiene datos. No se sembraron datos.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "--> ERROR CRÍTICO: Fallo en la inicialización de la base de datos o siembra.");
    }
}


// Mapeo del endpoint de productos (Minimal API)
app.MapGet("/productos", async (string? q, ApplicationDbContext db) =>
{
    try
    {
        if (!await db.Database.CanConnectAsync())
        {
            Console.WriteLine("--> ADVERTENCIA: No se puede conectar a la base de datos en el endpoint.");
            return Results.Problem("Error: No se pudo conectar a la base de datos.");
        }
        
        var query = db.Productos.AsQueryable(); 
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p => p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                     p.Descripcion.Contains(q, StringComparison.OrdinalIgnoreCase));
        }
        var productos = await query.ToListAsync();
        Console.WriteLine($"--> DEBUG: API /productos devolviendo {productos.Count} productos.");
        return Results.Ok(productos);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> ERROR: Error en endpoint /productos: {ex.Message}. StackTrace: {ex.StackTrace}");
        return Results.Problem("Error interno del servidor al obtener productos.");
    }
});

// Endpoint para actualizar el stock (existente)
app.MapPost("/stock/update", async (List<StockUpdateDto> updates, ApplicationDbContext db) =>
{
    try
    {
        foreach (var update in updates)
        {
            var producto = await db.Productos.FindAsync(update.ProductoId);
            if (producto != null)
            {
                producto.Stock -= update.CantidadVendida;
                if (producto.Stock < 0)
                {
                    producto.Stock = 0;
                }
            }
        }
        await db.SaveChangesAsync();
        Console.WriteLine("--> DEBUG: Stock actualizado en la base de datos.");
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> ERROR: Error al actualizar stock: {ex.Message}. StackTrace: {ex.StackTrace}");
        return Results.Problem("Error interno al actualizar el stock.");
    }
});


app.MapRazorPages();
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
