using System;
using System.Collections.Generic;
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

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program {
    static void Main() {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();  // Crea la base si no existe

        if (!db.Preguntas.Any()) {
            CargarPreguntas(db);
        }

        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine() ?? "Alumno";

        TomarExamen(db, nombre);

        Console.WriteLine("\n¿Desea ver reportes? (S/N): ");
        var op = Console.ReadLine()?.Trim().ToUpper();
        if (op == "S") {
            MostrarMenuReportes(db);
        }
    }

    static void TomarExamen(DatosContexto db, string nombre) {
        var preguntas = db.Preguntas
                          .ToList()
                          .OrderBy(p => Guid.NewGuid())
                          .Take(5)
                          .ToList();

        var resultado = new ResultadoExamen {
            NombreAlumno = nombre,
            TotalPreguntas = preguntas.Count
        };

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine($"""
                {p.Enunciado}

                A) {p.RespuestaA}
                B) {p.RespuestaB}
                C) {p.RespuestaC}
            """);

            string? respuesta;
            do {
                Console.Write("Tu respuesta (A, B o C): ");
                respuesta = Console.ReadLine()?.Trim().ToUpper();
            } while (respuesta != "A" && respuesta != "B" && respuesta != "C");

            bool esCorrecta = respuesta == p.Correcta;

            resultado.Respuestas.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaAlumno = respuesta,
                EsCorrecta = esCorrecta
            });

            if (esCorrecta) resultado.CantidadCorrectas++;
        }

        resultado.NotaFinal = resultado.CantidadCorrectas * 10.0 / resultado.TotalPreguntas;
        db.ResultadosExamen.Add(resultado);
        db.SaveChanges();

        Console.Clear();
        Console.WriteLine($"""
            Examen finalizado.
            Alumno: {resultado.NombreAlumno}
            Correctas: {resultado.CantidadCorrectas}/{resultado.TotalPreguntas}
            Nota final: {resultado.NotaFinal:F1}
        """);
    }

    static void MostrarMenuReportes(DatosContexto db) {
        while (true) {
            Console.Clear();
            Console.WriteLine(""" 
                === MENÚ DE REPORTES ===

                1. Ver todos los exámenes rendidos
                2. Filtrar resultados por nombre
                3. Ranking de mejores alumnos
                4. Informe estadístico por pregunta
                5. Salir
            """);

            Console.Write("Seleccione una opción: ");
            var op = Console.ReadLine();

            switch (op) {
                case "1":
                    VerExamenes(db);
                    break;
                case "2":
                    FiltrarPorNombre(db);
                    break;
                case "3":
                    MostrarRanking(db);
                    break;
                case "4":
                    InformePorPregunta(db);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }

            Console.WriteLine("\nPresione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    static void VerExamenes(DatosContexto db) {
        var examenes = db.ResultadosExamen
            .OrderByDescending(e => e.ResultadoExamenId)
            .ToList();

        Console.WriteLine("=== TODOS LOS EXÁMENES ===");
        foreach (var ex in examenes) {
            Console.WriteLine($"{ex.ResultadoExamenId:000} - {ex.NombreAlumno} - Nota: {ex.NotaFinal:F1} - Correctas: {ex.CantidadCorrectas}/{ex.TotalPreguntas}");
        }
    }

    static void FiltrarPorNombre(DatosContexto db) {
        Console.Write("Ingrese nombre del alumno: ");
        var nombre = Console.ReadLine() ?? "";

        var resultados = db.ResultadosExamen
            .Where(r => r.NombreAlumno.Contains(nombre))
            .ToList();

        Console.WriteLine($"=== EXÁMENES DE {nombre} ===");
        foreach (var r in resultados) {
            Console.WriteLine($"{r.ResultadoExamenId:000} - Nota: {r.NotaFinal:F1} - Correctas: {r.CantidadCorrectas}/{r.TotalPreguntas}");
        }
    }

    static void MostrarRanking(DatosContexto db) {
        var ranking = db.ResultadosExamen
            .GroupBy(r => r.NombreAlumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(r => r.NotaFinal)
            })
            .OrderByDescending(r => r.MejorNota)
            .ToList();

        Console.WriteLine("=== RANKING DE MEJORES ALUMNOS ===");
        int pos = 1;
        foreach (var r in ranking) {
            Console.WriteLine($"{pos++}. {r.Alumno} - Mejor Nota: {r.MejorNota:F1}");
        }
    }

    static void InformePorPregunta(DatosContexto db) {
        var informe = db.RespuestasExamen
            .Include(r => r.Pregunta)
            .GroupBy(r => r.Pregunta)
            .Select(g => new {
                Pregunta = g.Key.Enunciado,
                Total = g.Count(),
                Correctas = g.Count(r => r.EsCorrecta)
            })
            .ToList();

        Console.WriteLine("=== ESTADÍSTICAS POR PREGUNTA ===");
        foreach (var item in informe) {
            double porcentaje = item.Total == 0 ? 0 : (item.Correctas * 100.0 / item.Total);
            Console.WriteLine($"""
                Pregunta: {item.Pregunta}
                Respondida: {item.Total} veces
                Correctas: {item.Correctas} ({porcentaje:F1}%)
            """);
        }
    }

    static void CargarPreguntas(DatosContexto db) {
        var preguntas = new List<Pregunta> {
            new() { Enunciado = "¿Cual es el lenguaje de programacion desarrollado por Microsoft y utilizado principalmente en .NET?", RespuestaA = "Java", RespuestaB = "C#", RespuestaC = "Python", Correcta = "B" },
            new() { Enunciado = "¿Cuantos dias tiene una semana?", RespuestaA = "5", RespuestaB = "8", RespuestaC = "7", Correcta = "C" },
            new() { Enunciado = "¿Cuantos balones de oro tiene Lionel Messi?", RespuestaA = "6", RespuestaB = "7", RespuestaC = "8", Correcta = "C" },
            new() { Enunciado = "¿Cual es el operador de igualdad en C#?", RespuestaA = "==", RespuestaB = "=", RespuestaC = "!=", Correcta = "A" },
            new() { Enunciado = "¿Cual es el resultado de 20+20*2?", RespuestaA = "80", RespuestaB = "60", RespuestaC = "70", Correcta = "B" },
            new() { Enunciado = "¿Cuantos mundiales tiene la Seleccion Argentina?", RespuestaA = "3", RespuestaB = "2", RespuestaC = "4", Correcta = "A" },
            new() { Enunciado = "¿Cual es el operador de designacion en C#?", RespuestaA = "==", RespuestaB = "=", RespuestaC = "!=", Correcta = "B" },
            new() { Enunciado = "¿Que tipo de dato se usa para almacenar decimales?", RespuestaA = "int", RespuestaB = "string", RespuestaC = "double", Correcta = "C" },
            new() { Enunciado = "¿Que palabra clave se usa para definir una clase en C#?", RespuestaA = "class", RespuestaB = "struct", RespuestaC = "object", Correcta = "A" },
            new() { Enunciado = "¿Cual es el valor booleano que representa falso?", RespuestaA = "0", RespuestaB = "false", RespuestaC = "null", Correcta = "B" }
        };

        db.Preguntas.AddRange(preguntas);
        db.SaveChanges();
    }
}
