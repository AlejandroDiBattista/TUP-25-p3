using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using servidor.Data;
using servidor.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Agregar servicio de logging para capturar errores
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// ✅ Agregar soporte para controladores API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Configurar la conexión a la base de datos SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// ✅ Configurar CORS para permitir solicitudes desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Registrar servicios necesarios para la API
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<VentaService>();
builder.Services.AddScoped<CarritoService>();

var app = builder.Build();

// 🛠️ Configuración en modo desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

// 🔥 Aplicar CORS
app.UseCors("AllowClientApp");

// 📌 Habilitar enrutamiento y autorización
app.UseRouting();
app.UseAuthorization();

// 🛠️ Mapear controladores API
app.MapControllers();

// ✅ Ruta base para verificar que el servidor está en funcionamiento
app.MapGet("/", () => "Servidor API está en funcionamiento");

// 🔥 Ruta para obtener productos desde el servicio
app.MapGet("/api/productos", async (ProductoService servicio) =>
{
    try
    {
        var productos = await servicio.ObtenerProductosAsync();
        return Results.Ok(productos);
    }
    catch (Exception ex)
    {
        app.Logger.LogError($"Error en /api/productos: {ex.Message}");
        return Results.Problem("Error al obtener productos.");
    }
});

// ✅ Ruta para confirmar una compra
app.MapPost("/api/comprar", async (VentaService ventaService, servidor.Models.Venta venta) =>
{
    try
    {
        var resultado = await ventaService.RegistrarVentaAsync(venta);
        return resultado ? Results.Ok("Compra confirmada") : Results.Problem("Error al procesar la compra.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError($"Error en /api/comprar: {ex.Message}");
        return Results.Problem("Error al procesar la compra.");
    }
});

// 🔥 Inicializar productos en la base de datos si aún no existen
using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    var productoService = servicios.GetRequiredService<ProductoService>();
    await productoService.AgregarProductosInicialesAsync();
}

// 🚀 Ejecutar la aplicación
app.Run();
