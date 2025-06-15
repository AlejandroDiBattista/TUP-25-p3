using Microsoft.EntityFrameworkCore;
using Servidor.Models;

namespace Servidor.Data;

public class TiendaDbContext : DbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración para Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Descripcion).HasMaxLength(500);
            entity.Property(p => p.ImagenUrl).HasMaxLength(500);
        });

        // Configuración para Compra
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Total).HasColumnType("decimal(18,2)");
            entity.Property(c => c.NombreCliente).IsRequired().HasMaxLength(100);
            entity.Property(c => c.ApellidoCliente).IsRequired().HasMaxLength(100);
            entity.Property(c => c.EmailCliente).IsRequired().HasMaxLength(200);
        });

        // Configuración para ItemCompra
        modelBuilder.Entity<ItemCompra>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.PrecioUnitario).HasColumnType("decimal(18,2)");

            // Relaciones
            entity.HasOne(i => i.Producto)
                  .WithMany(p => p.ItemsCompra)
                  .HasForeignKey(i => i.ProductoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.Compra)
                  .WithMany(c => c.Items)
                  .HasForeignKey(i => i.CompraId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Datos de ejemplo - Celulares y accesorios
        modelBuilder.Entity<Producto>().HasData(
            new Producto
            {
                Id = 1,
                Nombre = "iPhone 15 Pro",
                Descripcion = "Smartphone Apple iPhone 15 Pro con chip A17 Pro, pantalla Super Retina XDR de 6.1 pulgadas",
                Precio = 1299.99m,
                Stock = 15,
                ImagenUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 2,
                Nombre = "Samsung Galaxy S24 Ultra",
                Descripcion = "Smartphone Samsung Galaxy S24 Ultra con S Pen, pantalla Dynamic AMOLED 2X de 6.8 pulgadas",
                Precio = 1199.99m,
                Stock = 12,
                ImagenUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 3,
                Nombre = "Google Pixel 8 Pro",
                Descripcion = "Smartphone Google Pixel 8 Pro con chip Tensor G3, pantalla LTPO OLED de 6.7 pulgadas",
                Precio = 999.99m,
                Stock = 8,
                ImagenUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 4,
                Nombre = "AirPods Pro (3ª generación)",
                Descripcion = "Auriculares inalámbricos Apple AirPods Pro con cancelación activa de ruido",
                Precio = 249.99m,
                Stock = 25,
                ImagenUrl = "https://images.unsplash.com/photo-1606220588913-b3aacb4d2f46?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 5,
                Nombre = "Samsung Galaxy Watch 6",
                Descripcion = "Smartwatch Samsung Galaxy Watch 6 con GPS, monitor de salud y resistencia al agua",
                Precio = 329.99m,
                Stock = 18,
                ImagenUrl = "https://images.unsplash.com/photo-1551816230-ef5deaed4a26?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 6,
                Nombre = "Cargador Inalámbrico MagSafe",
                Descripcion = "Cargador inalámbrico MagSafe para iPhone con alineación magnética perfecta",
                Precio = 39.99m,
                Stock = 30,
                ImagenUrl = "https://images.unsplash.com/photo-1615526675159-e248c3021d3f?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 7,
                Nombre = "Funda Protectora Premium",
                Descripcion = "Funda protectora premium con protección militar y soporte para carga inalámbrica",
                Precio = 29.99m,
                Stock = 45,
                ImagenUrl = "https://images.unsplash.com/photo-1697008230017-c0a2a252b19a?q=80&w=1974&auto=format&fit=crop"
            },
            new Producto
            {
                Id = 8,
                Nombre = "Power Bank 20000mAh",
                Descripcion = "Batería externa de 20000mAh con carga rápida USB-C",
                Precio = 49.99m,
                Stock = 22,
                ImagenUrl = "https://images.unsplash.com/photo-1706275399494-fb26bbc5da63?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 9,
                Nombre = "Cable USB-C Premium",
                Descripcion = "Cable USB-C Premium de 2 metros con transferencia de datos ultra rápida hasta 10Gbps y carga rápida de 100W",
                Precio = 19.99m,
                Stock = 50,
                ImagenUrl = "https://images.unsplash.com/photo-1657181253444-66c4745d5a86?w=300&h=300&fit=crop"
            },
            new Producto
            {
                Id = 10,
                Nombre = "Mouse Inalámbrico",
                Descripcion = "Mouse inalámbrico ergonómico con sensor óptico de alta precisión y batería de larga duración",
                Precio = 34.99m,
                Stock = 15,
                ImagenUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=300&h=300&fit=crop"
            }
        );
    }
}
