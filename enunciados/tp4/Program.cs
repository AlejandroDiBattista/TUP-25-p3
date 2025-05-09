using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
}

class Examen {
    public int ExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int Correctas { get; set; }
    public int TotalPreguntas { get; set; }
    public float NotaFinal { get; set; }
    public List<Respuesta> Respuestas { get; set; } = new();
}

class Respuesta {
    public int RespuestaId { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Examen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ExamenId);

        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Pregunta)
            .WithMany()
            .HasForeignKey(r => r.PreguntaId);
    }
}

class Program {
    static void Main(string[] args) {
        using (var db = new DatosContexto()) {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Console.WriteLine("Base de datos creada correctamente.");

            // Poblar la tabla de preguntas
            db.Preguntas.AddRange(
                new Pregunta {
                    Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                    RespuestaA = "Java",
                    RespuestaB = "C#",
                    RespuestaC = "Python",
                    Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Qué significa SQL?",
                    RespuestaA = "Structured Query Language",
                    RespuestaB = "Simple Query Language",
                    RespuestaC = "Standard Query Language",
                    Correcta = "A"
                }
            );
            db.SaveChanges();

            Console.WriteLine("Preguntas iniciales agregadas.");
        }
    }
}