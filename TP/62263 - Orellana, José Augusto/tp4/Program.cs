using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext
    {
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
        public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    }

class Program
{
    static void Main(string[] args)
    {
        using (var context = new DatosContexto())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n======== MENÚ ========");
                Console.WriteLine("1. Registrar pregunta");
                Console.WriteLine("2. Tomar examen");
                Console.WriteLine("3. Mostrar reportes");
                Console.WriteLine("0. Salir");

                Console.WriteLine("--------------------------------------");
                Console.Write("Seleccione una opción: ");
                var opcion = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(opcion))
                {
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("El valor ingresado no puede estar vacío.");
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Presione una tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                switch (opcion)
                {
                    case "1":
                        // Registrar pregunta
                        RegistrarPregunta(context);
                        break;
                    case "2":
                        // Tomar examen
                        TomarExamen(context);
                        break;
                    case "3":
                        // Mostrar reportes
                        MostrarReportes(context);
                        break;
                    case "0":
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Saliendo...");
                        Console.WriteLine("======================================");
                        return;
                    default:
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Opción no válida. Intentá nuevamente.");
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    static void RegistrarPregunta(DatosContexto context)
    {
        Console.Clear();
        Console.WriteLine("\n====== Registrar Pregunta ======");
        Console.WriteLine("--------------------------------------");

        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine();
        Console.WriteLine("--------------------------------------");

        while (string.IsNullOrWhiteSpace(enunciado))
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("El enunciado no puede estar vacío.");
            Console.WriteLine("--------------------------------------");
            Console.Write("Enunciado: ");
            enunciado = Console.ReadLine();
        }

        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.Write("Respuesta A: ");
        string respuestaA = Console.ReadLine();
        Console.WriteLine("--------------------------------------");

        while (string.IsNullOrWhiteSpace(respuestaA))
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("La respuesta A no puede estar vacía.");
            Console.WriteLine("--------------------------------------");
            Console.Write("Respuesta A: ");
            respuestaA = Console.ReadLine();
        }

        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.Write("Respuesta B: ");
        string respuestaB = Console.ReadLine();
        Console.WriteLine("--------------------------------------");

        while (string.IsNullOrWhiteSpace(respuestaB))
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("La respuesta B no puede estar vacía.");
            Console.WriteLine("--------------------------------------");
            Console.Write("Respuesta B: ");
            respuestaB = Console.ReadLine();
        }

        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.Write("Respuesta C: ");
        string respuestaC = Console.ReadLine();
        Console.WriteLine("--------------------------------------");

