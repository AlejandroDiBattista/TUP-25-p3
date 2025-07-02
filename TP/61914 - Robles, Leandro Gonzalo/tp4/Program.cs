using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = ""; // A, B o C
    public List<Respuesta> Respuestas { get; set; } = new();
}

class Examen
{
    public int ExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int TotalPreguntas { get; set; }
    public int Correctas { get; set; }
    public double NotaFinal { get; set; }
    public List<Respuesta> Respuestas { get; set; } = new();
}

class Respuesta
{
    public int RespuestaId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public bool FueCorrecta { get; set; }
}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program
{
    static void Main()
    {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        while (true)
        {
            Console.WriteLine("\n1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver exámenes rendidos");
            Console.WriteLine("4. Ranking de alumnos");
            Console.WriteLine("5. Estadísticas por pregunta");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            string? opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": RegistrarPregunta(db); break;
                case "2": TomarExamen(db); break;
                case "3": VerExamenes(db); break;
                case "4": MostrarRanking(db); break;
                case "5": EstadisticasPregunta(db); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida"); break;
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db)
    {
        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine() ?? "";
        Console.Write("Respuesta A: ");
        string a = Console.ReadLine() ?? "";
        Console.Write("Respuesta B: ");
        string b = Console.ReadLine() ?? "";
        Console.Write("Respuesta C: ");
        string c = Console.ReadLine() ?? "";
        Console.Write("Correcta (A/B/C): ");
        string correcta = Console.ReadLine()?.ToUpper() ?? "";

        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        };

        db.Preguntas.Add(pregunta);
        db.SaveChanges();

        Console.WriteLine("Pregunta registrada correctamente.");
    }

    static void TomarExamen(DatosContexto db)
    {
        Console.Write("Nombre del alumno: ");
        string nombre = Console.ReadLine()?.Trim() ?? "Alumno";

        var preguntas = db.Preguntas
            .AsEnumerable()
            .OrderBy(p => Guid.NewGuid())
            .Take(5)
            .ToList();

        if (!preguntas.Any())
        {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }

        int correctas = 0;
        var respuestas = new List<Respuesta>();

        foreach (var pregunta in preguntas)
        {
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");
            Console.Write("Respuesta (A/B/C): ");
            string? respuesta = Console.ReadLine()?.ToUpper().Trim();

            bool esCorrecta = respuesta == pregunta.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new Respuesta
            {
                PreguntaId = pregunta.PreguntaId,
                FueCorrecta = esCorrecta
            });
        }

        int totalPreguntas = preguntas.Count;
        double notaFinal = totalPreguntas > 0 ? (correctas / (double)totalPreguntas) * 10 : 0.0;
        if (double.IsNaN(notaFinal)) notaFinal = 0.0;

        var examen = new Examen
        {
            Alumno = nombre,
            TotalPreguntas = totalPreguntas,
            Correctas = correctas,
            NotaFinal = notaFinal,
            Respuestas = respuestas
        };

        db.Examenes.Add(examen);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Respuestas correctas: {correctas}/{totalPreguntas}. Nota final: {notaFinal:F2}");
    }

    static void VerExamenes(DatosContexto db)
    {
        Console.Write("Nombre del alumno (vacío para todos): ");
        string filtro = Console.ReadLine()?.Trim() ?? "";
        var examenes = db.Examenes
            .Where(e => filtro == "" || e.Alumno.Contains(filtro))
            .OrderByDescending(e => e.ExamenId)
            .ToList();

        foreach (var e in examenes)
        {
            Console.WriteLine($"\nAlumno: {e.Alumno} - Nota: {e.NotaFinal:F2} ({e.Correctas}/{e.TotalPreguntas})");
        }
    }

    static void MostrarRanking(DatosContexto db)
    {
        var ranking = db.Examenes
            .GroupBy(e => e.Alumno)
            .Select(g => new { Alumno = g.Key, MejorNota = g.Max(e => e.NotaFinal) })
            .OrderByDescending(x => x.MejorNota)
            .ToList();

        Console.WriteLine("\nRanking de mejores alumnos:");
        foreach (var r in ranking)
        {
            Console.WriteLine($"{r.Alumno}: {r.MejorNota:F2}");
        }
    }

    static void EstadisticasPregunta(DatosContexto db)
    {
        var stats = db.Preguntas
            .Select(p => new
            {
                p.Enunciado,
                Total = p.Respuestas.Count(),
                Correctas = p.Respuestas.Count(r => r.FueCorrecta)
            })
            .ToList();

        Console.WriteLine("\nEstadísticas por pregunta:");
        foreach (var s in stats)
        {
            double porcentaje = s.Total > 0 ? (s.Correctas / (double)s.Total) * 100 : 0;
            Console.WriteLine($"\n{s.Enunciado}\n - Respondida: {s.Total} veces\n - Correctas: {porcentaje:F1}%");
        }
    }
}
