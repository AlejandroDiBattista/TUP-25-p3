using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ItemCarrito> ItemsCarrito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Coca Cola", Descripcion = "Botella 1.5L", Precio = 900, Stock = 50, ImagenUrl = "/imagenes/coca.jpg" },
                new Producto { Id = 2, Nombre = "Pepsi", Descripcion = "Botella 1.5L", Precio = 850, Stock = 30, ImagenUrl = "/imagenes/pepsi.jpg" },
                new Producto { Id = 3, Nombre = "Notebook HP", Descripcion = "8GB RAM, 512GB SSD", Precio = 420000, Stock = 10, ImagenUrl = "/imagenes/hp.jpg" },
                new Producto { Id = 4, Nombre = "Mouse Logitech", Descripcion = "Inalámbrico", Precio = 12000, Stock = 100, ImagenUrl = "/imagenes/mouse.jpg" },
                new Producto { Id = 5, Nombre = "Auriculares Sony", Descripcion = "Bluetooth", Precio = 25000, Stock = 25, ImagenUrl = "/imagenes/sony.jpg" },
                new Producto { Id = 6, Nombre = "Teclado Redragon", Descripcion = "Mecánico", Precio = 18000, Stock = 40, ImagenUrl = "/imagenes/teclado.jpg" },
                new Producto { Id = 7, Nombre = "Fanta", Descripcion = "Botella 2L", Precio = 870, Stock = 60, ImagenUrl = "/imagenes/fanta.jpg" },
                new Producto { Id = 8, Nombre = "Heladera Samsung", Descripcion = "No Frost", Precio = 720000, Stock = 5, ImagenUrl = "/imagenes/heladera.jpg" },
                new Producto { Id = 9, Nombre = "Cargador Tipo-C", Descripcion = "Samsung 25W", Precio = 7500, Stock = 80, ImagenUrl = "/imagenes/cargador.jpg" },
                new Producto { Id = 10, Nombre = "Smart TV LG", Descripcion = "50 pulgadas 4K", Precio = 580000, Stock = 7, ImagenUrl = "/imagenes/tvlg.jpg" }
            );
        }
    }
}
