using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data;

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
    new Producto { Id = 1, Nombre = "Gorra Clásica Negra", Descripcion = "Gorra de algodón color negro", Precio = 7500, Stock = 20, ImagenUrl = "/img/gorra1.jpg" },
    new Producto { Id = 2, Nombre = "Gorra Azul Deportiva", Descripcion = "Gorra ideal para actividades al aire libre", Precio = 8200, Stock = 15, ImagenUrl = "/img/gorra2.jpg" },
    new Producto { Id = 3, Nombre = "Gorra Roja Urbana", Descripcion = "Estilo urbano, con visera curva", Precio = 7900, Stock = 10, ImagenUrl = "/img/gorra3.jpg" },
    new Producto { Id = 4, Nombre = "Gorra Trucker Blanca", Descripcion = "Con red y visera plana", Precio = 8800, Stock = 18, ImagenUrl = "/img/gorra4.jpg" },
    new Producto { Id = 5, Nombre = "Gorra Militar Verde", Descripcion = "Diseño camuflado", Precio = 9500, Stock = 12, ImagenUrl = "/img/gorra5.jpg" },
    new Producto { Id = 6, Nombre = "Gorra Negra con Logo", Descripcion = "Logo bordado, ajuste regulable", Precio = 8900, Stock = 22, ImagenUrl = "/img/gorra6.jpg" },
    new Producto { Id = 7, Nombre = "Gorra de Jean", Descripcion = "Estilo retro, material denim", Precio = 7800, Stock = 14, ImagenUrl = "/img/gorra7.jpg" },
    new Producto { Id = 8, Nombre = "Gorra Bordó", Descripcion = "Color vino tinto, visera recta", Precio = 8400, Stock = 16, ImagenUrl = "/img/gorra8.jpg" },
    new Producto { Id = 9, Nombre = "Gorra Gris Básica", Descripcion = "Gorra simple, liviana", Precio = 7200, Stock = 25, ImagenUrl = "/img/gorra9.jpg" },
    new Producto { Id = 10, Nombre = "Gorra Edición Limitada", Descripcion = "Diseño exclusivo de colección", Precio = 15000, Stock = 5, ImagenUrl = "/img/gorra10.jpg" }
);

    }
}
