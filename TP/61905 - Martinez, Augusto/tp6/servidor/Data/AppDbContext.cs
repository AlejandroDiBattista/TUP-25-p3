using Microsoft.EntityFrameworkCore;
using servidor.Models;  // Ajusta el namespace según tu proyecto

namespace servidor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Agrega DbSet para tus modelos
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
    }
}
// Asegúrate de que el namespace y las referencias sean correctas según tu estructura de proyecto