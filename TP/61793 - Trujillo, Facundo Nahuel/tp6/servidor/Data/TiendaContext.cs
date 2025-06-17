using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;

namespace Servidor.Data
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
            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Producto)
                .WithMany()
                .HasForeignKey(ic => ic.ProductoId);

            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Compra)
                .WithMany(c => c.Items)
                .HasForeignKey(ic => ic.CompraId);

            modelBuilder.Entity<ItemCarrito>()
                .HasOne(ic => ic.Carrito)
                .WithMany(c => c.Items)
                .HasForeignKey(ic => ic.CarritoId);

            modelBuilder.Entity<ItemCarrito>()
                .HasOne(ic => ic.Producto)
                .WithMany()
                .HasForeignKey(ic => ic.ProductoId);
        }
    }
}