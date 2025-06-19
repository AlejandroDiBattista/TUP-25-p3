using Microsoft.EntityFrameworkCore;

namespace Servidor.Models
{
    public class TiendaContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 15 productos de ejemplo: solo dulces
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Chocolate Milka", Descripcion = "Tableta de chocolate con leche 100g", Precio = 950, Stock = 38, ImagenUrl = "https://images.example.com/milka.jpg" },
                new Producto { Id = 2, Nombre = "Bon o Bon", Descripcion = "Bombón de chocolate relleno", Precio = 120, Stock = 100, ImagenUrl = "https://images.example.com/bonobon.jpg" },
                new Producto { Id = 3, Nombre = "Sugus", Descripcion = "Caramelos masticables surtidos", Precio = 80, Stock = 200, ImagenUrl = "https://images.example.com/sugus.jpg" },
                new Producto { Id = 4, Nombre = "Rocklets", Descripcion = "Confites de chocolate 40g", Precio = 250, Stock = 60, ImagenUrl = "https://images.example.com/rocklets.jpg" },
                new Producto { Id = 5, Nombre = "Mogul", Descripcion = "Gomitas frutales 50g", Precio = 180, Stock = 80, ImagenUrl = "https://images.example.com/mogul.jpg" },
                new Producto { Id = 6, Nombre = "Tita", Descripcion = "Galletita bañada en chocolate", Precio = 90, Stock = 120, ImagenUrl = "https://images.example.com/tita.jpg" },
                new Producto { Id = 7, Nombre = "Rhodesia", Descripcion = "Galletita bañada en chocolate", Precio = 90, Stock = 120, ImagenUrl = "https://images.example.com/rhodesia.jpg" },
                new Producto { Id = 8, Nombre = "Menta Cristal", Descripcion = "Caramelos de menta", Precio = 70, Stock = 150, ImagenUrl = "https://images.example.com/mentacristal.jpg" },
                new Producto { Id = 9, Nombre = "Caramelos Arcor", Descripcion = "Caramelos duros surtidos", Precio = 60, Stock = 180, ImagenUrl = "https://images.example.com/arcor.jpg" },
                new Producto { Id = 10, Nombre = "Chocolinas", Descripcion = "Galletitas de chocolate 170g", Precio = 350, Stock = 50, ImagenUrl = "https://images.example.com/chocolinas.jpg" },
                new Producto { Id = 11, Nombre = "Alfajor Jorgito", Descripcion = "Alfajor de dulce de leche", Precio = 180, Stock = 90, ImagenUrl = "https://images.example.com/jorgito.jpg" },
                new Producto { Id = 12, Nombre = "Bananita Dolca", Descripcion = "Dulce de banana bañado en chocolate", Precio = 100, Stock = 110, ImagenUrl = "https://images.example.com/bananita.jpg" },
                new Producto { Id = 13, Nombre = "Kinder Bueno", Descripcion = "Barra de chocolate con leche y avellanas", Precio = 400, Stock = 40, ImagenUrl = "https://images.example.com/kinderbueno.jpg" },
                new Producto { Id = 14, Nombre = "KitKat", Descripcion = "Barra de chocolate con obleas", Precio = 350, Stock = 45, ImagenUrl = "https://images.example.com/kitkat.jpg" },
                new Producto { Id = 15, Nombre = "M&M's", Descripcion = "Confites de chocolate 45g", Precio = 300, Stock = 55, ImagenUrl = "https://images.example.com/mms.jpg" }
            );
        }
    }
}