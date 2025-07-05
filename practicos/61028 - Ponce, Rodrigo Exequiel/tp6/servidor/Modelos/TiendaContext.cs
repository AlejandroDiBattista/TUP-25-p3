using Microsoft.EntityFrameworkCore;

namespace servidor.Modelos
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "BAGGY HACK CLEAR", Talle = "S, M, L, XL", Precio = 80000, Stock = 20, ImagenUrl = "BAGGY.jpg" },
                new Producto { Id = 2, Nombre = "HOODIE BLANCO ADDRIPS", Talle = "M, L, XL", Precio = 15000, Stock = 15, ImagenUrl = "HOODIE.jpg" },
                new Producto { Id = 3, Nombre = "BAGGY HACK WASHED BLACK", Talle = "S, M, L", Precio = 92000, Stock = 10, ImagenUrl = "BAGGY-2.jpg" },
                new Producto { Id = 4, Nombre = "SWEATER ATHLETIC INGLES", Talle = "M, L", Precio = 72000, Stock = 10, ImagenUrl = "SWEATER.jpg" },
                new Producto { Id = 5, Nombre = "JEAN ADDRIPS CELESTE", Talle = "S, M, L, XL", Precio = 80000, Stock = 25, ImagenUrl = "JEAN.jpg" },
                new Producto { Id = 6, Nombre = "BUZO TREKKER BLANCO", Talle = "M, L, XL", Precio = 90000, Stock = 18, ImagenUrl = "BUZO.jpg" },
                new Producto { Id = 7, Nombre = "BAGGY JAMES", Talle = "S, M, L, XL", Precio = 85000, Stock = 22, ImagenUrl = "BAGGY-3.jpg" },
                new Producto { Id = 8, Nombre = "BUZO POLAR NEGRO", Talle = "S, M, L", Precio = 76000, Stock = 12, ImagenUrl = "BUZO-2.jpg" }
                // Agrega más productos aquí
            );
        }
    }
}
