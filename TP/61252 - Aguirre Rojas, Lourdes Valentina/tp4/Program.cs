using System;
using System.Collections.Generic;
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
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int Correctas { get; set; }
    public int Total { get; set; }
    public double Nota { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    public bool Correcto { get; set; }
}

class ExamenDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; } = null!;
    public DbSet<ResultadoExamen> Resultados { get; set; } = null!;
    public DbSet<RespuestaExamen> Respuestas { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=examen.db");
}

class Program
{
    static void Main()
    {
        using var db = new ExamenDbContext();
        db.Database.EnsureCreated();
        CargarPreguntasIniciales(db);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("SISTEMA DE EXAMENES MULTIPLE CHOICE");
            Console.WriteLine("1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver reportes");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione opción: ");

            switch (Console.ReadLine())
            {
                case "1": RegistrarPregunta(db); break;
                case "2": TomarExamen(db); break;
                case "3": VerReportes(db); break;
                case "4": return;
                default:
                    Console.WriteLine("Opción inválida");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void RegistrarPregunta(ExamenDbContext db)
    {
        var p = new Pregunta();
        Console.Write("Enunciado: ");
        p.Enunciado = Console.ReadLine() ?? "";
        Console.Write("Opción A: ");
        p.RespuestaA = Console.ReadLine() ?? "";
        Console.Write("Opción B: ");
        p.RespuestaB = Console.ReadLine() ?? "";
        Console.Write("Opción C: ");
        p.RespuestaC = Console.ReadLine() ?? "";
        Console.Write("Respuesta correcta (A/B/C): ");
        p.Correcta = Console.ReadLine()?.ToUpper() ?? "A";

        db.Preguntas.Add(p);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada correctamente.");
        Console.ReadKey();
    }

    static void TomarExamen(ExamenDbContext db)
    {
        Console.Write("Nombre del alumno: ");
        string nombre = Console.ReadLine() ?? "";
        var preguntas = db.Preguntas.ToList();

        if (!preguntas.Any())
        {
            Console.WriteLine("No hay preguntas registradas.");
            Console.ReadKey();
            return;
        }

        var seleccionadas = preguntas.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();
        int correctas = 0;
        var resultado = new ResultadoExamen { Alumno = nombre, Total = seleccionadas.Count };

        foreach (var p in seleccionadas)
        {
            Console.Clear();
            Console.WriteLine(p.Enunciado);
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("Tu respuesta (A/B/C): ");
            string r = Console.ReadLine()?.ToUpper() ?? "";

            bool esCorrecta = r == p.Correcta;
            if (esCorrecta) correctas++;

            resultado.Respuestas.Add(new RespuestaExamen
            {
                PreguntaId = p.PreguntaId,
                Correcto = esCorrecta
            });
        }

        resultado.Correctas = correctas;
        resultado.Nota = Math.Round(correctas * 10.0 / resultado.Total, 2);
        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nResultado: {correctas}/{resultado.Total} - Nota: {resultado.Nota}/10");
        Console.ReadKey();
    }

    static void VerReportes(ExamenDbContext db)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("REPORTES");
            Console.WriteLine("1. Todos los exámenes");
            Console.WriteLine("2. Filtrar por alumno");
            Console.WriteLine("3. Ranking mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("5. Volver");
            Console.Write("Seleccione: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    foreach (var e in db.Resultados.OrderByDescending(e => e.Fecha))
                        Console.WriteLine($"{e.Alumno} - {e.Fecha:g} - {e.Nota}/10");
                    Console.ReadKey();
                    break;
                case "2":
                    Console.Write("Nombre: ");
                    string nombre = Console.ReadLine() ?? "";
                    var resultados = db.Resultados
                        .Where(r => r.Alumno.Contains(nombre))
                        .OrderByDescending(r => r.Fecha)
                        .ToList();
                    Console.Clear();
                    foreach (var r in resultados)
                        Console.WriteLine($"{r.Fecha:g} - Nota: {r.Nota}/10");
                    Console.ReadKey();
                    break;
                case "3":
                    Console.Clear();
                    var ranking = db.Resultados
                        .GroupBy(r => r.Alumno)
                        .Select(g => new { Alumno = g.Key, MejorNota = g.Max(r => r.Nota) })
                        .OrderByDescending(x => x.MejorNota)
                        .ToList();
                    foreach (var r in ranking)
                        Console.WriteLine($"{r.Alumno} - {r.MejorNota}/10");
                    Console.ReadKey();
                    break;
                case "4":
                    Console.Clear();
                    var estadisticas = db.Preguntas
                        .Include(p => p.Respuestas)
                        .Select(p => new
                        {
                            p.Enunciado,
                            Total = p.Respuestas.Count,
                            Correctas = p.Respuestas.Count(r => r.Correcto)
                        }).ToList();
                    foreach (var e in estadisticas)
                    {
                        double porcentaje = e.Total > 0 ? Math.Round(e.Correctas * 100.0 / e.Total, 2) : 0;
                        Console.WriteLine($"{e.Enunciado} - Respondida {e.Total} veces - {porcentaje}% correctas");
                    }
                    Console.ReadKey();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void CargarPreguntasIniciales(ExamenDbContext db)
    {
        if (db.Preguntas.Any()) return;

        var preguntas = new List<Pregunta>
        {
            new Pregunta
            {
                Enunciado = "¿Qué lenguaje se utiliza principalmente para desarrollar aplicaciones en Android?",
                RespuestaA = "Java",
                RespuestaB = "Python",
                RespuestaC = "Swift",
                Correcta = "A"
            },
            new Pregunta
            {
                Enunciado = "¿Qué significa HTML?",
                RespuestaA = "HyperText Markup Language",
                RespuestaB = "HighText Machine Language",
                RespuestaC = "Hyper Transfer Markup Language",
                Correcta = "A"
            },
            new Pregunta
            {
                Enunciado = "¿Cuál es el lenguaje más usado para crear estilos en páginas web?",
                RespuestaA = "HTML",
                RespuestaB = "CSS",
                RespuestaC = "JavaScript",
                Correcta = "B"
            },
            new Pregunta
            {
                Enunciado = "¿Para qué se utiliza la estructura 'if' en C#?",
                RespuestaA = "Para declarar variables",
                RespuestaB = "Para realizar decisiones condicionales",
                RespuestaC = "Para repetir instrucciones",
                Correcta = "B"
            },
            new Pregunta
            {
                Enunciado = "¿Qué tipo de base de datos es MySQL?",
                RespuestaA = "Relacional",
                RespuestaB = "NoSQL",
                RespuestaC = "Jerárquica",
                Correcta = "A"
            }
        };

        db.Preguntas.AddRange(preguntas);
        db.SaveChanges();
    }
}