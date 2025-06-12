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
        
        modelBuilder.Entity<ItemCompra>(entity =>
        {
            entity.HasKey(ic => new { ic.CompraId, ic.ProductoId });

            entity.HasOne(ic => ic.Producto)
                .WithMany(p => p.ItemsCompra) 
                .HasForeignKey(ic => ic.ProductoId)
                .IsRequired();

            entity.HasOne(ic => ic.Compra)
                .WithMany(c => c.Items) 
                .HasForeignKey(ic => ic.CompraId)
                .IsRequired();
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.Property(c => c.Total).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
        });
    }
}