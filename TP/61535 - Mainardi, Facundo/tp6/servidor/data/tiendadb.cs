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
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "Huion Kamvas Pro 16",
                    Descripcion = "Tableta con pantalla y con lapiz",
                    Precio = 500000,
                    Stock = 5,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_755188-CBT72056473429_102023-O.webp"
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "Gadnic T601",
                    Descripcion = "Tableta profesional con pantalla y con lapiz",
                    Precio = 1500000,
                    Stock = 2,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_865680-MLA82981419793_032025-F.webp"
                }
            );
        }
    }
}