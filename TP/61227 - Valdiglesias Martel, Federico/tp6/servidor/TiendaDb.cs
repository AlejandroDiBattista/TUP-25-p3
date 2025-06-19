using Microsoft.EntityFrameworkCore; 
using servidor.modelos; 

namespace servidor.Data;

public class TiendaDb : DbContext
{
    public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; } 
    public DbSet<ItemCompra> ItemsCompra { get; set; } 
}