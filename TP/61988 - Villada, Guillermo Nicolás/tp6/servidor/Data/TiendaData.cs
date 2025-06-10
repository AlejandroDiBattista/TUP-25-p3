using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            ImagenUrl = "NickzOriginal.jpg"
        },
        new Producto
        {
            Id = 2,
            Nombre = "Remera",
            Descripcion = "Remera Nickz Reflectiva (Limited Edition)",
            Precio = 25000,
            Stock = 10,
            ImagenUrl = "NickzReflectiva.jpg"
        },
        new Producto
        {
            Id = 3,
            Nombre = "Remera Nickz Violeta",
            Descripcion = "Remera Nickz Violeta (CR Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "NickzVioleta.jpg"
        },
        new Producto
        {
            Id = 4,
            Nombre = "Remera Nickz Verde",
            Descripcion = "Remera Nickz Verde (Argentina Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "NickzVerde.jpg"
        },
        new Producto
        {
            Id = 5,
            Nombre = "Remera Nickz Naranja",
            Descripcion = "Remera Nickz Naranja (Halloween Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "NickzNaranja.jpg"
        },
        new Producto
        {
            Id = 6,
            Nombre = "Remera Nickz Candy",
            Descripcion = "Remera Nickz Chicle (Candy Edition)",
            Precio = 20000,
            Stock = 15,
            ImagenUrl = "NickzChicle.jpg"
        },
        new Producto
        {
            Id = 7,
            Nombre = "Remera Nickz Invertida",
            Descripcion = "Remera Nickz Colores Invertidos (Ultra Limited Edition)",
            Precio = 100000,
            Stock = 3,
            ImagenUrl = "NickzInvertida.jpg"
        },
        new Producto
        {
            Id = 8,
            Nombre = "Zapatillas Tommy Hilfiger",
            Descripcion = "Zapatillas Tommy (B&N Edition)",
            Precio = 200000,
            Stock = 15,
            ImagenUrl = "ZapatillasTommy.jpg"
        },
        new Producto
        {
            Id = 9,
            Nombre = "Pod Desechable Elfbar",
            Descripcion = "Elfbar Ice King (40.000 Puffs)",
            Precio = 35000,
            Stock = 200,
            ImagenUrl = "Elfbar.jpg"
        },
        new Producto
        {
            Id = 10,
            Nombre = "Pod Desechable Lost Mary",
            Descripcion = "Lost Mary Mixer doble sabor (30.000 Puffs)",
            Precio = 35000,
            Stock = 200,
            ImagenUrl = "LostMaryMixer.jpg"
        },
        new Producto
        {
            Id = 11,
            Nombre = "Cartera Louis Vuitton",
            Descripcion = "Cartera Louis Vuitton de Dama",
            Precio = 500000,
            Stock = 3,
            ImagenUrl = "BolsoLv.jpg"
        },
    };

    public static Dictionary<Guid, List<ItemCarrito>> Carritos { get; set; } = new();
    public static List<Compra> Compras { get; set; } = new();

    private static string StockFile => "stock.json";
    private static string ComprasFile => "compras.json";

    public static void GuardarStock()
    {
        var stockList = Productos.Select(p => new StockData { Id = p.Id, Stock = p.Stock }).ToList();
        File.WriteAllText(StockFile, JsonSerializer.Serialize(stockList));
    }

    public static void CargarStock()
    {
        if (!File.Exists(StockFile)) return;
        var stockList = JsonSerializer.Deserialize<List<StockData>>(File.ReadAllText(StockFile));
        if (stockList is null) return;
        foreach (var stock in stockList)
        {
            var prod = Productos.FirstOrDefault(p => p.Id == stock.Id);
            if (prod != null) prod.Stock = stock.Stock;
        }
    }

    public static void GuardarCompras()
    {
        File.WriteAllText(ComprasFile, JsonSerializer.Serialize(Compras));
    }

    public static void CargarCompras()
    {
        if (!File.Exists(ComprasFile)) return;
        var compras = JsonSerializer.Deserialize<List<Compra>>(File.ReadAllText(ComprasFile));
        if (compras is not null)
            Compras = compras;
    }

    private class StockData
    {
        public int Id { get; set; }
        public int Stock { get; set; }
    }
}