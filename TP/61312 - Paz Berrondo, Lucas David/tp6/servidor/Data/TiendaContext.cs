using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data;

/// <summary>
/// Contexto de base de datos para la tienda online.
/// Maneja todas las operaciones de persistencia usando Entity Framework Core con SQLite.
/// </summary>
public class TiendaContext : DbContext
{
    /// <summary>
    /// Constructor que recibe las opciones de configuración del contexto.
    /// </summary>
    /// <param name="options">Opciones de configuración del DbContext</param>
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
    {
    }

    /// <summary>
    /// Tabla de productos en la base de datos.
    /// Contiene el catálogo completo de productos disponibles.
    /// </summary>
    public DbSet<Producto> Productos { get; set; }

    /// <summary>
    /// Tabla de compras confirmadas.
    /// Almacena el historial de todas las transacciones completadas.
    /// </summary>
    public DbSet<Compra> Compras { get; set; }

    /// <summary>
    /// Tabla de items de compra.
    /// Detalle de productos incluidos en cada compra.
    /// </summary>
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    /// <summary>
    /// Tabla de carritos temporales.
    /// Almacena carritos de compra antes de confirmar la compra.
    /// </summary>
    public DbSet<Carrito> Carritos { get; set; }

    /// <summary>
    /// Tabla de items de carrito.
    /// Productos agregados a carritos temporales.
    /// </summary>
    public DbSet<ItemCarrito> ItemsCarrito { get; set; }

    /// <summary>
    /// Configura el modelo de datos y las relaciones entre entidades.
    /// Se ejecuta cuando EF construye el modelo interno de la base de datos.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de EF</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            // Clave primaria
            entity.HasKey(p => p.Id);

            // Configuración de propiedades
            entity.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Descripcion)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(p => p.Precio)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Precisión para valores monetarios

            entity.Property(p => p.Stock)
                .IsRequired();

            entity.Property(p => p.ImagenUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Índices para mejorar consultas
            entity.HasIndex(p => p.Nombre);
        });

        // Configuración de la entidad Compra
        modelBuilder.Entity<Compra>(entity =>
        {
            // Clave primaria
            entity.HasKey(c => c.Id);

            // Configuración de propiedades
            entity.Property(c => c.Fecha)
                .IsRequired();

            entity.Property(c => c.Total)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(c => c.NombreCliente)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.ApellidoCliente)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.EmailCliente)
                .IsRequired()
                .HasMaxLength(200);

            // Índices
            entity.HasIndex(c => c.Fecha);
            entity.HasIndex(c => c.EmailCliente);
        });

        // Configuración de la entidad ItemCompra
        modelBuilder.Entity<ItemCompra>(entity =>
        {
            // Clave primaria
            entity.HasKey(i => i.Id);

            // Configuración de propiedades
            entity.Property(i => i.Cantidad)
                .IsRequired();

            entity.Property(i => i.PrecioUnitario)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Relación con Producto (muchos ItemCompra -> un Producto)
            entity.HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar producto si tiene items de compra

            // Relación con Compra (muchos ItemCompra -> una Compra)
            entity.HasOne(i => i.Compra)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CompraId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar items si se elimina la compra

            // Índices para mejorar consultas
            entity.HasIndex(i => i.ProductoId);
            entity.HasIndex(i => i.CompraId);
        });

        // Configuración de la entidad Carrito
        modelBuilder.Entity<Carrito>(entity =>
        {
            // Clave primaria
            entity.HasKey(c => c.Id);

            // El Id se genera automáticamente
        });

        // Configuración de la entidad ItemCarrito
        modelBuilder.Entity<ItemCarrito>(entity =>
        {
            // Clave primaria
            entity.HasKey(i => i.Id);

            // Configuración de propiedades
            entity.Property(i => i.Cantidad)
                .IsRequired();

            // Relación con Producto (muchos ItemCarrito -> un Producto)
            entity.HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con Carrito (muchos ItemCarrito -> un Carrito)
            entity.HasOne(i => i.Carrito)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CarritoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para mejorar consultas
            entity.HasIndex(i => i.ProductoId);
            entity.HasIndex(i => i.CarritoId);
        });
    }
}
