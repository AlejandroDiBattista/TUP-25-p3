using Microsoft.EntityFrameworkCore;

namespace servidor.Modelos
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Semilla de productos (10 productos de ejemplo)
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Mouse Gamer", Descripcion = "Mouse RGB con 6 botones", Precio = 5000, Stock = 20, ImagenUrl = "/assets/Mouse_Gamer.jpeg" },
                new Producto { Id = 2, Nombre = "Teclado Mecánico", Descripcion = "Switch blue", Precio = 12000, Stock = 15, ImagenUrl = "/assets/TecladoGamerMecanico.png" },
                new Producto { Id = 3, Nombre = "Auriculares Inalámbricos", Descripcion = "Bluetooth 5.0", Precio = 8000, Stock = 10, ImagenUrl = "/assets/Auriculares_Inalambricos.jpg" },
                new Producto { Id = 4, Nombre = "Monitor 24''", Descripcion = "Full HD 75Hz", Precio = 35000, Stock = 8, ImagenUrl = "/assets/MonitorLed.jpeg" },
                new Producto { Id = 5, Nombre = "Silla Gamer", Descripcion = "Ergonómica", Precio = 45000, Stock = 5, ImagenUrl = "/assets/Silla_Gamer.jpg" },
                new Producto { Id = 6, Nombre = "Notebook 15''", Descripcion = "i5 10ma gen", Precio = 200000, Stock = 6, ImagenUrl = "/assets/Notebook_i5.jpg" },
                new Producto { Id = 7, Nombre = "Tablet", Descripcion = "Android 11", Precio = 50000, Stock = 7, ImagenUrl = "/assets/Tablet.jpg" },
                new Producto { Id = 8, Nombre = "Smartphone", Descripcion = "128GB", Precio = 95000, Stock = 10, ImagenUrl = "/assets/SmarthPhone.jpg" },
                new Producto { Id = 9, Nombre = "Webcam HD", Descripcion = "1080p", Precio = 7000, Stock = 12, ImagenUrl = "/assets/Webcam_hd.jpg" },
                new Producto { Id = 10, Nombre = "Disco SSD 1TB", Descripcion = "NVMe", Precio = 30000, Stock = 9, ImagenUrl = "/assets/DiscoSolido.jpg" }
            );
        }
    }
}