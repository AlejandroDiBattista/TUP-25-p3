using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed de productos de ejemplo
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Celular X", Descripcion = "Smartphone 6.5''", Precio = 150000, Stock = 10, ImagenUrl = "/img/celularx.jpeg" },
                new Producto { Id = 2, Nombre = "Auriculares Pro", Descripcion = "Bluetooth, cancelación de ruido", Precio = 35000, Stock = 15, ImagenUrl = "/img/auricularespro.jpeg" },
                new Producto { Id = 3, Nombre = "Notebook Ultra", Descripcion = "Intel i7, 16GB RAM", Precio = 420000, Stock = 5, ImagenUrl = "/img/notebookultra.jpeg" },
                new Producto { Id = 4, Nombre = "Mouse Gamer", Descripcion = "RGB, 12000 DPI", Precio = 12000, Stock = 20, ImagenUrl = "/img/mousegamer.jpeg" },
                new Producto { Id = 5, Nombre = "Teclado Mecánico", Descripcion = "Switch Blue, retroiluminado", Precio = 18000, Stock = 12, ImagenUrl = "/img/tecladomecanico.jpeg" },
                new Producto { Id = 6, Nombre = "Monitor 24''", Descripcion = "Full HD, IPS", Precio = 65000, Stock = 8, ImagenUrl = "/img/monitor24.jpeg" },
                new Producto { Id = 7, Nombre = "Cargador Rápido", Descripcion = "USB-C, 30W", Precio = 7000, Stock = 25, ImagenUrl = "/img/cargador.jpeg" },
                new Producto { Id = 8, Nombre = "Cable USB", Descripcion = "Tipo C, 1m", Precio = 2500, Stock = 30, ImagenUrl = "/img/cableusb.jpeg" },
                new Producto { Id = 9, Nombre = "Gaseosa 1.5L", Descripcion = "Bebida cola", Precio = 1200, Stock = 50, ImagenUrl = "/img/gaseosa.jpeg" },
                new Producto { Id = 10, Nombre = "Auriculares In-ear", Descripcion = "Cableados, 3.5mm", Precio = 4000, Stock = 18, ImagenUrl = "/img/auricularesinear.jpeg" }
            );
        }
    }
}
