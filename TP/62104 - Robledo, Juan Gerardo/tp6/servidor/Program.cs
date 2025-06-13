using Microsoft.EntityFrameworkCore;
using servidor.data;
using servidor.models;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework para guardar tienda.db
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5247")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Crear la base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    context.Database.EnsureCreated();
}


// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();
