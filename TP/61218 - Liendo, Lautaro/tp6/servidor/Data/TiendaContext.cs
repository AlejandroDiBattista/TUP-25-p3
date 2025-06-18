// Data/TiendaContext.cs
using Microsoft.EntityFrameworkCore;
using servidor.Models; // Importa los nuevos modelos

namespace servidor.Data
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> opcionesContexto) : base(opcionesContexto) { }
        
        // Cambiamos los nombres de los DbSets para que reflejen los nuevos nombres de las clases
        public DbSet<ArticuloInventario> InventarioArticulos { get; set; } // Antes Productos
        public DbSet<RegistroCompra> HistorialCompras { get; set; } // Antes Compras
        public DbSet<DetalleCompra> DetallesDeCompras { get; set; } // Antes ItemsCarrito

        // Puedes añadir configuraciones adicionales aquí si es necesario
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Opcional: configurar nombres de tablas si quieres que sean diferentes al DbSet
            // modelBuilder.Entity<ArticuloInventario>().ToTable("Articulos");
            // modelBuilder.Entity<RegistroCompra>().ToTable("Compras");
            // modelBuilder.Entity<DetalleCompra>().ToTable("ItemsDeCompra");

            // Configurar la relación entre RegistroCompra y DetalleCompra
            modelBuilder.Entity<RegistroCompra>()
                .HasMany(rc => rc.Detalles)
                .WithOne()
                .HasForeignKey(dc => dc.RegistroCompraId);

            base.OnModelCreating(modelBuilder);
        }
    }
}