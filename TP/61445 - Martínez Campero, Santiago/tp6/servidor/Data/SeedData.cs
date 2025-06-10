using System.Linq;
using Compartido; 

public static class SeedData
{
    public static void Initialize(TiendaDbContext context)
    {
        if (context.Productos.Any())
        {
            return;
        }

        var productos = new Producto[]
        {
            new Producto { Nombre = "Laptop Gamer Pro", Descripcion = "Laptop de alta gama para gaming y productividad", Precio = 1500.99m, Stock = 10, ImagenUrl = "images/laptop_gamer.jpg" },
            new Producto { Nombre = "Teclado Mecánico RGB", Descripcion = "Teclado mecánico con iluminación RGB personalizable", Precio = 120.50m, Stock = 25, ImagenUrl = "images/teclado_mecanico.jpg" },
            new Producto { Nombre = "Mouse Inalámbrico Ergonómico", Descripcion = "Mouse diseñado para confort y precisión", Precio = 75.00m, Stock = 30, ImagenUrl = "images/mouse_ergonomico.jpg" },
            new Producto { Nombre = "Monitor Curvo Ultrawide 34\"", Descripcion = "Monitor ultrawide para una experiencia inmersiva", Precio = 650.00m, Stock = 8, ImagenUrl = "images/monitor_ultrawide.jpg" },
            new Producto { Nombre = "Silla Gamer Confort Max", Descripcion = "Silla ergonómica para largas sesiones de juego", Precio = 300.75m, Stock = 15, ImagenUrl = "images/silla_gamer.jpg" },
            new Producto { Nombre = "Auriculares con Micrófono Pro", Descripcion = "Auriculares de alta fidelidad con micrófono cancelador de ruido", Precio = 180.20m, Stock = 20, ImagenUrl = "images/auriculares_pro.jpg" },
            new Producto { Nombre = "Webcam Full HD 1080p", Descripcion = "Webcam para streaming y videoconferencias", Precio = 90.00m, Stock = 22, ImagenUrl = "images/webcam_hd.jpg" },
            new Producto { Nombre = "Disco SSD NVMe 1TB", Descripcion = "Unidad de estado sólido de alta velocidad", Precio = 220.50m, Stock = 18, ImagenUrl = "images/ssd_nvme.jpg" },
            new Producto { Nombre = "Router WiFi 6 Avanzado", Descripcion = "Router de última generación para máxima velocidad y cobertura", Precio = 250.00m, Stock = 12, ImagenUrl = "images/router_wifi6.jpg" },
            new Producto { Nombre = "Alfombrilla XL Gaming", Descripcion = "Alfombrilla extra grande para mouse y teclado", Precio = 40.00m, Stock = 40, ImagenUrl = "images/alfombrilla_xl.jpg" },
            new Producto { Nombre = "Tarjeta Gráfica RTX 4070", Descripcion = "Potente tarjeta gráfica para juegos en 4K", Precio = 700.00m, Stock = 7, ImagenUrl = "images/rtx_4070.jpg" }
        };

        foreach (Producto p in productos)
        {
            context.Productos.Add(p);
        }
        context.SaveChanges();
    }
}
