using servidor;
using Microsoft.EntityFrameworkCore;
using servidor.Models;



var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar controladores si es necesario
builder.Services.AddControllers();

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.EnsureCreated();

    if (!context.Productos.Any())
    {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "Licuadora", Descripcion = "Licuadora con jarra de vidrio y 3 velocidades", Precio = 18000, Stock = 10, ImagenUrl = "https://i.ibb.co/q3THG727/licuadora.jpg" },
            new Producto { Nombre = "Lavarropas", Descripcion = "Lavarropas automatico", Precio = 200000, Stock = 10, ImagenUrl = "https://i.ibb.co/FbWHv1Bv/lavarropas.png" },
            new Producto { Nombre = "Microondas", Descripcion = "Microondas digital con descongelado rápido", Precio = 70000, Stock = 10, ImagenUrl = "https://i.ibb.co/HpfC9wHZ/microondas.png" },
            new Producto { Nombre = "Cafetera", Descripcion = "Cafetera electrica", Precio = 20000, Stock = 10, ImagenUrl = "https://i.ibb.co/KTxxMm6/cafetera.png" },
            new Producto { Nombre = "Aspiradora", Descripcion = "Aspiradora ciclónica sin bolsa, con filtro HEPA", Precio = 30000, Stock = 10, ImagenUrl = "https://i.ibb.co/rGYrq7Qk/aspiradora.png" },
            new Producto { Nombre = "Plancha", Descripcion = "Plancha a vapor con base de cerámica y rociador", Precio = 12000, Stock = 10, ImagenUrl = "https://i.ibb.co/4gN0xxvt/plancha.jpg" },
            new Producto { Nombre = "Horno electrico", Descripcion = "Horno electrico de 45L", Precio = 50000, Stock = 10, ImagenUrl = "https://i.ibb.co/pB6NK6hN/horno-electrico.jpg" },
            new Producto { Nombre = "Heladera", Descripcion = "Heladera con freezer superior", Precio = 300000, Stock = 10, ImagenUrl = "https://i.ibb.co/8gVxMsbw/heladera.png" },
            new Producto { Nombre = "LED TV", Descripcion = "Smart TV LED de 43” con resolución Full HD y WiFi", Precio = 250000, Stock = 10, ImagenUrl = "https://i.ibb.co/GBGtgJ4/led-tv.jpg" },
            new Producto { Nombre = "Tostadora", Descripcion = "Tostadora doble ranura con selector de temperatura", Precio = 15000, Stock = 10, ImagenUrl = "https://i.ibb.co/Q30M8XX8/tostadora.jpg" },
        };

        context.Productos.AddRange(productos);
        context.SaveChanges();
    }
}

app.MapGet("/productos", async (AppDbContext db, string? search) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p => p.Nombre.Contains(search));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/api/carrito", async (AppDbContext db, Carrito item) =>
{
    db.Carritos.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/carrito/{item.Id}", item);
});

app.Run();