using Microsoft.EntityFrameworkCore;
using servidor.Data;

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
builder.Services.AddControllers();

// ⬅️ MOVER AQUÍ el DbContext (antes de Build)
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Construir la aplicación
var app = builder.Build();

// ⬅️ CREAR la base de datos si no existe
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated(); // Esto crea la DB y carga las gorras
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint de prueba
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();
