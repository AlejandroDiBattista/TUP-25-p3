using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TiendaDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseCors("AllowClientApp")

app.MapGet("/productos", async (TiendaDb db) ) =>
{
  return await db.Producotos
  .Where(p => p.Stock > 0)  
  .ToListAsync();
}

app.MapPost ("/Carrito", async (TiendaDb db, Producto producto) =>
{
    if (producto.Stock <= 0)
    {
        return Results.BadRequest("Producto sin stock");
    }

    var carrito = new Carrito
    {
        ProductoId = producto.Id,
        Cantidad = 1
    };

    db.Carritos.Add(carrito);
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});