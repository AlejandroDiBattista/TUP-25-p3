using Microsoft.EntityFrameworkCore;

namespace servidor.Models
{
    public class ModelsTiendaContext : DbContext
    {
        public ModelsTiendaContext(DbContextOptions<ModelsTiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
    }
}
