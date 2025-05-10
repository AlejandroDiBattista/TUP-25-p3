using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;


class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";

    public List<RespuestaExamen> RespuestasExamen { get; set; } = new();
}
class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public float NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen? ResultadoExamen { get; set; } 

    public int PreguntaId { get; set; }
    public Pregunta? Pregunta { get; set; }

    public string RespuestaSeleccionada { get; set; } = "";
    public bool EsCorrecta { get; set; }
}
class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program
{
    static void Main(string[] args)
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Sistema de Exámenes Multiple Choice");
                Console.WriteLine("1. Rendir Examen");
                Console.WriteLine("2. Reportes");
                Console.WriteLine("3. Agregar Pregunta");
                Console.WriteLine("4. Salir");
                Console.Write("\nSeleccione una opción: ");

                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        TomarExamen(db);
                        break;
                    case "2":
                        MostrarReportes(db);
                        break;
                    case "3":
                        AgregarPregunta(db);
                        break;
                    case "4":
                        Console.WriteLine("Gracias por usar el sistema. ¡Hasta luego!");
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }

                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    static void MostrarReportes(DatosContexto db)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Reportes");
            Console.WriteLine("1. Mostrar listado de todos los exámenes rendidos");
            Console.WriteLine("2. Filtrar resultados por nombre de alumno");
            Console.WriteLine("3. Mostrar ranking de los mejores alumnos");
            Console.WriteLine("4. Informe estadístico por pregunta");
            Console.WriteLine("5. Volver al menú principal");
            Console.Write("\nSeleccione una opción: ");

            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    VerReportes(db);
                    break;
                case "2":
                    FiltrarResultadosPorAlumno(db);
                    break;
                case "3":
                    MostrarRanking(db);
                    break;
                case "4":
                    InformeEstadisticoPorPregunta(db);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    static void VerReportes(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("Reportes de Exámenes:");

        var resultados = db.Resultados
            .OrderBy(r => r.NotaFinal)
            .ToList();

        if (resultados.Any())
        {
            Console.WriteLine("\nLista de exámenes:");
            foreach (var resultado in resultados)
            {
                Console.WriteLine($"Alumno: {resultado.Alumno}, Nota: {resultado.NotaFinal}/5");
            }
        }
        else
        {
            Console.WriteLine("No se han realizado exámenes aún.");
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void AgregarPregunta(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("Agregar Nueva Pregunta");

        Console.Write("Enunciado de la pregunta: ");
        string enunciado = Console.ReadLine() ?? "";

        Console.Write("Respuesta A: ");
        string respuestaA = Console.ReadLine() ?? ""; 

        Console.Write("Respuesta B: ");
        string respuestaB = Console.ReadLine() ?? "";

        Console.Write("Respuesta C: ");
        string respuestaC = Console.ReadLine() ?? ""; 

        Console.Write("Respuesta correcta (A/B/C): ");
        string correcta = (Console.ReadLine() ?? "").ToUpper(); 

        if (string.IsNullOrWhiteSpace(enunciado) || string.IsNullOrWhiteSpace(respuestaA) ||
            string.IsNullOrWhiteSpace(respuestaB) || string.IsNullOrWhiteSpace(respuestaC) ||
            (correcta != "A" && correcta != "B" && correcta != "C"))
        {
            Console.WriteLine("\nError: Todos los campos son obligatorios y la respuesta correcta debe ser A, B o C.");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        var nuevaPregunta = new Pregunta
        {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta
        };

        db.Preguntas.Add(nuevaPregunta);
        db.SaveChanges();

        Console.WriteLine("\nPregunta agregada exitosamente.");
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("Tomando examen...");

        
        Console.Write("Ingrese su nombre: ");
        string nombreAlumno = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(nombreAlumno)) {
            Console.WriteLine("El nombre no puede estar vacío. Intente nuevamente.");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        
        var preguntas = db.Preguntas.ToList(); 

        
        var preguntasAleatorias = preguntas
            .OrderBy(p => Guid.NewGuid()) 
            .Take(5)
            .ToList();

        if (!preguntasAleatorias.Any()) {
            Console.WriteLine("No hay preguntas disponibles para el examen.");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        double respuestasCorrectas = 0;
        var resultado = new ResultadoExamen {
            Alumno = nombreAlumno,
            CantidadCorrectas = 0,
            TotalPreguntas = preguntasAleatorias.Count,
            NotaFinal = 0,
            Respuestas = new List<RespuestaExamen>()
        };

        foreach (var pregunta in preguntasAleatorias) {
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"1) {pregunta.RespuestaA}");
            Console.WriteLine($"2) {pregunta.RespuestaB}");
            Console.WriteLine($"3) {pregunta.RespuestaC}");

            string respuesta = "";
            while (true) {
                Console.Write("Respuesta (A/B/C o 1/2/3): ");
                string entrada = (Console.ReadLine() ?? "").ToUpper();

                if (entrada == "A" || entrada == "1") {
                    respuesta = "A";
                    break;
                } else if (entrada == "B" || entrada == "2") {
                    respuesta = "B";
                    break;
                } else if (entrada == "C" || entrada == "3") {
                    respuesta = "C";
                    break;
                } else {
                    Console.WriteLine("Entrada no válida. Por favor, ingrese A, B, C, 1, 2 o 3.");
                }
            }

            bool esCorrecta = respuesta == pregunta.Correcta;
            if (esCorrecta) {
                respuestasCorrectas++;
            }

            // Registrar la respuesta individual
            var respuestaExamen = new RespuestaExamen {
                PreguntaId = pregunta.PreguntaId,
                RespuestaSeleccionada = respuesta,
                EsCorrecta = esCorrecta
            };
            resultado.Respuestas.Add(respuestaExamen); 
        }

        Console.WriteLine($"\nHas respondido correctamente {respuestasCorrectas} de {preguntasAleatorias.Count}.");
        double notaFinal = (respuestasCorrectas / preguntasAleatorias.Count) * 5;
        Console.WriteLine($"Tu nota final es: {notaFinal:F2}");

        // Registrar el resultado
        resultado.CantidadCorrectas = (int)respuestasCorrectas;
        resultado.NotaFinal = (float)notaFinal;
        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine("\nExamen finalizado. Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void FiltrarResultadosPorAlumno(DatosContexto db) {
        Console.Write("Ingrese el nombre del alumno: ");
        string nombreAlumno = Console.ReadLine() ?? "";

        var resultados = db.Resultados
            .Where(r => r.Alumno.Contains(nombreAlumno))
            .OrderByDescending(r => r.NotaFinal)
            .ToList();

        if (resultados.Any()) {
            Console.WriteLine("\nResultados del alumno:");
            foreach (var resultado in resultados) {
                Console.WriteLine($"Alumno: {resultado.Alumno}, Nota: {resultado.NotaFinal}/5");
            }
        } else {
            Console.WriteLine("No se encontraron resultados para el alumno especificado.");
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarRanking(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("Ranking de los mejores alumnos:");

        var ranking = db.Resultados
            .GroupBy(r => r.Alumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(r => r.NotaFinal)
            })
            .OrderByDescending(r => r.MejorNota)
            .Take(10)
            .ToList();

        foreach (var alumno in ranking) {
            Console.WriteLine($"Alumno: {alumno.Alumno}, Mejor Nota: {alumno.MejorNota}/5");
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void InformeEstadisticoPorPregunta(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("Informe estadístico por pregunta:");

        var estadisticas = db.Preguntas
            .Select(p => new {
                Pregunta = p.Enunciado,
                TotalRespuestas = p.RespuestasExamen.Count(),
                RespuestasCorrectas = p.RespuestasExamen.Count(r => r.EsCorrecta),
                PorcentajeCorrectas = p.RespuestasExamen.Count() > 0
                    ? (p.RespuestasExamen.Count(r => r.EsCorrecta) * 100.0 / p.RespuestasExamen.Count())
                    : 0
            })
            .ToList();

        foreach (var estadistica in estadisticas) {
            Console.WriteLine($"Pregunta: {estadistica.Pregunta}");
            Console.WriteLine($"Total de Respuestas: {estadistica.TotalRespuestas}");
            Console.WriteLine($"Respuestas Correctas: {estadistica.RespuestasCorrectas}");
            Console.WriteLine($"Porcentaje Correctas: {estadistica.PorcentajeCorrectas:F2}%\n");
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}

