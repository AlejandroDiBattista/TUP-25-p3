using System;
using System.Collections.Generic;
using servidor.Models;

namespace servidor.Data;

public static class TiendaData
{
    // Lista de productos iniciales
    public static List<Producto> Productos { get; set; } = new List<Producto>
    {
        new Producto
        {
            Id = 1,
            Nombre = "Celular",
            Descripcion = "Xiaomi Redmi Note 12",
            Precio = 150000,
            Stock = 10,
            ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_739446-MLA54965116941_042023-F.webp"
        },
        new Producto
        {
            Id = 2,
            Nombre = "Notebook",
            Descripcion = "HP 15s Intel Core i5 11va Gen",
            Precio = 420000,
            Stock = 5,
            ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_948737-MLA51829376989_102022-F.webp"
        },
        new Producto
        {
            Id = 3,
            Nombre = "Auriculares",
            Descripcion = "Logitech G435 Lightspeed",
            Precio = 70000,
            Stock = 15,
            ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_947731-MLA46930967287_082021-F.webp"
        }
    };

    // Diccionario que guarda los carritos por ID
    public static Dictionary<Guid, List<ItemCarrito>> Carritos { get; set; } = new();

    // Lista de compras realizadas
    public static List<Compra> Compras { get; set; } = new();
}
