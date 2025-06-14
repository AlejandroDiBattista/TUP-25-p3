using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Celular A", Descripcion = "Celular gama media", Precio = 120000, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 2, Nombre = "Celular B", Descripcion = "Celular gama alta", Precio = 250000, Stock = 8, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 3, Nombre = "Auriculares", Descripcion = "Auriculares inalámbricos", Precio = 15000, Stock = 20, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 4, Nombre = "Cargador", Descripcion = "Cargador rápido", Precio = 5000, Stock = 15, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 5, Nombre = "Notebook", Descripcion = "Notebook 15''", Precio = 350000, Stock = 5, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 6, Nombre = "Mouse", Descripcion = "Mouse inalámbrico", Precio = 3000, Stock = 25, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 7, Nombre = "Teclado", Descripcion = "Teclado mecánico", Precio = 8000, Stock = 12, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 8, Nombre = "Monitor", Descripcion = "Monitor 24''", Precio = 60000, Stock = 7, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 9, Nombre = "Gaseosa", Descripcion = "Botella 2L", Precio = 1200, Stock = 30, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 10, Nombre = "Cable USB", Descripcion = "Cable USB-C", Precio = 2000, Stock = 18, ImagenUrl = "https://via.placeholder.com/150" }
            );
        }
    }
}
