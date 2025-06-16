using System.Collections.Generic;

namespace TiendaApi.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }

    public static class ProductosData
    {
        public static List<Producto> productos = new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Camiseta", Precio = 2500 },
            new Producto { Id = 2, Nombre = "Pantalón", Precio = 4500 },
            new Producto { Id = 3, Nombre = "Zapatillas", Precio = 8000 }
            new Producto {
                Id = 1,
                Nombre = "Camiseta De Brasil",
                Descripcion = "Camiseta de titular de la selección de Brasil.",
                Precio = 15000,
                ImagenUrl = "https://www.dexter.com.ar/on/demandware.static/-/Sites-365-dabra-catalog/default/dwc951865d/products/NI_DN0680-741/NI_DN0680-741-1.JPG"
            },
            new Producto {
                Id = 2,
                Nombre = "Camiseta Titular Del Real Madrid",
                Descripcion = "Camiseta adidas de la temporada 25/26.",
                Precio = 30000,
                ImagenUrl = "https://assets.adidas.com/images/w_600,f_auto,q_auto/78b62417f1e042aeb25e3353d278de3b_9366/Camiseta_Titular_Real_Madrid_24-25_Version_Jugador_Blanco_IX8095_HM1.jpg"
            },
            new Producto {
                Id = 3,
                Nombre = "Short Deportivo Puma",
                Descripcion = "Short deportivo para uso diario.",
                Precio = 9500,
                ImagenUrl = "https://d3fvqmu2193zmz.cloudfront.net/items_2/uid_commerces.1/uid_items_2.FDC2JKU0FBII/500x500/64DBA1557DDB7-Short-Deportivo-Hombre-Teamliga-Training-Shorts-2--Open-Pockets.webp"
            },
            new Producto {
                Id = 4,
                Nombre = "Short Del Club Atletico River Plate",
                Descripcion = "Short de poliéster para Partido.",
                Precio = 7500,
                ImagenUrl = "https://rossettiar.vtexassets.com/arquivos/ids/1815434-800-auto?v=638608841275130000&width=800&height=auto&aspect=true"
            },
            new Producto {
                Id = 5,
                Nombre = "Short Deportivo adidas",
                Descripcion = "Short deportivo para uso diario Adidas.",
                Precio = 8200,
                ImagenUrl = "https://www.becoenlinea.com/wp-content/uploads/2023/12/FT6685.webp"
            },
            new Producto {
                Id = 6,
                Nombre = "Camiseta Titular de River Plate",
                Descripcion = "Camiseta titular versión jugador.",
                Precio = 27000,
                ImagenUrl = "https://templofutbol.vtexassets.com/arquivos/ids/20359721/JN6465-A.jpg?v=638852428398900000"
            },
            new Producto {
                Id = 7,
                Nombre = "Camiseta retro de Argentina",
                Descripcion = "Camiseta de retro de maradona color azul.",
                Precio = 12000,
                ImagenUrl = "https://acdn-us.mitiendanube.com/stores/145/898/products/1-3c7d94bd28b57b38f817047553855720-480-0.jpg"
            },
            new Producto {
                Id = 8,
                Nombre = "Camiseta de entrenamiento de river plate",
                Descripcion = "Camiseta para entrenamiento.",
                Precio = 5000,
                ImagenUrl = "https://media2.solodeportes.com.ar/media/catalog/product/cache/7c4f9b393f0b8cb75f2b74fe5e9e52aa/r/e/remera-de-river-adidas-entrenamiento-bordo-100020is5542001-1.jpg"
            },
            new Producto {
                Id = 9,
                Nombre = "Camiseta suplente de boca juniors",
                Descripcion = "Camiseta de boca juniors adidas, color amarillo con azul.",
                Precio = 8000,
                ImagenUrl = "https://rossettiar.vtexassets.com/arquivos/ids/2024722-800-auto?v=638799114890200000&width=800&height=auto&aspect=true"
            },
            new Producto {
                Id = 10,
                Nombre = "Camiseta de la Seleción Argentina",
                Descripcion = "Camiseta de Argentina, ideal para los partidos.",
                Precio = 10000,
                ImagenUrl = "https://images.footballfanatics.com/argentina-national-team/mens-adidas-lionel-messi-blue-argentina-national-team-2024-away-replica-player-jersey_ss5_p-201337023+pv-1+u-en6cedqvd7z0tktoxwun+v-21nsj30uwjo88x5z5hlw.jpg?_hv=2&w=900"
            }
        };
    }
}