using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = ""; // A, B o C

    public ICollection<RespuestaExamen> Respuestas { get; set; }
}

class ResultadoExamen
{
    public int Id { get; set; }
    public string NombreAlumno { get; set; }
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public ICollection<RespuestaExamen> Respuestas { get; set; }
}

class RespuestaExamen
{
    public int Id { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public string RespuestaAlumno { get; set; }
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program
{
    static void Main(string[] args)
    {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        while (true)
        {
            Console.WriteLine("\n--- MENÚ ---");
            Console.WriteLine("1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver reportes");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": RegistrarPregunta(db); break;
                case "2": TomarExamen(db); break;
                case "3": VerReportes(db); break;
                case "0": return;
                default: Console.WriteLine("OPCION INVÁLIDA."); break;
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db)
    {
        Console.WriteLine("\n--- Registrar Pregunta ---");
        var p = new Pregunta();
        Console.Write("Enunciado: "); p.Enunciado = Console.ReadLine();
        Console.Write("Respuesta A: "); p.RespuestaA = Console.ReadLine();
        Console.Write("Respuesta B: "); p.RespuestaB = Console.ReadLine();
        Console.Write("Respuesta C: "); p.RespuestaC = Console.ReadLine();
        Console.Write("Respuesta correcta (A/B/C): "); p.Correcta = Console.ReadLine().ToUpper();

        db.Preguntas.Add(p);
        db.SaveChanges();
        Console.WriteLine("PREGUNTA REGISTRADA CORRECTAMENTE.");
    }

    static void TomarExamen(DatosContexto db)
{
    Console.Write("\nNombre del alumno: ");
    string alumno = Console.ReadLine();

    var preguntas = db.Preguntas.ToList();

    if (preguntas.Count == 0)
    {
        Console.WriteLine("NO HAY PREGUNTAS REGISTRADAS.");
        return;
    }

    var random = new Random();
    preguntas = preguntas.OrderBy(x => random.Next()).Take(5).ToList();

    var resultado = new ResultadoExamen
    {
        NombreAlumno = alumno,
        TotalPreguntas = preguntas.Count,
        Respuestas = new List<RespuestaExamen>()
    };

    int correctas = 0;

    foreach (var p in preguntas)
    {
        Console.WriteLine($"\n{p.Enunciado}");
        Console.WriteLine($"A. {p.RespuestaA}");
        Console.WriteLine($"B. {p.RespuestaB}");
        Console.WriteLine($"C. {p.RespuestaC}");
        Console.Write("Respuesta: ");
        string respuesta = Console.ReadLine().ToUpper();
        bool esCorrecta = respuesta == p.Correcta;
        if (esCorrecta) correctas++;

        resultado.Respuestas.Add(new RespuestaExamen
        {
            PreguntaId = p.Id,
            RespuestaAlumno = respuesta,
            EsCorrecta = esCorrecta
        });
    }

    resultado.CantidadCorrectas = correctas;
    resultado.NotaFinal = Math.Round((10.0 * correctas) / preguntas.Count, 2);

    db.Resultados.Add(resultado);
    db.SaveChanges();

    Console.WriteLine($"\nExamen finalizado. Correctas: {correctas}/{preguntas.Count} - Nota: {resultado.NotaFinal}");
}

    static void VerReportes(DatosContexto db)
    {
        Console.WriteLine("\n--- REPORTES ---");
        Console.WriteLine("1. Ver todos los exámenes");
        Console.WriteLine("2. Filtrar por alumno");
        Console.WriteLine("3. Ranking de mejores alumnos");
        Console.WriteLine("4. Estadísticas por pregunta");
        Console.Write("Opción: ");
        string op = Console.ReadLine();

        switch (op)
        {
            case "1":
                foreach (var r in db.Resultados)
                    Console.WriteLine($"{r.Fecha:dd/MM/yyyy HH:mm} - {r.NombreAlumno} - Nota: {r.NotaFinal}");
                break;

            case "2":
                Console.Write("Nombre del alumno: ");
                var nombre = Console.ReadLine();
                var resultados = db.Resultados.Where(r => r.NombreAlumno == nombre).ToList();
                if (resultados.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: EL ALUMNO INGRESADO NO TIENE EXAMENES REGISTRADOS.");
                }
                foreach (var r in resultados)
                    Console.WriteLine($"{r.Fecha:dd/MM/yyyy HH:mm} - Nota: {r.NotaFinal}");
                break;

            case "3":
                var ranking = db.Resultados
                    .GroupBy(r => r.NombreAlumno)
                    .Select(g => new { Alumno = g.Key, MejorNota = g.Max(r => r.NotaFinal) })
                    .OrderByDescending(x => x.MejorNota);

                foreach (var entry in ranking)
                    Console.WriteLine($"{entry.Alumno} - Mejor Nota: {entry.MejorNota}");
                break;

            case "4":
                var estadisticas = db.Preguntas
                    .Select(p => new
                    {
                        p.Enunciado,
                        Total = p.Respuestas.Count(),
                        Correctas = p.Respuestas.Count(r => r.EsCorrecta)
                    }).ToList();

                foreach (var stat in estadisticas)
                {
                    double porcentaje = stat.Total == 0 ? 0 : (100.0 * stat.Correctas / stat.Total);
                    Console.WriteLine($"\n{stat.Enunciado}\nVeces respondida: {stat.Total}, % correctas: {porcentaje:F2}%");
                }
                break;

            default:
                Console.WriteLine("OPCION INVÁLIDA.");
                break;
        }
    }
}