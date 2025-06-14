using System.Collections.Generic;

namespace TiendaApi.Models
{
    public static class ProductosData
    {
        public static List<Producto> productos = new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Camiseta", Precio = 2500 },
            new Producto { Id = 2, Nombre = "Pantalón", Precio = 4500 },
            new Producto { Id = 3, Nombre = "Zapatillas", Precio = 8000 }
        };
    }
}