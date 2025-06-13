using Microsoft.EntityFrameworkCore;
using servidor.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5184", "http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar DbContext
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Construir la aplicación
var app = builder.Build();

// Crear la base de datos si no existe
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");
app.UseRouting();

app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint de prueba
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoint de productos
app.MapGet("/api/productos", async (TiendaDbContext db) =>
    await db.Productos.ToListAsync()
);
app.MapPost("/api/compras", async (servidor.Data.TiendaDbContext db, servidor.Models.Compra compra) =>
{
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();