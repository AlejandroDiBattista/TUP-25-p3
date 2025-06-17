using Microsoft.EntityFrameworkCore;
using servidor.ModeloDatos;

namespace servidor.ModeloDatos
{
    public class TiendaContexto : DbContext
    {
        // Constructor que recibe opciones de configuración para la base de datos
        public TiendaContexto(DbContextOptions<TiendaContexto> options) : base(options) { }

        // representan las tablas en la base de datos
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación:Producto-ItemCompra (1 a M)
            modelBuilder.Entity<Producto>()
                .HasMany(p => p.ItemsCompra)
                .WithOne(i => i.Producto)
                .HasForeignKey(i => i.ProductoId);

            // Relación:Compra-ItemCompra (1 a M)
            modelBuilder.Entity<Compra>()
                .HasMany(c => c.ItemsCompra)
                .WithOne(i => i.Compra)
                .HasForeignKey(i => i.CompraId);

            
            
        // Inserta datos iniciales para la tabla Productos
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Celular Samsung A06", Descripcion = "Pantalla 6.6'' - 128GB - 4GB RAM", Precio = 280000, Stock = 15, ImagenUrl = "img/a.jpg" },
                new Producto { Id = 2, Nombre = "iPhone 15 Pro Max", Descripcion = "Smartphone de Apple con chip A17 Pro y cámara avanzada", Precio = 1950000, Stock = 30, ImagenUrl = "img/iphone.jpg" },
                new Producto { Id = 3, Nombre = "Apple MacBook Air M3", Descripcion = "Notebook liviana con procesador Apple Silicon M3", Precio = 2100000, Stock = 15, ImagenUrl = "img/macbook-air.jpg" },
                new Producto { Id = 4, Nombre = "PlayStation 5 Slim", Descripcion = "Consola de videojuegos de Sony con diseño más delgado", Precio = 950000, Stock = 25, ImagenUrl = "img/PlayStation5.jpg" },
                new Producto { Id = 5, Nombre = "Xbox Series X", Descripcion = "Consola de Microsoft con 1 TB de almacenamiento y juegos 4K", Precio = 870000, Stock = 12, ImagenUrl = "img/Microsoft-Xbox-Series-X.jpg" },
                new Producto { Id = 6, Nombre = "Notebook HP 14", Descripcion = "Intel i5 - 8GB RAM - 256GB SSD", Precio = 385000, Stock = 5, ImagenUrl = "img/notebook-hp-.jpg" },
                new Producto { Id = 7, Nombre = "Silla Gamer", Descripcion = "Ergonómica, reclinable, color negro y rojo", Precio = 115000, Stock = 7, ImagenUrl = "img/sillaGamer.jpg" },
                new Producto { Id = 8, Nombre = "Apple Watch Series 9", Descripcion = "Reloj inteligente con sensores de salud y chip S9.", Precio = 420000, Stock = 50, ImagenUrl = "img/aple-watch.jpg" },
                new Producto { Id = 9, Nombre = "Monitor LED 24", Descripcion = "Full HD - HDMI - VGA", Precio = 95000, Stock = 10, ImagenUrl = "img/monitor.jpg" },
                new Producto { Id = 10, Nombre = "AirPods Pro 2 (USB-C)", Descripcion = "Auriculares con cancelación de ruido y sonido espacial", Precio = 127000, Stock = 18, ImagenUrl = "img/AirPods-si.jpg" }
            );
        }
    }
}
