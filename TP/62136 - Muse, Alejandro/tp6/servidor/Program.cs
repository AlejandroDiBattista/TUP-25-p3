using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

app.UseStaticFiles(); // 

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Cargar productos 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "ASUS Zenfone 12 Ultra", Descripcion = "Celular gama alta, 512GB, 12GB RAM", Precio = 950000, Stock = 10, ImagenUrl = "/celulares/ASUS Zenfone 12 Ultra.png" },
            new Producto { Nombre = "Samsung Galaxy S24 Ultra", Descripcion = "Celular premium, 1TB, 16GB RAM", Precio = 1200000, Stock = 8, ImagenUrl = "/celulares/Samsung Galaxy S24 Ultra.png" },
            new Producto { Nombre = "iPhone 15 Pro Max", Descripcion = "Apple, 512GB, 12GB RAM", Precio = 1300000, Stock = 7, ImagenUrl = "/celulares/iPhone 15 Pro Max.png" },
            new Producto { Nombre = "Motorola Edge 50 Pro", Descripcion = "Motorola, 256GB, 8GB RAM", Precio = 650000, Stock = 12, ImagenUrl = "/celulares/Motorola Edge 50 Pro.png" },
            new Producto { Nombre = "Xiaomi 14 Ultra", Descripcion = "Xiaomi, 512GB, 12GB RAM", Precio = 800000, Stock = 15, ImagenUrl = "/celulares/Xiaomi 14 Ultra.png" },
            new Producto { Nombre = "Google Pixel 8 Pro", Descripcion = "Google, 256GB, 12GB RAM", Precio = 900000, Stock = 9, ImagenUrl = "/celulares/Google Pixel 8 Pro.png" },
            new Producto { Nombre = "OnePlus 12", Descripcion = "OnePlus, 512GB, 16GB RAM", Precio = 850000, Stock = 11, ImagenUrl = "/celulares/OnePlus 12.png" },
            new Producto { Nombre = "Honor Magic 6 Pro", Descripcion = "Honor, 512GB, 12GB RAM", Precio = 780000, Stock = 10, ImagenUrl = "/celulares/Honor Magic 6 Pro.png" },
            new Producto { Nombre = "Sony Xperia 1 VI", Descripcion = "Sony, 256GB, 12GB RAM", Precio = 950000, Stock = 6, ImagenUrl = "/celulares/Sony Xperia 1 VI.png" },
            new Producto { Nombre = "Realme GT 6", Descripcion = "Realme, 256GB, 8GB RAM", Precio = 600000, Stock = 14, ImagenUrl = "/celulares/Realme GT 6.png" }
        );
        db.SaveChanges();
    }
}

app.Run();
