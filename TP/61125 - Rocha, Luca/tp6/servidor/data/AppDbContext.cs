using Microsoft.EntityFrameworkCore;
using TiendaOnline.Server.Models;

namespace TiendaOnline.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Puedes agregar configuraciones si es necesario
        }
    }
}
