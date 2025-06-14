// servidor/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using servidor.Models; // Asegúrate de incluir este using

namespace servidor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de las relaciones
            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Producto)
                .WithMany()
                .HasForeignKey(ic => ic.ProductoId);

            modelBuilder.Entity<ItemCompra>()
                .HasOne(ic => ic.Compra)
                .WithMany(c => c.Items)
                .HasForeignKey(ic => ic.CompraId);

            // Carga de datos iniciales de los perfumes (¡ACTUALIZA LAS IMAGEN_URLS!)
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Chanel N°5", Descripcion = "Clásico e icónico, floral-aldehídico.", Precio = 150.00m, Stock = 25, ImagenUrl = "https://ejemplo.com/chanel_no5.jpg" },
                new Producto { Id = 2, Nombre = "Dior Sauvage", Descripcion = "Fresco y amaderado, para hombres.", Precio = 120.00m, Stock = 30, ImagenUrl = "https://ejemplo.com/dior_sauvage.jpg" },
                new Producto { Id = 3, Nombre = "Tom Ford Black Orchid", Descripcion = "Oscuro, opulento y especiado.", Precio = 200.00m, Stock = 15, ImagenUrl = "https://ejemplo.com/tom_ford_black_orchid.jpg" },
                new Producto { Id = 4, Nombre = "Jo Malone Wood Sage & Sea Salt", Descripcion = "Fresco y mineral, unisex.", Precio = 90.00m, Stock = 40, ImagenUrl = "https://ejemplo.com/jo_malone_woodsage.jpg" },
                new Producto { Id = 5, Nombre = "Gucci Bloom", Descripcion = "Floral blanco, empolvado.", Precio = 110.00m, Stock = 20, ImagenUrl = "https://ejemplo.com/gucci_bloom.jpg" },
                new Producto { Id = 6, Nombre = "Paco Rabanne 1 Million", Descripcion = "Amaderado especiado, para hombres.", Precio = 95.00m, Stock = 35, ImagenUrl = "https://ejemplo.com/paco_rabanne_1million.jpg" },
                new Producto { Id = 7, Nombre = "Lancôme La Vie Est Belle", Descripcion = "Gourmand floral, dulce.", Precio = 105.00m, Stock = 28, ImagenUrl = "https://ejemplo.com/lancome_lavie.jpg" },
                new Producto { Id = 8, Nombre = "Versace Eros", Descripcion = "Aromático fougère, para hombres.", Precio = 85.00m, Stock = 45, ImagenUrl = "https://ejemplo.com/versace_eros.jpg" },
                new Producto { Id = 9, Nombre = "YSL Black Opium", Descripcion = "Oriental especiado, café, vainilla.", Precio = 115.00m, Stock = 22, ImagenUrl = "https://ejemplo.com/ysl_black_opium.jpg" },
                new Producto { Id = 10, Nombre = "Carolina Herrera Good Girl", Descripcion = "Floral oriental, dulce.", Precio = 130.00m, Stock = 18, ImagenUrl = "https://ejemplo.com/ch_good_girl.jpg" }
            );
        }
    }
}