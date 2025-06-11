using servidor.Data;
using servidor.Models;

namespace servidor.Services;

/// <summary>
/// Servicio encargado de poblar la base de datos con datos iniciales.
/// Se ejecuta al iniciar la aplicación para asegurar que haya productos disponibles.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Inicializa la base de datos con productos de ejemplo si está vacía.
    /// Los productos son consistentes (temática de tecnología) con imágenes representativas.
    /// </summary>
    /// <param name="context">Contexto de la base de datos</param>
    public static void SeedDatabase(TiendaContext context)
    {
        // Verificar si ya existen productos en la base de datos
        if (context.Productos.Any())
        {
            // Si ya hay productos, no necesitamos hacer seeding
            return;
        }

        // Lista de 10 productos consistentes de tecnología
        var productos = new List<Producto>
        {
            new Producto
            {
                Nombre = "iPhone 15 Pro",
                Descripcion = "Smartphone Apple con pantalla de 6.1 pulgadas, chip A17 Pro, cámara de 48MP y 128GB de almacenamiento. Disponible en color titanio natural.",
                Precio = 1299.99m,
                Stock = 15,
                ImagenUrl = "https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "Samsung Galaxy S24 Ultra",
                Descripcion = "Smartphone premium con pantalla de 6.8 pulgadas, S Pen integrado, cámara de 200MP y 256GB de almacenamiento. Ideal para productividad.",
                Precio = 1199.99m,
                Stock = 12,
                ImagenUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "MacBook Air M3",
                Descripcion = "Laptop ultradelgada con chip M3, pantalla de 13.6 pulgadas, 8GB RAM y 256GB SSD. Perfecta para trabajo y estudios.",
                Precio = 1099.99m,
                Stock = 8,
                ImagenUrl = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "AirPods Pro 2",
                Descripcion = "Auriculares inalámbricos con cancelación activa de ruido, audio espacial y hasta 6 horas de reproducción. Incluye estuche de carga.",
                Precio = 249.99m,
                Stock = 25,
                ImagenUrl = "https://images.unsplash.com/photo-1572569511254-d8f925fe2cbb?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "iPad Pro 11\"",
                Descripcion = "Tablet profesional con chip M4, pantalla Liquid Retina de 11 pulgadas, 128GB y compatible con Apple Pencil Pro.",
                Precio = 799.99m,
                Stock = 10,
                ImagenUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "Apple Watch Series 9",
                Descripcion = "Smartwatch con pantalla Always-On, GPS, resistente al agua y monitoreo avanzado de salud. Caja de 41mm en aluminio.",
                Precio = 399.99m,
                Stock = 18,
                ImagenUrl = "https://images.unsplash.com/photo-1546868871-7041f2a55e12?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "Sony WH-1000XM5",
                Descripcion = "Auriculares over-ear con la mejor cancelación de ruido del mercado, 30 horas de batería y audio de alta resolución.",
                Precio = 399.99m,
                Stock = 14,
                ImagenUrl = "https://images.unsplash.com/photo-1583394838336-acd977736f90?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "Nintendo Switch OLED",
                Descripcion = "Consola híbrida con pantalla OLED de 7 pulgadas, 64GB de almacenamiento interno y dock para jugar en TV.",
                Precio = 349.99m,
                Stock = 20,
                ImagenUrl = "https://images.unsplash.com/photo-1578303512597-81e6cc155b3e?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "Logitech MX Master 3S",
                Descripcion = "Mouse inalámbrico de precisión para productividad, con scroll electromagnético y hasta 70 días de batería.",
                Precio = 99.99m,
                Stock = 30,
                ImagenUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=400&h=400&fit=crop"
            },
            new Producto
            {
                Nombre = "Samsung 4K Monitor 27\"",
                Descripcion = "Monitor UHD de 27 pulgadas con resolución 4K, tecnología HDR10 y conectividad USB-C. Ideal para trabajo profesional.",
                Precio = 329.99m,
                Stock = 6,
                ImagenUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&h=400&fit=crop"
            }
        };

        // Agregar los productos al contexto
        context.Productos.AddRange(productos);

        // Guardar los cambios en la base de datos
        context.SaveChanges();

        // Log para indicar que se completó el seeding
        Console.WriteLine($"✅ Base de datos inicializada con {productos.Count} productos de tecnología.");
    }
}
