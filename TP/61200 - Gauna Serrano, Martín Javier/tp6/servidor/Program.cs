using TiendaApi.Models;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));
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



builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseInMemoryDatabase("TiendaDb")); // O usa UseSqlite si prefieres SQLite






var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });


// servidor/Program.cs

app.MapGet("/productos", async (TiendaDbContext db, string? buscar) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(buscar))
    {
        query = query.Where(p => p.Nombre.Contains(buscar) || p.Descripcion.Contains(buscar));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});


app.Run();
;


