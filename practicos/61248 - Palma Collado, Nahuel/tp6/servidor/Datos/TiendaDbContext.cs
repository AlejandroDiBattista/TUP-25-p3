using Microsoft.EntityFrameworkCore;
using TiendaOnline.Modelos;

namespace TiendaOnline.Datos
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemDeCompra> ItemsDeCompra => Set<ItemDeCompra>();
    }
}