using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using var db = new ExamenContext();
CargarPreguntasIniciales(db);
db.Database.EnsureCreated();

bool salir = false;

while (!salir)
{
    Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
    Console.WriteLine("1. Agregar pregunta");
    Console.WriteLine("2. Tomar examen");
    Console.WriteLine("3. Ver historial de exámenes");
    Console.WriteLine("4. Ver ranking de alumnos");
    Console.WriteLine("5. Estadísticas por pregunta");
    Console.WriteLine("0. Salir");

    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1": AgregarPregunta(); break;
        case "2": TomarExamen(); break;
        case "3": MostrarHistorial(); break;
        case "4": MostrarRanking(); break;
        case "5": MostrarEstadisticas(); break;
        case "0": salir = true; break;
        default: Console.WriteLine("Opción inválida."); break;
    }
}

static void CargarPreguntasIniciales(ExamenContext db)
{
    if (db.Preguntas.Count() >= 5) return;

    var preguntas = new List<Pregunta>
    {
        new() { Enunciado = "¿Cuál es el tipo de dato para números enteros?", OpcionA = "int", OpcionB = "string", OpcionC = "bool", RespuestaCorrecta = 'A' },
        new() { Enunciado = "¿Qué operador se usa para sumar?", OpcionA = "*", OpcionB = "+", OpcionC = "-", RespuestaCorrecta = 'B' },
        new() { Enunciado = "¿Cuál es el resultado de 10 % 3?", OpcionA = "1", OpcionB = "0", OpcionC = "3", RespuestaCorrecta = 'A' },
        new() { Enunciado = "¿Qué palabra clave se usa para declarar una variable?", OpcionA = "int", OpcionB = "new", OpcionC = "var", RespuestaCorrecta = 'C' },
        new() { Enunciado = "¿Qué estructura permite repetir un bloque?", OpcionA = "if", OpcionB = "while", OpcionC = "switch", RespuestaCorrecta = 'B' }
    };

    db.Preguntas.AddRange(preguntas);
    db.SaveChanges();
}

void AgregarPregunta()
{
    Console.Write("Enunciado: ");
    string enunciado = Console.ReadLine()!;
    Console.Write("Opción A: ");
    string a = Console.ReadLine()!;
    Console.Write("Opción B: ");
    string b = Console.ReadLine()!;
    Console.Write("Opción C: ");
    string c = Console.ReadLine()!;
    Console.Write("Respuesta correcta (A/B/C): ");
    char r = char.ToUpper(Console.ReadLine()![0]);

    db.Preguntas.Add(new Pregunta
    {
        Enunciado = enunciado,
        OpcionA = a,
        OpcionB = b,
        OpcionC = c,
        RespuestaCorrecta = r
    });
    db.SaveChanges();
    Console.WriteLine("Pregunta guardada.");
}

void TomarExamen()
{
    Console.Write("Nombre del alumno: ");
    string alumno = Console.ReadLine()!;

    var preguntas = db.Preguntas.AsEnumerable().OrderBy(p => Guid.NewGuid()).Take(5).ToList();

    if (preguntas.Count == 0)
    {
        Console.WriteLine("No hay preguntas disponibles.");
        return;
    }

    int correctas = 0;
    var respuestas = new List<RespuestaExamen>();

    foreach (var p in preguntas)
    {
        Console.WriteLine($"\n{p.Enunciado}");
        Console.WriteLine($"A: {p.OpcionA}");
        Console.WriteLine($"B: {p.OpcionB}");
        Console.WriteLine($"C: {p.OpcionC}");
        Console.Write("Respuesta: ");
        char resp = char.ToUpper(Console.ReadLine()![0]);
        bool esCorrecta = resp == p.RespuestaCorrecta;
        if (esCorrecta) correctas++;
        respuestas.Add(new RespuestaExamen {
            PreguntaId = p.Id,
            RespuestaAlumno = resp,
            EsCorrecta = esCorrecta
        });
    }

    double nota = Math.Round((double)correctas / preguntas.Count * 10, 2);

    var examen = new ResultadoExamen
    {
        Alumno = alumno,
        CantidadCorrectas = correctas,
        TotalPreguntas = preguntas.Count,
        Nota = nota,
        Respuestas = respuestas
    };

    db.Examenes.Add(examen);
    db.SaveChanges();

    Console.WriteLine($"\nResultado: {correctas}/{preguntas.Count} correctas. Nota: {nota}");
}

void MostrarHistorial()
{
    var examenes = db.Examenes
        .OrderByDescending(e => e.Id)
        .ToList();

    foreach (var ex in examenes)
    {
        Console.WriteLine($"{ex.Alumno} → {ex.CantidadCorrectas}/{ex.TotalPreguntas} → Nota: {ex.Nota}");
    }
}

void MostrarRanking()
{
    var mejores = db.Examenes
        .GroupBy(e => e.Alumno)
        .Select(g => new
        {
            Alumno = g.Key,
            MejorNota = g.Max(x => x.Nota)
        })
        .OrderByDescending(x => x.MejorNota)
        .ToList();

    Console.WriteLine("\n--- Ranking de alumnos ---");
    foreach (var r in mejores)
    {
        Console.WriteLine($"{r.Alumno}: {r.MejorNota}");
    }
}

void MostrarEstadisticas()
{
    var estadisticas = db.Preguntas
        .Include(p => p.Respuestas)
        .ToList();

    foreach (var p in estadisticas)
    {
        int total = p.Respuestas.Count;
        int correctas = p.Respuestas.Count(r => r.EsCorrecta);
        double porcentaje = total > 0 ? (double)correctas / total * 100 : 0;
        Console.WriteLine($"\n{p.Enunciado}");
        Console.WriteLine($"Respondida {total} veces. Correctas: {porcentaje:F1}%");
    }
}



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
    public string Alumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double Nota { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class RespuestaExamen
{
    public int Id { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen Resultado { get; set; } = null!;
    public char RespuestaAlumno { get; set; }
    public bool EsCorrecta { get; set; }
}



public class ExamenContext : DbContext
{
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<ResultadoExamen> Examenes => Set<ResultadoExamen>();
    public DbSet<RespuestaExamen> Respuestas => Set<RespuestaExamen>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examenes.db");
    }
}
