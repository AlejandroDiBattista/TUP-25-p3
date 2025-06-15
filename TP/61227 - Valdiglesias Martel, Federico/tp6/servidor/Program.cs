using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TiendaDb>(opt => opt.UseSqlite("Data Source=tienda.db"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowClientApp");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDb>();
    db.Database.EnsureCreated();
    if (!db.Productos.Any())
    {
        Console.WriteLine(">>> Cargando productos de ejemplo...");
        // Pega aquí la lista completa de los 20 productos.
        db.Productos.AddRange(new Producto
        {
            Nombre = "Camiseta Titular Boca Juniors 24/25",
            Descripcion = "La nueva camiseta oficial de Boca Juniors para la temporada 2024/2025. Tecnología AEROREADY.",
            Precio = 89999M,
            Stock = 40,
            ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/f95f4638779544068564b73a2468d6f5_9366/Camiseta_Titular_Boca_Juniors_24-25_Azul_IT3725_01_laydown.jpg"
        },
    new Producto
    {
        Nombre = "Camiseta Titular River Plate 24/25",
        Descripcion = "La nueva camiseta oficial de River Plate con la banda roja tradicional. Temporada 2024/2025.",
        Precio = 89999M,
        Stock = 45,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/433b5c3179294155bb24b53139b867c2_9366/Camiseta_Titular_River_Plate_24-25_Blanco_IV9725_01_laydown.jpg"
    },

    // --- Productos Adidas ---
    new Producto
    {
        Nombre = "Zapatillas Adidas Ultraboost 1.0",
        Descripcion = "Zapatillas de running con retorno de energía increíble y comodidad superior.",
        Precio = 149999M,
        Stock = 30,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/c2a17334731842288565af2400827223_9366/Zapatillas_Ultraboost_1.0_Negro_HQ4206_01_standard.jpg"
    },
    new Producto
    {
        Nombre = "Shorts Adidas Tiro 24",
        Descripcion = "Shorts de fútbol con tecnología de absorción AEROREADY, hechos con materiales reciclados.",
        Precio = 49999M,
        Stock = 80,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/d40ea73683c341ad8429b35b643f8216_9366/Shorts_de_Entrenamiento_Tiro_24_Negro_IN9234_21_model.jpg"
    },
    new Producto
    {
        Nombre = "Pantalón Adidas Tiro 24",
        Descripcion = "Pantalón de entrenamiento con un ajuste ceñido y cierres en los tobillos para mayor comodidad.",
        Precio = 79999M,
        Stock = 65,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/b3a3ce4f54ef429d8995c531d054d9fa_9366/Pantalon_de_Entrenamiento_Tiro_24_Negro_IN9232_21_model.jpg"
    },
    new Producto
    {
        Nombre = "Zapatillas Adidas Superstar",
        Descripcion = "El icónico modelo de zapatillas urbanas con la clásica puntera de caucho. Un clásico atemporal.",
        Precio = 109999M,
        Stock = 70,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/7ed0855435194229a525aad6009a0497_9366/Zapatillas_Superstar_Blanco_EG4958_01_standard.jpg"
    },

    // --- Productos Puma ---
    new Producto
    {
        Nombre = "Botines Puma Future 7 Match",
        Descripcion = "Botines de fútbol diseñados para creadores de juego, con texturas 3D para un mejor agarre.",
        Precio = 94999M,
        Stock = 25,
        ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,b_rgb:fafafa,w_600,h_600/global/107715/01/sv01/fnd/ARG/fmt/png/Botines-Future-7-Match"
    },
    new Producto
    {
        Nombre = "Zapatillas Puma Velocity Nitro 3",
        Descripcion = "Zapatillas de running con amortiguación NITRO para una pisada ligera y receptiva.",
        Precio = 139999M,
        Stock = 35,
        ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,b_rgb:fafafa,w_750,h_750/global/379495/03/sv01/fnd/ARG/fmt/png/Zapatillas-de-running-Velocity-Nitro%E2%84%A2-3"
    },
    new Producto
    {
        Nombre = "Buzo con Capucha Puma Essentials",
        Descripcion = "Buzo clásico de algodón con capucha, ideal para un look casual y deportivo.",
        Precio = 74999M,
        Stock = 60,
        ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,b_rgb:fafafa,w_750,h_750/global/670004/01/mod01/fnd/ARG/fmt/png/Buzo-con-Capucha-Essentials-Logo"
    },
    new Producto
    {
        Nombre = "Mochila Puma Phase",
        Descripcion = "Mochila versátil y duradera con compartimento principal espacioso y bolsillo frontal.",
        Precio = 47999M,
        Stock = 95,
        ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,b_rgb:fafafa,w_600,h_600/global/075487/01/fnd/ARG/fmt/png/Mochila-Phase"
    },

    // --- Productos Topper ---
    new Producto
    {
        Nombre = "Zapatillas Topper Squat",
        Descripcion = "Calzado de training robusto y estable, ideal para el gimnasio y levantamiento de pesas.",
        Precio = 85999M,
        Stock = 55,
        ImagenUrl = "https://www.topper.com.ar/on/demandware.static/-/Sites-topper-master-catalog/default/dw18335cfb/59660_1.jpg"
    },
    new Producto
    {
        Nombre = "Zapatillas Topper Lona Clásicas",
        Descripcion = "El icónico modelo de lona de Topper, un clásico atemporal para uso diario en color negro.",
        Precio = 49999M,
        Stock = 120,
        ImagenUrl = "https://www.topper.com.ar/on/demandware.static/-/Sites-topper-master-catalog/default/dw83748c97/23306_1.jpg"
    },
    new Producto
    {
        Nombre = "Remera Topper T-Shirt Basic",
        Descripcion = "Remera de algodón suave y cómoda, con logo de Topper en el pecho. Color blanco.",
        Precio = 29999M,
        Stock = 150,
        ImagenUrl = "https://www.topper.com.ar/on/demandware.static/-/Sites-topper-master-catalog/default/dw2c19a9a3/61202_1.jpg"
    },
    new Producto
    {
        Nombre = "Short Topper Urbano",
        Descripcion = "Short de rústico liviano, cómodo y versátil para tus actividades diarias y de tiempo libre.",
        Precio = 42999M,
        Stock = 85,
        ImagenUrl = "https://www.topper.com.ar/on/demandware.static/-/Sites-topper-master-catalog/default/dw2b349d47/165243_1.jpg"
    },

    // --- Accesorios Adicionales ---
    new Producto
    {
        Nombre = "Medias de Fútbol Adidas Adisock",
        Descripcion = "Medias de fútbol acolchadas con diseño anatómico para un ajuste perfecto. Pack x1.",
        Precio = 24999M,
        Stock = 200,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/23a85325697342799307b22a6133f6a2_9366/Medias_de_futbol_Adisock_23_Blanco_HT6303_01_standard.jpg"
    },
    new Producto
    {
        Nombre = "Gorra Puma Ess Cap",
        Descripcion = "Gorra deportiva clásica con visera curvada y logo Puma bordado.",
        Precio = 34999M,
        Stock = 90,
        ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,b_rgb:fafafa,w_600,h_600/global/052919/01/fnd/ARG/fmt/png/Gorra-Ess"
    },
    new Producto
    {
        Nombre = "Campera Rompevientos Adidas Own The Run",
        Descripcion = "Campera de running ultraligera y resistente al viento, con detalles reflectantes.",
        Precio = 99999M,
        Stock = 40,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/e232b4b9148d4a66a1a1ae940120560f_9366/Chaqueta_Rompevientos_Own_the_Run_Negro_H59272_21_model.jpg"
    },
    new Producto
    {
        Nombre = "Pelota de Fútbol Puma Orbita",
        Descripcion = "Pelota de fútbol N°5 con una construcción de 32 paneles para una trayectoria de vuelo predecible.",
        Precio = 54999M,
        Stock = 75,
        ImagenUrl = "https://images.puma.com/image/upload/f_auto,q_auto,b_rgb:fafafa,w_600,h_600/global/084100/01/sv01/fnd/ARG/fmt/png/Pelota-de-futbol-Orbita-6-MS"
    },
    new Producto
    {
        Nombre = "Ojotas Adidas Adilette Aqua",
        Descripcion = "Ojotas de secado rápido, ideales para la ducha después de entrenar o para usar en la pileta.",
        Precio = 44999M,
        Stock = 110,
        ImagenUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/5895999528864826975aab8300d5a373_9366/Ojotas_Adilette_Aqua_Negro_F35543_01_standard.jpg"
    },
    new Producto
    {
        Nombre = "Bolso Topper Training",
        Descripcion = "Bolso deportivo de tamaño mediano con compartimento principal amplio y bolsillos laterales.",
        Precio = 69999M,
        Stock = 50,
        ImagenUrl = "https://www.topper.com.ar/on/demandware.static/-/Sites-topper-master-catalog/default/dw17d87bd9/165246_1.jpg"
    });
        db.SaveChanges();
    }
}


// Endpoint para obtener productos (no cambia)
app.MapGet("/api/productos", async (string? q, TiendaDb db) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        query = query.Where(p => p.Nombre.ToLower().Contains(q.ToLower()));
    }
    return await query.ToListAsync();
});

// Endpoint para crear una compra (carrito)
app.MapPost("/api/compras", async (TiendaDb db) =>
{
    var nuevaCompra = new Compra();
    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();
    return Results.Created($"/api/compras/{nuevaCompra.Id}", nuevaCompra);
});

app.Run();
