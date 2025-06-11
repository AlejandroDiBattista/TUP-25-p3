using Microsoft.EntityFrameworkCore;
using servidor.Models;

using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data {
    public class TiendaDbContext : DbContext {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Gorra Adidas Negra", Descripcion = "Gorra deportiva clásica", Precio = 7500, Stock = 10, ImagenUrl = "https://example.com/adidas_negra.jpg" },
                new Producto { Id = 2, Nombre = "Gorra Nike Blanca", Descripcion = "Diseño minimalista y liviano", Precio = 8000, Stock = 12, ImagenUrl = "https://example.com/nike_blanca.jpg" },
                new Producto { Id = 3, Nombre = "Gorra Puma Azul", Descripcion = "Estilo urbano con visera curva", Precio = 7200, Stock = 15, ImagenUrl = "https://example.com/puma_azul.jpg" },
                new Producto { Id = 4, Nombre = "Gorra Vans Roja", Descripcion = "Ideal para skaters y streetwear", Precio = 6800, Stock = 9, ImagenUrl = "https://example.com/vans_roja.jpg" },
                new Producto { Id = 5, Nombre = "Gorra Lacoste Verde", Descripcion = "Con logo bordado clásico", Precio = 9500, Stock = 7, ImagenUrl = "https://example.com/lacoste_verde.jpg" },
                new Producto { Id = 6, Nombre = "Gorra Under Armour Gris", Descripcion = "Resistente al sudor", Precio = 7900, Stock = 11, ImagenUrl = "https://example.com/ua_gris.jpg" },
                new Producto { Id = 7, Nombre = "Gorra DC Shoes Negra", Descripcion = "Gorra plana con logo", Precio = 7300, Stock = 14, ImagenUrl = "https://example.com/dc_negra.jpg" },
                new Producto { Id = 8, Nombre = "Gorra Quiksilver Blanca", Descripcion = "Estilo surfero y relajado", Precio = 7050, Stock = 10, ImagenUrl = "https://example.com/quik_blanca.jpg" },
                new Producto { Id = 9, Nombre = "Gorra Element Marrón", Descripcion = "Diseño ecológico", Precio = 7600, Stock = 8, ImagenUrl = "https://example.com/element_marron.jpg" },
                new Producto { Id = 10, Nombre = "Gorra New Era Yankees", Descripcion = "Gorra oficial del equipo NY", Precio = 9900, Stock = 6, ImagenUrl = "https://example.com/newera_yankees.jpg" }
            );
        }
    }
}
