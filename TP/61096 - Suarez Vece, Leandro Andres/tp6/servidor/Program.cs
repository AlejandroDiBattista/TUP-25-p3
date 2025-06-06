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

app.MapGet("/carrito/{id}", async (int id, IPruductServices servicio) =>
{
    var productos = await servicio.GetPorductsCarrito(id);
    return Results.Ok(productos);
});

app.MapPost("/carrito", async (IPruductServices servicio) =>
{
    var compra = new CompraDto();
    await servicio.CarritoInit(compra);
    return Results.Ok();
});

app.MapPut("/carrito/{id}", async (int id, ItemCompraDto dto, IPruductServices servicio) =>
{
    await servicio.ActualizarCarrito(id, dto);
    return Results.Ok();
});

app.MapPut("/carrito/{id}/confirmar", async (int id, ConfirmarCompraDto dto, IPruductServices servicio) =>
{
    await servicio.ConfirmarCompra(id, dto);
    return Results.Ok();
});


app.MapDelete("/carrito/{id}", async (int id, IPruductServices servicio) =>
{
    await servicio.ElimnarCarrito(id);
    return Results.Ok();
});

app.MapDelete("/carrito/{id}/{id2}", async (int id, int id2, IPruductServices servicio) =>
{
    await servicio.ElimnarPorudctoCarrito(id, id2);
    return Results.Ok();
});

app.Run();
