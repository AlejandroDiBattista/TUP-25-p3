using Microsoft.EntityFrameworkCore;
using TiendaOnline.Backend.Models;

namespace TiendaOnline.Backend.Data
{
    public class TiendaContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Compra> Compras { get; set; } = null!;
        public DbSet<ItemCompra> ItemsCompra { get; set; } = null!;
        public DbSet<Carrito>      Carritos      { get; set; } = null!;
        public DbSet<CarritoItem>  ItemsCarrito  { get; set; } = null!;

        public TiendaContext(DbContextOptions<TiendaContext> opts)
            : base(opts) { }

    }
}
