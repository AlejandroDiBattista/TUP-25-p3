using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string RespuestaA { get; set; } = string.Empty;
    public string RespuestaB { get; set; } = string.Empty;
    public string RespuestaC { get; set; } = string.Empty;
    public string Correcta { get; set; } = string.Empty;
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = string.Empty;
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();

    // Asegurarse de que no sea NULL, inicializando en el constructor
    public ResultadoExamen()
    {
        Respuestas = new List<RespuestaExamen>();
    }
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = new Pregunta();  // Inicializamos aquí
    public string RespuestaDada { get; set; } = string.Empty;
    public bool EsCorrecta { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = new ResultadoExamen();  // Inicializamos aquí
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
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE EXÁMENES ===");
            Console.WriteLine("1) Registrar pregunta");
            Console.WriteLine("2) Tomar examen");
            Console.WriteLine("3) Ver reportes");
            Console.WriteLine("0) Salir");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": RegistrarPregunta(db); break;
                case "2": TomarExamen(db); break;
                case "3": VerReportes(db); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("=== REGISTRAR NUEVA PREGUNTA ===");

        var p = new Pregunta();

        Console.Write("Enunciado: ");
        p.Enunciado = Console.ReadLine() ?? string.Empty;

        Console.Write("Respuesta A: ");
        p.RespuestaA = Console.ReadLine() ?? string.Empty;

        Console.Write("Respuesta B: ");
        p.RespuestaB = Console.ReadLine() ?? string.Empty;

        Console.Write("Respuesta C: ");
        p.RespuestaC = Console.ReadLine() ?? string.Empty;

        Console.Write("Respuesta correcta (A/B/C): ");
        p.Correcta = Console.ReadLine()?.ToUpper() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(p.Enunciado) || string.IsNullOrWhiteSpace(p.RespuestaA) ||
            string.IsNullOrWhiteSpace(p.RespuestaB) || string.IsNullOrWhiteSpace(p.RespuestaC) ||
            string.IsNullOrWhiteSpace(p.Correcta))
        {
            Console.WriteLine("Todos los campos son obligatorios.");
            return;
        }

        db.Preguntas.Add(p);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada correctamente.");
        Console.ReadKey();
    }

    static void TomarExamen(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("=== TOMAR EXAMEN ===");
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("El nombre es obligatorio.");
            return;
        }

        var totalDisponibles = db.Preguntas.Count();
        var cantidad = Math.Min(5, totalDisponibles);

        var preguntas = db.Preguntas
            .OrderBy(p => EF.Functions.Random())
            .Take(cantidad)
            .ToList();

        if (!preguntas.Any())
        {
            Console.WriteLine("No hay preguntas registradas.");
            Console.ReadKey();
            return;
        }

        var respuestas = new List<RespuestaExamen>();
        int correctas = 0;

        foreach (var p in preguntas)
        {
            Console.Clear();
            Console.WriteLine($"{p.Enunciado}");
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("Respuesta (A/B/C): ");
            var respuesta = Console.ReadLine()?.ToUpper() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(respuesta) || !"ABC".Contains(respuesta))
            {
                Console.WriteLine("Respuesta inválida.");
                continue;
            }

            bool esCorrecta = respuesta == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = p.PreguntaId,
                RespuestaDada = respuesta,
                EsCorrecta = esCorrecta
            });
        }

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            CantidadCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = correctas * 10.0 / preguntas.Count,
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.Clear();
        Console.WriteLine("Examen finalizado.");
        Console.WriteLine($"Correctas: {correctas}/{preguntas.Count}");
        Console.WriteLine($"Nota final: {resultado.NotaFinal}");
        Console.ReadKey();
    }

    static void VerReportes(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("=== REPORTES ===");
        Console.WriteLine("1) Listado de exámenes");
        Console.WriteLine("2) Filtrar por alumno");
        Console.WriteLine("3) Ranking de alumnos");
        Console.WriteLine("4) Estadísticas por pregunta");
        Console.WriteLine("0) Volver");
        Console.Write("Opción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                foreach (var r in db.Resultados)
                    Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal}");
                break;
            case "2":
                Console.Write("Nombre del alumno: ");
                var nombre = Console.ReadLine() ?? string.Empty;
                var filtrados = db.Resultados
                    .Where(r => r.NombreAlumno.ToLower().Contains(nombre.ToLower()))
                    .ToList();
                foreach (var r in filtrados)
                    Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal}");
                break;
            case "3":
                var ranking = db.Resultados
                    .GroupBy(r => r.NombreAlumno)
                    .Select(g => new
                    {
                        Alumno = g.Key,
                        MejorNota = g.Max(r => r.NotaFinal)
                    })
                    .OrderByDescending(x => x.MejorNota)
                    .ToList();

                foreach (var r in ranking)
                    Console.WriteLine($"{r.Alumno} - Mejor nota: {r.MejorNota}");
                break;
            case "4":
                var estadisticas = db.Respuestas
                    .Include(r => r.Pregunta)
                    .GroupBy(r => r.Pregunta.Enunciado)
                    .Select(g => new
                    {
                        Enunciado = g.Key,
                        Total = g.Count(),
                        Correctas = g.Count(r => r.EsCorrecta)
                    }).ToList();

                foreach (var e in estadisticas)
                {
                    Console.WriteLine($"Pregunta: {e.Enunciado}");
                    Console.WriteLine($"Respondida: {e.Total} veces");
                    Console.WriteLine($"Porcentaje correctas: {e.Correctas * 100.0 / e.Total:0.0}%\n");
                }
                break;
        }

        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}
