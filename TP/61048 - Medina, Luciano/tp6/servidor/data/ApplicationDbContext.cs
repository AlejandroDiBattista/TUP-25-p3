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
                    new Producto { Id = 1, Nombre = "Chanel N°5", Descripcion = "Clásico e icónico, floral-aldehídico.", Precio = 150.00m, Stock = 25, ImagenUrl = "https://i5.walmartimages.com/seo/Chanel-No-5-Eau-de-Parfum-Spray-Perfume-for-Women-3-4-oz-100-ml_a41d96f8-fe32-487d-9f77-ce30d05d8b72.f0424d696214b0da62c279964a8174fd.jpeg" },
                    new Producto { Id = 2, Nombre = "Dior Sauvage", Descripcion = "Fresco y amaderado, para hombres.", Precio = 120.00m, Stock = 30, ImagenUrl = "https://www.myperfumeshop.qa/cdn/shop/files/dior-sauvage-edt-perfume-cologne-408783.png?v=1742526282&width=400" },
                    new Producto { Id = 3, Nombre = "Tom Ford Black Orchid", Descripcion = "Oscuro, opulento y especiado.", Precio = 200.00m, Stock = 15, ImagenUrl = "https://static.sweetcare.com/img/prd/488/v-638235637318829790/tom-ford-000011tf_03.webp" },
                    new Producto { Id = 4, Nombre = "Jo Malone Wood Sage & Sea Salt", Descripcion = "Fresco y mineral, unisex.", Precio = 90.00m, Stock = 40, ImagenUrl = "https://api-assets.wikiparfum.com/_resized/nyf437ux1psfya0ekv1imq67nlx0gq8qxbdkiw2rcrkc47cug5ly8vi1ogyk-w250-q85.webp" },
                    new Producto { Id = 5, Nombre = "Gucci Bloom", Descripcion = "Floral blanco, empolvado.", Precio = 110.00m, Stock = 20, ImagenUrl = "https://mcgrocer.com/cdn/shop/files/gucci-bloom-for-her-eau-de-toilette-50ml-40505979896046.jpg?v=1741307863" },
                    new Producto { Id = 6, Nombre = "Paco Rabanne 1 Million", Descripcion = "Amaderado especiado, para hombres.", Precio = 95.00m, Stock = 35, ImagenUrl = "https://www.farmaciasrp.com.ar/22845-large_default/paco-rabanne-1-million-elixir-parfum-intense-50-ml.jpg" },
                    new Producto { Id = 7, Nombre = "Lancôme La Vie Est Belle", Descripcion = "Gourmand floral, dulce.", Precio = 105.00m, Stock = 28, ImagenUrl = "https://www.farmacialeloir.com.ar/img/articulos/2024/08/imagen1_lancome_la_vie_est_belle_eau_de_parfum_x_75ml_imagen1.webp" },
                    new Producto { Id = 8, Nombre = "Versace Eros", Descripcion = "Aromático fougère, para hombres.", Precio = 85.00m, Stock = 45, ImagenUrl = "https://i5.walmartimages.com/seo/Versace-Eros-Eau-De-Toilette-Natural-Spray-Cologne-for-Men-6-7-oz_db99fcd0-1642-47d8-9fe4-901b3de6fbb8_1.cdfc3acf51b7b1159936f22e63daf3fe.jpeg" },
                    new Producto { Id = 9, Nombre = "YSL Black Opium", Descripcion = "Oriental especiado, café, vainilla.", Precio = 115.00m, Stock = 22, ImagenUrl = "https://static.sweetcare.com/img/prd/488/v-638200527221023353/yves-saint-laurent-017473ys_03.webp" },
                    new Producto { Id = 10, Nombre = "Carolina Herrera Good Girl", Descripcion = "Floral oriental, dulce.", Precio = 130.00m, Stock = 18, ImagenUrl = "https://d3cdlnm7te7ky2.cloudfront.net/media/catalog/product/cache/e8f012862bd8df4f2e4f3ce158c4a16c/d/-/d-good-girl-edp_1.jpg" }
                );
            }
        }
    }