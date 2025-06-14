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
            new Producto { Nombre = "Agua 500Ml", Descripcion = "Agua mineral en botella pequeña, ideal para hidratación rápida.", Precio = 500.0m, Stock = 10, ImagenUrl = "https://statics.dinoonline.com.ar/imagenes/full_600x600_ma/3040129_f.jpg" },
            new Producto { Nombre = "Cerveza 473CC", Descripcion = "Cerveza en lata de 473Ml, refrescante y lista para consumir.", Precio = 1200.0m, Stock = 15, ImagenUrl = "https://jumboargentina.vtexassets.com/arquivos/ids/588459/Cerveza-Rubia-Quilmes-Clasica-473-Ml-Lata-1-244314.jpg?v=637280467414430000" },
            new Producto { Nombre = "Coca-Cola 2L", Descripcion = "Bebida gaseosa sabor cola en presentación familiar de 2 litros.", Precio = 2500.0m, Stock = 20, ImagenUrl = "https://fratalmacenar.vtexassets.com/arquivos/ids/156118/7790895000997.jpg?v=638358478530470000" },
            new Producto { Nombre = "Pepsi 2L", Descripcion = "Bebida gaseosa sabor cola, alternativa a Coca-Cola, formato 2 litros.", Precio = 2000.0m, Stock = 25, ImagenUrl = "https://jumboargentina.vtexassets.com/arquivos/ids/788223/Gaseosa-Pepsi-Cola-Botella-2-Lt-1-248207.jpg?v=638249457797100000" },  
            new Producto { Nombre = "Vodka 1L", Descripcion = "Bebida alcohólica destilada, ideal para cócteles y tragos.", Precio = 9000m, Stock = 30, ImagenUrl = "https://www.craftmoments.com.ar/wp-content/uploads/2022/12/smf_21_685x1200.jpg" },
            new Producto { Nombre = "Pritty 2L", Descripcion = "Bebida gaseosa genérica, ideal para reuniones o consumo diario.", Precio = 2500m, Stock = 8, ImagenUrl = "https://jumboargentina.vtexassets.com/arquivos/ids/821504/Gaseosa-Pritty-Lim-n-2-25-L-1-21031.jpg?v=638507146251430000" },
            new Producto { Nombre = "Monster Energy 473Cc", Descripcion = "Bebida energética con cafeína, ideal para obtener un impulso rápido.", Precio = 2500m, Stock = 12, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/157/846/products/diseno-sin-titulo-2022-05-12t104019-0881-1cbf34825e6f43a73a16523628660184-1024-1024.png" },
            new Producto { Nombre = "Fanta 2.25L", Descripcion = "Gaseosa sabor naranja, refrescante y frutada, formato 2 litros.", Precio = 2000m, Stock = 5, ImagenUrl = "https://dcdn-us.mitiendanube.com/stores/001/151/835/products/77908950010171-f5d162eb6218e6544815890789301064-640-0.jpg" },
            new Producto { Nombre = "Mirinda 2L", Descripcion = "Gaseosa sabor manzana, refrescante y frutada, formato 2 litros.", Precio = 2500m, Stock = 18, ImagenUrl = "https://elnenearg.vtexassets.com/arquivos/ids/166619/GASEOSA-MIRINDA-MANZANA-X2-25-1-11512.jpg?v=638149989672370000" },
            new Producto { Nombre = "Secco 3L", Descripcion = "Gaseosa económica en envase de gran tamaño (3 litros).", Precio = 2000m, Stock = 7, ImagenUrl = "https://mandados.com.ar/1317-large_default/gaseosa-secco-3-lt-naranja.jpg" }
        };
        context.Productos.AddRange(products);
        context.SaveChanges();
    }
}