using Microsoft.EntityFrameworkCore;
using Servidor.Models;

namespace Servidor.Data;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<DetalleCompra> DetallesCompras { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<ItemCarrito> ItemsCarrito { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>().HasData(
     new Producto { Id = 1, Nombre = "Camiseta de fútbol Inter", Descripcion = "Modelo Retro 2012/2013", Precio = 8000.00m, Stock = 10, ImagenUrl = "/imagenes/3a4bed5882aa0026981e07e2ee9c6ed2.jpg" },
     new Producto { Id = 2, Nombre = "Camiseta de fútbol Real Madrid", Descripcion = "Modelo Retro 1999/2000", Precio = 9500.00m, Stock = 5, ImagenUrl = "/imagenes/528af923e61317453e9a761674c5e040.jpg" },
     new Producto { Id = 3, Nombre = "Camiseta de fútbol Boca Juniors", Descripcion = "Modelo Retro 1998/1999", Precio = 9000.00m, Stock = 7, ImagenUrl = "/imagenes/03ddd2e5d92dbb21f0501f704bca1de9.jpg" },
     new Producto { Id = 4, Nombre = "Camiseta de fútbol Borussia Dortmund", Descripcion = "Modelo Retro 2013/2014", Precio = 4000.00m, Stock = 8, ImagenUrl = "/imagenes/9c8af3ba7529610d89b06096247552fc.jpg" },
     new Producto { Id = 5, Nombre = "Camiseta de fútbol Atletico Tucumán", Descripcion = "Modelo Retro 2012", Precio = 7000.00m, Stock = 15, ImagenUrl = "/imagenes/542a001d4f6b03edb143f320b220b5d7.jpg" },
     new Producto { Id = 6, Nombre = "Camiseta de fútbol Selección Italia", Descripcion = "Modelo Retro ", Precio = 8250.00m, Stock = 3, ImagenUrl = "/imagenes/7579afdf0e7e8cc92cd8bfd79c610381.jpg" },
     new Producto { Id = 7, Nombre = "Camiseta de fútbol Real Madrid", Descripcion = "Modelo Retro 1998/2000", Precio = 7350.00m, Stock = 6, ImagenUrl = "/imagenes/a9184fa085b21f0ba123a9cd7103148d.jpg" },
     new Producto { Id = 8, Nombre = "Camiseta de fútbol River Plate", Descripcion = "Modelo Retro 1996", Precio = 9450.00m, Stock = 4, ImagenUrl = "/imagenes/a452159f6193fe98782e42dd9687ffa4.jpg" },
     new Producto { Id = 9, Nombre = "Camiseta de fútbol Boca Juniors", Descripcion = "Modelo Retro 2007", Precio = 5550.00m, Stock = 1, ImagenUrl = "/imagenes/cd11d87f448389e60c7bb108796cfab7.jpg" },
     new Producto { Id = 10, Nombre = "Camiseta de fútbol Milan", Descripcion = "Modelo Retro 2012/2013", Precio = 5600.00m, Stock = 12, ImagenUrl = "/imagenes/ce4b1803a674d7e715cc90b610366632.jpg" }
 );


    }


}