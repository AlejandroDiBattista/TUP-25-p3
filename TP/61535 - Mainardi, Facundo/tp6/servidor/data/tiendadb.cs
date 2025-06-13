using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;
namespace Servidor.Stock;
public class tienda : DbContext
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tienda.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Huion Inspiroy Q11K V2", Descripcion = "Tableta sin pantalla y con lapiz ", Precio = 250000, Stock = 10, ImagenUrl = "https://example.com/iphone13.jpg" },
        );
    }
}