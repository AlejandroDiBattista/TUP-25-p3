using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

// MODELOS
public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string OpcionA { get; set; } = "";
    public string OpcionB { get; set; } = "";
    public string OpcionC { get; set; } = "";
    public string RespuestaCorrecta { get; set; } = ""; // A, B o C
    public ICollection<RespuestaExamen>? Respuestas { get; set; }
}

public class ResultadoExamen
{
    public int Id { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public ICollection<RespuestaExamen>? Respuestas { get; set; }
}

public class RespuestaExamen
{
    public int Id { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public string RespuestaDada { get; set; } = "";
    public bool EsCorrecta { get; set; }
}


public class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=examen.db");
}

// PROGRAMA PRINCIPAL
class Program
{
    static void Main()
    {
        using var db = new DatosContexto();
        db.Database.Migrate();

        while (true)
        {
            Console.WriteLine("\n--- SISTEMA DE EXÁMENES ---");
            Console.WriteLine("1. Registrar nueva pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver reportes");
            Console.WriteLine("4. Ver registro de respuestas");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            var opc = Console.ReadLine();

            switch (opc)
            {
                case "1": RegistrarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": VerReportes(); break;
                case "4": VerRegistroRespuestas(); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
    }

    static void RegistrarPregunta()
    {
        using var db = new DatosContexto();

        Console.WriteLine("\n--- REGISTRO DE PREGUNTA ---");
        Console.Write("Enunciado: ");
        var enunciado = Console.ReadLine()?.Trim();

        Console.Write("Opción A: ");
        var a = Console.ReadLine()?.Trim();

        Console.Write("Opción B: ");
        var b = Console.ReadLine()?.Trim();

        Console.Write("Opción C: ");
        var c = Console.ReadLine()?.Trim();

        Console.Write("Respuesta correcta (A, B, C): ");
        var correcta = Console.ReadLine()?.Trim().ToUpper();

        if (!new[] { "A", "B", "C" }.Contains(correcta))
        {
            Console.WriteLine("Respuesta correcta inválida.");
            return;
        }

        db.Preguntas.Add(new Pregunta
        {
            Enunciado = enunciado!,
            OpcionA = a!,
            OpcionB = b!,
            OpcionC = c!,
            RespuestaCorrecta = correcta!
        });

        db.SaveChanges();
        Console.WriteLine("? Pregunta guardada con éxito.");
    }

    static void TomarExamen()
    {
        using var db = new DatosContexto();

        Console.Write("\nNombre del alumno: ");
        var alumno = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(alumno))
        {
            Console.WriteLine("? El nombre del alumno no puede estar vacío.");
            return;
        }

        var preguntas = db.Preguntas.ToList()
                            .OrderBy(p => Guid.NewGuid())
                            .Take(Math.Min(10, db.Preguntas.Count()))
                            .ToList();

        if (!preguntas.Any())
        {
            Console.WriteLine("?? No hay preguntas registradas.");
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var p in preguntas)
        {
            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"A) {p.OpcionA}");
            Console.WriteLine($"B) {p.OpcionB}");
            Console.WriteLine($"C) {p.OpcionC}");

            string? r;
            do
            {
                Console.Write("Tu respuesta (A, B, C): ");
                r = Console.ReadLine()?.Trim().ToUpper();
            } while (!new[] { "A", "B", "C" }.Contains(r));

            bool esCorrecta = r == p.RespuestaCorrecta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = p.Id,
                RespuestaDada = r!,
                EsCorrecta = esCorrecta
            });
        }

        double nota = (correctas / (double)preguntas.Count) * 10;

        var resultado = new ResultadoExamen
        {
            NombreAlumno = alumno!,
            RespuestasCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = Math.Round(nota, 2),
            Respuestas = new List<RespuestaExamen>()
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        foreach (var respuesta in respuestas)
        {
            respuesta.ResultadoExamenId = resultado.Id;
        }

        db.Respuestas.AddRange(respuestas);
        db.SaveChanges();

        Console.WriteLine($"\n? Examen finalizado. Nota: {resultado.NotaFinal}/10");
    }

    static void VerReportes()
    {
        using var db = new DatosContexto();

        Console.WriteLine("\n--- REPORTES ---");
        Console.WriteLine("1. Ver todos los exámenes");
        Console.WriteLine("2. Filtrar por alumno");
        Console.WriteLine("3. Ranking de alumnos");
        Console.WriteLine("4. Estadísticas por pregunta");
        Console.Write("Opción: ");
        var opc = Console.ReadLine();

        switch (opc)
        {
            case "1":
                var exams = db.Resultados.ToList();
                foreach (var e in exams)
                    Console.WriteLine($"{e.NombreAlumno} - Nota: {e.NotaFinal}/10 - Correctas: {e.RespuestasCorrectas}/{e.TotalPreguntas}");
                break;

            case "2":
                Console.Write("Nombre del alumno: ");
                var nombre = Console.ReadLine()?.Trim();
                var porAlumno = db.Resultados.Where(r => r.NombreAlumno == nombre).ToList();
                foreach (var e in porAlumno)
                    Console.WriteLine($"Nota: {e.NotaFinal} - Correctas: {e.RespuestasCorrectas}/{e.TotalPreguntas}");
                break;

            case "3":
                var ranking = db.Resultados
                    .GroupBy(r => r.NombreAlumno)
                    .Select(g => new
                    {
                        Alumno = g.Key,
                        MejorNota = g.Max(x => x.NotaFinal)
                    })
                    .OrderByDescending(r => r.MejorNota)
                    .ToList();

                foreach (var r in ranking)
                    Console.WriteLine($"{r.Alumno}: {r.MejorNota}/10");
                break;

            case "4":
                var preguntas = db.Preguntas.Include(p => p.Respuestas).ToList();

                foreach (var p in preguntas)
                {
                    int total = p.Respuestas?.Count ?? 0;
                    int correctas = p.Respuestas?.Count(r => r.EsCorrecta) ?? 0;
                    double porcentaje = total > 0 ? (correctas / (double)total) * 100 : 0;

                    Console.WriteLine($"\n{p.Enunciado}");
                    Console.WriteLine($"Respondida: {total} veces - Aciertos: {porcentaje:F2}%");
                }
                break;

            default:
                Console.WriteLine("? Opción no válida.");
                break;
        }
    }

    //registro de respuestas
    static void VerRegistroRespuestas()
    {
        using var db = new DatosContexto();

        var respuestas = db.Respuestas
            .Include(r => r.Pregunta)
            .Include(r => r.ResultadoExamen)
            .ToList();

        Console.WriteLine("\n--- REGISTRO DE RESPUESTAS ---");
        int num = 1;
        foreach (var r in respuestas)
        {
            string simbolo = r.EsCorrecta ? "?" : "?";
            Console.WriteLine($"\n{num++}. Alumno: {r.ResultadoExamen.NombreAlumno}");
            Console.WriteLine($"   Pregunta: {r.Pregunta.Enunciado}");
            Console.WriteLine($"   Respuesta dada: {r.RespuestaDada} {simbolo}");

            if (!r.EsCorrecta)
            {
                Console.WriteLine($"   Respuesta correcta: {r.Pregunta.RespuestaCorrecta}");
            }
        }
    }
}
