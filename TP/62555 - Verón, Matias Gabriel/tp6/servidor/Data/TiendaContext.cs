using Microsoft.EntityFrameworkCore;
using Servidor.Models;

namespace Servidor.Data;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<DetalleCompra> DetallesCompras { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Libro - El Señor de los Anillos: La Comunidad del Anillo", Descripcion = "Primera parte de la trilogía épica de J.R.R. Tolkien.", Precio = 8000.00m, Stock = 10, ImagenUrl = "https://example.com/producto1.jpg" },
            new Producto { Id = 2, Nombre = "Libro - El Hobbit (Edición ilustrada)", Descripcion = "Precuela de El Señor de los Anillos, con ilustraciones a color.", Precio = 9500.00m, Stock = 5, ImagenUrl = "" }
            new Producto { Id = 3, Nombre = "Producto 3", Descripcion = "Descripción del producto 3", Precio = 300.00m, Stock = 2, ImagenUrl = "" }
            new Producto { Id = 4, Nombre = "Producto 4", Descripcion = "Descripción del producto 4", Precio = 400.00m, Stock = 8, ImagenUrl = "" },
            new Producto { Id = 5, Nombre = "Producto 5", Descripcion = "Descripción del producto 5", Precio = 150.00m, Stock = 15, ImagenUrl = "" },
            new Producto { Id = 6, Nombre = "Producto 6", Descripcion = "Descripción del producto 6", Precio = 250.00m, Stock = 3, ImagenUrl = "" },
            new Producto { Id = 7, Nombre = "Producto 7", Descripcion = "Descripción del producto 7", Precio = 350.00m, Stock = 6, ImagenUrl = "" },
            new Producto { Id = 8, Nombre = "Producto 8", Descripcion = "Descripción del producto 8", Precio = 450.00m, Stock = 4, ImagenUrl = "" },
            new Producto { Id = 9, Nombre = "Producto 9", Descripcion = "Descripción del producto 9", Precio = 550.00m, Stock = 1, ImagenUrl = "" },
            new Producto { Id = 10, Nombre = "Producto 10", Descripcion = "Descripción del producto 10", Precio = 600.00m, Stock = 12, ImagenUrl = "https://example.com/producto10.jpg" }
        );

    }

    {

    }

}