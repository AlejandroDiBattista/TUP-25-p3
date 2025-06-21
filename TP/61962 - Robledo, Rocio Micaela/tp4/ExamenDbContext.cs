using Microsoft.EntityFrameworkCore;
using tp4.Models;

namespace tp4
{
    public class ExamenDbContext : DbContext
    {
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
        public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=examenes.db");
        }
    }
}
