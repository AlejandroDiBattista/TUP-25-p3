using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string OpcionA { get; set; } = "";
    public string OpcionB { get; set; } = "";
    public string OpcionC { get; set; } = "";
    public char RespuestaCorrecta { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class ResultadoExamen
{
    public int Id { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int TotalCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class RespuestaExamen
{
    public int Id { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    public char RespuestaSeleccionada { get; set; }
    public bool EsCorrecta { get; set; }
}

public class ExamenDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<ResultadoExamen> Resultados => Set<ResultadoExamen>();
    public DbSet<RespuestaExamen> Respuestas => Set<RespuestaExamen>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program
{
    static void Main()
    {
        using var db = new ExamenDbContext();
        db.Database.Migrate();

        while (true)
        {
            Console.WriteLine("\n1. Registrar pregunta\n2. Tomar examen\n3. Ver reportes\n0. Salir");
            var op = Console.ReadLine();
            switch (op)
            {
                case "1": RegistrarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": MostrarReportes(); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
    }

    static void RegistrarPregunta()
    {
        using var db = new ExamenDbContext();
        var pregunta = new Pregunta();

        Console.Write("Enunciado: ");
        pregunta.Enunciado = Console.ReadLine()!;
        Console.Write("Opción A: ");
        pregunta.OpcionA = Console.ReadLine()!;
        Console.Write("Opción B: ");
        pregunta.OpcionB = Console.ReadLine()!;
        Console.Write("Opción C: ");
        pregunta.OpcionC = Console.ReadLine()!;

        char resp;
        do
        {
            Console.Write("Respuesta correcta (A/B/C): ");
            resp = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();
        } while (resp != 'A' && resp != 'B' && resp != 'C');

        pregunta.RespuestaCorrecta = resp;

        db.Preguntas.Add(pregunta);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada correctamente.");
    }

    static void TomarExamen()
    {
        using var db = new ExamenDbContext();
        Console.Write("Nombre del alumno: ");
        string nombre = Console.ReadLine()!;

       var preguntas = db.Preguntas.ToList().OrderBy(x => Guid.NewGuid()).Take(5).ToList();
        if (preguntas.Count == 0)
        {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            TotalPreguntas = preguntas.Count
        };

        foreach (var p in preguntas)
        {
            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"A. {p.OpcionA}");
            Console.WriteLine($"B. {p.OpcionB}");
            Console.WriteLine($"C. {p.OpcionC}");
            Console.Write("Respuesta: ");
            char resp = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            bool correcto = resp == p.RespuestaCorrecta;
            if (correcto) resultado.TotalCorrectas++;

            resultado.Respuestas.Add(new RespuestaExamen
            {
                Pregunta = p,
                RespuestaSeleccionada = resp,
                EsCorrecta = correcto
            });
        }

        resultado.NotaFinal = resultado.TotalCorrectas;
        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Correctas: {resultado.TotalCorrectas}/{resultado.TotalPreguntas}. Nota: {resultado.NotaFinal}/5.");
    }

    static void MostrarReportes()
    {
        using var db = new ExamenDbContext();

        Console.WriteLine("\n1. Ver todos los exámenes");
        Console.WriteLine("2. Filtrar por nombre");
        Console.WriteLine("3. Ranking de mejores alumnos");
        Console.WriteLine("4. Estadísticas por pregunta");
        Console.Write("Opción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                foreach (var r in db.Resultados)
                    Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal}");
                break;

            case "2":
                Console.Write("Nombre: ");
                var nombre = Console.ReadLine();
                var filtrados = db.Resultados.Where(r => r.NombreAlumno == nombre);
                foreach (var r in filtrados)
                    Console.WriteLine($"Nota: {r.NotaFinal}");
                break;

            case "3":
                var ranking = db.Resultados
                    .GroupBy(r => r.NombreAlumno)
                    .Select(g => new { Alumno = g.Key, MejorNota = g.Max(x => x.NotaFinal) })
                    .OrderByDescending(x => x.MejorNota)
                    .ToList();

                foreach (var a in ranking)
                    Console.WriteLine($"{a.Alumno}: {a.MejorNota}");
                break;

            case "4":
                var preguntas = db.Preguntas.Include(p => p.Respuestas);
                foreach (var p in preguntas)
                {
                    int total = p.Respuestas.Count;
                    int correctas = p.Respuestas.Count(r => r.EsCorrecta);
                    double porcentaje = total > 0 ? (100.0 * correctas) / total : 0;
                    Console.WriteLine($"{p.Enunciado}\n  Total: {total}, Correctas: {correctas}, %: {porcentaje:F2}%");
                }
                break;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }
}