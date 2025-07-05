using System.Linq;
using TiendaOnline.Servidor.Data;
using TiendaOnline.Servidor.Models;

namespace TiendaOnline.Servidor.Data;
public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Productos.Any()) return;

        var products = new Producto[]
        {
            new Producto { Nombre = "Cable HDMI", Descripcion = "2,5 Metros, Calidad 4K", Precio = 1900m, Stock = 10, ImagenUrl = "https://ventiontech.com/cdn/shop/articles/4K-Optical-Fiber-HDMI-Cable.jpg?v=1660909828" },
            new Producto { Nombre = "Auriculares Alpina", Descripcion = "Inalámbricos bluetooth", Precio = 4500m, Stock = 15, ImagenUrl = "https://www.heavenimagenes.com/heavencommerce/68ac9d04-8767-4aca-9951-49f2fea1383b/images/v2/ALPINA/33503_xlarge.jpg" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Carga rápida 20W", Precio = 1200m, Stock = 18, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/643/020/products/cargador-samsung-45w-4893c150a89d09f45217138810793562-1024-1024.png" },
            new Producto { Nombre = "Funda para celular", Descripcion = "Protección silicona", Precio = 800m, Stock = 25, ImagenUrl = "https://dcdn-us.mitiendanube.com/stores/134/586/products/funda-celular-iphone-original1-c3e25ff26831cb227115126403733044-1024-1024.jpg" },  
            new Producto { Nombre = "Luces LED", Descripcion = "RGB, 10 metros", Precio = 300m, Stock = 30, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_824347-MLA78065047219_072024-O.webp" },
            new Producto { Nombre = "Cámara web", Descripcion = "Full HD 1080p", Precio = 5500m, Stock = 8, ImagenUrl = "https://i.blogs.es/9f75b6/logitech-c920/650_1200.jpg" },
            new Producto { Nombre = "Mouse inalámbrico", Descripcion = "Ergonómico", Precio = 2000m, Stock = 12, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/236/748/products/mouse-inalambrico-logitech-m1701-f725e4c7fe0397d5e216028785053496-1024-1024.jpg" },
            new Producto { Nombre = "Teclado mecánico", Descripcion = "Switch rojo", Precio = 7500m, Stock = 5, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/593/734/products/ganon60-pack11-0df8c37877cb3580bd16618857215873-1024-1024.jpg" },
            new Producto { Nombre = "Tarjeta SD 128GB", Descripcion = "Clase 10", Precio = 3500m, Stock = 18, ImagenUrl = "https://tienda.starware.com.ar/wp-content/uploads/2021/09/memoria-sd-sandisk-extreme-pro-128gb-sdxc-a2-c10-u3-v30-4k-2432-4638.jpg" },
            new Producto { Nombre = "Monitor 24", Descripcion = "LED Full HD", Precio = 25000m, Stock = 8, ImagenUrl = "https://mexx-img-2019.s3.amazonaws.com/Monitor-led-24-samsung-bordes-finos_39232_1.jpeg" }
        };
        context.Productos.AddRange(products);
        context.SaveChanges();
    }
}