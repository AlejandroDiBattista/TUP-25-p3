using Microsoft.EntityFrameworkCore;
using TiendaApi.Models;

namespace TiendaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
}
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Producto>().ToTable("Productos");
        modelBuilder.Entity<Carrito>().ToTable("Carritos");
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
    }
}