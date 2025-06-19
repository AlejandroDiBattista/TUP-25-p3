using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Compra> Compras { get; set; } = null!;
        public DbSet<ItemCompra> ItemsCompra { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Compra)
                .WithMany(c => c.ItemsCompra)
                .HasForeignKey(ic => ic.CompraId);
        }
    }
}
