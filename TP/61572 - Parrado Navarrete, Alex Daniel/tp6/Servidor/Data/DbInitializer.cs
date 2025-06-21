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
            new Producto { Nombre = "Gaseosa Pepsi 500ml",           Descripcion = "Bebida gaseosa sabor cola",           Precio = 1000m,  Stock = 50, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/003/795/536/products/77918134201181-02c1892ab7b2181d5e15952575329430-1024-1024-7d9e54ab089ddab94e169902079956241-90c755834d7f058cb216990208444790-1024-1024.jpg" },
            new Producto { Nombre = "Agua Mineral 500ml",           Descripcion = "Agua mineral sin gas",              Precio = 700m,  Stock = 50, ImagenUrl = "https://jumboargentina.vtexassets.com/arquivos/ids/795828/Agua-Villavicencio-Sin-Gas-500cc-1-997464.jpg?v=638313501973800000" },
            new Producto { Nombre = "Cepita 1L",           Descripcion = "Jugo natural de naranja",           Precio = 2000m,  Stock = 50, ImagenUrl = "https://dcdn-us.mitiendanube.com/stores/004/639/248/products/jugo-cepita-naranja-1lt-e212e88ebfda8aac6b17260878472011-1024-1024.png" },
            new Producto { Nombre = "Lays 50g",         Descripcion = "Papas fritas clásicas",            Precio = 800m,  Stock = 50, ImagenUrl = "https://cdn1.npcdn.net/images/16866585729fb692a4312b7f87e12873bf5bbe9a7d.webp?md5id=db93e1f9860b074ef224878a047a5407&new_width=1000&new_height=1000&w=1610591606&from=jpg" },
            new Producto { Nombre = "Pepitos 118g", Descripcion = "Galletas rellenas de chocolate",   Precio = 1500m,  Stock = 50, ImagenUrl = "https://www.rimoldimayorista.com.ar/datos/uploads/mod_catalogo/31308/pepitos-gall-cadbury-610e7b0acf1f3.jpg" },
            new Producto { Nombre = "Diversion 100g",         Descripcion = "Paquete de galletas surtidas",         Precio = 1600m,  Stock = 50, ImagenUrl = "https://arcorencasa.com/wp-content/uploads/2024/10/20241009-14336.webp" },
            new Producto { Nombre = "7-up 500ml",      Descripcion = "Gaseosa sabor limón",               Precio = 1200m,  Stock = 50, ImagenUrl = "https://d1on8qs0xdu5jz.cloudfront.net/webapp/images/fotos/b/0000000000/30_1.jpg" },
            new Producto { Nombre = "Doritos 50g",              Descripcion = "Snacks fritos clasicos",           Precio = 2000m,  Stock = 50, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_776406-MLU78229765540_082024-O.webp" },
            new Producto { Nombre = "Maní Salado 100g",             Descripcion = "Maní tostado y salado",             Precio = 1400m,  Stock = 50, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_951457-MLU74155535156_012024-O.webp" },
            new Producto { Nombre = "Huevo Kinder",              Descripcion = "Huevo de Chocolate con leche",               Precio = 2200m,  Stock = 50, ImagenUrl = "https://camoga.ar/media/catalog/product/cache/17183a23c5d57b885c9e1f3d66234d68/5/0/50081000_huevo_kinder_con_leche_sorpresa_x20_gramos.jpg" }
        };

        context.Productos.AddRange(products);
        context.SaveChanges();
    }
}