using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<ItemCarrito> ItemsCarrito { get; set; }    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Productos
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Descripcion).IsRequired().HasMaxLength(1000);
            entity.Property(p => p.Precio).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(p => p.Stock).IsRequired();
            entity.Property(p => p.ImagenUrl).IsRequired().HasMaxLength(500);
            entity.HasIndex(p => p.Nombre);
        });

        // Compras
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Fecha).IsRequired();
            entity.Property(c => c.Total).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(c => c.NombreCliente).IsRequired().HasMaxLength(100);
            entity.Property(c => c.ApellidoCliente).IsRequired().HasMaxLength(100);
            entity.Property(c => c.EmailCliente).IsRequired().HasMaxLength(200);
            entity.HasIndex(c => c.Fecha);
            entity.HasIndex(c => c.EmailCliente);
        });

        // Items de Compra
        modelBuilder.Entity<ItemCompra>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Cantidad).IsRequired();
            entity.Property(i => i.PrecioUnitario).IsRequired().HasColumnType("decimal(18,2)");
            
            entity.HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.Compra)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(i => i.ProductoId);
            entity.HasIndex(i => i.CompraId);
        });

        // Carritos
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(c => c.Id);
        });

        // Items de Carrito
        modelBuilder.Entity<ItemCarrito>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Cantidad).IsRequired();

            entity.HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.Carrito)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CarritoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(i => i.ProductoId);
            entity.HasIndex(i => i.CarritoId);
        });
    }
}
