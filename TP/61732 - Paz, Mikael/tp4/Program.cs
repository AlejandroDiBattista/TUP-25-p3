using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

// Modelos
public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; }
    public string OpcionA { get; set; }
    public string OpcionB { get; set; }
    public string OpcionC { get; set; }
    public char RespuestaCorrecta { get; set; }
}

public class Examen
{
    public int Id { get; set; }
    public string NombreAlumno { get; set; }
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class RespuestaExamen
{
    public int Id { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public bool RespondidaCorrectamente { get; set; }
}

// Contexto de base de datos
public class AppDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examenes.db");
    }
}

// Lógica del sistema
public class SistemaExamen
{
    public static void RegistrarPregunta()
    {
        using var db = new AppDbContext();
        Console.WriteLine("Ingrese el enunciado de la pregunta:");
        string enunciado = Console.ReadLine();

        Console.WriteLine("Ingrese la opción A:");
        string opcionA = Console.ReadLine();

        Console.WriteLine("Ingrese la opción B:");
        string opcionB = Console.ReadLine();

        Console.WriteLine("Ingrese la opción C:");
        string opcionC = Console.ReadLine();

        Console.WriteLine("Ingrese la respuesta correcta (A, B, C):");
        char correcta = Console.ReadLine().ToUpper()[0];

        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            OpcionA = opcionA,
            OpcionB = opcionB,
            OpcionC = opcionC,
            RespuestaCorrecta = correcta
        };

        db.Preguntas.Add(pregunta);
        db.SaveChanges();
        Console.WriteLine("✅ Pregunta registrada exitosamente!");
    }

    public static void TomarExamen()
    {
        using var db = new AppDbContext();
        Console.WriteLine("Ingrese su nombre:");
        string nombreAlumno = Console.ReadLine();

        var preguntas = db.Preguntas.OrderBy(r => Guid.NewGuid()).Take(5).ToList();

        if (preguntas.Count == 0)
        {
            Console.WriteLine("❌ No hay suficientes preguntas en la base de datos.");
            return;
        }

        var examen = new Examen { NombreAlumno = nombreAlumno, TotalPreguntas = preguntas.Count };
        var respuestasExamen = new List<RespuestaExamen>();

        foreach (var pregunta in preguntas)
        {
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"A) {pregunta.OpcionA}");
            Console.WriteLine($"B) {pregunta.OpcionB}");
            Console.WriteLine($"C) {pregunta.OpcionC}");
            Console.WriteLine("Ingrese su respuesta:");

            char respuesta = Console.ReadLine().ToUpper()[0];
            bool esCorrecta = respuesta == pregunta.RespuestaCorrecta;

            respuestasExamen.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.Id,
                RespondidaCorrectamente = esCorrecta
            });

            if (esCorrecta) examen.CantidadCorrectas++;
        }

        examen.NotaFinal = (double)examen.CantidadCorrectas / preguntas.Count * 10;
        examen.Respuestas = respuestasExamen;

        db.Examenes.Add(examen);
        db.SaveChanges();
        Console.WriteLine($"✅ Examen finalizado. Nota obtenida: {examen.NotaFinal}/10");
    }

    public static void MostrarRanking()
    {
        using var db = new AppDbContext();

        var ranking = db.Examenes
            .OrderByDescending(e => e.NotaFinal)
            .Take(5)
            .Select(e => new { e.NombreAlumno, e.NotaFinal })
            .ToList();

        Console.WriteLine("\n📊 Ranking de los mejores alumnos:");
        foreach (var alumno in ranking)
        {
            Console.WriteLine($"{alumno.NombreAlumno}: {alumno.NotaFinal}/10");
        }
    }
}

// Programa principal
class Program
{
    static void Main()
    {
        // Asegura la creación de la base de datos al iniciar
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
        }

        while (true)
        {
            Console.WriteLine("\n🎓 Sistema de Exámenes - Menú");
            Console.WriteLine("1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Mostrar ranking");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine();
            switch (opcion)
            {
                case "1":
                    SistemaExamen.RegistrarPregunta();
                    break;
                case "2":
                    SistemaExamen.TomarExamen();
                    break;
                case "3":
                    SistemaExamen.MostrarRanking();
                    break;
                case "4":
                    Console.WriteLine("👋 Hasta pronto!");
                    return;
                default:
                    Console.WriteLine("❌ Opción no válida. Intente de nuevo.");
                    break;
            }
        }
    }
}
