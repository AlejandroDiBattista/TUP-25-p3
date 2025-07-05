using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);

    //     // Relación Compra-ItemCompra
    //     modelBuilder.Entity<Compra>()
    //         .HasMany(c => c.Items)
    //         .WithOne(i => i.Compra)
    //         .HasForeignKey(i => i.CompraId);

    //     // Relación Producto-ItemCompra
    //     modelBuilder.Entity<ItemCompra>()
    //         .HasOne(i => i.Producto)
    //         .WithMany()
    //         .HasForeignKey(i => i.ProductoId);
    // }
}