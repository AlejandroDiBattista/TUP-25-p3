using Microsoft.EntityFrameworkCore;
using servidor;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios CORS que ya tenías
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Registrar el DbContext para Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// 3. Usar CORS
app.UseCors("AllowClientApp");

// Por ahora no definimos endpoints, lo haremos en el siguiente paso.

app.Run();