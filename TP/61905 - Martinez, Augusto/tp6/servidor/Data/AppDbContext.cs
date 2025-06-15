using Microsoft.EntityFrameworkCore;
using servidor.Models;  

namespace servidor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Venta> Ventas { get; set; } 
        public DbSet<VentaItem> VentaItems { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔥 Configurar relaciones en la base de datos
            modelBuilder.Entity<VentaItem>()
                .HasOne(v => v.Producto) 
                .WithMany()
                .HasForeignKey(v => v.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VentaItem>()
                .HasOne(v => v.Venta)
                .WithMany(v => v.Items)
                .HasForeignKey(v => v.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarritoItem>()
                .HasOne(ci => ci.Carrito)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CarritoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Carrito>()
                .HasKey(c => c.Id); // ✅ Corrección: Movido aquí

            // 🔹 Opcional: Inicialización de productos si la tabla está vacía
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Laptop Dell XPS 13", Descripcion = "Ultrabook con Intel i7.", Precio = 1299.99m, Stock = 15, ImagenUrl = "/images/laptop.jpg" },
                new Producto { Id = 2, Nombre = "iPhone 14 Pro", Descripcion = "Smartphone con pantalla OLED.", Precio = 999.00m, Stock = 10, ImagenUrl = "/images/iphone.jpg" }
            );
        }
    }
}
