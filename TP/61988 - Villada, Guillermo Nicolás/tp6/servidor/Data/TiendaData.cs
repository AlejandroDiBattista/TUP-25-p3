using System;
using System.Collections.Generic;
using servidor.Models;

namespace servidor.Data;

public static class TiendaData
{
    public static List<Producto> Productos { get; set; } = new List<Producto>
    {
        new Producto { Id = 1, Nombre = "Celular", Descripcion = "Xiaomi Redmi Note 12", Precio = 150000, Stock = 10, ImagenUrl = "https://..." },
        // Más productos...
    };

    public static Dictionary<Guid, List<ItemCarrito>> Carritos { get; set; } = new();

    public static List<Compra> Compras { get; set; } = new();
}
