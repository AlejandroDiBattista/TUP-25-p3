using Microsoft.EntityFrameworkCore;
using TP4.Models;

namespace TP4.Data;

public class ExamenContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=examen.db");
}