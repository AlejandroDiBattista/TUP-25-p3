using Microsoft.EntityFrameworkCore;
using shared.Models;

namespace servidor.Data;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<ItemCarritoEntity> ItemsCarrito { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ImagenUrl).IsRequired().HasMaxLength(500);
        });

        // Configuración de Compra
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NombreCliente).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ApellidoCliente).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EmailCliente).IsRequired().HasMaxLength(200);
        });

        // Configuración de ItemCompra
        modelBuilder.Entity<ItemCompra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.CompraId).HasMaxLength(36);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
            
            entity.HasOne<Compra>()
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CompraId);
                
            entity.HasOne<Producto>()
                .WithMany()
                .HasForeignKey(e => e.ProductoId);
        });

        // Configuración de Carrito
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
        });

        // Configuración de ItemCarritoEntity
        modelBuilder.Entity<ItemCarritoEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.CarritoId).HasMaxLength(36);
            
            entity.HasOne<Carrito>()
                .WithMany()
                .HasForeignKey(e => e.CarritoId);
                
            entity.HasOne<Producto>()
                .WithMany()
                .HasForeignKey(e => e.ProductoId);
        });
    }
}

public class Carrito
{
    public string Id { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

public class ItemCarritoEntity
{
    public string Id { get; set; } = string.Empty;
    public string CarritoId { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}