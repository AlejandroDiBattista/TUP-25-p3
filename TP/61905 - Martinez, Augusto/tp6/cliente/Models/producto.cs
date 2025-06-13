namespace Cliente.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class ProductoService
{
    public List<Producto> ObtenerProductos()
    {
        return new List<Producto>
        {
            new Producto
            {
                Id = 1,
                Nombre = "Laptop Dell XPS 13",
                Descripcion = "Ultrabook de 13” con procesador Intel i7, 16GB RAM y 512GB SSD.",
                Precio = 1299.99m,
                Stock = 15,
                ImagenUrl = "https://via.placeholder.com/150?text=Laptop"
            },
            new Producto
            {
                Id = 2,
                Nombre = "iPhone 14 Pro",
                Descripcion = "Smartphone Apple con pantalla OLED, 128GB y cámara de 48MP.",
                Precio = 999.00m,
                Stock = 10,
                ImagenUrl = "https://via.placeholder.com/150?text=iPhone"
            },
            new Producto
            {
                Id = 3,
                Nombre = "Audífonos Sony WH-1000XM5",
                Descripcion = "Cancelación activa de ruido, conexión Bluetooth y carga rápida.",
                Precio = 349.99m,
                Stock = 25,
                ImagenUrl = "https://via.placeholder.com/150?text=Audifonos"
            },
            new Producto
            {
                Id = 4,
                Nombre = "Smartwatch Samsung Galaxy Watch 6",
                Descripcion = "Monitor de salud, llamadas y GPS integrado.",
                Precio = 279.00m,
                Stock = 18,
                ImagenUrl = "https://via.placeholder.com/150?text=Smartwatch"
            },
            new Producto
            {
                Id = 5,
                Nombre = "Mouse Logitech MX Master 3",
                Descripcion = "Ergonómico, inalámbrico, recargable y con múltiples botones.",
                Precio = 99.99m,
                Stock = 30,
                ImagenUrl = "https://via.placeholder.com/150?text=Mouse"
            },
            new Producto
            {
                Id = 6,
                Nombre = "Monitor LG UltraWide 34”",
                Descripcion = "Resolución QHD, 75Hz, ideal para productividad y diseño.",
                Precio = 449.00m,
                Stock = 12,
                ImagenUrl = "https://via.placeholder.com/150?text=Monitor"
            },
            new Producto
            {
                Id = 7,
                Nombre = "Teclado Mecánico Keychron K8",
                Descripcion = "Retroiluminado, hot-swappable, Bluetooth y cable USB-C.",
                Precio = 89.00m,
                Stock = 20,
                ImagenUrl = "https://via.placeholder.com/150?text=Teclado"
            },
            new Producto
            {
                Id = 8,
                Nombre = "Cámara Canon EOS R10",
                Descripcion = "Mirrorless de 24MP, video 4K y enfoque automático rápido.",
                Precio = 1099.00m,
                Stock = 8,
                ImagenUrl = "https://via.placeholder.com/150?text=Camara"
            },
            new Producto
            {
                Id = 9,
                Nombre = "Impresora HP Smart Tank 7301",
                Descripcion = "Multifuncional, impresión económica y conectividad Wi-Fi.",
                Precio = 249.00m,
                Stock = 10,
                ImagenUrl = "https://via.placeholder.com/150?text=Impresora"
            },
            new Producto
            {
                Id = 10,
                Nombre = "Bocina JBL Charge 5",
                Descripcion = "Portátil, resistente al agua, batería de larga duración.",
                Precio = 149.00m,
                Stock = 22,
                ImagenUrl = "https://via.placeholder.com/150?text=Bocina"
            }
        };
    }
}
