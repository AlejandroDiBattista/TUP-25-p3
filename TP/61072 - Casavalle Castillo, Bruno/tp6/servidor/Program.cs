using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using servidor.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=C:\\Users\\Bruno Casavalle\\Desktop\\TUP-25-p3\\TP\\61072 - Casavalle Castillo, Bruno\\tp6\\servidor\\tienda.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.EnsureCreated();

    db.Productos.RemoveRange(db.Productos);
    db.SaveChanges();

    db.Productos.AddRange(
        new Producto { Nombre = "Celular Samsung A54", Descripcion = "Pantalla AMOLED", Precio = 200000, Stock = 12, ImagenUrl = "image/Samsung_a52.jpg" },
        new Producto { Nombre = "Notebook HP", Descripcion = "Intel i5, 8GB RAM", Precio = 480000, Stock = 7, ImagenUrl = "image/Notebook_HP.jpg" },
        new Producto { Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico", Precio = 9500, Stock = 25, ImagenUrl = "image/Mouse_logitech.jpg" },
        new Producto { Nombre = "Teclado Redragon", Descripcion = "Teclado mecánico RGB", Precio = 18000, Stock = 10, ImagenUrl = "image/teclado_redragon.jpg" },
        new Producto { Nombre = "Auriculares Sony", Descripcion = "Cancelación de ruido", Precio = 27000, Stock = 15, ImagenUrl = "image/Auris_Sony.jpg" },
        new Producto { Nombre = "Monitor LG 24\"", Descripcion = "Full HD", Precio = 125000, Stock = 9, ImagenUrl = "image/Monitos_LG.jpg" },
        new Producto { Nombre = "Parlantes Bluetooth", Descripcion = "Potentes y portátiles", Precio = 32000, Stock = 18, ImagenUrl = "image/Parlantes_Bluetooth_JBL.jpg" },
        new Producto { Nombre = "Tablet Lenovo", Descripcion = "10.1 pulgadas, 64GB", Precio = 150000, Stock = 8, ImagenUrl = "image/Tablet_Lenovo.jpeg" },
        new Producto { Nombre = "Smartwatch Xiaomi", Descripcion = "Pantalla AMOLED, GPS", Precio = 45000, Stock = 20, ImagenUrl = "image/Smartwatch_Xiaomi.jpg" },
        new Producto { Nombre = "iPhone 13", Descripcion = "128GB, chip A15 Bionic", Precio = 590000, Stock = 5, ImagenUrl = "image/iPhone_13.jpg" },
new Producto { Nombre = "Notebook Lenovo Ideapad", Descripcion = "Ryzen 5, 16GB RAM, SSD 512GB", Precio = 520000, Stock = 4, ImagenUrl = "image/Lenovo_Ideapad.jpeg" },
new Producto { Nombre = "Smartwatch Xiaomi Watch S1", Descripcion = "Pantalla AMOLED, GPS, resistente al agua", Precio = 92000, Stock = 8, ImagenUrl = "image/Xiaomi_Watch_S1.jpeg" }
    );
    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/api/productos", async (TiendaDbContext db, string? buscar) =>
{
    var productos = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(buscar))
        productos = productos.Where(p => p.Nombre.Contains(buscar));

    return await productos.ToListAsync();
});

app.MapGet("/api/productos/{id:int}", async (int id, TiendaDbContext db) =>
{
    var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == id);
    if (producto == null)
        return Results.NotFound();
    return Results.Ok(producto);
});

app.MapPost("/api/compras", async (CompraDto compraDto, TiendaDbContext db) =>
{
    if (compraDto.Items == null || !compraDto.Items.Any())
        return Results.BadRequest("La compra no contiene productos.");

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = compraDto.NombreCliente,
        ApellidoCliente = compraDto.ApellidoCliente,
        EmailCliente = compraDto.EmailCliente,
        Total = 0,
        Items = new List<ItemCompra>()
    };

    foreach (var itemDto in compraDto.Items)
    {
        var producto = await db.Productos.FindAsync(itemDto.ProductoId);
        if (producto == null)
            return Results.BadRequest($"Producto con ID {itemDto.ProductoId} no encontrado.");

        if (producto.Stock < itemDto.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {producto.Nombre}.");

        producto.Stock -= itemDto.Cantidad;

        var itemCompra = new ItemCompra
        {
            ProductoId = producto.Id,
            Cantidad = itemDto.Cantidad,
            PrecioUnitario = producto.Precio
        };

        compra.Items.Add(itemCompra);
        compra.Total += itemDto.Cantidad * producto.Precio;
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    return Results.Ok(new { compra.Id, compra.Fecha, compra.Total });
});

app.Run();