using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using Microsoft.AspNetCore.Mvc;
using Compartido.Dtos;
using Compartido.Models;

using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7295") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Build
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    context.Database.Migrate();

    if (!context.Productos.Any())
    {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "iPhone 15", Descripcion = "Smartphone Apple", Precio = 1200, Stock = 10, ImagenUrl = "iphone.jpg" },
            new Producto { Nombre = "Samsung Galaxy S23", Descripcion = "Smartphone Samsung", Precio = 1100, Stock = 15, ImagenUrl = "samsung.jpg" },
            new Producto { Nombre = "Xiaomi Redmi Note 12", Descripcion = "Smartphone Xiaomi", Precio = 500, Stock = 20, ImagenUrl = "xiaomi.jpg" }
        };

        context.Productos.AddRange(productos);
        context.SaveChanges();
    }
}

// ✅ Agregamos el endpoint que faltaba:
app.MapGet("/productos", async (TiendaContext db) =>
{
    var productos = await db.Productos.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/compras", async (CompraDto compraDto, TiendaContext db) =>
{
    foreach (var item in compraDto.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto is null) return Results.NotFound($"Producto {item.ProductoId} no encontrado");

        if (producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {producto.Nombre}");

        producto.Stock -= item.Cantidad;
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});


app.UseCors("AllowBlazorClient");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();


app.Run("http://localhost:5000");
