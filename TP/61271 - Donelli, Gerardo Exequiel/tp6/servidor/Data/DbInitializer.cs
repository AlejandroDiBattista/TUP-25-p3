using servidor.Models;

namespace servidor.Data;

public static class ProductosSeed
{ 
    public static void Initialize(TiendaContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Productos.Any())
        {
            var productos = ProductosSeed.ObtenerProductos();
            context.Productos.AddRange(productos);
            context.SaveChanges();
        }
    }

    public static List<Producto> ObtenerProductos()
    {
        return new List<Producto>
        {
            new Producto
            {
                Nombre = "Arroz Largo Fino",
                Descripcion = "Arroz largo fino calidad 00000, paquete de 1kg",
                Precio = 999.99M,
                Stock = 500,
                ImagenUrl = "images/arroz-largo-fino-pulido.webp"
            },
            new Producto
            {
                Nombre = "Aceite de Girasol",
                Descripcion = "Aceite de girasol 100% puro, botella de 1.5L",
                Precio = 123.99M,
                Stock = 0,
                ImagenUrl = "images/Aceite-girasol.webp"
            },
            new Producto
            {
                Nombre = "Leche Entera",
                Descripcion = "Leche entera larga vida, 1 litro",
                Precio = 541.49M,
                Stock = 400,
                ImagenUrl = "images/Leche-Entera-Larga-Vida-1-Lt-_1.webp"
            },
            new Producto
            {
                Nombre = "Fideos Spaghetti",
                Descripcion = "Fideos tipo spaghetti, paquete de 500g",
                Precio = 100.20M,
                Stock = 600,
                ImagenUrl = "images/fideos-Lucchetti.webp"
            },
            new Producto
            {
                Nombre = "Harina de Trigo",
                Descripcion = "Harina de trigo común 000, paquete de 1kg",
                Precio = 188.10M,
                Stock = 450,
                ImagenUrl = "images/Harina-trigo-000.webp"
            },
            new Producto
            {
                Nombre = "Azúcar Blanca",
                Descripcion = "Azúcar refinada blanca, paquete de 1kg",
                Precio = 1999.35M,
                Stock = 550,
                ImagenUrl = "images/azucar-blaco.webp"
            },
            new Producto
            {
                Nombre = "Yerba Mate",
                Descripcion = "Yerba mate con palo tradicional, paquete de 1kg",
                Precio = 3777.49M,
                Stock = 280,
                ImagenUrl = "images/Yerba_Mate_Elaborada_con_Palo,_1_kg.jpg"
            },
            new Producto
            {
                Nombre = "Galletitas Dulces",
                Descripcion = "Galletitas dulces surtidas",
                Precio = 152.75M,
                Stock = 350,
                ImagenUrl = "images/Surtido-galletas.png"
            },
            new Producto
            {
                Nombre = "Huevos",
                Descripcion = "Maple de huevos frescos blancos, docena",
                Precio = 220.90M,
                Stock = 200,
                ImagenUrl = "images/huevos.png"
            },
            new Producto
            {
                Nombre = "Pan Lactal",
                Descripcion = "Pan lactal blanco sin corteza, 500g",
                Precio = 252.50M,
                Stock = 200,
                ImagenUrl = "images/Pan-lactal.webp"
            }
        };
    }
}
