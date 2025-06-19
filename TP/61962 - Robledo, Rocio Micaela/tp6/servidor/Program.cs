using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// ======== CONFIGURACIÓN DE SERVICIOS ========

// Configurar CORS para permitir llamadas desde el cliente Blazor (frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5119") // URL del frontend Blazor WebAssembly
            .AllowAnyHeader()                     // Permite cualquier encabezado
            .AllowAnyMethod()                     // Permite cualquier método HTTP (GET, POST, PUT, DELETE)
    );
});

// Registrar el contexto de base de datos usando SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db")); // Nombre del archivo de la base de datos SQLite

// Registrar herramientas para documentar la API con Swagger
builder.Services.AddEndpointsApiExplorer(); // Explora los endpoints disponibles
builder.Services.AddSwaggerGen();           // Genera documentación Swagger para probar la API

// Agrega soporte para los controladores (como CarritosController, ProductosController)
builder.Services.AddControllers();

var app = builder.Build();

// ======== CONFIGURACIÓN DE MIDDLEWARES ========

if (app.Environment.IsDevelopment())
{
    // Habilita Swagger UI sólo en entorno de desarrollo
    app.UseSwagger();        // Genera JSON con documentación
    app.UseSwaggerUI();      // Interfaz visual para probar la API desde el navegador
}

app.UseHttpsRedirection();   // Redirige automáticamente de HTTP a HTTPS
app.UseCors("AllowFrontend"); // Aplica la política de CORS configurada arriba
app.UseStaticFiles();         // Habilita servir archivos estáticos (como imágenes)

app.MapControllers();         // Habilita las rutas definidas por los controladores ([Route] en cada controller)

app.Run(); // Inicia la aplicación
