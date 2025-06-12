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
                new Producto{Nombre="Zapatilla Urbana Clásica", Descripcion="Comodidad y estilo para el día a día.", Precio=75.00m, Stock=50, ImagenUrl="/images/Productos/zapatilla_urbana_clasica.jpg"},
                new Producto{Nombre="Botín de Cuero Elegante", Descripcion="Ideal para ocasiones formales, durabilidad garantizada.", Precio=120.00m, Stock=30, ImagenUrl="/images/Productos/botin_cuero_elegante.jpg"},
                new Producto{Nombre="Sandalia de Verano", Descripcion="Frescura y ligereza para tus días soleados.", Precio=45.00m, Stock=70, ImagenUrl="/images/Productos/sandalia_verano.jpg"},
                new Producto{Nombre="Deportivo de Running", Descripcion="Máximo rendimiento y amortiguación para corredores.", Precio=90.00m, Stock=40, ImagenUrl="/images/Productos/deportivo_running.jpg"},
                new Producto{Nombre="Zapato Oxford Casual", Descripcion="Versatilidad para combinar con cualquier atuendo.", Precio=85.00m, Stock=25, ImagenUrl="/images/Productos/zapato_oxford_casual.jpg"},
                new Producto{Nombre="Bota de Montaña Impermeable", Descripcion="Protección y agarre para tus aventuras al aire libre.", Precio=150.00m, Stock=20, ImagenUrl="/images/Productos/bota_montana_impermeable.jpg"},
                new Producto{Nombre="Mocasín Confort", Descripcion="Suavidad y adaptabilidad para un uso prolongado.", Precio=60.00m, Stock=60, ImagenUrl="/images/Productos/mocasin_confort.jpg"},
                new Producto{Nombre="Stiletto Negro Clásico", Descripcion="Elegancia atemporal para complementar tu look de noche.", Precio=100.00m, Stock=35, ImagenUrl="/images/Productos/stiletto_negro_clasico.jpg"},
                new Producto{Nombre="Zapatilla de Skate", Descripcion="Agarre y resistencia para trucos y uso diario.", Precio=70.00m, Stock=45, ImagenUrl="/images/Productos/zapatilla_skate.jpg"},
                new Producto{Nombre="Alpargata de Yute", Descripcion="Estilo rústico y natural para un toque veraniego.", Precio=35.00m, Stock=80, ImagenUrl="/images/Productos/alpargata_yute.jpg"}
            };

            foreach (Producto p in productos)
            {
                context.Productos.Add(p);
            }
            context.SaveChanges();
        }
    }
}
