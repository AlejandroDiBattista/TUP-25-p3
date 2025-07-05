using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string OpcionA { get; set; } = "";
    public string OpcionB { get; set; } = "";
    public string OpcionC { get; set; } = "";
    public string RespuestaCorrecta { get; set; } = "";

    public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class ResultadoExamen
{
    public int Id { get; set; }
    public string Alumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen
{
    public int Id { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class AppDbContext : DbContext
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
    static void Main()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        while (true)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Registrar pregunta manualmente");
            Console.WriteLine("2. Cargar pregunta de ejemplo (del profesor)");
            Console.WriteLine("3. Tomar examen");
            Console.WriteLine("4. Ver resultados");
            Console.WriteLine("5. Ranking de alumnos");
            Console.WriteLine("6. Estadísticas por pregunta");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");

            var opcion = Console.ReadLine();
            Console.Clear();
            switch (opcion)
            {
                case "1": RegistrarPregunta(db); break;
                case "2": CargarPreguntaEjemplo(db); break;
                case "3": TomarExamen(db); break;
                case "4": VerResultados(db); break;
                case "5": VerRanking(db); break;
                case "6": EstadisticasPreguntas(db); break;
                case "0": return;
            }
        }
    }

    static void RegistrarPregunta(AppDbContext db)
    {
        Console.Write("Enunciado: ");
        var enunciado = Console.ReadLine();

        Console.Write("Opción A: ");
        var a = Console.ReadLine();

        Console.Write("Opción B: ");
        var b = Console.ReadLine();

        Console.Write("Opción C: ");
        var c = Console.ReadLine();

        Console.Write("Respuesta correcta (A/B/C): ");
        var correcta = Console.ReadLine()?.ToUpper();

        db.Preguntas.Add(new Pregunta
        {
            Enunciado = enunciado,
            OpcionA = a,
            OpcionB = b,
            OpcionC = c,
            RespuestaCorrecta = correcta
        });

        db.SaveChanges();
        Console.WriteLine("Pregunta registrada con éxito.");
    }

    static void CargarPreguntaEjemplo(AppDbContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        var p = new Pregunta
        {
            Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
            OpcionA = "Java",
            OpcionB = "C#",
            OpcionC = "Python",
            RespuestaCorrecta = "B"
        };

        db.Preguntas.Add(p);
        db.SaveChanges();

        foreach (var pregunta in db.Preguntas)
        {
            Console.WriteLine($"""
                #{pregunta.Id:000}
                {pregunta.Enunciado}
                A) {pregunta.OpcionA}
                B) {pregunta.OpcionB}
                C) {pregunta.OpcionC}
            """);
        }
    }

    static void TomarExamen(AppDbContext db)
    {
        if (!db.Preguntas.Any())
        {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }

        Console.Write("Nombre del alumno: ");
        var alumno = Console.ReadLine();

        var preguntas = db.Preguntas.ToList().OrderBy(p => Guid.NewGuid()).Take(5);
        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var pregunta in preguntas)
        {
            Console.WriteLine($"""
                {pregunta.Enunciado}
                A) {pregunta.OpcionA}
                B) {pregunta.OpcionB}
                C) {pregunta.OpcionC}
            """);
            Console.Write("Respuesta (A/B/C): ");
            var respuesta = Console.ReadLine()?.ToUpper();
            bool esCorrecta = respuesta == pregunta.RespuestaCorrecta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                Pregunta = pregunta,
                RespuestaAlumno = respuesta ?? "",
                EsCorrecta = esCorrecta
            });
        }

        var resultado = new ResultadoExamen
        {
            Alumno = alumno,
            CantidadCorrectas = correctas,
            TotalPreguntas = preguntas.Count(),
            NotaFinal = (correctas / (double)preguntas.Count()) * 10,
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();
        Console.WriteLine($"\nFinalizado. Nota: {resultado.NotaFinal:F1}/10");
    }

    static void VerResultados(AppDbContext db)
    {
        foreach (var r in db.Resultados)
        {
            Console.WriteLine($"Alumno: {r.Alumno}, Nota: {r.NotaFinal:F1}, Correctas: {r.CantidadCorrectas}/{r.TotalPreguntas}");
        }
    }

    static void VerRanking(AppDbContext db)
    {
        var ranking = db.Resultados
            .GroupBy(r => r.Alumno)
            .Select(g => new
            {
                Alumno = g.Key,
                MejorNota = g.Max(r => r.NotaFinal)
            })
            .OrderByDescending(x => x.MejorNota);

        Console.WriteLine("\n--- Ranking de Mejores Alumnos ---");
        foreach (var entry in ranking)
        {
            Console.WriteLine($"{entry.Alumno}: {entry.MejorNota:F1}");
        }
    }

    static void EstadisticasPreguntas(AppDbContext db)
    {
        var preguntas = db.Preguntas.Include(p => p.Respuestas);

        foreach (var p in preguntas)
        {
            int total = p.Respuestas.Count();
            int correctas = p.Respuestas.Count(r => r.EsCorrecta);
            double porcentaje = total > 0 ? (correctas / (double)total) * 100 : 0;

            Console.WriteLine($"""
                Pregunta: {p.Enunciado}
                Respondida: {total} veces
                Porcentaje de acierto: {porcentaje:F1}%
            """);
        }
    }
}
