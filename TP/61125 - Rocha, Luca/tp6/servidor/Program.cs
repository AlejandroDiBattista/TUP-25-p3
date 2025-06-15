using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Modelos;
using System;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Habilitar CORS para Blazor y otros clientes externos
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 🔹 Configurar EF Core con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// 🔹 Configurar servicios de API
builder.Services.AddControllers();

// 🔹 Habilitar Swagger para probar endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔥 Activar Swagger y página de errores en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// ✅ Activar CORS correctamente
app.UseCors("AllowClientApp");

// 🔹 Habilitar archivos estáticos para imágenes en `wwwroot/images/`
app.UseStaticFiles();

// 🔹 Habilitar controladores para que los endpoints funcionen con `ProductosController`
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();