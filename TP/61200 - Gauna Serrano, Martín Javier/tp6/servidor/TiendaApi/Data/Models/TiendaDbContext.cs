using Microsoft.EntityFrameworkCore;

namespace TiendaApi.Models
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Carrito> Carritos { get; set; }
public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Producto>().ToTable("Productos");
            modelBuilder.Entity<Carrito>().ToTable("Carritos");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        }
    }
}