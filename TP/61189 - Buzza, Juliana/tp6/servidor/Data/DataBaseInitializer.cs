using servidor.Models; 
using Microsoft.EntityFrameworkCore;

namespace servidor.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated(); 

            if (context.Productos.Any())
            {
                return; 
            }

            var productos = new Producto[]
            {
                new Producto{Nombre="Chata Mary jane Pine", Descripcion="Chata mary jane 100% cuero color negro liso, brillo medio.", Precio=75.00m, Stock=50, ImagenUrl="/Imagenes/Productos/chatamaryjane.jpg"},
                new Producto{Nombre="Bota Cata Caña Alta", Descripcion="Bota caña alta de cuero color negro liso y leve brillo.", Precio=120.00m, Stock=30, ImagenUrl="/Imagenes/Productos/botacatanegro.jpg"},
                new Producto{Nombre="Bota Cata Caña Alta Marron", Descripcion="Bota caña alta de color marron oxidado y leve brillo.", Precio=45.00m, Stock=70, ImagenUrl="/Imagenes/Productos/botacatamarron.jpg"},
                new Producto{Nombre="Sueco Andi Negro", Descripcion="Sueco 100% de cuero liso color negro brillante.", Precio=90.00m, Stock=40, ImagenUrl="/Imagenes/Productos/suecoandinegro.jpg"},
                new Producto{Nombre="Sueco Andi Merlot", Descripcion="Sueco 100% cuero de cuero liso color bordo intenso.", Precio=85.00m, Stock=25, ImagenUrl="/Imagenes/Productos/suecoandi.jpg"},
                new Producto{Nombre="Mocasin Zoe Black", Descripcion="Mocasin 100% cuero liso, brillo medio, color negro.", Precio=150.00m, Stock=20, ImagenUrl="/Imagenes/Productos/mocasinzoenegro.jpg"},
                new Producto{Nombre="Sandalia Malibu Black", Descripcion="Sandalias 100% de cuero negro liso.", Precio=60.00m, Stock=60, ImagenUrl="/Imagenes/Productos/sandaliasnegro.jpg"},
                new Producto{Nombre="Ojota Pine Black", Descripcion="Sandalia ojota de cuero liso.", Precio=100.00m, Stock=35, ImagenUrl="/Imagenes/Productos/ojotacuero.jpg"},
                new Producto{Nombre="Franciscana Taylor Óxido", Descripcion="Sandalia modelo franciscana de cuero efecto óxido", Precio=70.00m, Stock=45, ImagenUrl="/Imagenes/Productos/sandaliasoxido.jpg"},
                new Producto{Nombre="Bota China Cacao", Descripcion="Bota caña alta de cuero gamuzado color marron.", Precio=35.00m, Stock=80, ImagenUrl="/Imagenes/Productos/botachina.jpg"}
            };

            foreach (Producto p in productos)
            {
                context.Productos.Add(p);
            }
            context.SaveChanges();
        }
    }
}
