using Microsoft.EntityFrameworkCore;
using servidor.ModeloDatos;

namespace servidor.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opciones) : base(opciones)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}
