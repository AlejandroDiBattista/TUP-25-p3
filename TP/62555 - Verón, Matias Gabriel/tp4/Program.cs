using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int CantCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany()
            .HasForeignKey(r => r.PreguntaId);
    }
}

class Program {
    static void Main() {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        if (!db.Preguntas.Any()) {
            db.Preguntas.AddRange(new List<Pregunta> {
                
    
                new() {
                    Enunciado = "¿Qué método de Entity Framework Core se utiliza para guardar cambios en la base de datos?",
                    RespuestaA = "SaveChanges()",
                    RespuestaB = "CommitChanges()",
                    RespuestaC = "UpdateDatabase()",
                    Correcta = "A"
                },
                new() {
                    Enunciado = "¿Qué palabra clave se utiliza para definir una clase en C#?",
                    RespuestaA = "define",
                    RespuestaB = "class",
                    RespuestaC = "struct",
                    Correcta = "B"
                },
                new() {
                    Enunciado = "¿Qué método se utiliza para imprimir en consola en C#?",
                    RespuestaA = "Console.WriteLine",
                    RespuestaB = "System.Print",
                    RespuestaC = "Console.Output",
                    Correcta = "A"
                },
                new() {
                    Enunciado = "¿Qué operador se utiliza para comparar igualdad en C#?",
                    RespuestaA = "=",
                    RespuestaB = "==",
                    RespuestaC = "===",
                    Correcta = "B"
                },
                new() {
                    Enunciado = "¿Qué estructura se utiliza para manejar excepciones en C#?",
                    RespuestaA = "try-catch",
                    RespuestaB = "if-else",
                    RespuestaC = "switch-case",
                    Correcta = "A"
                }
            }
            );

            db.SaveChanges();
        }

        while (true)
        {
            Console.WriteLine("""
            === SISTEMA DE EXÁMENES ===

            1. Rendir examen
            2. Ver exámenes rendidos
            3. Ver ranking
            4. Estadísticas por pregunta
            5. Salir
            """);

            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            if (opcion == "1")
            {
                TomarExamen(db);
            }
            else if (opcion == "2")
            {
                Examenes(db);
            }
            else if (opcion == "3")
            {
                Ranking(db);
            }
            else if (opcion == "4")
            {
                Estadisticas(db);
            }
            else if (opcion == "5")
            {
                break;
            }
            else
            {
                Console.WriteLine("Opción inválida.");
            }

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }
    }

   static void TomarExamen(DatosContexto db)
{
    Console.Write("Nombre del alumno: ");
    var alumno = Console.ReadLine() ?? "Anónimo";

    var preguntas = db.Preguntas
        .ToList()                       
        .OrderBy(p => Guid.NewGuid())   
        .Take(5)                        
        .ToList();

    int correctas = 0;
    var respuestas = new List<RespuestaExamen>();

    foreach (var p in preguntas)
    {
        Console.Clear();
        Console.WriteLine($"""
        {p.Enunciado}

        A) {p.RespuestaA}
        B) {p.RespuestaB}
        C) {p.RespuestaC}
        """);

        string? r;
        do
        {
            Console.Write("Respuesta (A/B/C): ");
            r = Console.ReadLine()?.Trim().ToUpper();
        } while (r != "A" && r != "B" && r != "C");

        bool esCorrecta = r == p.Correcta;
        if (esCorrecta) correctas++;

        respuestas.Add(new RespuestaExamen
        {
            PreguntaId = p.PreguntaId,
            RespuestaAlumno = r,
            EsCorrecta = esCorrecta
        });
    }

    var resultado = new ResultadoExamen
    {
        Alumno = alumno,
        CantCorrectas = correctas,
        TotalPreguntas = preguntas.Count,
        NotaFinal = Math.Round(correctas * 10.0 / preguntas.Count, 2),
        Respuestas = respuestas
    };

    db.Resultados.Add(resultado);
    db.SaveChanges();

    Console.WriteLine($"\nExamen terminado. Respuestas correctas: {correctas}. Nota: {resultado.NotaFinal}/10");
}

    static void Examenes(DatosContexto db)
    {
        Console.Write("Filtrar por alumno (dejar vacío para todos): ");
        var filtro = Console.ReadLine();

        var examenes = db.Resultados
            .Where(e => filtro == "" || e.Alumno.Contains(filtro))
            .OrderByDescending(e => e.ResultadoExamenId)
            .ToList();

        foreach (var e in examenes)
        {
            Console.WriteLine($"#{e.ResultadoExamenId:000} - {e.Alumno} - Nota: {e.NotaFinal}/10 ({e.CantCorrectas}/{e.TotalPreguntas})");
        }
    }

    static void Ranking(DatosContexto db)
    {
        var ranking = db.Resultados
            .GroupBy(e => e.Alumno)
            .Select(g => new
            {
                Alumno = g.Key,
                MejorNota = g.Max(e => e.NotaFinal)
            })
            .OrderByDescending(r => r.MejorNota)
            .Take(10)
            .ToList();

        Console.WriteLine("Mejores alumnos:");
        foreach (var r in ranking)
        {
            Console.WriteLine($"{r.Alumno} - Nota: {r.MejorNota}/10");
        }
    }

    static void Estadisticas(DatosContexto db)
    {
        var stats = db.Respuestas
            .GroupBy(r => r.PreguntaId)
            .Select(g => new
            {
                Pregunta = g.First().Pregunta.Enunciado,
                Total = g.Count(),
                Correctas = g.Count(r => r.EsCorrecta)
            })
            .ToList();

        foreach (var s in stats)
        {
            var porcentaje = s.Total == 0 ? 0 : (s.Correctas * 100.0 / s.Total);
            Console.WriteLine($"\n{s.Pregunta}\nRespondida: {s.Total} veces - Correctas: {porcentaje:0.0}%");
        }
    }
}
