using Microsoft.EntityFrameworkCore;
using servidor.Models;
namespace servidor.Data
{
 public class AppDbContext : DbContext
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=tienda.db");
}
}
