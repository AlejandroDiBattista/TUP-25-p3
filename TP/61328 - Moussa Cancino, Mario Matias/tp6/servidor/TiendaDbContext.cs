using Microsoft.EntityFrameworkCore;

namespace servidor
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "iPhone 14 Pro", Descripcion = "El último smartphone de Apple con chip A16 Bionic.", Precio = 999.99m, Stock = 50, ImagenUrl = "https://laplatacells.com.ar/img/Public/1169/62057-producto-iphone-14-pro-space-black-pdp-image-position-1a-mxla.jpg" },
                new Producto { Id = 2, Nombre = "Samsung Galaxy S23 Ultra", Descripcion = "El buque insignia de Samsung con un S Pen integrado.", Precio = 1199.99m, Stock = 40, ImagenUrl = "https://images.samsung.com/is/image/samsung/p6pim/in/2302/gallery/in-galaxy-s23-s918-446812-sm-s918bzrcins-534868449?$684_547_PNG$" },
                new Producto { Id = 3, Nombre = "Google Pixel 7 Pro", Descripcion = "La magia de Google en un teléfono, con el chip Tensor G2.", Precio = 899.00m, Stock = 60, ImagenUrl = "https://www.alemaniacell.com/uploads/imagen-principal23306-1-1690494033.JPG" },
                new Producto { Id = 4, Nombre = "Funda de Silicona para iPhone", Descripcion = "Protección suave y elegante para tu iPhone.", Precio = 49.00m, Stock = 150, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/643/020/products/silicone-case-con-logo-iphone-16-pro-azul-oscuro-18c6317df9dedf6c3817260731913348-640-0.png" },
                new Producto { Id = 5, Nombre = "Cargador Rápido USB-C 30W", Descripcion = "Carga tu dispositivo a toda velocidad.", Precio = 35.50m, Stock = 200, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_903078-MLU77836658064_072024-O.webp" },
                new Producto { Id = 6, Nombre = "AirPods Pro (2da Gen)", Descripcion = "Cancelación de ruido activa y audio espacial.", Precio = 249.00m, Stock = 80, ImagenUrl = "https://ipoint.com.ar/25134-thickbox_default/apple-airpods-pro-2da-generacion.jpg" },
                new Producto { Id = 7, Nombre = "Protector de Pantalla de Vidrio", Descripcion = "Máxima protección contra rayones y golpes.", Precio = 25.00m, Stock = 300, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/078/254/products/full-glue-03-8c85bbc0b5da12d85016298455797656-640-0-972d15eba548f5d7f317107817449057-640-0.png" },
                new Producto { Id = 8, Nombre = "Samsung Galaxy Watch 5", Descripcion = "Monitor de salud avanzado y diseño moderno.", Precio = 279.99m, Stock = 70, ImagenUrl = "https://cdn.kemik.gt/2023/06/R-920-BLACK-SAMSUNG-1200X1200-1-1-768x768.-700x700.jpg" },
                new Producto { Id = 9, Nombre = "Batería Externa 10000mAh", Descripcion = "Nunca te quedes sin batería fuera de casa.", Precio = 45.00m, Stock = 120, ImagenUrl = "https://static.bidcom.com.ar/publicacionesML/productos/KCABLE03/1000x1000-KCABLE03.jpg" },
                new Producto { Id = 10, Nombre = "Soporte de Coche Magnético", Descripcion = "Mantén tu teléfono seguro y a la vista mientras conduces.", Precio = 22.99m, Stock = 180, ImagenUrl = "https://dcdn-us.mitiendanube.com/stores/002/611/582/products/soporte-auto-8ffc2a832392b199f917190750449559-1024-1024.webp" }
            );
        }
    }
}