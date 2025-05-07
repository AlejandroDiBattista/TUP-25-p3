using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = ""; // "A", "B" o "C"

    public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;

    public string RespuestaSeleccionada { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program {
    static void Main() {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        while (true) {
            Console.Clear();
            Console.WriteLine("=== MENÚ PRINCIPAL ===");
            Console.WriteLine("1. Registrar nueva pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver reportes");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion) {
                case "1": RegistrarPregunta(db); break;
                case "2": TomarExamen(db); break;
                case "3": MostrarReportes(db); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida"); break;
            }

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }
    }

    static void RegistrarPregunta(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== REGISTRAR PREGUNTA ===");

        var p = new Pregunta();
        Console.Write("Enunciado: ");
        p.Enunciado = Console.ReadLine() ?? "";
        Console.Write("Respuesta A: ");
        p.RespuestaA = Console.ReadLine() ?? "";
        Console.Write("Respuesta B: ");
        p.RespuestaB = Console.ReadLine() ?? "";
        Console.Write("Respuesta C: ");
        p.RespuestaC = Console.ReadLine() ?? "";
        Console.Write("Respuesta correcta (A, B o C): ");
        p.Correcta = Console.ReadLine()!.ToUpper();

        db.Preguntas.Add(p);
        db.SaveChanges();

        Console.WriteLine("Pregunta registrada correctamente.");
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== TOMAR EXAMEN ===");
        Console.Write("Nombre del alumno: ");
        string nombre = Console.ReadLine() ?? "";

        var preguntas = db.Preguntas.ToList()
                            .OrderBy(p => Guid.NewGuid())
                            .Take(5)
                            .ToList();

        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine(p.Enunciado);
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("Respuesta (A, B, C): ");
            string respuesta = Console.ReadLine()!.ToUpper();

            bool esCorrecta = respuesta == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaSeleccionada = respuesta,
                EsCorrecta = esCorrecta
            });
        }

        var resultado = new ResultadoExamen {
            NombreAlumno = nombre,
            CantidadCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = Math.Round(correctas * 10.0 / preguntas.Count, 2),
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado.");
        Console.WriteLine($"Correctas: {correctas} de {preguntas.Count}");
        Console.WriteLine($"Nota: {resultado.NotaFinal}/10");
    }

    static void MostrarReportes(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== REPORTES ===");
        Console.WriteLine("1. Ver todos los exámenes");
        Console.WriteLine("2. Filtrar por alumno");
        Console.WriteLine("3. Ranking de mejores notas");
        Console.WriteLine("4. Estadísticas por pregunta");
        Console.Write("Opción: ");
        string op = Console.ReadLine()!;

        switch (op) {
            case "1":
                Console.Clear();
                var todos = db.Resultados.ToList();
                foreach (var r in todos) {
                    Console.WriteLine($"{r.NombreAlumno} - {r.NotaFinal}/10 ({r.CantidadCorrectas}/{r.TotalPreguntas})");
                }
                break;

            case "2":
                Console.Write("Nombre del alumno: ");
                string alumno = Console.ReadLine() ?? "";
                var filtrados = db.Resultados.Where(r => r.NombreAlumno == alumno).ToList();
                foreach (var r in filtrados) {
                    Console.WriteLine($"{r.NombreAlumno} - {r.NotaFinal}/10 ({r.CantidadCorrectas}/{r.TotalPreguntas})");
                }
                break;

            case "3":
                var ranking = db.Resultados
                                .OrderByDescending(r => r.NotaFinal)
                                .Select(r => new {
                                    r.NombreAlumno,
                                    r.NotaFinal
                                })
                                .ToList();

                Console.Clear();
                foreach (var item in ranking) {
                    Console.WriteLine($"{item.NombreAlumno} - {item.NotaFinal}/10");
                }
                break;

            case "4":
                var stats = db.Preguntas
                    .Select(p => new {
                        Enunciado = p.Enunciado,
                        VecesRespondida = p.Respuestas.Count,
                        PorcentajeCorrectas = p.Respuestas.Count == 0 ? 0 :
                            p.Respuestas.Count(r => r.EsCorrecta) * 100.0 / p.Respuestas.Count
                    })
                    .ToList();

                Console.Clear();
                foreach (var s in stats) {
                    Console.WriteLine($"""
                    Pregunta: {s.Enunciado}
                    Respondida: {s.VecesRespondida} veces
                    % Correctas: {Math.Round(s.PorcentajeCorrectas, 2)}%
                    """);
                }
                break;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }
}
