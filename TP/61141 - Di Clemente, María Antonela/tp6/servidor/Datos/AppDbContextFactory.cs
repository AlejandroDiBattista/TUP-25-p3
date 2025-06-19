using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using servidor.Datos;

namespace servidor
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=productos.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
