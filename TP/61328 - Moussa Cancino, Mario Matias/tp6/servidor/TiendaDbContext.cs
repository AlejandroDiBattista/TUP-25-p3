using Microsoft.EntityFrameworkCore;

namespace servidor
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "iPhone 14 Pro", Descripcion = "El último smartphone de Apple con chip A16 Bionic.", Precio = 999.99m, Stock = 50, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 2, Nombre = "Samsung Galaxy S23 Ultra", Descripcion = "El buque insignia de Samsung con un S Pen integrado.", Precio = 1199.99m, Stock = 40, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 3, Nombre = "Google Pixel 7 Pro", Descripcion = "La magia de Google en un teléfono, con el chip Tensor G2.", Precio = 899.00m, Stock = 60, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 4, Nombre = "Funda de Silicona para iPhone", Descripcion = "Protección suave y elegante para tu iPhone.", Precio = 49.00m, Stock = 150, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 5, Nombre = "Cargador Rápido USB-C 30W", Descripcion = "Carga tu dispositivo a toda velocidad.", Precio = 35.50m, Stock = 200, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 6, Nombre = "AirPods Pro (2da Gen)", Descripcion = "Cancelación de ruido activa y audio espacial.", Precio = 249.00m, Stock = 80, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 7, Nombre = "Protector de Pantalla de Vidrio", Descripcion = "Máxima protección contra rayones y golpes.", Precio = 25.00m, Stock = 300, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 8, Nombre = "Samsung Galaxy Watch 5", Descripcion = "Monitor de salud avanzado y diseño moderno.", Precio = 279.99m, Stock = 70, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 9, Nombre = "Batería Externa 10000mAh", Descripcion = "Nunca te quedes sin batería fuera de casa.", Precio = 45.00m, Stock = 120, ImagenUrl = "https://placehold.co/300x200/png" },
                new Producto { Id = 10, Nombre = "Soporte de Coche Magnético", Descripcion = "Mantén tu teléfono seguro y a la vista mientras conduces.", Precio = 22.99m, Stock = 180, ImagenUrl = "https://placehold.co/300x200/png" }
            );
        }
    }
}