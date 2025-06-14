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
                new Producto{Nombre="CHATA MARY JANE PINE", Descripcion="Chata mary jane 100% cuero color negro liso, brillo medio.", Precio=80000m, Stock=10, ImagenUrl="/Imagenes/Productos/chatamaryjane.jpg"},
                new Producto{Nombre="BOTA CATA CAÑA ALTA", Descripcion="Bota caña alta de cuero color negro liso y leve brillo.", Precio=120000m, Stock=10, ImagenUrl="/Imagenes/Productos/botacatanegro.jpg"},
                new Producto{Nombre="BOTA CATA CAÑA ALTA MARRON", Descripcion="Bota caña alta de color marron oxidado y leve brillo.", Precio=120000m, Stock=10, ImagenUrl="/Imagenes/Productos/botacatamarron.jpg"},
                new Producto{Nombre="SUECO ANDI NEGRO", Descripcion="Sueco 100% de cuero liso color negro brillante.", Precio=90000m, Stock=10, ImagenUrl="/Imagenes/Productos/suecoandinegro.jpg"},
                new Producto{Nombre="SUECO ANDI MERLOT", Descripcion="Sueco 100% cuero de cuero liso color bordo intenso.", Precio=90000m, Stock=10, ImagenUrl="/Imagenes/Productos/suecoandi.jpg"},
                new Producto{Nombre="MOCASIN ZOE BLACK", Descripcion="Mocasin 100% cuero liso, brillo medio, color negro.", Precio=80000m, Stock=10, ImagenUrl="/Imagenes/Productos/mocasinzoenegro.jpg"},
                new Producto{Nombre="SANDALIA MALIBU BLACK", Descripcion="Sandalias 100% de cuero negro liso.", Precio=75000m, Stock=10, ImagenUrl="/Imagenes/Productos/sandaliasnegro.jpg"},
                new Producto{Nombre="OJOTA PINE BLACK", Descripcion="Sandalia ojota de cuero liso.", Precio=60000m, Stock=10, ImagenUrl="/Imagenes/Productos/ojotacuero.jpg"},
                new Producto{Nombre="FRANCISCANA TAYLOR ÓXIDO", Descripcion="Sandalia modelo franciscana de cuero efecto óxido", Precio=80000m, Stock=10, ImagenUrl="/Imagenes/Productos/sandaliasoxido.jpg"},
                new Producto{Nombre="BOTA CHINA CACAO", Descripcion="Bota caña alta de cuero gamuzado color marron.", Precio=120000m, Stock=10, ImagenUrl="/Imagenes/Productos/botachina.jpg"}
            };

            foreach (Producto p in productos)
            {
                context.Productos.Add(p);
            }
            context.SaveChanges();
        }
    }
}
