using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using servidor.Data;
using servidor.Services;
using System.Text.Json.Serialization; // ⚠️ Agregá este using

var builder = WebApplication.CreateBuilder(args);

// ✅ Configurar la conexión a la base de datos SQLite
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tienda.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// ✅ Agregar soporte para controladores API + evitar ciclos JSON 🔁
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddEndpointsApiExplorer();

// 🔥 Agregar servicio de logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// ✅ Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Registrar servicios de aplicación
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

app.UseCors("AllowClientApp");

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Servidor API está en funcionamiento");

// ✅ Inicializar productos si es necesario
async Task InicializarProductos()
{
    using var scope = app.Services.CreateScope();
    var servicios = scope.ServiceProvider;
    var productoService = servicios.GetRequiredService<ProductoService>();

    try
    {
        await productoService.AgregarProductosInicialesAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError($"Error al inicializar productos: {ex.Message}");
    }
}

await InicializarProductos();
app.Run();
