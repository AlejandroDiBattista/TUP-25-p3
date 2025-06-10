using Microsoft.EntityFrameworkCore;
namespace servidor.Models
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Gorra Red Bull Racing 2025", Descripcion = "Gorra oficial del equipo Red Bull Racing F1 temporada 2025", Precio = 25000, Stock = 15, ImagenUrl = "https://f1store.com/redbull2025cap.jpg" },
                new Producto { Id = 2, Nombre = "Remera Ferrari 2025", Descripcion = "Remera oficial Scuderia Ferrari F1 2025", Precio = 35000, Stock = 12, ImagenUrl = "https://f1store.com/ferrari2025shirt.jpg" },
                new Producto { Id = 3, Nombre = "Buzo Mercedes AMG 2025", Descripcion = "Buzo oficial Mercedes AMG Petronas F1 2025", Precio = 60000, Stock = 8, ImagenUrl = "https://f1store.com/mercedes2025hoodie.jpg" },
                new Producto { Id = 4, Nombre = "Miniatura McLaren MCL39", Descripcion = "Auto a escala McLaren MCL39 2025", Precio = 45000, Stock = 10, ImagenUrl = "https://f1store.com/mclaren2025mini.jpg" },
                new Producto { Id = 5, Nombre = "Taza Alpine F1 2025", Descripcion = "Taza oficial Alpine F1 Team 2025", Precio = 9000, Stock = 20, ImagenUrl = "https://f1store.com/alpine2025mug.jpg" },
                new Producto { Id = 6, Nombre = "Llavero Aston Martin 2025", Descripcion = "Llavero oficial Aston Martin F1 2025", Precio = 5000, Stock = 25, ImagenUrl = "https://f1store.com/astonmartin2025key.jpg" },
                new Producto { Id = 7, Nombre = "Campera Williams Racing 2025", Descripcion = "Campera oficial Williams Racing F1 2025", Precio = 70000, Stock = 7, ImagenUrl = "https://f1store.com/williams2025jacket.jpg" },
                new Producto { Id = 8, Nombre = "Bandera Haas F1 2025", Descripcion = "Bandera oficial Haas F1 Team 2025", Precio = 12000, Stock = 18, ImagenUrl = "https://f1store.com/haas2025flag.jpg" },
                new Producto { Id = 9, Nombre = "Botella Sauber F1 2025", Descripcion = "Botella oficial Stake F1 Team Kick Sauber 2025", Precio = 8000, Stock = 22, ImagenUrl = "https://f1store.com/sauber2025bottle.jpg" },
                new Producto { Id = 10, Nombre = "Poster Visa Cash App RB 2025", Descripcion = "Poster oficial Visa Cash App RB F1 Team 2025", Precio = 6000, Stock = 30, ImagenUrl = "https://f1store.com/vcarb2025poster.jpg" }
        );
        }
    }
}