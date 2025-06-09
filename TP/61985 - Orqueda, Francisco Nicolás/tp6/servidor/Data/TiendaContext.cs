using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico", Precio = 6500, Stock = 20, ImagenUrl = "https://example.com/mouse.jpg" },
                new Producto { Id = 2, Nombre = "Teclado Redragon", Descripcion = "Teclado mecánico RGB", Precio = 12000, Stock = 15, ImagenUrl = "https://example.com/teclado.jpg" },
                new Producto { Id = 3, Nombre = "Monitor Samsung", Descripcion = "Monitor 24 pulgadas", Precio = 55000, Stock = 10, ImagenUrl = "https://example.com/monitor.jpg" },
                new Producto { Id = 4, Nombre = "Auriculares JBL", Descripcion = "Auriculares Bluetooth", Precio = 14000, Stock = 18, ImagenUrl = "https://example.com/auriculares.jpg" },
                new Producto { Id = 5, Nombre = "Notebook Lenovo", Descripcion = "Notebook Ryzen 5", Precio = 280000, Stock = 5, ImagenUrl = "https://example.com/notebook.jpg" },
                new Producto { Id = 6, Nombre = "Impresora HP", Descripcion = "Impresora multifunción", Precio = 75000, Stock = 8, ImagenUrl = "https://example.com/impresora.jpg" },
                new Producto { Id = 7, Nombre = "Silla Gamer", Descripcion = "Silla ergonómica", Precio = 85000, Stock = 7, ImagenUrl = "https://example.com/silla.jpg" },
                new Producto { Id = 8, Nombre = "Pendrive Sandisk", Descripcion = "Pendrive 64GB", Precio = 3200, Stock = 25, ImagenUrl = "https://example.com/pendrive.jpg" },
                new Producto { Id = 9, Nombre = "Disco SSD Kingston", Descripcion = "SSD 480GB", Precio = 18000, Stock = 12, ImagenUrl = "https://example.com/ssd.jpg" },
                new Producto { Id = 10, Nombre = "Cámara Web Logitech", Descripcion = "Full HD 1080p", Precio = 21000, Stock = 6, ImagenUrl = "https://example.com/camara.jpg" }
            );
        }
    }
}
