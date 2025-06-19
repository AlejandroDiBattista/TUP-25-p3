using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

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

app.UseStaticFiles(); //.

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoint para obtener todos los productos (con búsqueda opcional por nombre)
app.MapGet("/productos", async (TiendaDbContext db, string? q) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q));

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

// Cargar productos 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "ASUS Zenfone 12 Ultra", Descripcion = "Celular gama alta, 512GB, 12GB RAM", Precio = 950000, Stock = 10, ImagenUrl = "/celulares/ASUS Zenfone 12 Ultra.png" },
            new Producto { Nombre = "Google Pixel 9 Pro XL", Descripcion = "Google, 256GB, 12GB RAM", Precio = 900000, Stock = 9, ImagenUrl = "/celulares/Google Pixel 9 Pro XL.png" },
            new Producto { Nombre = "iPhone 16 Pro Max", Descripcion = "Apple, 512GB, 12GB RAM", Precio = 1300000, Stock = 7, ImagenUrl = "/celulares/iPhone 16 Pro Max.png" },
            new Producto { Nombre = "Motorola Edge 50 Pro", Descripcion = "Motorola, 256GB, 8GB RAM", Precio = 650000, Stock = 12, ImagenUrl = "/celulares/Motorola Edge 50 Pro.png" },
            new Producto { Nombre = "OnePlus 13", Descripcion = "OnePlus, 512GB, 16GB RAM", Precio = 850000, Stock = 11, ImagenUrl = "/celulares/OnePlus 13.png" },
            new Producto { Nombre = "OPPO Find X8 Pro", Descripcion = "OPPO, 512GB, 12GB RAM", Precio = 800000, Stock = 8, ImagenUrl = "/celulares/OPPO Find X8 Pro.png" },
            new Producto { Nombre = "Realme GT 7 Pro", Descripcion = "Realme, 256GB, 8GB RAM", Precio = 600000, Stock = 14, ImagenUrl = "/celulares/Realme GT 7 Pro.png" },
            new Producto { Nombre = "Samsung Galaxy S25 Ultra", Descripcion = "Celular premium, 1TB, 16GB RAM", Precio = 1200000, Stock = 8, ImagenUrl = "/celulares/Samsung Galaxy S25 Ultra.png" },
            new Producto { Nombre = "vivo X200 Pro", Descripcion = "vivo, 512GB, 12GB RAM", Precio = 780000, Stock = 10, ImagenUrl = "/celulares/vivo X200 Pro.png" },
            new Producto { Nombre = "Xiaomi 15 Ultra", Descripcion = "Xiaomi, 512GB, 12GB RAM", Precio = 800000, Stock = 15, ImagenUrl = "/celulares/Xiaomi 15 Ultra.png" }
        );
        db.SaveChanges();
    }
}
app.MapPut("/carritos/confirmar", async (TiendaDbContext db, CompraDTO compraDto) =>
{
    if (compraDto.Items == null || compraDto.Items.Count == 0)
        return Results.BadRequest("El carrito está vacío.");

    // Validar stock
    foreach (var item in compraDto.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null || producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay stock suficiente para {producto?.Nombre ?? "producto desconocido"}.");
    }

    // Crear la compra
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = compraDto.Total,
        NombreCliente = compraDto.NombreCliente,
        ApellidoCliente = compraDto.ApellidoCliente,
        EmailCliente = compraDto.EmailCliente,
        Items = new List<ItemCompra>()
    };
    foreach (var item in compraDto.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
        compra.Items.Add(new ItemCompra
        {
            ProductoId = item.ProductoId,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        });
    }
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok("Compra registrada correctamente.");
});
app.Run();