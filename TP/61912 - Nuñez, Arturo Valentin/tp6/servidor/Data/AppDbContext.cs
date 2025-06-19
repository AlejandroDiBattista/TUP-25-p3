using Microsoft.EntityFrameworkCore;
using Servidor.Models;

namespace Servidor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Semilla de 10 productos Xiaomi con imágenes locales
            modelBuilder.Entity<Product>().HasData(
                new Product {
                    Id = 1,
                    Nombre = "Xiaomi Redmi Note 11",
                    Descripcion = "Smartphone 6.43\" AMOLED, Snapdragon 680",
                    Precio = 45000m,
                    Stock = 30,
                    ImagenUrl = "/images/1000x1000-CXRED11X.jpg"
                },
                new Product {
                    Id = 2,
                    Nombre = "Xiaomi Poco X3 Pro",
                    Descripcion = "Smartphone 6.67\" 120Hz, Snapdragon 860",
                    Precio = 60000m,
                    Stock = 20,
                    ImagenUrl = "/images/Xiaomi-Poco-X3-Pro-256.jpg"
                },
                new Product {
                    Id = 3,
                    Nombre = "Xiaomi Mi 11 Lite",
                    Descripcion = "Smartphone 6.55\" AMOLED, Snapdragon 732G",
                    Precio = 55000m,
                    Stock = 15,
                    ImagenUrl = "/images/xiaomiMi11Lite.jpg"
                },
                new Product {
                    Id = 4,
                    Nombre = "Xiaomi Redmi 10",
                    Descripcion = "Smartphone 6.5\" FHD+, Helio G88",
                    Precio = 30000m,
                    Stock = 25,
                    ImagenUrl = "/images/redmi10.jpg"
                },
                new Product {
                    Id = 5,
                    Nombre = "Xiaomi Redmi 9A",
                    Descripcion = "Smartphone 6.53\" HD+, Helio G25",
                    Precio = 20000m,
                    Stock = 40,
                    ImagenUrl = "/images/xiaomi9.jpg"
                },
                new Product {
                    Id = 6,
                    Nombre = "Xiaomi Mi Band 6",
                    Descripcion = "Smartband con pantalla AMOLED de 1.56\"",
                    Precio = 9000m,
                    Stock = 50,
                    ImagenUrl = "/images/band6.jpg"
                },
                new Product {
                    Id = 7,
                    Nombre = "Xiaomi Redmi Buds 3",
                    Descripcion = "Auriculares TWS con ANC",
                    Precio = 8000m,
                    Stock = 35,
                    ImagenUrl = "/images/buds3.jpg"
                },
                new Product {
                    Id = 8,
                    Nombre = "Xiaomi Mi Watch Lite",
                    Descripcion = "Smartwatch con GPS integrado",
                    Precio = 12000m,
                    Stock = 22,
                    ImagenUrl = "/images/WatchLite.jpg"
                },
                new Product {
                    Id = 9,
                    Nombre = "Xiaomi Power Bank 10000mAh",
                    Descripcion = "Batería portátil USB-C y USB-A",
                    Precio = 7000m,
                    Stock = 45,
                    ImagenUrl = "/images/powebank.jpg"
                },
                new Product {
                    Id = 10,
                    Nombre = "Xiaomi TV Stick",
                    Descripcion = "Reproductor multimedia Full HD",
                    Precio = 15000m,
                    Stock = 18,
                    ImagenUrl = "/images/stick.jpg"
                }
            );
        }
    }
}