        while (string.IsNullOrWhiteSpace(respuestaC))
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("La respuesta C no puede estar vacía.");
            Console.WriteLine("--------------------------------------");
            Console.Write("Respuesta C: ");
            respuestaC = Console.ReadLine();
        }

        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.Write("Respuesta correcta (A, B o C): ");
        string respuestaCorrecta = Console.ReadLine().ToUpper();

        while (respuestaCorrecta != "A" && respuestaCorrecta != "B" && respuestaCorrecta != "C")
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("La respuesta correcta debe ser A, B o C.");
            Console.WriteLine("--------------------------------------");
            Console.Write("Respuesta correcta (A, B o C): ");
            respuestaCorrecta = Console.ReadLine().ToUpper();
        }
        
        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = respuestaCorrecta
        };

        context.Preguntas.Add(pregunta);
        context.SaveChanges();

        Console.WriteLine("======================================");
        Console.WriteLine("Pregunta registrada con éxito.");
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void TomarExamen(DatosContexto context)
    {
        Console.Clear();
        Console.WriteLine("\n====== Tomar Examen ======");
        Console.WriteLine("--------------------------------------");
        Console.Write("Nombre del alumno: ");
        string nombreAlumno = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(nombreAlumno))
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("El nombre no puede estar vacío.");
            Console.WriteLine("--------------------------------------");
            Console.Write("Nombre del alumno: ");
            nombreAlumno = Console.ReadLine();
        }

        var preguntas = context.Preguntas.ToList()
            .OrderBy(p => Guid.NewGuid())
            .Take(5)
            .ToList();

        if (preguntas.Count == 0)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("No hay preguntas registradas.");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        int correctas = 0;

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombreAlumno,
            TotalPreguntas = preguntas.Count,
            Respuestas = new List<RespuestaExamen>()
        };

        foreach (var pregunta in preguntas)
        {
            Console.Clear();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"{pregunta.Enunciado}");
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");

            string respuestaAlumno;
            do
            {
                Console.Write("Respuesta (A, B o C): ");
                respuestaAlumno = Console.ReadLine().ToUpper();
            } while (respuestaAlumno != "A" && respuestaAlumno != "B" && respuestaAlumno != "C");

            bool esCorrecta = respuestaAlumno == pregunta.Correcta;
            if (esCorrecta) correctas++;

            resultado.Respuestas.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.PreguntaId,
                RespuestaAlumno = respuestaAlumno,
                EsCorrecta = esCorrecta
            });
        }

        resultado.CantidadCorrectas = correctas;
        resultado.NotaFinal = correctas * 10.0 / preguntas.Count;

        context.ResultadosExamen.Add(resultado);
        context.SaveChanges();

        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine($"Examen terminado. {nombreAlumno}");
        Console.WriteLine($"Respuestas correctas: {correctas} de {preguntas.Count}");
        Console.WriteLine($"Nota final: {resultado.NotaFinal:F2}");

        Console.WriteLine("--------------------------------------");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarReportes(DatosContexto context)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n=== Reportes ===");
            Console.WriteLine("1. Listar exámenes");
            Console.WriteLine("2. Filtrar por alumno");
            Console.WriteLine("3. Ranking mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("0. Volver");

            Console.WriteLine("--------------------------------------");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(opcion))
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("El valor ingresado no puede estar vacío.");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("Presione una tecla para continuar...");
                Console.ReadKey();
                continue;
            }

            switch (opcion)
            {
                case "1":
                    ListarExamenes(context);
                    break;
                case "2":
                    FiltrarPorAlumno(context);
                    break;
                case "3":
                    RankingAlumnos(context);
                    break;
                case "4":
                    EstadisticasPorPregunta(context);
                    break;
                case "0":
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Volviendo al menú principal...");
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Presione una tecla para continuar...");
                    Console.ReadKey();
                    return;
                default:
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Opción no válida. Intentá nuevamente.");
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void ListarExamenes(DatosContexto context)
    {
        Console.Clear();
        Console.WriteLine("\n====== Listar Exámenes ======");
        Console.WriteLine("--------------------------------------");
        var examenes = context.ResultadosExamen.ToList();

        if (!examenes.Any())
        {
            Console.WriteLine("No hay exámenes registrados.");
            Console.WriteLine("======================================");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        foreach (var examen in examenes)
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"{examen.NombreAlumno}: {examen.CantidadCorrectas}/{examen.TotalPreguntas} -> Nota: {examen.NotaFinal:F2}");
        }

        Console.WriteLine("======================================");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void FiltrarPorAlumno(DatosContexto context)
    {
        Console.Clear();
        Console.WriteLine("\n====== Filtrar por Alumno ======");
        Console.WriteLine("--------------------------------------");
        Console.Write("Ingrese nombre del alumno para filtrar: ");
        var nombre = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("El nombre no puede estar vacío.");
            Console.WriteLine("--------------------------------------");
            Console.Write("Ingrese nombre del alumno para filtrar: ");
            nombre = Console.ReadLine();
        }

        Console.WriteLine("--------------------------------------");

        var examenes = context.ResultadosExamen
            .ToList()
            .Where(e => e.NombreAlumno.Contains(nombre, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!examenes.Any())
        {
            Console.WriteLine("======================================");
            Console.WriteLine($"No se encontraron exámenes para el alumno: {nombre}");
            Console.WriteLine("======================================");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        foreach (var examen in examenes)
        {
            Console.WriteLine($"{examen.NombreAlumno}: {examen.CantidadCorrectas}/{examen.TotalPreguntas} -> Nota: {examen.NotaFinal:F2}");
        }

        Console.WriteLine("======================================");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void RankingAlumnos(DatosContexto context)
    {
        Console.Clear();
        Console.WriteLine("\n====== Ranking Mejores Alumnos ======");
        Console.WriteLine("--------------------------------------");

        var ranking = context.ResultadosExamen
            .GroupBy(e => e.NombreAlumno)
            .Select(g => new { Nombre = g.Key, MejorNota = g.Max(e => e.NotaFinal) })
            .OrderByDescending(r => r.MejorNota)
            .ToList();

        if (!ranking.Any())
        {
            Console.WriteLine("No hay datos para el ranking.");
            Console.WriteLine("======================================");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        foreach (var r in ranking)
        {
            Console.WriteLine($"{r.Nombre}: {r.MejorNota:F2}");
        }

        Console.WriteLine("======================================");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void EstadisticasPorPregunta(DatosContexto context)
    {
        Console.Clear();
        Console.WriteLine("\n====== Estadísticas por Pregunta ======");
        Console.WriteLine("--------------------------------------");

        var stats = context.RespuestasExamen
            .Include(r => r.Pregunta)
            .GroupBy(r => r.PreguntaId)
            .Select(g => new
            {
                Pregunta = g.First().Pregunta.Enunciado,
                Total = g.Count(),
                Correctas = g.Count(r => r.EsCorrecta),
                Porcentaje = g.Count(r => r.EsCorrecta) * 100.0 / g.Count()
            })
            .ToList();

        if (!stats.Any())
        {
            Console.WriteLine("No hay estadísticas disponibles.");
            Console.WriteLine("======================================");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        foreach (var s in stats)
        {
            Console.WriteLine($"{s.Pregunta} -> Respondida: {s.Total} veces, Correctas: {s.Correctas}, {s.Porcentaje:F2}% correcto");
        }

        Console.WriteLine("======================================");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}