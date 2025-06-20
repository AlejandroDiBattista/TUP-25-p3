using Microsoft.EntityFrameworkCore;
using servidor.ModelosServidor;

namespace servidor.ModelosServidor
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options)
            : base(options)
        {
        }

        // DbSet para cada tabla de la base de datos
        public DbSet<Producto> Productos     { get; set; }
        public DbSet<Compra> Compras         { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        // use SQLite apuntando al archivo tienda.db
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=tienda.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Producto>().HasData(
                
                new Producto
                {
                    Id = 1,
                    Nombre = "iPhone 16",
                    Descripcion = "Color: Ultramarine",
                    Precio = 650000m,
                    Stock = 20,
                    ImagenUrl = "iPhone16.jpg"
                },
                
                new Producto
                {
                    Id = 2,
                    Nombre = "iPhone 15",
                    Descripcion = "Color: Pink",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "iPhone15.jpg"
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "iPhone 14 Pro",
                    Descripcion = "Color: Gold",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "iPhone14Pro.jpg"
                },
                new Producto
                {
                    Id = 4,
                    Nombre = "iPhone 13 Pro",
                    Descripcion = "Color: Graphite",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "iPhone13Pro.jpg"
                },
                new Producto
                {
                    Id = 5,
                    Nombre = "iPhone 12",
                    Descripcion = "Color: Red",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "iPhone12.jpg"
                },
                new Producto
                {
                    Id = 6,
                    Nombre = "iPhone 11",
                    Descripcion = "Color: White",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "iPhone11.jpg"
                },
                new Producto
                {
                    Id = 7,
                    Nombre = "iPhone XR",
                    Descripcion = "Color: Blue",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "iPhone16.jpg"
                },
                new Producto
                {
                    Id = 8,
                    Nombre = "Funda Silicona iPhone 12",
                    Descripcion = "Color: Red",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "FundaSilicona.jpg"
                },
                new Producto
                {
                    Id = 9,
                    Nombre = "iPad Air 5",
                    Descripcion = "Color: Gray",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "iPadAir5.jpg"
                },
                new Producto
                {
                    Id = 10,
                    Nombre = "Cargador Original Apple",
                    Descripcion = "Cargador 20w más cable Lightning",
                    Precio = 420000m,
                    Stock = 20,
                    ImagenUrl = "CargadorConCable.jpg"
                }

            );
        }
    }
}
