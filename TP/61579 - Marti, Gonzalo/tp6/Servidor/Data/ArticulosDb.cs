using Microsoft.EntityFrameworkCore;
using TiendaOnline.Servidor.Models;

namespace TiendaOnline.Servidor.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }
}