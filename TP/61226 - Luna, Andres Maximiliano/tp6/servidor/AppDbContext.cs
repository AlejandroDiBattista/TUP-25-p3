using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Compra)
                .WithMany(c => c.Items)
                .HasForeignKey(ic => ic.CompraId);

            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Producto)
                .WithMany(p => p.ItemsCompra)
                .HasForeignKey(ic => ic.ProductoId);

            modelBuilder.Entity<ItemOrden>()
            .HasOne(io => io.Orden)
            .WithMany(o => o.Items)
            .HasForeignKey(io => io.OrdenId);
        }


        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<ItemOrden> ItemsOrden { get; set; }

        

    }
}