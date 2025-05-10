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
    public string Correcta   { get; set; } = "";
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int CantCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta? Pregunta { get; set; }
    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen? ResultadoExamen { get; set; }
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
    static void Main(string[] args) {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        if (!db.Preguntas.Any()) {
            db.Preguntas.AddRange(
                new Pregunta {
                    Enunciado = "¿Lenguaje principal de .NET?",
                    RespuestaA = "Java", RespuestaB = "C#", RespuestaC = "Python", Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Base de datos liviana en archivos?",
                    RespuestaA = "Oracle", RespuestaB = "PostgreSQL", RespuestaC = "SQLite", Correcta = "C"
                },
                new Pregunta {
                    Enunciado = "¿Qué es una clase en C#?",
                    RespuestaA = "Un tipo de dato", RespuestaB = "Un método", RespuestaC = "Un bucle", Correcta = "A"
                },
                new Pregunta {
                    Enunciado = "¿Qué palabra clave se usa para herencia en C#?",
                    RespuestaA = "inherits", RespuestaB = "extends", RespuestaC = ":", Correcta = "C"
                },
                new Pregunta {
                    Enunciado = "¿Qué hace 'using' en C#?",
                    RespuestaA = "Importa espacios de nombres", RespuestaB = "Declara variables", RespuestaC = "Repite código", Correcta = "A"
                },
                new Pregunta {
                    Enunciado = "¿Qué operador se utiliza para comparar igualdad en C#?",
                    RespuestaA = "=", RespuestaB = "==", RespuestaC = "!=", Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Cuál es el tipo de dato que representa números decimales?",
                    RespuestaA = "int", RespuestaB = "bool", RespuestaC = "double", Correcta = "C"
                },
                new Pregunta {
                    Enunciado = "¿Cuál de estas estructuras permite repetir instrucciones?",
                    RespuestaA = "if", RespuestaB = "while", RespuestaC = "using", Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Qué método imprime en la consola?",
                    RespuestaA = "Console.WriteLine", RespuestaB = "print()", RespuestaC = "System.out.println", Correcta = "A"
                },
                new Pregunta {
                    Enunciado = "¿Qué palabra clave se usa para definir una clase en C#?",
                    RespuestaA = "function", RespuestaB = "var", RespuestaC = "class", Correcta = "C"
                }
            );
            db.SaveChanges();
        }

        Console.Write("Nombre del alumno: ");
        string alumno = Console.ReadLine() ?? "Anónimo";

        var preguntas = db.Preguntas.OrderBy(x => EF.Functions.Random()).Take(5).ToList();
        var respuestas = new List<RespuestaExamen>();
        int correctas = 0;

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($" A) {p.RespuestaA}");
            Console.WriteLine($" B) {p.RespuestaB}");
            Console.WriteLine($" C) {p.RespuestaC}");
            Console.Write("Tu respuesta (A, B o C): ");
            string resp = Console.ReadLine()?.ToUpper() ?? "";

            bool esCorrecta = resp == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaAlumno = resp,
                EsCorrecta = esCorrecta
            });
        }

        double nota = (10.0 * correctas) / preguntas.Count;

        var resultado = new ResultadoExamen {
            Alumno = alumno,
            CantCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = Math.Round(nota, 2),
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nFinalizado. Correctas: {correctas}/{preguntas.Count}, Nota: {nota:0.00}/10\n");

        MostrarMenuReportes(db);
    }

    static void MostrarMenuReportes(DatosContexto db) {
        while (true) {
            Console.WriteLine("\n--- Reportes ---");
            Console.WriteLine("1. Ver todos los exámenes");
            Console.WriteLine("2. Filtrar por alumno");
            Console.WriteLine("3. Ranking de mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            var op = Console.ReadLine();

            switch (op) {
                case "1":
                    foreach (var r in db.Resultados)
                        Console.WriteLine($"{r.Alumno}: {r.NotaFinal}/10 ({r.CantCorrectas}/{r.TotalPreguntas})");
                    break;
                case "2":
                    Console.Write("Nombre: ");
                    string nombre = Console.ReadLine() ?? "";
                    var filtrar = db.Resultados.Where(r => r.Alumno.ToLower().Contains(nombre.ToLower()));
                    foreach (var r in filtrar)
                        Console.WriteLine($"{r.Alumno}: {r.NotaFinal}/10 ({r.CantCorrectas}/{r.TotalPreguntas})");
                    break;
                case "3":
                    var ranking = db.Resultados
                        .GroupBy(r => r.Alumno)
                        .Select(g => new { Alumno = g.Key, MejorNota = g.Max(x => x.NotaFinal) })
                        .OrderByDescending(x => x.MejorNota);
                    foreach (var r in ranking)
                        Console.WriteLine($"{r.Alumno}: {r.MejorNota}/10");
                    break;
                case "4":
                    var stats = db.Respuestas
                        .GroupBy(r => r.PreguntaId)
                        .Select(g => new {
                            Pregunta = db.Preguntas.First(p => p.PreguntaId == g.Key),
                            Total = g.Count(),
                            Correctas = g.Count(x => x.EsCorrecta)
                        });
                    foreach (var s in stats)
                        Console.WriteLine($"\n{s.Pregunta.Enunciado}\n Total: {s.Total}, % Correctas: {(100.0 * s.Correctas / s.Total):0.00}%");
                    break;
                case "0": return;
                default: Console.WriteLine("Opción inválida"); break;
            }
        }
    }
}
