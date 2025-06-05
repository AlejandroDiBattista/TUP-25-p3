using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MYContext;
using Services;
using Servidor.Dto;


var builder = WebApplication.CreateBuilder(args);

// agregar servicios : Instalar EF Core y SQLite
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=./tienda-onlone.db"));
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

//inyeccion de dependencias
builder.Services.AddScoped<IPruductServices, ProductService>();


// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });


app.MapGet("/productos", async (string? busqueda, IPruductServices servicio) =>
{
    var productos = await servicio.GetPorducts(busqueda);
    return Results.Ok(productos);
});

app.MapPost("/carrito", async (CarritoDto carrito, IPruductServices servicio) =>
{
    await servicio.CarritoInit(carrito);
    return Results.Ok();
});

app.Run();
