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
                    Descripcion = "Tableta sin pantalla digital, con lapiz y cargador incluido",
                    Precio = 250000,
                    Stock = 10,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_984330-MLA40805046216_022020-O.webp"
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "Huion Kamvas Pro 16",
                    Descripcion = "Tableta con pantalla digital, con lapiz y cargador incluido",
                    Precio = 350000,
                    Stock = 5,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_755188-CBT72056473429_102023-O.webp"
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "Huion HS611 space grey",
                    Descripcion = "Tableta profesional sin pantalla digital, con lapiz y cargador incluido",
                    Precio = 1500000,
                    Stock = 2,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_674228-MLA43703331753_102020-O.webp"
                },
                new Producto
                {
                    Id = 4,
                    Nombre = "Gadnic T601",
                    Descripcion = "Tableta profesional sin pantalla digital, con lapiz y cargador incluido",
                    Precio = 100000,
                    Stock = 4,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_865680-MLA82981419793_032025-F.webp"
                },
                new Producto
                {
                    Id = 5,
                    Nombre = "Veikk A15pro",
                    Descripcion = "Tableta profesional sin pantalla digital, con lapiz y cargador incluido",
                    Precio = 175000,
                    Stock = 6,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_659572-CBT49756152659_042022-O.webp"
                },
                new Producto
                {
                    Id = 6,
                    Nombre = "Xp-pen Artist 12 Pro Easy Fhd",
                    Descripcion = "Tableta profesional con pantalla digital, con lapiz y cargador incluido",
                    Precio = 325000,
                    Stock = 7,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_601371-MLU72645985677_112023-O.webp"
                },
                new Producto
                {
                    Id = 7,
                    Nombre = "XP-Pen Star G430S",
                    Descripcion = "Tableta profesional sin pantalla digital, con lapiz y cargador incluido",
                    Precio = 1500000,
                    Stock = 8,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_686067-MLA32157823326_092019-O.webpp"
                },
                new Producto
                {
                    Id = 8,
                    Nombre = "Huion Inspiroy H320M",
                    Descripcion = "Tableta profesional con pantalla digital, con lapiz y cargador incluido",
                    Precio = 193000,
                    Stock = 9,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_661053-MLA43226379324_082020-O.webp"
                },
                new Producto
                {
                    Id = 9,
                    Nombre = "Ugee M708",
                    Descripcion = "Tableta profesional con pantalla digital, con lapiz y cargador incluido",
                    Precio = 162000,
                    Stock = 1,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_747114-CBT82204893009_012025-O.webp"
                },
                new Producto
                {
                    Id = 10,
                    Nombre = "Gaomon S620",
                    Descripcion = "Tableta profesional con pantalla digital, con lapiz y cargador incluido",
                    Precio = 86000,
                    Stock = 3,
                    ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_910772-MLA40187105955_122019-O.webp"
                }
            );
        }
    }
}