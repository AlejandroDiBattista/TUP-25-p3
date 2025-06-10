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

// Agregar contexto de base de datos con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Seed de productos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Samsung Galaxy S23", Descripcion = "Smartphone de última generación con cámara de 108MP", Precio = 299999.99M, Stock = 10, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/sm-s911bzklaro/gallery/ar-galaxy-s23-s911-sm-s911bzklaro-thumb-534863401" },
            new Producto { Nombre = "iPhone 15 Pro", Descripcion = "Apple iPhone 15 Pro 256GB", Precio = 399999.99M, Stock = 8, ImagenUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-15-pro-model-unselect-gallery-1-202309?wid=512&hei=512&fmt=jpeg&qlt=95&.v=1692923778665" },
            new Producto { Nombre = "PlayStation 5", Descripcion = "Consola Sony PlayStation 5 Digital Edition", Precio = 499999.99M, Stock = 5, ImagenUrl = "https://www.sony.com/image/ps5-digital-edition.png" },
            new Producto { Nombre = "Smart TV Samsung 50''", Descripcion = "Televisor 4K UHD Smart TV 50 pulgadas", Precio = 350000.00M, Stock = 6, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/ua50au7000gxcz/gallery/ar-uhd-au7000-ua50au7000gxcz-530183401" },
            new Producto { Nombre = "Notebook Lenovo IdeaPad 3", Descripcion = "Notebook 15.6'' Ryzen 5 8GB 512GB SSD", Precio = 420000.00M, Stock = 7, ImagenUrl = "https://www.lenovo.com/medias/ideapad-3-15alc6-hero.png" },
            new Producto { Nombre = "Auriculares Bluetooth JBL", Descripcion = "Auriculares inalámbricos JBL Tune 510BT", Precio = 29999.99M, Stock = 20, ImagenUrl = "https://www.jbl.com.ar/on/demandware.static/-/Sites-masterCatalog_Harman/default/dw7e2e2e2e/JBL_TUNE510BT_Product%20Image_Hero_Black.png" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Cargador rápido 25W", Precio = 7999.99M, Stock = 30, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/ep-ta800nwegaro/gallery/ar-ep-ta800nwegaro-ep-ta800nwegaro-530183401" },
            new Producto { Nombre = "Funda para iPhone 15", Descripcion = "Funda de silicona original Apple", Precio = 14999.99M, Stock = 25, ImagenUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MQ0E3?wid=2000&hei=2000&fmt=jpeg&qlt=95&.v=1660803972361" },
            new Producto { Nombre = "Teclado Logitech K380", Descripcion = "Teclado Bluetooth multidispositivo", Precio = 24999.99M, Stock = 12, ImagenUrl = "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/keyboards/k380/gallery/k380-rose-gallery-1-us.png?v=1" },
            new Producto { Nombre = "Mouse Logitech M185", Descripcion = "Mouse inalámbrico compacto", Precio = 9999.99M, Stock = 18, ImagenUrl = "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/m185/gallery/m185-grey-gallery-1.png?v=1" }
        );
        db.SaveChanges();
    }
}

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

app.Run();
