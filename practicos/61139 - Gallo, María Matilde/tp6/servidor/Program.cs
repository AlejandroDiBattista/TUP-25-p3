using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Endpoints;
using Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con SQLite
builder.Services.AddDbContext<TiendaContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger para documentación de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para permitir llamadas desde Blazor WebAssembly (localhost:5177)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Usar CORS
app.UseCors("AllowClientApp");

// Activar Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Crear base de datos si no existe y completar productos faltantes
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();

    // Lista completa de productos esperados
    var productosEsperados = new List<Producto>
    {
        new Producto { Id = 1, Nombre = "Zapatillas Adidas Gazelle Bold", Descripcion = "Diseño rosa con verde. Plataforma triple y gamuza supersuave para un estilo clásico renovado.", Precio = 150000, Stock = 50, ImagenUrl = "zapa1.jpg.webp" },
        new Producto { Id = 2, Nombre = "Zapatillas Adidas London", Descripcion = "Verde y rosa vibrantes. Las adidas London Green Pink están inspiradas en la colaboración Gucci x Gazelle.", Precio = 250000, Stock = 30, ImagenUrl = "zapa2.jpg.webp" },
        new Producto { Id = 3, Nombre = "Zapatillas Puma Palermo", Descripcion = "Modelo clásico de los 80. Rosa vibrante con esencia futbolera británica.", Precio = 185000, Stock = 30, ImagenUrl = "zapa3.jpg.webp" },
        new Producto { Id = 4, Nombre = "Zapatillas Nike Air Max", Descripcion = "Comodidad y estilo para correr con tecnología Air Max.", Precio = 200000, Stock = 40, ImagenUrl = "zapa4.jpg.webp" },
        new Producto { Id = 5, Nombre = "Zapatilla Dc", Descripcion = "Diseño retro con comodidad moderna.", Precio = 180000, Stock = 35, ImagenUrl = "zapa5.jpg.webp" },
        new Producto { Id = 6, Nombre = "Zapatillas New Balance 574", Descripcion = "Estilo clásico y comodidad todo el día.", Precio = 220000, Stock = 45, ImagenUrl = "zapa6.jpg.jpg" },
        new Producto { Id = 7, Nombre = "Zapatillas Nike Dunk Low", Descripcion = "Perfectas para correr largas distancias con soporte extra.", Precio = 270000, Stock = 25, ImagenUrl = "zapa7.jpg.webp" },
        new Producto { Id = 8, Nombre = "Zapatillas Converse Chuck Taylor", Descripcion = "Clásicas y atemporales, con estilo urbano.", Precio = 140000, Stock = 50, ImagenUrl = "zapa8.jpg.webp" },
        new Producto { Id = 9, Nombre = "Zapatillas Vans Old Skool", Descripcion = "Diseño icónico para uso casual y skateboarding.", Precio = 160000, Stock = 40, ImagenUrl = "zapa9.jpg.webp" },
        new Producto { Id = 10, Nombre = "Zapatillas Saucony Jazz", Descripcion = "Comodidad y estilo para uso diario y entrenamiento.", Precio = 190000, Stock = 30, ImagenUrl = "zapa10.jpg.jpg" }
    };

    // Verificar cuáles faltan y agregarlos
    foreach (var producto in productosEsperados)
    {
        if (!db.Productos.Any(p => p.Id == producto.Id))
        {
            db.Productos.Add(producto);
        }
    }

    db.SaveChanges();

    //  función para corregir URLs
    CorregirUrlsImagenes(db);
}

// Función para corregir URLs de imágenes en la base
void CorregirUrlsImagenes(TiendaContext db)
{
    bool cambios = false;

    var productos = db.Productos.ToList();

    foreach (var p in productos)
    {
        if (p.ImagenUrl == "img/zapa1.jpg")
        {
            p.ImagenUrl = "zapa1.jpg.webp";
            cambios = true;
        }
        if (p.ImagenUrl == "img/zapa2.jpg")
        {
            p.ImagenUrl = "zapa2.jpg.webp";
            cambios = true;
        }
        if (p.ImagenUrl == "img/zapa3.jpg")
        {
            p.ImagenUrl = "zapa3.jpg.webp";
            cambios = true;
        }
        if (p.ImagenUrl == "zapa6.jpg.webp")
        {
            p.ImagenUrl = "zapa6.jpg.jpg";
            cambios = true;
        }
    }

    if (cambios)
    {
        db.SaveChanges();
        Console.WriteLine("URLs de imágenes corregidas.");
    }
}

// Mapear endpoints personalizados
app.MapProductos();
app.MapCarrito();
app.MapCompras();

// Endpoint raíz para verificar que el servidor está corriendo
app.MapGet("/", () => "Servidor API está funcionando");

app.Run();
