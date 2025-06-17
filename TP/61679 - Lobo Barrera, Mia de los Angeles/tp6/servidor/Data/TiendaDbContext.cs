using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

namespace servidor.Data
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
        public DbSet<Carrito> Carritos => Set<Carrito>();
        public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData
            (
                new Producto { Id = 1, Nombre = "Camiseta Scuderia Ferrari F1 2025 rojo", Descripcion = "Toma una prenda esencial y dale el auténtico toque de carreras. Ninguna pieza de ropa te va acompañar tan lejos como esta.", Stock = 100, Precio = 102.50m, ImagenUrl = "img/ferrari.png" },
                new Producto { Id = 2, Nombre = "Camiseta Hass F1 Moneygram 2025", Descripcion = "El favorito que funciona en cualquier situación.", Stock = 100, Precio = 94, ImagenUrl = "img/hass.png" },
                new Producto { Id = 3, Nombre = "Gorra Team Nico Hulkenberg Stake Sauber F1 2025", Descripcion = "El estilo de tu equipo favorito de carreras.", Stock = 100, Precio = 59, ImagenUrl = "img/kicksauber.png" },
                new Producto { Id = 4, Nombre = "Camiseta Piloto McLaren Oscar Piastri F1 2025", Descripcion = "El favorito que siempre funciona en cualquier situación, con el estilo de un verdadero profesional.", Stock = 100, Precio = 104, ImagenUrl = "img/mclaren.png" },
                new Producto { Id = 5, Nombre = "Camiseta Piloto Red Bull Racing Max Verstappen F1 2025", Descripcion = "El favorito que siempre funciona en cualquier situación, con el estilo de un verdadero profesional.", Stock = 100, Precio = 104, ImagenUrl = "img/redbullracing.png.avif" },
                new Producto { Id = 6, Nombre = "Camiseta Piloto VCARB Hugo F1 2025", Descripcion = "Un favorito que siempre funciona en cualquier situación.", Stock = 100, Precio = 116, ImagenUrl = "img/vcarb.png" },
                new Producto { Id = 7, Nombre = "Camiseta Williams Racing F1 2025", Descripcion = "El favorito de todos, porque siempre funciona en cualquier situación.", Stock = 100, Precio = 85.50m, ImagenUrl = "img/williams.png" },
                new Producto { Id = 8, Nombre = "Camiseta Piloto Mercedes F1 2025", Descripcion = "Toma una prenda esencial y dale el auténtico toque de carreras.", Stock = 100, Precio = 110, ImagenUrl = "img/mercedes.png" },
                new Producto { Id = 9, Nombre = "Camiseta Piloto Alpine F1 2025", Descripcion = "El favorito de todos, siempre funciona en cualquier situación.", Stock = 100, Precio = 90, ImagenUrl = "img/alpine.png" },
                new Producto { Id = 10, Nombre = "Camiseta Piloto Aston Martin F1 2025 Fernando Alonso", Descripcion = "El favorito de todos, que te da un auténtico toque de profesional.", Stock = 100, Precio = 99, ImagenUrl = "img/astonmartin.png" }
            );

            }
        }
    }