using Microsoft.EntityFrameworkCore;

namespace servidor.Modulos
{
    public class TiendaContexto : DbContext
    {
        public TiendaContexto(DbContextOptions<TiendaContexto> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
    }
}