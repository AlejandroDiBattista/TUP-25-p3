using Microsoft.EntityFrameworkCore;

namespace servidor.Modelos
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // productos de ejemploo
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Mouse Logitech", Descripcion = "Mouse óptico inalámbrico", Precio = 3500, Stock = 25, ImagenUrl = "mouse.jpg" },
                new Producto { Id = 2, Nombre = "Teclado Redragon", Descripcion = "Teclado mecánico RGB", Precio = 7800, Stock = 10, ImagenUrl = "teclado.jpg" }
                // hasta tener 10
            );
        }
    }
}
