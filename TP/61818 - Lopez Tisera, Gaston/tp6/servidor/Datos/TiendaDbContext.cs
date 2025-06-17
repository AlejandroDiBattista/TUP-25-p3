using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;

namespace Servidor.Datos
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Producto)
                .WithMany(p => p.ItemsCompra)
                .HasForeignKey(ic => ic.ProductoId);

            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Compra)
                .WithMany(c => c.ItemsCompra)
                .HasForeignKey(ic => ic.CompraId);
        }
    }
}