using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data
{
    public class InventarioContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=productos.db");
    }
}

