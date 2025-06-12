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
            new Producto { Nombre = "Auriculares Bluetooth", Descripcion = "Auriculares inalámbricos con micrófono", Precio = 12999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Mouse Gamer", Descripcion = "Mouse óptico RGB con 6 botones", Precio = 8999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Teclado Mecánico", Descripcion = "Teclado mecánico con retroiluminación LED", Precio = 15999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Monitor 24''", Descripcion = "Monitor LED Full HD", Precio = 54999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Smartwatch", Descripcion = "Reloj inteligente con seguimiento de actividad", Precio = 21999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Tablet 10''", Descripcion = "Tablet con pantalla HD y Android", Precio = 44999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Cable USB-C", Descripcion = "Cable de carga rápida 1m", Precio = 1999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Disco SSD 500GB", Descripcion = "Disco de estado sólido SATA 2.5''", Precio = 29999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Parlante Bluetooth", Descripcion = "Altavoz portátil resistente al agua", Precio = 11999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Webcam Full HD", Descripcion = "Cámara web para videollamadas", Precio = 9999, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
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