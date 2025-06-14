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

// Registrar ProductoService como singleton
builder.Services.AddSingleton<ProductoService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint que usa la instancia inyectada de ProductoService
app.MapGet("/api/productos", (ProductoService servicio) => {
    return Results.Ok(servicio.ObtenerProductos());
});

app.Run();
