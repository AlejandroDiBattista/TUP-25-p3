using TuApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddSingleton<ProductoService>();
builder.Services.AddControllers();
var app = builder.Build();



// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/productos", () => {
    var servicio = new ProductoService();
    return Results.Ok(servicio.ObtenerProductos());
});

app.Run();
