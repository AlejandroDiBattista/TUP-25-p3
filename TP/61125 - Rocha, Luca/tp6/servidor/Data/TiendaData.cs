using System.Collections.Generic;
using servidor.Models;

namespace servidor.Data
{
    public static class TiendaData
    {
        public static List<Producto> Productos => new List<Producto>
        {
            new Producto { Nombre = "Tubos", Descripcion = "Tubos de ensayo", Precio = 20000, Stock = 10, ImagenUrl = "tubos.jpg" },
            new Producto { Nombre = "Balanza", Descripcion = "Balanza", Precio = 25000, Stock = 10, ImagenUrl = "balanza.jpg" },
            new Producto { Nombre = "Manta", Descripcion = "Manta de Calefaccion", Precio = 20000, Stock = 15, ImagenUrl = "manta2.jpg" },
            new Producto { Nombre = "Matraz", Descripcion = "Matraz", Precio = 20000, Stock = 15, ImagenUrl = "matraz2.jpg" },
            new Producto { Nombre = "Piseta", Descripcion = "Piseta", Precio = 20000, Stock = 15, ImagenUrl = "piseta.jpg" },
            new Producto { Nombre = "Probeta", Descripcion = "Probeta", Precio = 20000, Stock = 15, ImagenUrl = "probeta.jpg" },
            new Producto { Nombre = "Termometro", Descripcion = "Mide la temperatura", Precio = 100000, Stock = 3, ImagenUrl = "termometro.jpg" },
            
        };
    }
}