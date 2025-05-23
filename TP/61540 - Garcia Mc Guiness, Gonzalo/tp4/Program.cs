using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.Respuestas)
            .HasForeignKey(r => r.PreguntaId);
    }
}

class Program {
    static void Main() {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        while (true) {
            Console.WriteLine("""
            === Sistema de Exámenes ===
            1. Registrar pregunta
            2. Rendir examen
            3. Ver reportes
            4. Salir
            """);

            Console.Write("Opción: ");
            var opcion = Console.ReadLine();
            Console.Clear();

            if (opcion == "1") RegistrarPregunta(db);
            else if (opcion == "2") RendirExamen(db);
            else if (opcion == "3") MostrarReportes(db);
            else if (opcion == "4") break;
            else Console.WriteLine("Opción inválida");
        }
    }

    static void RegistrarPregunta(DatosContexto db) {
        Console.WriteLine("=== Registrar Pregunta ===");
        Console.Write("Enunciado: "); string enunciado = Console.ReadLine()!;
        Console.Write("Respuesta A: "); string a = Console.ReadLine()!;
        Console.Write("Respuesta B: "); string b = Console.ReadLine()!;
        Console.Write("Respuesta C: "); string c = Console.ReadLine()!;
        Console.Write("Correcta (A/B/C): "); string correcta = Console.ReadLine()!.ToUpper();

        var p = new Pregunta {
            Enunciado = enunciado, RespuestaA = a,
            RespuestaB = b, RespuestaC = c,
            Correcta = correcta
        };

        db.Preguntas.Add(p);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada correctamente.");
    }

    static void RendirExamen(DatosContexto db) {
        Console.Write("Nombre del alumno: ");
        string nombre = Console.ReadLine()!;
        var preguntas = db.Preguntas.OrderBy(p => EF.Functions.Random()).Take(5).ToList();

        if (preguntas.Count == 0) {
            Console.WriteLine("No hay preguntas para realizar un examen.");
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var p in preguntas) {
            Console.WriteLine($"""

                {p.Enunciado}

                 A) {p.RespuestaA}
                 B) {p.RespuestaB}
                 C) {p.RespuestaC}
            """);
            Console.Write("Respuesta: ");
            string rta = Console.ReadLine()!.ToUpper();

            bool esCorrecta = rta == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaAlumno = rta,
                EsCorrecta = esCorrecta
            });
        }

        double nota = Math.Round(correctas * 10.0 / preguntas.Count, 2);
        var resultado = new ResultadoExamen {
            NombreAlumno = nombre,
            CantCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = nota,
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"Examen finalizado. Nota: {nota}/10\n");
    }

    static void MostrarReportes(DatosContexto db) {
        Console.WriteLine("""
        === Reportes ===
        1. Ver todos los exámenes
        2. Filtrar por alumno
        3. Ranking por mejor nota
        4. Estadísticas por pregunta
        """);

        Console.Write("Opción: ");
        var op = Console.ReadLine();
        Console.Clear();

        if (op == "1") {
            var todos = db.Resultados.ToList();
            foreach (var r in todos)
                Console.WriteLine($"{r.NombreAlumno}: {r.NotaFinal}/10 ({r.CantCorrectas}/{r.TotalPreguntas})");
        }
        else if (op == "2") {
            Console.Write("Nombre del alumno: ");
            string nombre = Console.ReadLine()!;
            var examenes = db.Resultados
                .Where(r => r.NombreAlumno.ToLower() == nombre.ToLower())
                .ToList();
            foreach (var r in examenes)
                Console.WriteLine($"{r.NombreAlumno}: {r.NotaFinal}/10");
        }
        else if (op == "3") {
            var ranking = db.Resultados
                .GroupBy(r => r.NombreAlumno)
                .Select(g => new {
                    Alumno = g.Key,
                    MejorNota = g.Max(x => x.NotaFinal)
                })
                .OrderByDescending(x => x.MejorNota)
                .Take(10)
                .ToList();

            foreach (var r in ranking)
                Console.WriteLine($"{r.Alumno}: {r.MejorNota}/10");
        }
        else if (op == "4") {
            var preguntas = db.Preguntas.Include(p => p.Respuestas).ToList();
            foreach (var p in preguntas) {
                int total = p.Respuestas.Count;
                int correctas = p.Respuestas.Count(r => r.EsCorrecta);
                double porcentaje = total > 0 ? (correctas * 100.0 / total) : 0;
                Console.WriteLine($"""
                    Pregunta: {p.Enunciado}
                    Respondida: {total} veces
                    % Correctas: {Math.Round(porcentaje, 2)}%
                """);
            }
        }
    }
}