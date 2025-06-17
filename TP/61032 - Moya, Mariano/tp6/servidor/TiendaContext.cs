using Microsoft.EntityFrameworkCore;
using Servidor.Models;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<ItemCarrito> ItemsCarrito => Set<ItemCarrito>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemCompra>()
            .HasOne(i => i.Producto)
            .WithMany(p => p.ItemsCompra)
            .HasForeignKey(i => i.ProductoId);

        modelBuilder.Entity<ItemCompra>()
            .HasOne(i => i.Compra)
            .WithMany(c => c.ItemsCompra)
            .HasForeignKey(i => i.CompraId);

        // Relación ItemCarrito - Producto
        modelBuilder.Entity<ItemCarrito>()
            .HasOne(i => i.Producto)
            .WithMany()
            .HasForeignKey(i => i.ProductoId);

        // Relación ItemCarrito - Carrito
        modelBuilder.Entity<ItemCarrito>()
            .HasOne(i => i.Carrito)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CarritoId);
    }
}
