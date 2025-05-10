using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


public class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
}

public class ResultadoExamen {
    public int Id { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class RespuestaExamen {
    public int Id { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;

    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

public class AppDbContext : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program {
    static void Main(string[] args) {
        using var db = new AppDbContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        while (true) {
            Console.Clear();
            Console.WriteLine("""
            SISTEMA DE EXÁMENES

            1) Registrar nueva pregunta
            2) Rendir examen
            3) Ver historial de exámenes
            4) Buscar por alumno
            5) Ranking de mejores alumnos
            6) Estadísticas por pregunta
            0) Salir
            """);

            Console.Write("Elija una opción: ");
            var op = Console.ReadLine();

            switch (op) {
                case "1": RegistrarPregunta(db); break;
                case "2": RendirExamen(db); break;
                case "3": MostrarHistorial(db); break;
                case "4": BuscarPorAlumno(db); break;
                case "5": MostrarRanking(db); break;
                case "6": EstadisticasPregunta(db); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida."); break;
            }

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }
    }

    static void RegistrarPregunta(AppDbContext db) {
        Console.Clear();
        Console.WriteLine("REGISTRO DE PREGUNTA");

        var p = new Pregunta();
        Console.Write("Enunciado: "); p.Enunciado = Console.ReadLine() ?? "";
        Console.Write("Respuesta A: "); p.RespuestaA = Console.ReadLine() ?? "";
        Console.Write("Respuesta B: "); p.RespuestaB = Console.ReadLine() ?? "";
        Console.Write("Respuesta C: "); p.RespuestaC = Console.ReadLine() ?? "";
        Console.Write("Correcta (A/B/C): "); p.Correcta = Console.ReadLine()?.ToUpper() ?? "A";

        db.Preguntas.Add(p);
        db.SaveChanges();

        Console.WriteLine("Pregunta registrada con éxito.");
    }

    static void RendirExamen(AppDbContext db) {
        Console.Clear();
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine() ?? "";

        var preguntas = db.Preguntas.ToList().OrderBy(p => Guid.NewGuid()).Take(5).ToList();

        if (preguntas.Count == 0) {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine($"{p.Enunciado}");
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");

            Console.Write("Tu respuesta (A/B/C): ");
            var rta = Console.ReadLine()?.ToUpper() ?? "";

            bool esCorrecta = rta == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaAlumno = rta,
                EsCorrecta = esCorrecta
            });
        }

        double nota = Math.Round((correctas / (double)preguntas.Count) * 10, 2);

        var resultado = new ResultadoExamen {
            NombreAlumno = nombre,
            CantCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = nota,
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Obtuviste {correctas}/{preguntas.Count} respuestas correctas. Nota final: {nota}");
    }

    static void MostrarHistorial(AppDbContext db) {
        Console.Clear();
        var resultados = db.Resultados.ToList();

        foreach (var r in resultados) {
            Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal} ({r.CantCorrectas}/{r.TotalPreguntas})");
        }
    }

    static void BuscarPorAlumno(AppDbContext db) {
        Console.Clear();
        Console.Write("Ingrese nombre a buscar: ");
        var nombre = Console.ReadLine() ?? "";

        var resultados = db.Resultados
            .Where(r => r.NombreAlumno.Contains(nombre))
            .ToList();

        foreach (var r in resultados) {
            Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal}");
        }
    }

    static void MostrarRanking(AppDbContext db) {
        Console.Clear();
        var ranking = db.Resultados
            .GroupBy(r => r.NombreAlumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(x => x.NotaFinal)
            })
            .OrderByDescending(x => x.MejorNota)
            .ToList();

        Console.WriteLine("Ranking de mejores alumnos:");
        foreach (var r in ranking) {
            Console.WriteLine($"{r.Alumno} - Nota: {r.MejorNota}");
        }
    }

    static void EstadisticasPregunta(AppDbContext db) {
        Console.Clear();
        var preguntas = db.Preguntas.ToList();

        foreach (var p in preguntas) {
            var total = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId);
            var correctas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta);
            double porcentaje = total == 0 ? 0 : 100.0 * correctas / total;

            Console.WriteLine($"""
            {p.Enunciado}
            Total respondida: {total}
            Porcentaje correctas: {porcentaje:F1}%
            """);
        }
    }
}