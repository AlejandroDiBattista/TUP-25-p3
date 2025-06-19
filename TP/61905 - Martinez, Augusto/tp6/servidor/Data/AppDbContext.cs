using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaItem> VentaItems { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Configuración de relaciones con `Cascade Delete`
            modelBuilder.Entity<VentaItem>()
                .HasOne(v => v.Producto)
                .WithMany(p => p.VentaItems)
                .HasForeignKey(v => v.ProductoId)
                .OnDelete(DeleteBehavior.Restrict); // 🔥 Evita borrar productos si tienen ventas asociadas

            modelBuilder.Entity<VentaItem>()
                .HasOne(v => v.Venta)
                .WithMany(v => v.VentaItems)
                .HasForeignKey(v => v.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarritoItem>()
                .HasOne(ci => ci.Carrito)
                .WithMany(c => c.CarritoItems)
                .HasForeignKey(ci => ci.CarritoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Carrito>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Venta>()
                .HasKey(v => v.Id);

            // ✅ Agregar índices en claves foráneas para mejorar rendimiento
            modelBuilder.Entity<VentaItem>()
                .HasIndex(v => v.ProductoId);

            modelBuilder.Entity<CarritoItem>()
                .HasIndex(ci => ci.CarritoId);

            // ✅ Asegurar que `Id` en `Producto` es autoincremental
            modelBuilder.Entity<Producto>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
