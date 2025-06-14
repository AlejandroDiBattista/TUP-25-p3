using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;

namespace Servidor.Stock
{
    public class Tienda : DbContext
    {
        public Tienda(DbContextOptions<Tienda> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    Id = 1,
                    Nombre = "Huion Inspiroy Q11K V2",
                    Descripcion = "Tableta sin pantalla y con lapiz",
                    Precio = 250000,
                    Stock = 10,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_984330-MLA40805046216_022020-O.webp"
                }
            );
        }
    }
}