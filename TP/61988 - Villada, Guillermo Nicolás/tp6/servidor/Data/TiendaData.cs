using System.Collections.Generic;
using servidor.Models;

namespace servidor.Data
{
    public static class TiendaData
    {
        public static List<Producto> Productos => new List<Producto>
        {
            new Producto { Nombre = "Remera", Descripcion = "Remera Nickz Original", Precio = 20000, Stock = 50, ImagenUrl = "RemeraNickzOG.jpg" },
            new Producto { Nombre = "Remera", Descripcion = "Remera Nickz Reflectiva (Limited Edition)", Precio = 25000, Stock = 10, ImagenUrl = "NickzReflectiva.jpg" },
            new Producto { Nombre = "Remera Nickz Violeta", Descripcion = "Remera Nickz Violeta (CR Edition)", Precio = 20000, Stock = 15, ImagenUrl = "NickzVioleta.jpg" },
            new Producto { Nombre = "Remera Nickz Verde", Descripcion = "Remera Nickz Verde (Argentina Edition)", Precio = 20000, Stock = 15, ImagenUrl = "NickzVerde.jpg" },
            new Producto { Nombre = "Remera Nickz Naranja", Descripcion = "Remera Nickz Naranja (Halloween Edition)", Precio = 20000, Stock = 15, ImagenUrl = "NickzNaranja.jpg" },
            new Producto { Nombre = "Remera Nickz Candy", Descripcion = "Remera Nickz Chicle (Candy Edition)", Precio = 20000, Stock = 15, ImagenUrl = "NickzChicle.jpg" },
            new Producto { Nombre = "Remera Nickz Invertida", Descripcion = "Remera Nickz Colores Invertidos (Ultra Limited Edition)", Precio = 100000, Stock = 3, ImagenUrl = "NickzInvertida.jpg" },
            new Producto { Nombre = "Zapatillas Tommy Hilfiger", Descripcion = "Zapatillas Tommy (B&N Edition)", Precio = 200000, Stock = 15, ImagenUrl = "ZapatillasTommy.jpg" },
            new Producto { Nombre = "Pod Desechable Elfbar", Descripcion = "Elfbar Ice King (40.000 Puffs)", Precio = 35000, Stock = 200, ImagenUrl = "Elfbar.jpg" },
            new Producto { Nombre = "Pod Desechable Lost Mary", Descripcion = "Lost Mary Mixer doble sabor (30.000 Puffs)", Precio = 35000, Stock = 200, ImagenUrl = "LostMaryMixer.jpg" },
            new Producto { Nombre = "Cartera Louis Vuitton", Descripcion = "Cartera Louis Vuitton de Dama", Precio = 500000, Stock = 3, ImagenUrl = "BolsoLv.jpg" }
        };
    }
}