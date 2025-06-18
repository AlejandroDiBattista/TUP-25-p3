using Microsoft.EntityFrameworkCore;
using tp4.Modelos;

namespace tp4.Datos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
        public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=examenes.db");
    }
}
