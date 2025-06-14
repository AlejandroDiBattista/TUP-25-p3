using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.models;
#nullable enable

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaDb>(options =>
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

var app = builder.Build();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new List<Producto> {
            new Producto { Nombre = "Auriculares", Descripcion = "Ares H120 RGB", Precio = 120000, Stock = 10, ImagenUrl = "http://localhost:5184/img/ARES-H120-RGB.png" },
            new Producto { Nombre = "Auriculares", Descripcion = "Pandora H350", Precio = 110000, Stock = 20, ImagenUrl = "http://localhost:5184/img/PANDORA-H350.png" },
            new Producto { Nombre = "Auriculares", Descripcion = "Zeus x Wireless H510", Precio = 250000, Stock = 6, ImagenUrl = "http://localhost:5184/img/ZEUS-X-WIRELESS-H510.png" },
            new Producto { Nombre = "Mouse", Descripcion = "Storm Elite", Precio = 20500, Stock = 30, ImagenUrl = "http://localhost:5184/img/STORM-ELITE.png" },
            new Producto { Nombre = "Mouse", Descripcion = "Invader M719", Precio = 16000, Stock = 14, ImagenUrl = "http://localhost:5184/img/INVADER-M719.png" },
            new Producto { Nombre = "Mouse", Descripcion = "Impact M908", Precio = 15000, Stock = 8, ImagenUrl = "http://localhost:5184/img/IMPACT-M908.png" },
            new Producto { Nombre = "Teclado", Descripcion = "Kumara K522", Precio = 10000, Stock = 24, ImagenUrl = "http://localhost:5184/img/KUMARA-K552.png" },
            new Producto { Nombre = "Teclado", Descripcion = "Deimos K599", Precio = 19000, Stock = 12, ImagenUrl = "http://localhost:5184/img/DEIMOS-K599.png" },
            new Producto { Nombre = "Teclado", Descripcion = "Ziggs K669", Precio = 20000, Stock = 8, ImagenUrl = "http://localhost:5184/img/ZIGGS-K669.png" },
            new Producto { Nombre = "Teclado", Descripcion = "DRAGONBORN K630", Precio = 25000, Stock = 16, ImagenUrl = "http://localhost:5184/img/DRAGONBORN-K630.png" }
        });
        db.SaveChanges();
    }
}

app.MapGet("/productos", async ([FromServices] TiendaDb db, [FromQuery] string? busqueda) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        var filtro = busqueda.Trim().ToLower();
        query = query.Where(p =>
            p.Nombre.ToLower().Contains(filtro) ||
            p.Descripcion.ToLower().Contains(filtro));
    }

    return await query.ToListAsync();
});

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();
