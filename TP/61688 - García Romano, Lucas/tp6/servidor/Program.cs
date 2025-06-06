using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));


builder.Services.AddControllers();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//permite que el cliente pueda acceder a la API
app.UseCors("AllowClientApp");

//Es para que la api reciva peteciones
app.MapGet("/", () => "Servidor API está en funcionamiento");

//Endpoint de prueba que devuelve un mensaje y la fecha actual
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

//Endpoint devuelve los productos desde la base de datos
app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync());

app.Run();