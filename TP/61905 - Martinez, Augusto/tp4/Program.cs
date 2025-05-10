using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
}

class RespuestaExamen {
    public int Id { get; set; }
    public int ResultadoExamenId { get; set; }
    public int PreguntaId { get; set; }
    public string RespuestaDada { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class ResultadoExamen {
    public int Id { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
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
        using (var db = new DatosContexto()) {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            CrearPreguntaInicial(db);

            while (true) {
                Console.Clear();
                Console.WriteLine("""
                    ===== Sistema de Examen =====
                    1. Agregar preguntas
                    2. Eliminar pregunta
                    3. Tomar examen
                    4. Ver resultados
                    5. Ver ranking
                    6. Ver estadísticas
                    7. Salir
                    """);

                Console.Write("Seleccione una opción: ");
                switch (Console.ReadLine()) {
                    case "1": AgregarPreguntas(db); break;
                    case "2": EliminarPregunta(db); break;
                    case "3": TomarExamen(db); break;
                    case "4": VerResultados(db); break;
                    case "5": VerRanking(db); break;
                    case "6": VerEstadisticas(db); break;
                    case "7": return;
                    default: Console.WriteLine("Opción inválida."); break;
                }
            }
        }
    }

    static void CrearPreguntaInicial(DatosContexto db) {
        if (!db.Preguntas.Any()) {
            db.Preguntas.Add(new Pregunta {
                Enunciado = "¿Cuál es el lenguaje de programación de .NET?",
                RespuestaA = "Java",
                RespuestaB = "C#",
                RespuestaC = "Python",
                Correcta = "B"
            });
            db.SaveChanges();
        }
    }

    static void AgregarPreguntas(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Agregar preguntas ===");
        while (true) {
            Console.Write("Enunciado (o enter para salir): ");
            var enunciado = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(enunciado)) break;

            Console.Write("Opción A: "); var a = Console.ReadLine();
            Console.Write("Opción B: "); var b = Console.ReadLine();
            Console.Write("Opción C: "); var c = Console.ReadLine();

            string correcta;
            do {
                Console.Write("¿Opción correcta? (A/B/C): ");
                correcta = Console.ReadLine()?.ToUpper().Trim();
            } while (correcta != "A" && correcta != "B" && correcta != "C");

            db.Preguntas.Add(new Pregunta {
                Enunciado = enunciado, RespuestaA = a, RespuestaB = b, RespuestaC = c, Correcta = correcta
            });
            db.SaveChanges();
            Console.WriteLine("Pregunta agregada.\n");
        }
    }

    static void EliminarPregunta(DatosContexto db) {
        Console.Clear();
        var preguntas = db.Preguntas.ToList();
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas.");
            Console.ReadKey(); return;
        }

        foreach (var p in preguntas) {
            Console.WriteLine($"{p.Id}. {p.Enunciado}");
        }

        Console.Write("ID a eliminar: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            var pregunta = db.Preguntas.Find(id);
            if (pregunta != null) {
                db.Preguntas.Remove(pregunta);
                db.SaveChanges();
                Console.WriteLine("Eliminada.");
            } else Console.WriteLine("ID no encontrado.");
        }
        Console.ReadKey();
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(nombre) || !nombre.All(c => char.IsLetter(c) || char.IsWhiteSpace(c))) {
            Console.WriteLine("Nombre inválido."); Console.ReadKey(); return;
        }

        var preguntas = db.Preguntas.OrderBy(p => Guid.NewGuid()).Take(10).ToList();
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas."); Console.ReadKey(); return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine($"{p.Enunciado}\nA) {p.RespuestaA}\nB) {p.RespuestaB}\nC) {p.RespuestaC}");

            string r;
            do {
                Console.Write("Respuesta: ");
                r = Console.ReadLine()?.ToUpper().Trim();
            } while (r != "A" && r != "B" && r != "C");

            bool esCorrecta = r == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen {
                PreguntaId = p.Id,
                RespuestaDada = r,
                EsCorrecta = esCorrecta
            });
        }

        var resultado = new ResultadoExamen {
            NombreAlumno = nombre,
            CantidadCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = correctas
        };
        db.Resultados.Add(resultado);
        db.SaveChanges();

        foreach (var r in respuestas) {
            r.ResultadoExamenId = resultado.Id;
            db.Respuestas.Add(r);
        }
        db.SaveChanges();

        Console.WriteLine($"Nota final: {resultado.NotaFinal}/10");
        Console.Write("¿Ver respuestas? (S/N): ");
        if (Console.ReadLine()?.ToUpper() == "S") {
            foreach (var r in respuestas) {
                var p = preguntas.First(x => x.Id == r.PreguntaId);
                Console.WriteLine($"\n{p.Enunciado}");
                Console.WriteLine($"Correcta: {p.Correcta} | Tu respuesta: {r.RespuestaDada} - {(r.EsCorrecta ? "Correcta" : "Incorrecta")}");
            }
        }
        Console.ReadKey();
    }

    static void VerResultados(DatosContexto db) {
        Console.Clear();
        Console.Write("Nombre del alumno (vacío para todos): ");
        string nombre = Console.ReadLine()?.Trim();

        var resultados = db.Resultados
            .Include(r => r.Respuestas)
            .Where(r => string.IsNullOrEmpty(nombre) || r.NombreAlumno.Contains(nombre))
            .ToList();

        if (!resultados.Any()) {
            Console.WriteLine("No hay resultados.");
        } else {
            foreach (var r in resultados) {
                Console.WriteLine($"\nAlumno: {r.NombreAlumno} - Nota: {r.NotaFinal}/10 - {r.CantidadCorrectas}/{r.TotalPreguntas} - Fecha: {r.Fecha}");
            }
        }
        Console.ReadKey();
    }

    static void VerRanking(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Ranking ===");

        var ranking = db.Resultados
            .OrderByDescending(r => r.NotaFinal)
            .ThenByDescending(r => r.Fecha)
            .Take(10)
            .ToList();

        if (!ranking.Any()) {
            Console.WriteLine("No hay resultados.");
        } else {
            foreach (var r in ranking) {
                Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal} - Fecha: {r.Fecha:dd/MM/yyyy}");
            }
        }
        Console.ReadKey();
    }

    static void VerEstadisticas(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Estadísticas ===");

        var preguntas = db.Preguntas.ToList();
        foreach (var p in preguntas) {
            var respuestas = db.Respuestas.Where(r => r.PreguntaId == p.Id).ToList();
            int total = respuestas.Count;
            int correctas = respuestas.Count(r => r.EsCorrecta);
            double porcentaje = total == 0 ? 0 : (correctas * 100.0 / total);

            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"Total: {total}, Correctas: {correctas}, Porcentaje: {porcentaje:F2}%");
        }
        Console.ReadKey();
    }
}