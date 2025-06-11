using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;
using Servidor.Endpoints; 

using Servidor.Data; 

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controllers si querés usar [ApiController]
builder.Services.AddControllers();

// Configurar EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Asegurarse de que la base de datos se cree automáticamente si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated(); // 👈 CREA la base si no existe
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");
app.MapProductoEndpoints(); // Asegúrate de que este método esté definido en tu clase ProductoApi

app.MapControllers(); // si usás controladores
app.MapGet("/", () => "Servidor API está en funcionamiento");

app.Run();