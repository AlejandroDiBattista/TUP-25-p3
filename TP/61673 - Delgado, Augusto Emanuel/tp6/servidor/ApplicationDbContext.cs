using Microsoft.EntityFrameworkCore;
using servidor.Models; 

namespace servidor
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Compra> Compras { get; set; } = null!;
        public DbSet<ItemCompra> ItemsCompra { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la relación entre Compra e ItemCompra
            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Compra)
                .WithMany(c => c.ItemsCompra)
                .HasForeignKey(ic => ic.CompraId);

            
            // Carga inicial de productos
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Smart TV 4K", Descripcion = "Televisor de 50 pulgadas con resolución 4K y HDR.", Precio = 450000m, Stock = 10, ImagenUrl = "/images/tv.jpg" },
                new Producto { Id = 2, Nombre = "Smartphone X10", Descripcion = "Teléfono de última generación con cámara de 108MP y pantalla AMOLED.", Precio = 300000m, Stock = 25, ImagenUrl = "/images/smartphone.jpg" },
                new Producto { Id = 3, Nombre = "Auriculares Bluetooth", Descripcion = "Auriculares inalámbricos con cancelación de ruido activa.", Precio = 50000m, Stock = 50, ImagenUrl = "/images/auriculares.jpg" },
                new Producto { Id = 4, Nombre = "Teclado Mecánico RGB", Descripcion = "Teclado gaming con switches Cherry MX y retroiluminación RGB personalizable.", Precio = 75000m, Stock = 15, ImagenUrl = "/images/teclado.jpg" },
                new Producto { Id = 5, Nombre = "Mouse Gamer Inalámbrico", Descripcion = "Mouse con sensor óptico de alta precisión y batería de larga duración.", Precio = 30000m, Stock = 20, ImagenUrl = "/images/mouse.jpg" },
                new Producto { Id = 6, Nombre = "Monitor Curvo 27''", Descripcion = "Monitor QHD con tasa de refresco de 144Hz y panel VA curvo.", Precio = 200000m, Stock = 8, ImagenUrl = "/images/monitor.jpg" },
                new Producto { Id = 7, Nombre = "Webcam Full HD", Descripcion = "Cámara web con micrófono integrado, ideal para videollamadas y streaming.", Precio = 25000m, Stock = 30, ImagenUrl = "/images/webcam.jpg" },
                new Producto { Id = 8, Nombre = "Disco SSD 1TB", Descripcion = "Unidad de estado sólido NVMe PCIe Gen4 para almacenamiento ultrarrápido.", Precio = 90000m, Stock = 12, ImagenUrl = "/images/ssd.jpg" },
                new Producto { Id = 9, Nombre = "Router Wi-Fi 6", Descripcion = "Router de doble banda con tecnología Wi-Fi 6 para conexiones ultrarrápidas y estables.", Precio = 60000m, Stock = 18, ImagenUrl = "/images/router.jpg" },
                new Producto { Id = 10, Nombre = "Impresora Multifunción", Descripcion = "Impresora, escáner y copiadora con conectividad Wi-Fi y impresión a doble cara.", Precio = 110000m, Stock = 7, ImagenUrl = "/images/impresora.jpg" }
            );
        }
    }
}