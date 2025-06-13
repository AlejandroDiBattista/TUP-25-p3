using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


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

// Configuración de EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores si es necesario
builder.Services.AddControllers();

// Configuración de serialización camelCase
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Poblar la base de datos con productos de ejemplo si está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Nombre = "Coca-Cola", Descripcion = "Bebida gaseosa 1.5L", Precio = 1200, Stock = 10, ImagenUrl = "https://www.myccba.africa/media/catalog/product/cache/5479647258cfabec4d973a924b24e3d0/1/7/1759-ZA_10.png" },
            new Producto { Nombre = "Pepsi", Descripcion = "Bebida gaseosa 1.5L", Precio = 1100, Stock = 8, ImagenUrl = "https://boozy.ph/cdn/shop/files/2024-2ndPlatforms-ProductImageTemplate_12_c0e1851b-cdca-4e5a-82b1-c73fc488fda1_grande.png?v=1727744884" },
            new Producto { Nombre = "Fanta", Descripcion = "Bebida naranja 1.5L", Precio = 1150, Stock = 7, ImagenUrl = "https://www.coca-cola.com/content/dam/onexp/py/es/brands/fanta/nuevos_renders/7840058000392.png" },
            new Producto { Nombre = "Sprite", Descripcion = "Bebida lima-limón 1.5L", Precio = 1150, Stock = 5, ImagenUrl = "https://www.myccba.africa/media/catalog/product/cache/5479647258cfabec4d973a924b24e3d0/6/1/6121-ZA_5_2.png" },
            new Producto { Nombre = "Fernet Branca", Descripcion = "Bebida alcoholica 1L", Precio = 10000, Stock = 3, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/211/660/products/branca-7501-f2d07ee453f8e36afd16050362910743-480-0.png" },
            new Producto { Nombre = "Heineken", Descripcion = "Bebida alcoholica 1L", Precio = 4000, Stock = 6, ImagenUrl = "https://png.pngtree.com/png-clipart/20240131/original/pngtree-heineken-bottle-beer-cut-photo-png-image_14191815.png" },
            new Producto { Nombre = "Mirinda", Descripcion = "Bebida naranja 480ml", Precio = 1000, Stock = 12, ImagenUrl = "https://www.shopping-d.com/cdn/shop/products/10MRO490P-AXL-12P-CT490ml.png?v=1733886847&width=1445" },
            new Producto { Nombre = "Agua mineral", Descripcion = "Bebida hidratante 500ml", Precio = 1000, Stock = 4, ImagenUrl = "https://png.pngtree.com/png-vector/20240913/ourmid/pngtree-mineral-water-bottle-png-image_13600124.png" },
            new Producto { Nombre = "Gaseosa Manaos", Descripcion = "Cola 2.25L", Precio = 800, Stock = 15, ImagenUrl = "https://www.gaitasdistribuidora.com.ar/datos/uploads/mod_catalogo/31047/manaos-cola-sin-az-6-x-2-25l-67925ace20cb7.png" },
            new Producto { Nombre = "Cigarrillos Malboro", Descripcion = "Tabaco 12 unidades", Precio = 3500, Stock = 20, ImagenUrl = "https://www.verdistribuciones.com/wp-content/uploads/2020/04/Marlboro-Rojo.png" }
        });
        db.SaveChanges();
    }
}

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();
