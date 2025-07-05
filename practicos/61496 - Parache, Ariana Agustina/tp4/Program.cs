using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta
{
    public int Id { get; set; }
    public string Texto { get; set; } = "";
    public string Opcion1 { get; set; } = "";
    public string Opcion2 { get; set; } = "";
    public string Opcion3 { get; set; } = "";
    public string Correcta { get; set; } = "";
    public List<RespuestaAlumno> Respuestas { get; set; } = new();
}

class Examen
{
    public int Id { get; set; }
    public string Estudiante { get; set; } = "";
    public int Aciertos { get; set; }
    public int Total { get; set; }
    public double Puntuacion { get; set; }
    public List<RespuestaAlumno> Detalles { get; set; } = new();
}

class RespuestaAlumno
{
    public int Id { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public bool EsCorrecta { get; set; }
}

class BDContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<RespuestaAlumno> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}


class Program
{
    static void Main(string[] args)
    {
        using var db = new BDContexto();
        db.Database.EnsureCreated();

        if (!db.Preguntas.Any())
        {
            SembrarPreguntas(db);
        }

        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE EVALUACIÓN ===");
            Console.WriteLine("[1] Iniciar nuevo examen");
            Console.WriteLine("[2] Consultar historial de exámenes");
            Console.WriteLine("[3] Buscar por estudiante");
            Console.WriteLine("[4] Ver ranking");
            Console.WriteLine("[5] Estadísticas por pregunta");
            Console.WriteLine("[0] Salir");
            Console.Write("\nOpción: ");
            switch (Console.ReadLine())
            {
                case "1":
                    IniciarExamen(db);
                    break;
                case "2":
                    MostrarHistorial(db);
                    break;
                case "3":
                    BuscarEstudiante(db);
                    break;
                case "4":
                    VerRanking(db);
                    break;
                case "5":
                    Estadisticas(db);
                    break;
                case "0":
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Entrada inválida.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void SembrarPreguntas(BDContexto db)
    {
        var preguntas = new List<Pregunta>
        {
            new() { Texto = "¿Qué operador se usa para comparar igualdad en C#?", Opcion1 = "==", Opcion2 = "=", Opcion3 = "!=", Correcta = "A" },
            new() { Texto = "¿Qué palabra clave se usa para definir una constante?", Opcion1 = "readonly", Opcion2 = "static", Opcion3 = "const", Correcta = "C" },
            new() { Texto = "¿Qué tipo representa un número decimal en C#?", Opcion1 = "decimal", Opcion2 = "int", Opcion3 = "float", Correcta = "A" },
            new() { Texto = "¿Qué clase se utiliza para manejar listas dinámicas?", Opcion1 = "List", Opcion2 = "ArrayList", Opcion3 = "Array", Correcta = "A" },
            new() { Texto = "¿Cuál es el método para convertir string a entero?", Opcion1 = "int.Parse()", Opcion2 = "ToInt()", Opcion3 = "Convert.String()", Correcta = "A" },
            new() { Texto = "¿Qué palabra clave define una clase base?", Opcion1 = "base", Opcion2 = "super", Opcion3 = "abstract", Correcta = "C" },
            new() { Texto = "¿Cuál es el tipo de dato lógico en C#?", Opcion1 = "bool", Opcion2 = "Boolean", Opcion3 = "bit", Correcta = "A" }
        };
        db.Preguntas.AddRange(preguntas);
        db.SaveChanges();
    }

    static void IniciarExamen(BDContexto db)
    {
        Console.Clear();
        Console.Write("Nombre del estudiante: ");
        string nombre = Console.ReadLine() ?? "Desconocido";

        var preguntas = db.Preguntas.AsEnumerable().OrderBy(p => Guid.NewGuid()).Take(5).ToList();

        var respuestas = new List<RespuestaAlumno>();
        int aciertos = 0;

        foreach (var q in preguntas)
        {
            Console.Clear();
            Console.WriteLine($"{q.Texto}");
            Console.WriteLine($"A) {q.Opcion1}");
            Console.WriteLine($"B) {q.Opcion2}");
            Console.WriteLine($"C) {q.Opcion3}");
            Console.Write("Respuesta: ");
            string? respuesta = Console.ReadLine()?.ToUpper();
            bool correcta = respuesta == q.Correcta;
            if (correcta) aciertos++;

            respuestas.Add(new RespuestaAlumno { PreguntaId = q.Id, EsCorrecta = correcta });
        }

        double nota = Math.Round(aciertos * 10.0 / preguntas.Count, 1);

        var examen = new Examen
        {
            Estudiante = nombre,
            Aciertos = aciertos,
            Total = preguntas.Count,
            Puntuacion = nota,
            Detalles = respuestas
        };

        db.Examenes.Add(examen);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Resultado: {nota:F1}/10 ({aciertos} correctas)");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarHistorial(BDContexto db)
    {
        Console.Clear();
        var examenes = db.Examenes.OrderByDescending(e => e.Id).ToList();
        foreach (var e in examenes)
        {
            Console.WriteLine($"{e.Estudiante}: {e.Puntuacion:F1} ({e.Aciertos}/{e.Total})");
        }
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void BuscarEstudiante(BDContexto db)
    {
        Console.Clear();
        Console.Write("Buscar por nombre: ");
        string filtro = Console.ReadLine() ?? "";
        var encontrados = db.Examenes
            .Where(e => e.Estudiante.Contains(filtro))
            .ToList();

        foreach (var e in encontrados)
        {
            Console.WriteLine($"{e.Estudiante}: {e.Puntuacion:F1} ({e.Aciertos}/{e.Total})");
        }
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void VerRanking(BDContexto db)
    {
        Console.Clear();
        var ranking = db.Examenes
            .GroupBy(e => e.Estudiante)
            .Select(g => new { Nombre = g.Key, NotaMaxima = g.Max(e => e.Puntuacion) })
            .OrderByDescending(x => x.NotaMaxima)
            .Take(10)
            .ToList();

        int puesto = 1;
        foreach (var r in ranking)
        {
            Console.WriteLine($"{puesto++}. {r.Nombre} - {r.NotaMaxima:F1}");
        }
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void Estadisticas(BDContexto db)
    {
        Console.Clear();
        var lista = db.Preguntas.Include(p => p.Respuestas).ToList();

        foreach (var p in lista)
        {
            int total = p.Respuestas.Count;
            int aciertos = p.Respuestas.Count(r => r.EsCorrecta);
            double porcentaje = total > 0 ? (aciertos * 100.0 / total) : 0;

            Console.WriteLine($"Pregunta #{p.Id}: {p.Texto}");
            Console.WriteLine($"  Respondida: {total} veces - Correctas: {porcentaje:F1}%\n");
        }

        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}
