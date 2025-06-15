using Microsoft.EntityFrameworkCore; // en relación al carrito
using Servidor.Models; // en relación al carrito

namespace Servidor.Data
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
        public DbSet<Carrito> Carritos => Set<Carrito>();           // lo agrego en relación al carrito
        public DbSet<ItemCarrito> ItemsCarrito => Set<ItemCarrito>(); // lo agrego en relación al carrito
    }
}