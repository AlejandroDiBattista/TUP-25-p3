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
    public string Alumno { get; set; }
    public int CantidadCorrecta { get; set; }
    public int TotalPreguntas { get; set; }
    public double Nota { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; }
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public string RespuestaAlumno { get; set; }
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
    static void Main(string[] args) {
        Console.WriteLine("Bienvenido al Sistema de Exámenes!");

        using (var db = new DatosContexto()) {
            db.Database.EnsureCreated();

            while (true) {
                MostrarMenu();

                string opcion = Console.ReadLine()?.ToUpper();

                switch (opcion) {
                    case "1":
                        RegistrarPreguntas(db);
                        break;

                    case "2":
                        TomarExamen(db);
                        break;

                    case "3":
                        MostrarReportes(db);
                        break;

                    case "4":
                        Console.WriteLine("Saliendo...");
                        return;

                    default:
                        Console.WriteLine("Opción no válida. Por favor, intente nuevamente.");
                        break;
                }
            }
        }
    }

    static void MostrarMenu() {
        Console.WriteLine("\n--- MENÚ ---");
        Console.WriteLine("1. Registrar preguntas");
        Console.WriteLine("2. Tomar examen");
        Console.WriteLine("3. Mostrar reportes");
        Console.WriteLine("4. Salir");
        Console.Write("Elija una opción: ");
    }

    static void TomarExamen(DatosContexto db) {
        Console.WriteLine("Ingresar nombre del alumno:");
        string alumno = Console.ReadLine();

        var preguntas = db.Preguntas
    .AsEnumerable() 
    .OrderBy(p => Guid.NewGuid()) 
    .Take(Math.Min(5, db.Preguntas.Count()))
    .ToList();

        int cantidadCorrecta = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var pregunta in preguntas) {
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");

            string respuestaAlumno;
            do {
                Console.Write("Tu respuesta (A/B/C): ");
                respuestaAlumno = Console.ReadLine()?.ToUpper();
            } while (respuestaAlumno != "A" && respuestaAlumno != "B" && respuestaAlumno != "C");

            bool esCorrecta = respuestaAlumno == pregunta.Correcta;

            if (esCorrecta) cantidadCorrecta++;

            respuestas.Add(new RespuestaExamen {
                PreguntaId = pregunta.PreguntaId,
                RespuestaAlumno = respuestaAlumno,
                EsCorrecta = esCorrecta
            });

            
            Console.WriteLine(esCorrecta ? "Respuesta correcta!" : "Respuesta incorrecta.");
        }

        var resultado = new ResultadoExamen {
            Alumno = alumno,
            CantidadCorrecta = cantidadCorrecta,
            TotalPreguntas = preguntas.Count,
            Nota = (double)cantidadCorrecta / preguntas.Count * 10,
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Alumno: {alumno}");
        Console.WriteLine($"Correctas: {cantidadCorrecta} / {preguntas.Count}");
        Console.WriteLine($"Nota final: {resultado.Nota:F2}");
    }

    static void RegistrarPreguntas(DatosContexto db) {
        Console.WriteLine("Ingresar enunciado de la pregunta:");
        string enunciado = Console.ReadLine() ?? "";

        Console.WriteLine("Ingresar respuesta A:");
        string respuestaA = Console.ReadLine() ?? "";

        Console.WriteLine("Ingresar respuesta B:");
        string respuestaB = Console.ReadLine() ?? "";

        Console.WriteLine("Ingresar respuesta C:");
        string respuestaC = Console.ReadLine() ?? "";

        Console.WriteLine("Ingresar respuesta correcta (A, B o C):");
        string correcta = Console.ReadLine()?.ToUpper();

        db.Preguntas.Add(new Pregunta {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta
        });
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada correctamente.");
    }

    static void MostrarReportes(DatosContexto db) {
        
        Console.WriteLine("\n--- Todos los exámenes rendidos ---");
        foreach (var r in db.Resultados) {
            Console.WriteLine($"Alumno: {r.Alumno}, Nota: {r.Nota:F2}, Correctas: {r.CantidadCorrecta}/{r.TotalPreguntas}");
        }

       
        Console.WriteLine("\n--- Filtrar resultados por nombre ---");
        Console.Write("Ingresar nombre del alumno para filtrar: ");
        var nombreBuscar = Console.ReadLine();
        var resultadosFiltrados = db.Resultados.Where(r => r.Alumno == nombreBuscar).ToList();
        if (resultadosFiltrados.Any()) {
            foreach (var r in resultadosFiltrados) {
                Console.WriteLine($"Alumno: {r.Alumno}, Nota: {r.Nota:F2}");
            }
        } else {
            Console.WriteLine("No se encontraron resultados para ese alumno.");
        }

        Console.WriteLine("\n--- Ranking de mejores alumnos (mejor nota) ---");
        var ranking = db.Resultados
            .GroupBy(r => r.Alumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(r => r.Nota)
            })
            .OrderByDescending(r => r.MejorNota)
            .ToList();

        foreach (var item in ranking) {
            Console.WriteLine($"Alumno: {item.Alumno}, Mejor Nota: {item.MejorNota:F2}");
        }

        
        Console.WriteLine("\n--- Estadísticas por pregunta ---");
        var estadisticas = db.Respuestas
            .GroupBy(r => r.PreguntaId)
            .Select(g => new {
                Pregunta = db.Preguntas.FirstOrDefault(p => p.PreguntaId == g.Key),
                Total = g.Count(),
                Correctas = g.Count(r => r.EsCorrecta)
            })
            .ToList();

        foreach (var e in estadisticas) {
            double porcentaje = e.Total > 0 ? (double)e.Correctas / e.Total * 100 : 0;
            Console.WriteLine($"\nPregunta: {e.Pregunta.Enunciado}");
            Console.WriteLine($"Respondida: {e.Total} veces");
            Console.WriteLine($"Correctas: {e.Correctas} ({porcentaje:F2}%)");
        }
    }
}
