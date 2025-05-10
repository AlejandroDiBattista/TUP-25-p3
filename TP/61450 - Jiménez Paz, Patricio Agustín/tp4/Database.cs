using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Logging;

namespace TP4_61450;

public class DatosContexto : DbContext
{
    static void Log(string mensaje) => File.AppendAllText("historia.log", $"{mensaje}\n\n");

    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<PreguntaExamen> PreguntasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
        //optionsBuilder.LogTo(Log, LogLevel.Information);
    }
}
