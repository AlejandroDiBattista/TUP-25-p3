using Microsoft.EntityFrameworkCore;
using servidor.modelos;
using servidor.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TiendaDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        Console.WriteLine(">>> Cargando productos de ejemplo...");
        db.Productos.AddRange(
            new Producto
            {
                Nombre = "Camiseta Titular Boca Juniors 24/25",
                Descripcion = "La nueva camiseta oficial de Boca Juniors para la temporada 2024/2025. Tecnología AEROREADY.",
                Precio = 89999M,
                Stock = 40,
                ImagenUrl = "https://assets.adidas.com/images/h_2000,f_auto,q_auto,fl_lossy,c_fill,g_auto/caa8debad02d4ecd8d0ced5bc5dd191e_9366/Camiseta_Titular_Boca_Juniors_24-25_Azul_IU1232_01_laydown.jpg"
            },
            new Producto
            {
                Nombre = "Camiseta Titular River Plate 24/25",
                Descripcion = "La nueva camiseta oficial de River Plate con la banda roja tradicional. Temporada 2024/2025.",
                Precio = 89999M,
                Stock = 45,
                ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/a52c7b4e92734f1d99e5a7b6b8310dd0_9366/Camiseta_Titular_River_Plate_24-25_Blanco_IX5097_01_laydown.jpg"
            },
            new Producto
            {
                Nombre = "Zapatillas Adidas Ultraboost 1.0",
                Descripcion = "Zapatillas de running con retorno de energía increíble y comodidad superior.",
                Precio = 149999M,
                Stock = 30,
                ImagenUrl = "https://www.dexter.com.ar/on/demandware.static/-/Sites-365-dabra-catalog/default/dw337a08f7/products/ADHQ4199/ADHQ4199-1.JPG"
            },
            new Producto
            {
                Nombre = "Shorts Adidas Tiro 24",
                Descripcion = "Shorts de fútbol con tecnología de absorción AEROREADY, hechos con materiales reciclados.",
                Precio = 49999M,
                Stock = 80,
                ImagenUrl = "https://assets.adidas.com/images/h_2000,f_auto,q_auto,fl_lossy,c_fill,g_auto/d05946265c9143d0ab7f900581e3c4e7_9366/Shorts_de_Entrenamiento_Argentina_Tiro_24_Gris_IQ0811_21_model.jpg"
            },
            new Producto
            {
                Nombre = "Pantalón Adidas Tiro 24",
                Descripcion = "Pantalón de entrenamiento con un ajuste ceñido y cierres en los tobillos para mayor comodidad.",
                Precio = 79999M,
                Stock = 65,
                ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/663419fd6dc94376929109be0cf67f6c_9366/TIRO_ES_PNT_W_Negro_JM5980_21_model.jpg"
            },
            new Producto
            {
                Nombre = "Zapatillas Adidas Superstar",
                Descripcion = "El icónico modelo de zapatillas urbanas con la clásica puntera de caucho. Un clásico atemporal.",
                Precio = 109999M,
                Stock = 70,
                ImagenUrl = "https://assets.adidas.com/images/h_2000,f_auto,q_auto,fl_lossy,c_fill,g_auto/439eaab892ba42ee8839ae02002d60f2_9366/Zapatillas_Superstar_ADV_Negro_GW6931_04_standard.jpg"
            },
            new Producto
            {
                Nombre = "Botines Puma Future 7 Match",
                Descripcion = "Botines de fútbol diseñados para creadores de juego, con texturas 3D para un mejor agarre.",
                Precio = 94999M,
                Stock = 25,
                ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,w_600,b_rgb:FAFAFA/global/images/108272/01/sv01/fnd/ARG/fmt/png"
            },
            new Producto
            {
                Nombre = "Zapatillas Puma Velocity Nitro 3",
                Descripcion = "Zapatillas de running con amortiguación NITRO para una pisada ligera y receptiva.",
                Precio = 139999M,
                Stock = 35,
                ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,w_600,b_rgb:FAFAFA/global/images/108008/01/sv01/fnd/ARG/fmt/png"
            },
            new Producto
            {
                Nombre = "Buzo con Capucha Puma Essentials",
                Descripcion = "Buzo clásico de algodón con capucha, ideal para un look casual y deportivo.",
                Precio = 74999M,
                Stock = 60,
                ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,w_600,b_rgb:FAFAFA/global/images/681440/30/fnd/ARG/fmt/png"
            },
            new Producto
            {
                Nombre = "Mochila Puma Phase",
                Descripcion = "Mochila versátil y duradera con compartimento principal espacioso y bolsillo frontal.",
                Precio = 47999M,
                Stock = 95,
                ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,w_600,b_rgb:FAFAFA/global/images/079943/01/fnd/ARG/fmt/png"
            },
            new Producto
            {
                Nombre = "Zapatillas Topper Squat",
                Descripcion = "Calzado de training robusto y estable, ideal para el gimnasio y levantamiento de pesas.",
                Precio = 85999M,
                Stock = 55,
                ImagenUrl = "https://equipovallejo.vtexassets.com/arquivos/ids/163227/27153-1004-Negro_2.jpg?v=637967766555030000"
            },
            new Producto
            {
                Nombre = "Zapatillas Topper Lona Clásicas",
                Descripcion = "El icónico modelo de lona de Topper, un clásico atemporal para uso diario en color negro.",
                Precio = 49999M,
                Stock = 120,
                ImagenUrl = "https://topperarg.vtexassets.com/arquivos/ids/304982-300-auto?v=638291019444900000&width=300&height=auto&aspect=true"
            }
        );
        db.SaveChanges();
    }
}


