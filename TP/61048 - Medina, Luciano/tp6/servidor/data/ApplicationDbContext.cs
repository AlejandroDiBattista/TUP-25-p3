    using Microsoft.EntityFrameworkCore;
    using servidor.Models;

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

                modelBuilder.Entity<ItemCompra>()
                    .HasOne(ic => ic.Producto)
                    .WithMany()
                    .HasForeignKey(ic => ic.ProductoId);

                modelBuilder.Entity<ItemCompra>()
                    .HasOne(ic => ic.Compra)
                    .WithMany(c => c.Items)
                    .HasForeignKey(ic => ic.CompraId);

                // Carga de datos iniciales con URLs de imágenes de internet
                modelBuilder.Entity<Producto>().HasData(
                    new Producto { Id = 1, Nombre = "Chanel N°5", Descripcion = "Clásico e icónico, floral-aldehídico.", Precio = 150.00m, Stock = 25, ImagenUrl = "https://http2.mlstatic.com/D_884842-MLA49096901581_022022-C.jpg" },
                    new Producto { Id = 2, Nombre = "Dior Sauvage", Descripcion = "Fresco y amaderado, para hombres.", Precio = 120.00m, Stock = 30, ImagenUrl = "https://m.media-amazon.com/images/I/41+LrAZN3GL._SY300_SX300_.jpg" },
                    new Producto { Id = 3, Nombre = "Tom Ford Black Orchid", Descripcion = "Oscuro, opulento y especiado.", Precio = 200.00m, Stock = 15, ImagenUrl = "https://a.cdnsbn.com/images/products/l/10104898006.jpg" },
                    new Producto { Id = 4, Nombre = "Jo Malone Wood Sage & Sea Salt", Descripcion = "Fresco y mineral, unisex.", Precio = 90.00m, Stock = 40, ImagenUrl = "https://m.media-amazon.com/images/I/71TUEIy6nzL._SL1500_.jpg" },
                    new Producto { Id = 5, Nombre = "Gucci Bloom", Descripcion = "Floral blanco, empolvado.", Precio = 110.00m, Stock = 20, ImagenUrl = "https://www.24evexia.com/image/cache/catalog/PERFUMES/2023/GUCCI-BLOOM-INTENSE-750x750.png" },
                    new Producto { Id = 6, Nombre = "Paco Rabanne 1 Million", Descripcion = "Amaderado especiado, para hombres.", Precio = 95.00m, Stock = 35, ImagenUrl = "https://m.media-amazon.com/images/I/71YHKvsfyCL._SL1500_.jpg" },
                    new Producto { Id = 7, Nombre = "Lancôme La Vie Est Belle", Descripcion = "Gourmand floral, dulce.", Precio = 105.00m, Stock = 28, ImagenUrl = "https://lindas.com.ar/wp-content/uploads/2024/09/52669.jpg" },
                    new Producto { Id = 8, Nombre = "Versace Eros", Descripcion = "Aromático fougère, para hombres.", Precio = 85.00m, Stock = 45, ImagenUrl = "https://m.media-amazon.com/images/I/51Tdo8yN3LL.jpg" },
                    new Producto { Id = 9, Nombre = "YSL Black Opium", Descripcion = "Oriental especiado, café, vainilla.", Precio = 115.00m, Stock = 22, ImagenUrl = "https://cdn.basler-beauty.de/out/pictures/generated/product/1/980_980_100/2536455-Yves-Saint-Laurent-Black-Opium-Le-Parfum-50-ml.bed571de.jpg" },
                    new Producto { Id = 10, Nombre = "Carolina Herrera Good Girl", Descripcion = "Floral oriental, dulce.", Precio = 130.00m, Stock = 18, ImagenUrl = "https://d3cdlnm7te7ky2.cloudfront.net/media/catalog/product/cache/e8f012862bd8df4f2e4f3ce158c4a16c/d/-/d-good-girl-edp_1.jpg" }
                );
            }
        }
    }