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
            Nombre = "Remera",
            Descripcion = "Remera Nickz Original",
            Precio = 20000,
            Stock = 50,
            ImagenUrl = "/imagenes/NickzOriginal.jpg"
        },
        new Producto
        {
            Id = 2,
            Nombre = "Remera",
            Descripcion = "Remera Nickz Reflectiva (Limited Edition)",
            Precio = 25000,
            Stock = 10,
            ImagenUrl = "/imagenes/NickzReflectiva.jpg"
        },
        new Producto
        {
            Id = 3,
            Nombre = "Remera Nickz Violeta",
            Descripcion = "Remera Nickz Violeta (CR Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "/imagenes/NickzVioleta.jpg"
        },
        new Producto
        {
            Id = 4,
            Nombre = "Remera Nickz Verde",
            Descripcion = "Remera Nickz Verde (Argentina Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "/imagenes/NickzVerde.jpg"
        },
        new Producto
        {
            Id = 5,
            Nombre = "Remera Nickz Naranja",
            Descripcion = "Remera Nickz Naranja (Halloween Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "/imagenes/NickzNaranja.jpg"
        },
        new Producto
        {
            Id = 6,
            Nombre = "Remera Nickz Candy",
            Descripcion = "Remera Nickz Chicle (Candy Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "/imagenes/NickzChicle.jpg"
        },
        new Producto
        {
            Id = 7,
            Nombre = "Remera Nickz Invertida",
            Descripcion = "Remera Nickz Colores Invertidos (Ultra Limited Edition)",
            Precio = 20000,
            Stock = 3,
            ImagenUrl = "/imagenes/NickzInvertida.jpg"
        },
    };

    // Diccionario que guarda los carritos por ID
    public static Dictionary<Guid, List<ItemCarrito>> Carritos { get; set; } = new();

    // Lista de compras realizadas
    public static List<Compra> Compras { get; set; } = new();
}
