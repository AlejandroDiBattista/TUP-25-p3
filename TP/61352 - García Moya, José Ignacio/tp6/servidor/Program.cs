using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);



// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de serialización camelCase
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Configuración de EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores si es necesario
builder.Services.AddControllers();

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
            new Producto { Nombre = "Smartphone Samsung Galaxy A34", Descripcion = "Pantalla 6.6'' AMOLED, 128GB", Precio = 200000, Stock = 10, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/ar/sm-a346ezkearo/gallery/ar-galaxy-a34-5g-sm-a346-sm-a346ezkearo-536286864?$720_576_PNG$" },
            new Producto { Nombre = "Notebook Lenovo IdeaPad 3", Descripcion = "Intel i5, 8GB RAM, 512GB SSD", Precio = 350000, Stock = 5, ImagenUrl = "https://www.lenovo.com/medias/lenovo-laptop-ideapad-3-15-intel-subseries-hero.png?context=bWFzdGVyfHJvb3R8MjA0Nzk5fGltYWdlL3BuZ3xoYTIvaDM0LzE0MzM4MzQxOTgxNzQyLnBuZ3xmZTZjMWFiZTI4YTI2M2RkZjk5MWJjZmE2NmU3MmY1ZDYxMmNlYzA0NjU1ZjI4YTA3YTc2YzBjMjBhZDkyODRh" },
            new Producto { Nombre = "Auriculares Inalámbricos JBL Tune 510BT", Descripcion = "Bluetooth, sonido JBL Pure Bass", Precio = 45000, Stock = 15, ImagenUrl = "https://jblargentina.com.ar/wp-content/uploads/2022/11/TUNE510BT-PRODUCT-IMAGE-HERO-BLUE-1605x1605px.png" },
            new Producto { Nombre = "Mouse Logitech G203", Descripcion = "RGB, 8000 DPI, gaming", Precio = 15000, Stock = 20, ImagenUrl = "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/gaming/g203/g203-gallery-1.png" },
            new Producto { Nombre = "Monitor LG 24'' Full HD", Descripcion = "Panel IPS, HDMI/VGA", Precio = 90000, Stock = 7, ImagenUrl = "https://www.lg.com/ar/images/monitores/md07555746/gallery/D-01.jpg" },
            new Producto { Nombre = "Teclado Mecánico Redragon Kumara", Descripcion = "Switch Blue, retroiluminado", Precio = 25000, Stock = 8, ImagenUrl = "https://resource.logitechg.com/content/dam/gaming/en/products/kumara-mechanical-keyboard/gallery/redragon-kumara-gallery-1.png" },
            new Producto { Nombre = "Smart TV Noblex 43''", Descripcion = "Full HD, Android TV", Precio = 230000, Stock = 4, ImagenUrl = "https://www.noblex.com.ar/media/catalog/product/cache/5479647258cfabec4d973a924b24e3d0/s/m/smart_tv_43_3_1.png" },
            new Producto { Nombre = "Parlante Bluetooth Sony SRS-XB13", Descripcion = "Compacto, resistente al agua", Precio = 30000, Stock = 12, ImagenUrl = "https://www.sony.com.ar/image/47158eb48676ec255431eb11e68d13fc?fmt=pjpeg&bgcolor=FFFFFF&bgc=FFFFFF&wid=2515&hei=1320" },
            new Producto { Nombre = "Disco Externo WD 1TB", Descripcion = "USB 3.0, portátil", Precio = 65000, Stock = 9, ImagenUrl = "https://www.westerndigital.com/content/dam/store/en-us/assets/products/portable/wd-elements-portable/gallery/wd-elements-portable-angled.png" },
            new Producto { Nombre = "Cámara Web Logitech C920", Descripcion = "1080p Full HD con micrófono", Precio = 75000, Stock = 6, ImagenUrl = "https://resource.logitech.com/w_700,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/webcams/c920/gallery/c920-gallery-1.png" }
        });
        db.SaveChanges();
    }
}

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint para obtener productos
app.MapGet("/productos", async ([FromServices] TiendaContext db, [FromQuery] string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q));
    return await query.ToListAsync();
});

// Endpoint para confirmar compra
app.MapPut("/carritos/{carrito}/confirmar", async ([FromServices] TiendaContext db, string carrito, [FromBody] CompraConfirmacionDto dto) =>
{
    foreach (var item in dto.Items)
    {
        var prod = await db.Productos.FindAsync(item.ProductoId);
        if (prod == null || prod.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {prod?.Nombre ?? "producto desconocido"}");
        prod.Stock -= item.Cantidad;
    }
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = dto.Items.Sum(i => i.PrecioUnitario * i.Cantidad),
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        Items = dto.Items.Select(i => new Item
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();

// DTOs para la confirmación de compra
public class CompraConfirmacionDto
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public List<ItemCompraDto> Items { get; set; }
}

public class ItemCompraDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
