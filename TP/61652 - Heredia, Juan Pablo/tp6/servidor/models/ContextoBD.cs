using Microsoft.EntityFrameworkCore;

namespace servidor.models
{
    public class TiendaDb : DbContext
    {
        public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<Compras> ItemsCompra => Set<Compras>();

    }
}