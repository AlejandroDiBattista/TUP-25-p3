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
               
                new Producto { Id = 1, Nombre = "Gorra Adidas Negra", Descripcion = "Gorra deportiva clásica", Precio = 7500, Stock = 10, ImagenUrl = "https://i.pinimg.com/736x/f6/ce/69/f6ce69e3d1ed84f461810dffa8d7ff44.jpg" },
                new Producto { Id = 2, Nombre = "Gorra Nike Blanca", Descripcion = "Diseño minimalista y liviano", Precio = 8000, Stock = 12, ImagenUrl = "https://i.pinimg.com/736x/53/e2/60/53e2602452b2086f4a19eb05a3eb73d3.jpg" },
                new Producto { Id = 3, Nombre = "Gorra Puma Azul", Descripcion = "Estilo urbano con visera curva", Precio = 7200, Stock = 15, ImagenUrl = "https://i.pinimg.com/736x/8a/ed/1e/8aed1ec02c58f10a4ce3a32fc113462a.jpg" },
                new Producto { Id = 4, Nombre = "Gorra Vans Roja", Descripcion = "Ideal para skaters y streetwear", Precio = 6800, Stock = 9, ImagenUrl = "https://i.pinimg.com/736x/b2/de/d1/b2ded101a4ad7ee5e9fa7c523acdcd10.jpg" },
                new Producto { Id = 5, Nombre = "Gorra Lacoste Verde", Descripcion = "Con logo bordado clásico", Precio = 9500, Stock = 7, ImagenUrl = "https://i.pinimg.com/736x/51/99/62/519962de6fc350c667bf7c5b6aa43e78.jpg" },
                new Producto { Id = 6, Nombre = "Gorra Under Armour Gris", Descripcion = "Resistente al sudor", Precio = 7900, Stock = 11, ImagenUrl = "https://i.pinimg.com/736x/06/87/25/0687250222be4c0a961f819495b950f5.jpg" },
                new Producto { Id = 7, Nombre = "Gorra DC Shoes Negra", Descripcion = "Gorra plana con logo", Precio = 7300, Stock = 14, ImagenUrl = "https://i.pinimg.com/736x/b7/9d/c3/b79dc3710e4703a5371687ecf73dd380.jpg" },
                new Producto { Id = 8, Nombre = "Gorra Quiksilver Blanca", Descripcion = "Estilo surfero y relajado", Precio = 7050, Stock = 10, ImagenUrl = "https://i.pinimg.com/736x/8b/8e/32/8b8e32f614f8f0740bc965e8ece575b0.jpg" },
                new Producto { Id = 9, Nombre = "Gorra new era Marrón", Descripcion = "Diseño Urbano ", Precio = 7600, Stock = 8, ImagenUrl = "https://i.pinimg.com/736x/48/39/b8/4839b813136b7479e33b316bf2627c87.jpg" },
                new Producto { Id = 10, Nombre = "Gorra MLB | New Era Cap", Descripcion = "Gorra oficial de la MLB", Precio = 9900, Stock = 6, ImagenUrl = "https://i.pinimg.com/736x/65/d9/86/65d986cb04cafd398169ed1ae4b0a9b4.jpg" }
            );
        }
    }
}
