// Servidor/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using servidor.Models; // Namespace corregido

namespace servidor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compra>()
                .HasMany(c => c.Items)
                .WithOne(ic => ic.Compra)
                .HasForeignKey(ic => ic.CompraId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Producto)
                .WithMany() 
                .HasForeignKey(ic => ic.ProductoId)
                .OnDelete(DeleteBehavior.Restrict); 

            base.OnModelCreating(modelBuilder);
        }
    }
}