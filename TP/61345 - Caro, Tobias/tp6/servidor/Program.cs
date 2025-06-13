using Microsoft.EntityFrameworkCore;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración CORS
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

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Lista de productos "fuente de la verdad"
    var productosFuente = new List<Producto>
    {
        new Producto { Nombre = "Google Pixel 8 Pro", Descripcion = "Google Pixel 8 Pro 128GB - Obsidiana", Precio = 999, Stock = 15, ImagenUrl = "https://celularesindustriales.com.ar/wp-content/uploads/71h9zq4viSL._AC_UF8941000_QL80_.jpg" },
        new Producto { Nombre = "Google Pixel Watch 2", Descripcion = "Google Pixel Watch 2 con LTE - Plata/Azul Celeste", Precio = 399, Stock = 20, ImagenUrl = "https://http2.mlstatic.com/D_609452-MLU77279991570_072024-C.jpg" },
        new Producto { Nombre = "Google Pixel Buds Pro", Descripcion = "Google Pixel Buds Pro - Porcelana", Precio = 199, Stock = 25, ImagenUrl = "https://i5.walmartimages.com/seo/Google-Pixel-Buds-Pro-Wireless-Earbuds-with-Active-Noise-Cancellation-Bluetooth-Earbuds-Fog_5f8d8e03-bfe9-4099-994c-cea7552ce02d.9cdcbd2e072b93f0fb8ec60dcfc98ca7.jpeg" },
        new Producto { Nombre = "Google Nest Hub Max", Descripcion = "Google Nest Hub Max con Asistente de Google", Precio = 229, Stock = 10, ImagenUrl = "https://lh3.googleusercontent.com/uQZNPuGyf7dKvtGZWjoiyGcPg_A44yUS2tx-o2--dyuwp9A1vR4Efh1UF28KKLpGUg=w895" },
        new Producto { Nombre = "Google Nest Doorbell (Batería)", Descripcion = "Google Nest Doorbell con batería", Precio = 179, Stock = 12, ImagenUrl = "https://i5.walmartimages.com/seo/Google-Nest-Doorbell-Battery-Video-Doorbell-Camera-Wireless-Doorbell-Security-Camera-Snow_807fb01e-45c2-43b7-815c-69a78955ee7f.aacdb8a51e4600aabdfdddb3d729343c.jpeg" },
        new Producto { Nombre = "Google Chromecast con Google TV (4K)", Descripcion = "Chromecast con Google TV - Nieve", Precio = 49, Stock = 30, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_820731-MEC49046529482_022022-O.webp" },
        new Producto { Nombre = "Google Nest Mini", Descripcion = "Google Nest Mini (2da gen.) - Tiza", Precio = 49, Stock = 40, ImagenUrl = "https://www.cdmarket.com.ar/image/0/1000_1300-nestmininegro.jpg" },
        new Producto { Nombre = "Google Wifi (Pack de 3)", Descripcion = "Sistema Wi-Fi en malla Google Wifi (3 unidades)", Precio = 199, Stock = 8, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_890254-MLA32739750061_112019-O.webp" },
        new Producto { Nombre = "Fitbit Charge 6", Descripcion = "Fitbit Charge 6 - Negro Obsidiana", Precio = 159, Stock = 18, ImagenUrl = "https://m.media-amazon.com/images/I/61ZtqtvoD2L.jpg" },
        new Producto { Nombre = "Google Pixel Tablet", Descripcion = "Google Pixel Tablet con base de carga y altavoz", Precio = 499, Stock = 7, ImagenUrl = "https://storage.googleapis.com/support-kms-prod/DwjEEz9EqLvL0HHbIZsdtjj2uMWg5KttRFxa" }
    };

    foreach (var producto in productosFuente)
    {
        var existente = db.Productos.FirstOrDefault(p => p.Nombre == producto.Nombre);
        if (existente == null)
        {
            db.Productos.Add(producto);
        }
        else
        {
            bool requiereActualizacion =
                existente.Descripcion != producto.Descripcion ||
                existente.Precio != producto.Precio ||
                existente.Stock != producto.Stock ||
                existente.ImagenUrl != producto.ImagenUrl;

            if (requiereActualizacion)
            {
                existente.Descripcion = producto.Descripcion;
                existente.Precio = producto.Precio;
                existente.Stock = producto.Stock;
                existente.ImagenUrl = producto.ImagenUrl;
            }
        }
    }

    db.SaveChanges();
}

// Middleware y endpoints
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors("AllowClientApp");
app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });
app.MapControllers();

app.Run();
