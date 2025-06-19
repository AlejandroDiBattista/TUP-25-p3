using Microsoft.EntityFrameworkCore;

public class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pregunta>()
            .HasMany(p => p.Respuestas)
            .WithOne(r => r.Pregunta)
            .HasForeignKey(r => r.PreguntaId);

        modelBuilder.Entity<ResultadoExamen>()
            .HasMany(r => r.Respuestas)
            .WithOne(re => re.ResultadoExamen)
            .HasForeignKey(re => re.ResultadoExamenId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(re => re.Pregunta)
            .WithMany()
            .HasForeignKey(re => re.PreguntaId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(re => re.Respuesta)
            .WithMany()
            .HasForeignKey(re => re.RespuestaId);
    }
}