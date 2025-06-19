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

            // Pre‑cargar datos de ejemplo
            modelBuilder.Entity<Producto>().HasData(
                
                new Producto
                {
                    Id = 1,
                    Nombre = "iPhone 13 Pro",
                    Descripcion = "Color Blue",
                    Precio = 650000m,
                    Stock = 15,
                    ImagenUrl = "url1"
                },
                
                new Producto
                {
                    Id = 2,
                    Nombre = "iPhone 12",
                    Descripcion = "Color Black",
                    Precio = 420000m,
                    Stock = 5,
                    ImagenUrl = "url2"
                }
            );
        }
    }
}
