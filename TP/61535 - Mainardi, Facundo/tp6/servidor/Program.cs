using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servidor.Modelos;
using Servidor.Stock;

var builder = WebApplication.CreateBuilder(args);

// 💡 Todos los servicios deben registrarse antes de Build()

// Agregar DbContext
builder.Services.AddDbContext<Tienda>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Registrar tienda como servicio
builder.Services.AddScoped<Tienda>();

// Agregar servicios CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores
builder.Services.AddControllers();

// Construir la app
var app = builder.Build();

// Usar el contexto para asegurarse de que la DB exista
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Tienda>();
    db.Database.EnsureCreated();
}

// Configurar middleware
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowClientApp");

app.MapControllers();


app.MapGet("/productos", async (Tienda db, string? q) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
    {
        query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});
app.MapGet("/", () => "Servidor API está en funcionamiento");
app.Run();