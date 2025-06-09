using Microsoft.EntityFrameworkCore;
using Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de EF Core con SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
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

// Cargar productos de ejemplo al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Monitor Samsung 27''", Descripcion = "Monitor LED Full HD 27 pulgadas", Precio = 120000, Stock = 8, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/ls27f350fhlczb/gallery/ar-led-monitor-sf350-ls27f350fhlczb-001-front-black.jpg" },
            new Producto { Nombre = "Teclado Mecánico HyperX Alloy", Descripcion = "Teclado mecánico RGB para gaming", Precio = 45000, Stock = 15, ImagenUrl = "https://media.kingston.com/hyperx/product/hx-product-keyboard-alloy-origins-1-lg.jpg" },
            new Producto { Nombre = "Mouse Gamer Razer DeathAdder", Descripcion = "Mouse óptico 16000 DPI", Precio = 35000, Stock = 20, ImagenUrl = "https://assets2.razerzone.com/images/pnx.assets/6e4e2e2e7b7e4e2e2e2e2e2e2e2e2e2e/razer-deathadder-v2-gallery-01.png" },
            new Producto { Nombre = "Auriculares Corsair HS50", Descripcion = "Auriculares gaming con micrófono", Precio = 38000, Stock = 10, ImagenUrl = "https://www.corsair.com/medias/sys_master/images/images/hb1/hb0/9119648571422/CA-9011170-NA-Gallery-HS50-01.png" },
            new Producto { Nombre = "Placa de Video NVIDIA RTX 4060", Descripcion = "8GB GDDR6, Ray Tracing", Precio = 650000, Stock = 4, ImagenUrl = "https://www.nvidia.com/content/dam/en-zz/Solutions/geforce/ada/rtx-4060/geforce-rtx-4060-shop-600-p@2x.png" },
            new Producto { Nombre = "Disco SSD Samsung 980 PRO 1TB", Descripcion = "NVMe PCIe Gen4", Precio = 95000, Stock = 12, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/mz-v8p1t0bw/gallery/ar-ssd-980-pro-mz-v8p1t0bw-530429-ar-mz-v8p1t0bw-530429-FrontBlack-370x370.png" },
            new Producto { Nombre = "Router TP-Link Archer AX10", Descripcion = "Wi-Fi 6, triple banda", Precio = 42000, Stock = 9, ImagenUrl = "https://static.tp-link.com/2020/202011/20201113/Archer-AX10(US1.0)-1.0-main-image.png" },
            new Producto { Nombre = "Tablet Samsung Galaxy Tab S6 Lite", Descripcion = "10.4'' 64GB WiFi", Precio = 210000, Stock = 6, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/sm-p613nzaaaro/gallery/ar-galaxy-tab-s6-lite-p613-sm-p613nzaaaro-530429-530429-FrontGray-370x370.png" },
            new Producto { Nombre = "Impresora HP Ink Tank 415", Descripcion = "Multifunción WiFi", Precio = 85000, Stock = 7, ImagenUrl = "https://www.hp.com/ar-es/shop/media/catalog/product/cache/207e23213cf636ccdef205098cf3c8a3/1/5/1518_1_1.jpg" },
            new Producto { Nombre = "Webcam Logitech C920", Descripcion = "Full HD 1080p", Precio = 32000, Stock = 14, ImagenUrl = "https://resource.logitech.com/w_800,c_limit,q_auto:best,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/webcams/c920/gallery/c920-gallery-1.png" }
        });
        db.SaveChanges();
    }
}

app.Run();
