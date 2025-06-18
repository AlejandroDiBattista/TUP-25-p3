using Microsoft.EntityFrameworkCore;
using SistemaExamenes.Console.Models;

namespace SistemaExamenes.Console.Data;

public class ExamenDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examenes.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de Pregunta
        modelBuilder.Entity<Pregunta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Enunciado).IsRequired().HasMaxLength(500);
            entity.Property(e => e.AlternativaA).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AlternativaB).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AlternativaC).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RespuestaCorrecta).IsRequired();
        });
        
        // Configuración de ResultadoExamen
        modelBuilder.Entity<ResultadoExamen>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreAlumno).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CantidadRespuestasCorrectas).IsRequired();
            entity.Property(e => e.TotalPreguntas).IsRequired();
            entity.Property(e => e.NotaFinal).IsRequired().HasPrecision(3, 2);
            entity.Property(e => e.FechaExamen).IsRequired();
        });
        
        // Configuración de RespuestaExamen
        modelBuilder.Entity<RespuestaExamen>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResultadoExamenId).IsRequired();
            entity.Property(e => e.PreguntaId).IsRequired();
            entity.Property(e => e.RespuestaSeleccionada).IsRequired();
            entity.Property(e => e.EsCorrecta).IsRequired();
            
            // Relaciones
            entity.HasOne(e => e.ResultadoExamen)
                  .WithMany(e => e.RespuestasExamen)
                  .HasForeignKey(e => e.ResultadoExamenId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Pregunta)
                  .WithMany(e => e.RespuestasExamen)
                  .HasForeignKey(e => e.PreguntaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 