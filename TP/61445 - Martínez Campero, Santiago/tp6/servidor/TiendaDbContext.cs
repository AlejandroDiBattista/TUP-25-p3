using Microsoft.EntityFrameworkCore;
using Compartido; 
public class TiendaDbContext : DbContext
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ItemCompra>()
            .HasOne(ic => ic.Producto)
            .WithMany()
            .HasForeignKey(ic => ic.ProductoId);

        modelBuilder.Entity<ItemCompra>()
            .HasOne(ic => ic.Compra)
            .WithMany(c => c.Items) 
            .HasForeignKey(ic => ic.CompraId);
    }
}