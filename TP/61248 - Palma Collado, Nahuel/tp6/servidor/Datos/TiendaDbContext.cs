using Microsoft.EntityFrameworkCore;
using TiendaOnline.Modelos;

namespace TiendaOnline.Datos
{
    public class CarritoDbContext : DbContext
    {
        public CarritoDbContext(DbContextOptions<CarritoDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Carrito> Compras => Set<Compra>();
        public DbSet<ItemDeCompra> ItemsDeCompra => Set<ItemDeCompra>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarritoProducto>()
                .HasKey(cp => new { cp.CarritoId, cp.ProductoId });
        }
    }
}