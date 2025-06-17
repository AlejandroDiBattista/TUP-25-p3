using Microsoft.EntityFrameworkCore;
using servidor.Models;
namespace servidor.Data
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

    }
}