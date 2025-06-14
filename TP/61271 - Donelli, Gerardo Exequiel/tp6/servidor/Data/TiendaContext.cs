using Microsoft.EntityFrameworkCore;

namespace servidor.Data;

public class TiendaContext : DbContext
{
    public DbSet<Models.Producto> Productos { get; set; }
    public DbSet<Models.Compra> Compras { get; set; }
    public DbSet<Models.ItemCompra> ItemsCompra { get; set; }

    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de Producto
        modelBuilder.Entity<Models.Producto>(p => {
            // Fluent API para configurar la entidad Producto
            
            //fluente es el que se usa para configurar las entidades
            p.Property(p => p.Nombre).IsRequired().HasMaxLength(100);//hasMaxLength establece la longitud máxima del campo
            p.Property(p => p.Descripcion).IsRequired().HasMaxLength(500);
            p.Property(p => p.Precio).IsRequired().HasPrecision(10, 2);//hasPrecision establece la precisión y escala del campo decimal
            p.Property(p => p.Stock).IsRequired();//isRequired indica que el campo es obligatorio
            p.Property(p => p.ImagenUrl).IsRequired().HasMaxLength(2000);
        });

        // Configuración de Compra
        modelBuilder.Entity<Models.Compra>(c => {
            c.Property(c => c.Fecha).IsRequired();
            c.Property(c => c.Total).IsRequired().HasPrecision(10, 2);
            c.Property(c => c.NombreCliente).IsRequired().HasMaxLength(100);
            c.Property(c => c.ApellidoCliente).IsRequired().HasMaxLength(100);
            c.Property(c => c.EmailCliente).IsRequired().HasMaxLength(100);
        });

        // Configuración de ItemCompra
        modelBuilder.Entity<Models.ItemCompra>(i => {
            i.Property(i => i.Cantidad).IsRequired();
            i.Property(i => i.PrecioUnitario).IsRequired().HasPrecision(10, 2);
            
            // Relaciones
            i.HasOne(i => i.Producto)//hasOne establece la relación de uno a muchos
             .WithMany(p => p.Items)//withMany establece la relación de muchos a uno
             .HasForeignKey(i => i.ProductoId)
             .OnDelete(DeleteBehavior.Restrict);

            i.HasOne(i => i.Compra)//hasOne establece la relación de uno a muchos
             .WithMany(c => c.Items)//withMany establece la relación de muchos a uno
             .HasForeignKey(i => i.CompraId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
