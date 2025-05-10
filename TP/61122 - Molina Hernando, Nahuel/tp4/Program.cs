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
    public ICollection<RespuestaExamen> RespuestasExamen { get; set; }
}

class Examen
{
    public int ExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program
{
    static void Main(string[] args)
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Registrar preguntas iniciales
            if (!db.Preguntas.Any())
            {
                db.Preguntas.AddRange(new List<Pregunta>
                {
                    new Pregunta
                    {
                        Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                        RespuestaA = "Java",
                        RespuestaB = "C#",
                        RespuestaC = "Python",
                        Correcta = "B"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Qué significa HTML?",
                        RespuestaA = "HyperText Markup Language",
                        RespuestaB = "HighText Machine Language",
                        RespuestaC = "Hyperlink and Text Markup Language",
                        Correcta = "A"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Qué es una clase en programación orientada a objetos?",
                        RespuestaA = "Un tipo de dato",
                        RespuestaB = "Una plantilla para crear objetos",
                        RespuestaC = "Un método especial",
                        Correcta = "B"
                    }
                });
                db.SaveChanges();
            }

            // Menú principal
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Sistema de Exámenes de Opción Múltiple");
                Console.WriteLine("1. Tomar examen");
                Console.WriteLine("2. Ver resultados");
                Console.WriteLine("3. Ver ranking");
                Console.WriteLine("4. Informe por pregunta");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        TomarExamen(db);
                        break;
                    case "2":
                        VerResultados(db);
                        break;
                    case "3":
                        VerRanking(db);
                        break;
                    case "4":
                        InformePorPregunta(db);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    static void TomarExamen(DatosContexto db)
    {
        Console.Clear();
        Console.Write("Ingrese su nombre: ");
        var alumno = Console.ReadLine();

        var preguntas = db.Preguntas.AsEnumerable().OrderBy(r => Guid.NewGuid()).Take(5).ToList();
        if (!preguntas.Any())
        {
            Console.WriteLine("No hay preguntas registradas. Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var pregunta in preguntas)
        {
            Console.Clear();
            Console.WriteLine($"Pregunta #{pregunta.PreguntaId}");
            Console.WriteLine(pregunta.Enunciado);
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");
            Console.Write("Seleccione una opción (A, B, C): ");
            var respuesta = Console.ReadLine()?.ToUpper();

            bool esCorrecta = respuesta == pregunta.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.PreguntaId,
                Pregunta = pregunta,
                EsCorrecta = esCorrecta
            });
        }

        var examen = new Examen
        {
            Alumno = alumno,
            RespuestasCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = (double)correctas / preguntas.Count * 10,
            Respuestas = respuestas
        };

        foreach (var r in respuestas)
        {
            r.Examen = examen;
        }

        db.Examenes.Add(examen);
        db.SaveChanges();

        Console.Clear();
        Console.WriteLine($"Examen finalizado. Respuestas correctas: {correctas}/{preguntas.Count}");
        Console.WriteLine($"Nota final: {examen.NotaFinal:F2}");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void VerResultados(DatosContexto db)
    {
        Console.Clear();
        Console.Write("Ingrese nombre del alumno para filtrar (dejar vacío para todos): ");
        string filtro = Console.ReadLine();
        var exámenesFiltrados = string.IsNullOrEmpty(filtro)
            ? db.Examenes.Include(e => e.Respuestas)
            : db.Examenes.Include(e => e.Respuestas).Where(e => e.Alumno.Contains(filtro));

        foreach (var examen in exámenesFiltrados)
        {
            Console.WriteLine($"Alumno: {examen.Alumno}");
            Console.WriteLine($"Respuestas correctas: {examen.RespuestasCorrectas}/{examen.TotalPreguntas}");
            Console.WriteLine($"Nota final: {examen.NotaFinal:F2}");
            Console.WriteLine(new string('-', 40));
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void VerRanking(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("Ranking de Mejores Alumnos (Nota más alta):");

        var ranking = db.Examenes
            .GroupBy(e => e.Alumno)
            .Select(g => new { Alumno = g.Key, MejorNota = g.Max(e => e.NotaFinal) })
            .OrderByDescending(r => r.MejorNota)
            .Take(10);

        foreach (var item in ranking)
        {
            Console.WriteLine($"{item.Alumno}: {item.MejorNota:F2}");
        }

        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void InformePorPregunta(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("Informe Estadístico por Pregunta:");

        var estadisticas = db.Preguntas
            .Select(p => new
            {
                Pregunta = p.Enunciado,
                VecesRespondida = p.RespuestasExamen.Count,
                PorcentajeCorrectas = p.RespuestasExamen.Any()
                    ? 100.0 * p.RespuestasExamen.Count(r => r.EsCorrecta) / p.RespuestasExamen.Count
                    : 0
            });

        foreach (var e in estadisticas)
        {
            Console.WriteLine($"- {e.Pregunta}");
            Console.WriteLine($"  Respondida: {e.VecesRespondida} veces");
            Console.WriteLine($"  Correctas: {e.PorcentajeCorrectas:F2}%");
            Console.WriteLine();
        }

        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}
