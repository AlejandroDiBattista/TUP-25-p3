using Examenes.Models;
using Microsoft.EntityFrameworkCore;

namespace Examenes.Data;

public class AppDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<ResultadoExamen> ResultadosExamenes => Set<ResultadoExamen>();
    public DbSet<RespuestaExamen> RespuestasExamen => Set<RespuestaExamen>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=examenes.db");
}