app.UseCors("AllowClientApp");




app.MapGet("/api/productos", async (string? q, TiendaDb db) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        query = query.Where(p => p.Nombre.ToLower().Contains(q.ToLower()));
    }
    return await query.ToListAsync();
});


app.MapPost("/api/compras", async (TiendaDb db) =>
{
    var nuevaCompra = new Compra();
    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();
    return Results.Created($"/api/compras/{nuevaCompra.Id}", nuevaCompra);
});


app.MapGet("/api/compras/{idCompra:int}", async (int idCompra, TiendaDb db) =>
{
    var compra = await db.Compras.FindAsync(idCompra);
    if (compra is null) return Results.NotFound("Compra no encontrada.");

    var detalles = await db.ItemsCompra.Where(d => d.CompraId == idCompra).ToListAsync();
    var respuesta = new CompraRespuestaDto { Id = compra.Id };
    decimal totalCalculado = 0;

    foreach (var detalle in detalles)
    {
        var producto = await db.Productos.FindAsync(detalle.ProductoId);
        if (producto != null)
        {
            respuesta.Items.Add(new DetalleRespuestaDto
            {
                ProductoId = producto.Id,
                NombreProducto = producto.Nombre,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                ImagenUrl = producto.ImagenUrl
            });
            totalCalculado += detalle.Cantidad * detalle.PrecioUnitario;
        }
    }
    respuesta.Total = totalCalculado;
    return Results.Ok(respuesta);
});


app.MapPut("/api/compras/{idCompra:int}/productos/{idProducto:int}", async (int idCompra, int idProducto, TiendaDb db) =>
{
    var compra = await db.Compras.FindAsync(idCompra);
    var producto = await db.Productos.FindAsync(idProducto);
    if (compra is null || producto is null) return Results.NotFound();
    if (producto.Stock <= 0) return Results.BadRequest("Sin stock.");

    var item = await db.ItemsCompra.FirstOrDefaultAsync(d => d.CompraId == idCompra && d.ProductoId == idProducto);
    if (item is null)
    {
        db.ItemsCompra.Add(new ItemCompra { CompraId = idCompra, ProductoId = idProducto, Cantidad = 1, PrecioUnitario = producto.Precio });
    }
    else
    {
        item.Cantidad++;
    }
    producto.Stock--;
    await db.SaveChangesAsync();
    return Results.Ok();
});


app.MapDelete("/api/compras/{idCompra:int}/productos/{idProducto:int}", async (int idCompra, int idProducto, TiendaDb db) =>
{

    var detalle = await db.ItemsCompra.FirstOrDefaultAsync(d => d.CompraId == idCompra && d.ProductoId == idProducto);
    if (detalle is null) return Results.NotFound("El producto no está en la compra.");
    var producto = await db.Productos.FindAsync(detalle.ProductoId);
    if (producto is not null) producto.Stock++;
    if (detalle.Cantidad > 1) detalle.Cantidad--;
    else db.ItemsCompra.Remove(detalle);
    await db.SaveChangesAsync();
    return Results.Ok();
});


app.MapPut("/api/compras/{idCompra:int}/confirmar", async (int idCompra, DatosClienteDto datos, TiendaDb db) =>
{
    var compra = await db.Compras.FindAsync(idCompra);
    if (compra is null) return Results.NotFound();

    var detalles = await db.ItemsCompra.Where(d => d.CompraId == idCompra).ToListAsync();
    if (!detalles.Any()) return Results.BadRequest("No se puede confirmar una compra vacía.");

    compra.NombreCliente = datos.Nombre;
    compra.ApellidoCliente = datos.Apellido;
    compra.EmailCliente = datos.Email;
    compra.Total = detalles.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

app.MapDelete("/api/compras/{idCompra:int}", async (int idCompra, TiendaDb db) =>
{
    var detallesDeLaCompra = await db.ItemsCompra
        .Where(d => d.CompraId == idCompra)
        .ToListAsync();

    if (!detallesDeLaCompra.Any())
    {
        var compraVacia = await db.Compras.FindAsync(idCompra);
        if (compraVacia != null)
        {
            db.Compras.Remove(compraVacia);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NotFound("Compra no encontrada.");
    }

    foreach (var detalle in detallesDeLaCompra)
    {
        var producto = await db.Productos.FindAsync(detalle.ProductoId);
        if (producto != null)
        {
            producto.Stock += detalle.Cantidad;
        }
    }

    db.ItemsCompra.RemoveRange(detallesDeLaCompra);
    var compraPrincipal = await db.Compras.FindAsync(idCompra);
    if (compraPrincipal != null)
    {
        db.Compras.Remove(compraPrincipal);
    }

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
