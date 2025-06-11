using Microsoft.EntityFrameworkCore;
using servidor;
using servidor.Models; 
var builder = WebApplication.CreateBuilder(args);

// Configura el DbContext para usar SQLite y define la cadena de conexión.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("DataSource=TiendaOnline.db")); // El archivo de la DB se creará en la carpeta de ejecución

// Habilita CORS para que Blazor (cliente) pueda comunicarse con el backend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            // Permite el acceso desde el origen de tu cliente Blazor.
            // Los puertos 7000 (HTTPS) y 5000 (HTTP) son los puertos por defecto para Blazor WASM.
            policy.WithOrigins("https://localhost:7000", "http://localhost:5000")
                  .AllowAnyHeader() // Permite cualquier encabezado en las solicitudes
                  .AllowAnyMethod(); // Permite cualquier método HTTP (GET, POST, PUT, DELETE, etc.)
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Para ver errores detallados en desarrollo
}

app.UseHttpsRedirection(); // Redirige de HTTP a HTTPS
app.UseCors(); // Usa la política CORS definida

// Aplica migraciones y carga los datos iniciales al iniciar la aplicación.
// Esto asegura que la base de datos se cree y se llene con los productos.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // Aplica cualquier migración pendiente a la base de datos
}

// ----------------------------------------------------------------------
// Definición de los Endpoints de la Minimal API
// ----------------------------------------------------------------------

// GET /productos (+ búsqueda por query)
app.MapGet("/productos", async (string? q, ApplicationDbContext db) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        // Búsqueda insensible a mayúsculas/minúsculas en Nombre y Descripción
        query = query.Where(p => p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                 p.Descripcion.Contains(q, StringComparison.OrdinalIgnoreCase));
    }
    return Results.Ok(await query.ToListAsync());
});

// ----------------------------------------------------------------------

app.Run();