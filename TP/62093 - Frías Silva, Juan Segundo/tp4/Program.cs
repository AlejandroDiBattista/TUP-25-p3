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
    public string Correcta { get; set; } = ""; 
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
    public Pregunta? Pregunta { get; set; }
    public int ResultadoExamenId { get; set; } 
    public ResultadoExamen? ResultadoExamen { get; set; }
    public string RespuestaDada { get; set; } = ""; 
    public bool EsCorrecta { get; set; }
}


class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db"); 
    }
}


class Program {
    static void Main(string[] args) {
        using var db = new DatosContexto();
        db.Database.EnsureCreated(); 

        while (true) {
            Console.WriteLine("\nSistema de Exámenes");
            Console.WriteLine("1. Agregar pregunta");
            Console.WriteLine("2. Tomar exame");
            Console.WriteLine("3. Ver reportes");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine() ?? "";
            Console.Clear();
            switch (opcion) {
                case "1": AgregarPregunta(db); break;
                case "2": TomarExamen(db); break;
                case "3": VerReportes(db); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida"); break;
            }
        }
    }

    // agrega preguntas a la base de datos
    static void AgregarPregunta(DatosContexto db) {
        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine() ?? "";
        Console.Write("Respuesta A: ");
        string a = Console.ReadLine() ?? "";
        Console.Write("Respuesta B: ");
        string b = Console.ReadLine() ?? "";
        Console.Write("Respuesta C: ");
        string c = Console.ReadLine() ?? "";
        Console.Write("Respuesta correcta (A/B/C): ");
        string correcta = (Console.ReadLine() ?? "").ToUpper();

        db.Preguntas.Add(new Pregunta {
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        });
        db.SaveChanges();
        Console.WriteLine("Pregunta agregada correctamente.");
    }

    
    static void TomarExamen(DatosContexto db) {
        if (!db.Preguntas.Any()) {
            Console.WriteLine("No hay preguntas cargadas.");
            return;
        }

        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine() ?? "";

       
        var preguntas = db.Preguntas.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

       
        foreach (var p in preguntas) {
            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("Respuesta: ");
            string r = (Console.ReadLine() ?? "").ToUpper();

            bool esCorrecta = r == p.Correcta; // Verifica si la respuesta es correcta
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaDada = r,
                EsCorrecta = esCorrecta
            });
        }

        // Crea y guarda el resultado del examen
        var resultado = new ResultadoExamen {
            NombreAlumno = nombre,
            CantidadCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = correctas,
            Respuestas = respuestas
        };

        db.ResultadosExamen.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Nota: {correctas}/5");
    }

    
    static void VerReportes(DatosContexto db) {
        Console.WriteLine("1. Ver todos los exámenes");
        Console.WriteLine("2. Filtrar por alumno");
        Console.WriteLine("3. Ranking por nota");
        Console.WriteLine("4. Estadísticas por pregunta");
        Console.Write("Opción: ");

        string op = Console.ReadLine() ?? "";
        Console.Clear();
        switch (op) {
            case "1":
               
                foreach (var r in db.ResultadosExamen.Include(x => x.Respuestas)) {
                    Console.WriteLine($"Alumno: {r.NombreAlumno}, Nota: {r.NotaFinal}, Correctas: {r.CantidadCorrectas}/{r.TotalPreguntas}");
                }
                break;
            case "2":
               
                Console.Write("Nombre del alumno: ");
                string nombre = Console.ReadLine() ?? "";
                var filtrados = db.ResultadosExamen.Where(r => r.NombreAlumno == nombre);
                foreach (var r in filtrados) {
                    Console.WriteLine($"Nota: {r.NotaFinal}, Correctas: {r.CantidadCorrectas}/{r.TotalPreguntas}");
                }
                break;
            case "3":
               
                var ranking = db.ResultadosExamen
                    .GroupBy(r => r.NombreAlumno)
                    .Select(g => new {
                        Nombre = g.Key,
                        MejorNota = g.Max(x => x.NotaFinal)
                    })
                    .OrderByDescending(x => x.MejorNota);
                foreach (var r in ranking) {
                    Console.WriteLine($"{r.Nombre}: {r.MejorNota}");
                }
                break;
            case "4":
                
                var estadisticas = db.Preguntas.Select(p => new {
                    p.Enunciado,
                    Total = db.RespuestasExamen.Count(r => r.PreguntaId == p.PreguntaId),
                    Correctas = db.RespuestasExamen.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta)
                });
                foreach (var e in estadisticas) {
                    double porcentaje = e.Total > 0 ? 100.0 * e.Correctas / e.Total : 0;
                    Console.WriteLine($"\nPregunta: {e.Enunciado}\nRespondida: {e.Total} veces\nCorrectas: {porcentaje:F2}%");
                }
                break;
            default:
                Console.WriteLine("Opción inválida");
                break;
        }
    }
}

