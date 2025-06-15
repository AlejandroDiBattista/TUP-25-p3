using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar la conexión a la base de datos SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configurar CORS para permitir solicitudes desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrar ProductoService para la inyección de dependencias
builder.Services.AddScoped<ProductoService>();

// Agregar soporte para controladores API
builder.Services.AddControllers();

var app = builder.Build();

// Configuración en modo desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Aplicar CORS
app.UseCors("AllowClientApp");

// Mapear controladores API
app.MapControllers();

// Ruta base para verificar que el servidor está corriendo
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ruta para obtener productos desde el servicio
app.MapGet("/api/productos", async (ProductoService servicio) =>
{
    return Results.Ok(await servicio.ObtenerProductosAsync());
});

// Inicializar productos en la base de datos si aún no existen
using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    var productoService = servicios.GetRequiredService<ProductoService>();
    await productoService.AgregarProductosInicialesAsync();
}

// Ejecutar la aplicación
app.Run();
