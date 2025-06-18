using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data;

public class TiendaDbContext : DbContext
{

    public DbSet<Compra> Compras => Set<Compra>();
public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
}
