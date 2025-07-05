using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int Correctas { get; set; }
    public int Total { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    public bool Correcta { get; set; }
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program {
    static void Main() {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        while (true) {
            Console.Clear();
            Console.WriteLine("1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver todos los exámenes");
            Console.WriteLine("4. Buscar por alumno");
            Console.WriteLine("5. Ranking de alumnos");
            Console.WriteLine("6. Estadísticas por pregunta");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            if (opcion == "0") break;
            if (opcion == "1") RegistrarPregunta(db);
            if (opcion == "2") TomarExamen(db);
            if (opcion == "3") VerExamenes(db);
            if (opcion == "4") BuscarAlumno(db);
            if (opcion == "5") Ranking(db);
            if (opcion == "6") Estadisticas(db);
        }
    }

    static void RegistrarPregunta(DatosContexto db) {
        Console.Clear();
        Console.Write("Enunciado: ");
        var enunciado = Console.ReadLine() ?? "";
        Console.Write("Respuesta A: ");
        var a = Console.ReadLine() ?? "";
        Console.Write("Respuesta B: ");
        var b = Console.ReadLine() ?? "";
        Console.Write("Respuesta C: ");
        var c = Console.ReadLine() ?? "";
        Console.Write("Correcta (A/B/C): ");
        var correcta = Console.ReadLine()?.ToUpper() ?? "";

        db.Preguntas.Add(new Pregunta {
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        });
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada.");
        Console.ReadKey();
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        Console.Write("Nombre del alumno: ");
        var alumno = Console.ReadLine() ?? "";

        var preguntas = db.Preguntas
    .AsEnumerable()
    .OrderBy(p => Guid.NewGuid())
    .Take(5)
    .ToList();

        var respuestas = new List<RespuestaExamen>();
        int correctas = 0;

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine(p.Enunciado);
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("Tu respuesta: ");
            var r = Console.ReadLine()?.ToUpper() ?? "";

            bool esCorrecta = r == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen {
                Pregunta = p,
                Correcta = esCorrecta
            });
        }

        double nota = correctas;
        var resultado = new ResultadoExamen {
            Alumno = alumno,
            Correctas = correctas,
            Total = preguntas.Count,
            NotaFinal = nota,
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"Examen finalizado. Nota: {nota}/5");
        Console.ReadKey();
    }

    static void VerExamenes(DatosContexto db) {
        Console.Clear();
        var examenes = db.Resultados.ToList();
        foreach (var e in examenes) {
            Console.WriteLine($"{e.Alumno} - Nota: {e.NotaFinal} ({e.Correctas}/{e.Total})");
        }
        Console.ReadKey();
    }

    static void BuscarAlumno(DatosContexto db) {
        Console.Clear();
        Console.Write("Nombre a buscar: ");
        var nombre = Console.ReadLine() ?? "";
        var resultados = db.Resultados
            .Where(e => e.Alumno.ToLower().Contains(nombre.ToLower()))
            .ToList();

        foreach (var e in resultados) {
            Console.WriteLine($"{e.Alumno} - Nota: {e.NotaFinal} ({e.Correctas}/{e.Total})");
        }
        Console.ReadKey();
    }

    static void Ranking(DatosContexto db) {
        Console.Clear();
        var ranking = db.Resultados
            .GroupBy(e => e.Alumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(x => x.NotaFinal)
            })
            .OrderByDescending(r => r.MejorNota)
            .ToList();

        foreach (var r in ranking) {
            Console.WriteLine($"{r.Alumno} - Mejor nota: {r.MejorNota}");
        }
        Console.ReadKey();
    }

    static void Estadisticas(DatosContexto db) {
        Console.Clear();
        var preguntas = db.Preguntas
    .AsEnumerable()
    .OrderBy(p => Guid.NewGuid())
    .Take(5)
    .ToList();


        foreach (var p in preguntas) {
            int total = p.Respuestas.Count;
            int correctas = p.Respuestas.Count(r => r.Correcta);
            double porcentaje = total == 0 ? 0 : (correctas * 100.0 / total);
            Console.WriteLine($"{p.Enunciado}");
            Console.WriteLine($"Respondida: {total} veces - Correctas: {porcentaje:0.0}%");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
    }