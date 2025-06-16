using servidor.Data;
using servidor.Models;

namespace servidor.Services;

// Servicio para poblar la base de datos con datos iniciales (productos gaming PC)
public static class DatabaseSeeder
{
    // Inicializa la BD con productos de ejemplo si está vacía
    public static void SeedDatabase(TiendaContext context)
    {
        if (context.Productos.Any())
        {
            return; // Ya hay productos, no necesitamos hacer seeding
        }
        
        // Productos de gaming de PC con precios en ARS
        var productos = new List<Producto>
        {
            new Producto
            {
                Nombre = "RTX 5090 Gaming X Trio",
                Descripcion = "Tarjeta gráfica NVIDIA GeForce RTX 5090 con 32GB GDDR6X. La más potente para gaming 4K y ray tracing. Incluye triple ventilador y RGB.",
                Precio = 1899990.00m,
                Stock = 5,
                ImagenUrl = "images/rtx5090.png"
            },
            new Producto
            {
                Nombre = "RTX 4080 Super",
                Descripcion = "Tarjeta gráfica RTX 4080 Super con 16GB GDDR6X. Excelente para gaming 4K y creación de contenido. Refrigeración avanzada.",
                Precio = 1199990.00m,
                Stock = 8,
                ImagenUrl = "images/rtx4080.png"
            },
            new Producto
            {
                Nombre = "AMD Ryzen 7 9800X3D",
                Descripcion = "Procesador AMD Ryzen 7 9800X3D de 8 núcleos y 16 hilos. Tecnología 3D V-Cache para máximo rendimiento en gaming.",
                Precio = 699990.00m,
                Stock = 12,
                ImagenUrl = "images/ryzen7-9800x3d.png"
            },
            new Producto
            {
                Nombre = "Intel Core i9-14900K",
                Descripcion = "Procesador Intel Core i9-14900K de 24 núcleos (8P+16E). Frecuencia hasta 6.0GHz. Ideal para gaming extremo y multitarea.",
                Precio = 589990.00m,
                Stock = 10,
                ImagenUrl = "images/i9-14900k.png"
            },
            new Producto
            {
                Nombre = "ASUS ROG Strix Z790-E",
                Descripcion = "Motherboard ASUS ROG Strix Z790-E Gaming WiFi. Socket LGA1700, DDR5, PCIe 5.0, WiFi 6E y RGB Aura Sync.",
                Precio = 449990.00m,
                Stock = 15,
                ImagenUrl = "images/asus-z790.png"
            },
            new Producto
            {
                Nombre = "G.Skill Trident Z5 DDR5-6000",
                Descripcion = "Memoria RAM DDR5-6000 32GB (2x16GB) G.Skill Trident Z5 RGB. CL30, optimizada para gaming y overclocking.",
                Precio = 299990.00m,
                Stock = 20,
                ImagenUrl = "images/gskill-ddr5.webp"
            },
            new Producto
            {
                Nombre = "Samsung 980 PRO NVMe 2TB",
                Descripcion = "SSD NVMe Samsung 980 PRO de 2TB. Velocidades hasta 7000MB/s. Perfecto para gaming y carga rápida de juegos.",
                Precio = 199990.00m,
                Stock = 25,
                ImagenUrl = "images/samsung-980pro.png"
            },
            new Producto
            {
                Nombre = "Corsair RM1000x 80+ Gold",
                Descripcion = "Fuente de poder Corsair RM1000x de 1000W, 80+ Gold, totalmente modular. Ventilador de 135mm ultra silencioso.",
                Precio = 189990.00m,
                Stock = 18,
                ImagenUrl = "images/corsair-rm1000x.png"
            },
            new Producto
            {
                Nombre = "NZXT Kraken X73 AIO",
                Descripcion = "Sistema de refrigeración líquida NZXT Kraken X73 de 360mm. Bomba RGB, 3 ventiladores de 120mm y software CAM.",
                Precio = 229990.00m,
                Stock = 12,
                ImagenUrl = "images/nzxt-kraken-x73.png"
            },
            new Producto
            {
                Nombre = "ASUS ROG Swift PG32UQX",
                Descripcion = "Monitor gaming ASUS ROG Swift de 32\" 4K 144Hz. Mini LED, HDR1400, G-SYNC Ultimate. La experiencia gaming definitiva.",
                Precio = 2999990.00m,
                Stock = 3,
                ImagenUrl = "images/asus-pg32uqx.png"
            }
        };

        // Agregar los productos al contexto
        context.Productos.AddRange(productos);

        // Guardar los cambios en la base de datos
        context.SaveChanges();        // Log para indicar que se completó el seeding
        Console.WriteLine($"✅ Base de datos inicializada con {productos.Count} productos de gaming de PC.");
    }
}
