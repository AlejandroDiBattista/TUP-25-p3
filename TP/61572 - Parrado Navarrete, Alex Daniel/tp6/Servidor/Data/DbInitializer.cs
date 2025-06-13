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
            new Producto { Nombre = "Celular XYZ", Descripcion = "Smartphone gama media", Precio = 19900m, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Auriculares ABC", Descripcion = "Inalámbricos bluetooth", Precio = 4500m, Stock = 15, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Cargador USB-C", Descripcion = "Carga rápida 20W", Precio = 1200m, Stock = 20, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Funda para celular", Descripcion = "Protección silicona", Precio = 800m, Stock = 25, ImagenUrl = "https://via.placeholder.com/150" },  
            new Producto { Nombre = "Gaseosa 2L", Descripcion = "Sabor cola", Precio = 300m, Stock = 30, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Cámara web", Descripcion = "Full HD 1080p", Precio = 5500m, Stock = 8, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Mouse inalámbrico", Descripcion = "Ergonómico", Precio = 2000m, Stock = 12, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Teclado mecánico", Descripcion = "Switch rojo", Precio = 7500m, Stock = 5, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Tarjeta SD 128GB", Descripcion = "Clase 10", Precio = 3500m, Stock = 18, ImagenUrl = "https://via.placeholder.com/150" },
            new Producto { Nombre = "Monitor 24", Descripcion = "LED Full HD", Precio = 25000m, Stock = 7, ImagenUrl = "https://via.placeholder.com/150" }
        };
        context.Productos.AddRange(products);
        context.SaveChanges();
    }
}