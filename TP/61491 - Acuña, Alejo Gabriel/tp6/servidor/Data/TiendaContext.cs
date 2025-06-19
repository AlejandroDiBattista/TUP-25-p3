using Microsoft.EntityFrameworkCore;
using servidor.Models;
using servidor.Data;
namespace servidor.Data;


public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<ItemCarrito> ItemsCarrito => Set<ItemCarrito>();
}