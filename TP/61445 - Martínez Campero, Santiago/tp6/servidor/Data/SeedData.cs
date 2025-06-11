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
            new Producto { Nombre = "Laptop Gamer Pro", Descripcion = "Laptop de alta gama para gaming y productividad", Precio = 1500.99m, Stock = 10, ImagenUrl = "http://localhost:5184/images/laptopGamerPro.jpg" },
            new Producto { Nombre = "Teclado Mecánico RGB", Descripcion = "Teclado mecánico con iluminación RGB personalizable", Precio = 120.50m, Stock = 25, ImagenUrl = "http://localhost:5184/images/tecladoMecanicoRGB.jpg" },
            new Producto { Nombre = "Mouse Inalámbrico Ergonómico", Descripcion = "Mouse diseñado para confort y precisión", Precio = 75.00m, Stock = 30, ImagenUrl = "http://localhost:5184/images/mouseInalambricoErgonomico.jpeg" },
            new Producto { Nombre = "Monitor Curvo Ultrawide 34\"", Descripcion = "Monitor ultrawide para una experiencia inmersiva", Precio = 650.00m, Stock = 8, ImagenUrl = "http://localhost:5184/images/monitorCurvo34Pulgadas.jpg" },
            new Producto { Nombre = "Silla Gamer Confort Max", Descripcion = "Silla ergonómica para largas sesiones de juego", Precio = 300.75m, Stock = 15, ImagenUrl = "http://localhost:5184/images/sillaGamerConfort.jpeg" }, 
            new Producto { Nombre = "Auriculares con Micrófono Pro", Descripcion = "Auriculares de alta fidelidad con micrófono cancelador de ruido", Precio = 180.20m, Stock = 20, ImagenUrl = "http://localhost:5184/images/AuricularesPro.jpeg" }, 
            new Producto { Nombre = "Webcam Full HD 1080p", Descripcion = "Webcam para streaming y videoconferencias", Precio = 90.00m, Stock = 22, ImagenUrl = "http://localhost:5184/images/WebcamFullHD.jpeg" },
            new Producto { Nombre = "Disco SSD NVMe 1TB", Descripcion = "Unidad de estado sólido de alta velocidad", Precio = 220.50m, Stock = 18, ImagenUrl = "http://localhost:5184/images/DiscoSDD1TB.jpeg" },
            new Producto { Nombre = "Router WiFi 6 Avanzado", Descripcion = "Router de última generación para máxima velocidad y cobertura", Precio = 250.00m, Stock = 12, ImagenUrl = "http://localhost:5184/images/RouterWIFI6.jpg" },
            new Producto { Nombre = "Alfombrilla XL Gaming", Descripcion = "Alfombrilla extra grande para mouse y teclado", Precio = 40.00m, Stock = 40, ImagenUrl = "http://localhost:5184/images/AlfomfrillaXL.jpeg" }, 
            new Producto { Nombre = "Tarjeta Gráfica RTX 4070", Descripcion = "Potente tarjeta gráfica para juegos en 4K", Precio = 700.00m, Stock = 7, ImagenUrl = "http://localhost:5184/images/RTX4070.jpeg" }
        };

        foreach (Producto p in productos)
        {
            context.Productos.Add(p);
        }
        context.SaveChanges();
    }
}
