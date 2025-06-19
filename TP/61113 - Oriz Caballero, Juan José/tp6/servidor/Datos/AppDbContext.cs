using Microsoft.EntityFrameworkCore;
using Servidor.Modelo;

namespace Servidor.Modelo
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ItemCarrito> ItemsCarrito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Alfeñique 'El Concepcionense' x12 un.", Descripcion = "Dulce regional tucumano hecho a partir de nuestras cañas de azucar.", Precio = 2500, Stock = 10, ImagenUrl = "images/alfeñiques.jpg" },
                new Producto { Id = 2, Nombre = "Mermelada de Arandanos 'Tía Yola' x475 grs.", Descripcion = "Mermelada organica fabrícada al estilo casero a partir de Arandanos.", Precio = 5000, Stock = 15, ImagenUrl = "images/mermelada-de-moras-casera.jpg" },
                new Producto { Id = 3, Nombre = "Dulce de cayote 'Tía Yola' x475 grs.", Descripcion = "Dulce regional organico realizado a partir desde el cayote.", Precio = 3800, Stock = 20, ImagenUrl = "images/cayote.jpg" },
                new Producto { Id = 4, Nombre = "Quesillo de cabra 'Sabores del valle' x200 grs.", Descripcion = "Quesillo de cabra elavorado de manera artesanal por productores de Tafí del Valle", Precio = 4600, Stock = 12, ImagenUrl = "images/quesillo.jpg" },
                new Producto { Id = 5, Nombre = "Tableta 'El Concepcionense'.", Descripcion = "Tabletas de 176grs elaboradas a partir de la caña de azucar", Precio = 2500, Stock = 12, ImagenUrl = "images/tableta.jpg" },
                new Producto { Id = 6, Nombre = "Licor de Naranja 'Tía yola' x456 ml", Descripcion = "Licor elaborado desde la naranja Agria perfecto como copetín o para alzar bizcochuelos y tortas.", Precio = 4000, Stock = 25, ImagenUrl = "images/licor.jpg" },
                new Producto { Id = 7, Nombre = "Nueces Confitadas 'Sabores del Valle' x12 un.", Descripcion = "Dulce regional realizado por nueces organicas, dulce de leche, glasé y chocolate", Precio = 4000, Stock = 10, ImagenUrl = "images/nueces.jpg" },
                new Producto { Id = 8, Nombre = "Alfajores de fruta 'Tía Yola' x6 un", Descripcion = "Alfajores realizados a partir de dulce de cayote, higo y membrillo.", Precio = 1800, Stock = 18, ImagenUrl = "images/alfajor.jpg" },
                new Producto { Id = 9, Nombre = "Charquí de llama 'Sabores del valle' x200 grs.", Descripcion = "Charqui elavorado de manera tradicional y sellado al vacío para su conservación por nosotros.", Precio = 2200, Stock = 16, ImagenUrl = "images/charqui.jpg" },
                new Producto { Id = 10, Nombre = "Escabeche de llama y Vizcacha 'Sabores del valle' x450 grs.", Descripcion = "Conserva de carne magra de llama y vizcacha al vinagre con verduras y condimentado con un mix de especias.", Precio = 4700, Stock = 25, ImagenUrl = "images/vizcacha.png" },
                new Producto { Id = 11, Nombre = "Vino patero 'Sabores del Valle' x750 ml", Descripcion = "Vino joven realizado a partir de uvas plantadas por las comunidades de Amaicha del Valle. De sabor dulce e intenso.", Precio = 4750, Stock = 35, ImagenUrl = "images/patero.jpg" }
            );
        }
    }
    
}

//preparar el traslado de datos
// dotnet ef migrations add AddCarrito
//dotnet ef database update
