using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
            InicializarPreguntas(db);
            MenuPrincipal(db);
        }
    }

    public static void InicializarPreguntas(DatosContexto db)
    {
        if (!db.Preguntas.Any())
        {
            var preguntas = new[]
            {
                new Pregunta
                {
                    Texto = "¿Cuál es la capital de Argentina?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "Buenos Aires", EsCorrecta = true },
                        new Respuesta { Texto = "Córdoba", EsCorrecta = false },
                        new Respuesta { Texto = "Rosario", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Cuánto es 2 + 2?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "3", EsCorrecta = false },
                        new Respuesta { Texto = "4", EsCorrecta = true },
                        new Respuesta { Texto = "5", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Cuál es el océano más grande?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "Atlántico", EsCorrecta = false },
                        new Respuesta { Texto = "Pacífico", EsCorrecta = true },
                        new Respuesta { Texto = "Índico", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Quién escribió 'Cien años de soledad'?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "Gabriel García Márquez", EsCorrecta = true },
                        new Respuesta { Texto = "Julio Cortázar", EsCorrecta = false },
                        new Respuesta { Texto = "Mario Vargas Llosa", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Cuál es el planeta más grande del sistema solar?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "Saturno", EsCorrecta = false },
                        new Respuesta { Texto = "Júpiter", EsCorrecta = true },
                        new Respuesta { Texto = "Neptuno", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿En qué año llegó el hombre a la Luna?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "1969", EsCorrecta = true },
                        new Respuesta { Texto = "1972", EsCorrecta = false },
                        new Respuesta { Texto = "1965", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Cuál es la fórmula química del agua?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "H2O", EsCorrecta = true },
                        new Respuesta { Texto = "CO2", EsCorrecta = false },
                        new Respuesta { Texto = "O2", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Quién pintó La Mona Lisa?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "Leonardo da Vinci", EsCorrecta = true },
                        new Respuesta { Texto = "Pablo Picasso", EsCorrecta = false },
                        new Respuesta { Texto = "Vincent van Gogh", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Qué país ganó el Mundial de Fútbol 2014?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "Brasil", EsCorrecta = false },
                        new Respuesta { Texto = "Alemania", EsCorrecta = true },
                        new Respuesta { Texto = "Argentina", EsCorrecta = false }
                    }
                },
                new Pregunta
                {
                    Texto = "¿Cuál es el metal más liviano?",
                    Respuestas = new()
                    {
                        new Respuesta { Texto = "Litio", EsCorrecta = true },
                        new Respuesta { Texto = "Aluminio", EsCorrecta = false },
                        new Respuesta { Texto = "Sodio", EsCorrecta = false }
                    }
                }
            };

            db.Preguntas.AddRange(preguntas);
            db.SaveChanges();
            Console.WriteLine("¡Preguntas de ejemplo cargadas!");
        }
    }

    public static void MenuPrincipal(DatosContexto db)
    {
        while (true)
        {
            Console.WriteLine("\n=== Menú Principal ===");
            Console.WriteLine("1. Tomar examen");
            Console.WriteLine("2. Ver reportes");
            Console.WriteLine("3. Salir");
            Console.Write("Elegí una opción: ");
            var op = Console.ReadLine();

            switch (op)
            {
                case "1":
                    TomarExamen(db);
                    break;
                case "2":
                    MostrarReportes(db);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    public static void TomarExamen(DatosContexto db)
    {
        Console.Write("Ingrese su nombre: ");
        var nombre = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("Nombre inválido.");
            return;
        }

        var preguntas = db.Preguntas.Include(p => p.Respuestas).ToList();
        var random = new Random();
        preguntas = preguntas.OrderBy(x => random.Next()).Take(5).ToList();

        if (preguntas.Count == 0)
        {
            Console.WriteLine("No hay preguntas disponibles para el examen.");
            return;
        }

        int correctas = 0;
        var respuestasExamen = new List<RespuestaExamen>();

        foreach (var pregunta in preguntas)
        {
            Console.WriteLine($"\nPregunta: {pregunta.Texto}");
            var respuestas = pregunta.Respuestas.OrderBy(r => random.Next()).ToList();
            char letra = 'a';
            var letraRespuesta = new Dictionary<char, Respuesta>();
            foreach (var respuesta in respuestas)
            {
                Console.WriteLine($"{letra}) {respuesta.Texto}");
                letraRespuesta[letra] = respuesta;
                letra++;
            }

            char respuestaElegida;
            while (true)
            {
                Console.Write("Elegí la letra de tu respuesta: ");
                var input = Console.ReadLine().Trim().ToLower();
                if (input.Length == 1 && letraRespuesta.ContainsKey(input[0]))
                {
                    respuestaElegida = input[0];
                    break;
                }
                Console.WriteLine("Letra inválida. Elegí a, b, c...");
            }

            var respuestaSeleccionada = letraRespuesta[respuestaElegida];
            bool esCorrecta = respuestaSeleccionada.EsCorrecta;
            if (esCorrecta) correctas++;

            respuestasExamen.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.Id,
                RespuestaId = respuestaSeleccionada.Id,
                EsCorrecta = esCorrecta
            });

            var correctaLetra = letraRespuesta.First(x => x.Value.EsCorrecta).Key;
            if (respuestaElegida == correctaLetra)
                Console.WriteLine("¡Respuesta correcta!");
            else
                Console.WriteLine($"Respuesta incorrecta. La correcta era: {correctaLetra}) {letraRespuesta[correctaLetra].Texto}");
        }

        double nota = correctas;
        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            CantidadCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = nota,
            Respuestas = respuestasExamen
        };

        db.ResultadosExamen.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. {nombre}, tu puntaje es {correctas}/{preguntas.Count} (Nota: {nota}).");
    }

    public static void MostrarReportes(DatosContexto db)
    {
        while (true)
        {
            Console.WriteLine("\n=== Reportes ===");
            Console.WriteLine("1. Listado de exámenes rendidos");
            Console.WriteLine("2. Filtrar resultados por alumno");
            Console.WriteLine("3. Ranking de mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("5. Volver");
            Console.Write("Elegí una opción: ");
            var op = Console.ReadLine();

            switch (op)
            {
                case "1":
                    ListarExamenes(db);
                    break;
                case "2":
                    FiltrarPorAlumno(db);
                    break;
                case "3":
                    RankingAlumnos(db);
                    break;
                case "4":
                    EstadisticasPreguntas(db);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    public static void ListarExamenes(DatosContexto db)
    {
        var examenes = db.ResultadosExamen.ToList();
        Console.WriteLine("\n--- Exámenes Rendidos ---");
        foreach (var ex in examenes)
        {
            Console.WriteLine($"Alumno: {ex.NombreAlumno} | Correctas: {ex.CantidadCorrectas}/{ex.TotalPreguntas} | Nota: {ex.NotaFinal}");
        }
    }

    public static void FiltrarPorAlumno(DatosContexto db)
    {
        Console.Write("Nombre del alumno a buscar: ");
        var nombre = Console.ReadLine().Trim();
        var examenes = db.ResultadosExamen.Where(e => e.NombreAlumno.ToLower().Contains(nombre.ToLower())).ToList();
        Console.WriteLine($"\n--- Exámenes de {nombre} ---");
        foreach (var ex in examenes)
        {
            Console.WriteLine($"Correctas: {ex.CantidadCorrectas}/{ex.TotalPreguntas} | Nota: {ex.NotaFinal}");
        }
    }

    public static void RankingAlumnos(DatosContexto db)
    {
        var ranking = db.ResultadosExamen
            .GroupBy(e => e.NombreAlumno)
            .Select(g => new { Alumno = g.Key, MejorNota = g.Max(e => e.NotaFinal) })
            .OrderByDescending(x => x.MejorNota)
            .ToList();

        Console.WriteLine("\n--- Ranking de Mejores Alumnos ---");
        foreach (var r in ranking)
        {
            Console.WriteLine($"{r.Alumno} | Mejor Nota: {r.MejorNota}");
        }
    }

    public static void EstadisticasPreguntas(DatosContexto db)
    {
        var preguntas = db.Preguntas.ToList();
        Console.WriteLine("\n--- Estadísticas por pregunta ---");
        foreach (var p in preguntas)
        {
            var total = db.RespuestasExamen.Count(r => r.PreguntaId == p.Id);
            var correctas = db.RespuestasExamen.Count(r => r.PreguntaId == p.Id && r.EsCorrecta);
            double porc = total > 0 ? (100.0 * correctas / total) : 0;
            Console.WriteLine($"\"{p.Texto}\" | Respondida: {total} veces | % correctas: {porc:0.00}%");
        }
    }
}