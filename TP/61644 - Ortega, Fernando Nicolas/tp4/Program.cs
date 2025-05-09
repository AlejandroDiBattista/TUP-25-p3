using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;

#region Modelos

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";

    public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int CantCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double Nota { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

#endregion

#region DbContext

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

#endregion

class Program {
    const int CantidadPreguntasExamen = 5;

    static void Main(string[] args) {
        using (var db = new DatosContexto()) {
            db.Database.EnsureCreated();

            while (true) {
                Console.WriteLine("\n--- MENÚ ---");
                Console.WriteLine("1. Registrar pregunta");
                Console.WriteLine("2. Tomar examen");
                Console.WriteLine("3. Ver reportes");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                switch (op) {
                    case "1": RegistrarPregunta(db); break;
                    case "2": TomarExamen(db); break;
                    case "3": MostrarReportes(db); break;
                    case "0": return;
                    default: Console.WriteLine("Opción inválida"); break;
                }
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db) {
        var p = new Pregunta();
        Console.Write("Enunciado: "); p.Enunciado = Console.ReadLine();
        Console.Write("Respuesta A: "); p.RespuestaA = Console.ReadLine();
        Console.Write("Respuesta B: "); p.RespuestaB = Console.ReadLine();
        Console.Write("Respuesta C: "); p.RespuestaC = Console.ReadLine();
        Console.Write("Respuesta correcta (A/B/C): "); p.Correcta = Console.ReadLine().ToUpper();

        db.Preguntas.Add(p);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada.");
    }

    static void TomarExamen(DatosContexto db) {
        Console.Write("Nombre del alumno: ");
        string alumno = Console.ReadLine();

        var preguntas = db.Preguntas.OrderBy(p => Guid.NewGuid()).Take(CantidadPreguntasExamen).ToList();
        if (preguntas.Count == 0) {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }

        var resultado = new ResultadoExamen { Alumno = alumno, TotalPreguntas = preguntas.Count };
        foreach (var p in preguntas) {
            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("Respuesta: ");
            string r = Console.ReadLine().ToUpper();

            bool correcta = r == p.Correcta;
            if (correcta) resultado.CantCorrectas++;

            resultado.Respuestas.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaAlumno = r,
                EsCorrecta = correcta
            });
        }

        resultado.Nota = Math.Round(10.0 * resultado.CantCorrectas / resultado.TotalPreguntas, 2);
        db.ResultadosExamen.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Correctas: {resultado.CantCorrectas}/{resultado.TotalPreguntas}. Nota: {resultado.Nota}");
    }

    static void MostrarReportes(DatosContexto db) {
        Console.WriteLine("\n--- REPORTES ---");
        Console.WriteLine("1. Listado de exámenes");
        Console.WriteLine("2. Filtrar por alumno");
        Console.WriteLine("3. Ranking de mejores alumnos");
        Console.WriteLine("4. Estadísticas por pregunta");
        Console.WriteLine("0. Volver");
        Console.Write("Opción: ");
        var op = Console.ReadLine();

        switch (op) {
            case "1":
                foreach (var r in db.ResultadosExamen)
                    Console.WriteLine($"{r.ResultadoExamenId}) {r.Alumno}: Nota {r.Nota} ({r.CantCorrectas}/{r.TotalPreguntas})");
                break;

            case "2":
                Console.Write("Nombre del alumno: ");
                string nombre = Console.ReadLine();
                var resultados = db.ResultadosExamen.Where(r => r.Alumno == nombre);
                foreach (var r in resultados)
                    Console.WriteLine($"{r.ResultadoExamenId}) Nota {r.Nota} ({r.CantCorrectas}/{r.TotalPreguntas})");
                break;

            case "3":
                var ranking = db.ResultadosExamen
                    .GroupBy(r => r.Alumno)
                    .Select(g => new {
                        Alumno = g.Key,
                        MejorNota = g.Max(x => x.Nota)
                    })
                    .OrderByDescending(x => x.MejorNota)
                    .ToList();

                foreach (var item in ranking)
                    Console.WriteLine($"{item.Alumno}: Mejor nota = {item.MejorNota}");
                break;

            case "4":
                var preguntas = db.Preguntas.Include(p => p.Respuestas);
                foreach (var p in preguntas) {
                    int total = p.Respuestas.Count;
                    int correctas = p.Respuestas.Count(r => r.EsCorrecta);
                    double porcentaje = total > 0 ? 100.0 * correctas / total : 0;
                    Console.WriteLine($"\nPregunta #{p.PreguntaId}");
                    Console.WriteLine(p.Enunciado);
                    Console.WriteLine($"Respondida: {total} veces. Correctas: {porcentaje:0.0}%");
                }
                break;

            case "0":
                return;
            default:
                Console.WriteLine("Opción inválida");
                break;
        }
    }
}