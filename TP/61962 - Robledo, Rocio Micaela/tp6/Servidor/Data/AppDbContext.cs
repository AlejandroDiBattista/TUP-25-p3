using Microsoft.EntityFrameworkCore;
using Servidor.Models;

namespace Servidor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ItemCarrito> ItemsCarrito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>().HasData(
new Producto { Id = 1, Nombre = "Botas de cuero", Descripcion = "Botas elegantes para el invierno", Precio = 14999, Stock = 10, ImagenUrl = "http://localhost:5164/img/botaslargas.webp"},
new Producto { Id = 2, Nombre = "Sandalias con plataforma", Descripcion = "Sandalias altas y cómodas para verano", Precio = 8999, Stock = 15, ImagenUrl = "http://localhost:5164/img/sandaliasconplataforma.webp" },
new Producto { Id = 3, Nombre = "Zapatos clásicos", Descripcion = "Zapatos de vestir con taco", Precio = 11999, Stock = 12, ImagenUrl = "http://localhost:5164/img/zapatosclasicos.webp"},
new Producto { Id = 4, Nombre = "Zapatillas urbanas", Descripcion = "Calzado cómodo y moderno para todos los días", Precio = 10499, Stock = 20, ImagenUrl = "http://localhost:5164/img/zapatillasurbanas.webp" },
new Producto { Id = 5, Nombre = "Botines con taco", Descripcion = "Botines de moda con cierre lateral", Precio = 13999, Stock = 8, ImagenUrl = "http://localhost:5164/img/botinesdetaco.webp" },
new Producto { Id = 6, Nombre = "Sandalias cruzadas", Descripcion = "Livianas, ideales para el verano", Precio = 7999, Stock = 18, ImagenUrl = "http://localhost:5164/img/sandaliascruzadas.webp" },
new Producto { Id = 7, Nombre = "Zapatos de charol", Descripcion = "Brillantes y formales, para eventos especiales", Precio = 12999, Stock = 6, ImagenUrl = "http://localhost:5164/img/zapatosdecharol.webp" },
new Producto { Id = 8, Nombre = "Botas texanas", Descripcion = "Estilo vaquero en cuero sintético", Precio = 14999, Stock = 7, ImagenUrl = "http://localhost:5164/img/botastexanas.webp" },
new Producto { Id = 9, Nombre = "Zuecos de verano", Descripcion = "Frescos, livianos y fáciles de calzar", Precio = 6499, Stock = 25, ImagenUrl = "http://localhost:5164/img/suecosdeverano.webp" },
new Producto { Id = 10, Nombre = "Zapatos nude", Descripcion = "Elegancia para toda ocasión", Precio = 11499, Stock = 10, ImagenUrl = "http://localhost:5164/img/zapatosnude.webp" }

            );
        }
    }
}
