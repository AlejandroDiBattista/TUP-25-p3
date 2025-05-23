using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
}
class ResultadoExamen {
    public int ResultadoExamenId {get; set; }
    public string Alumno {get; set; } = "";
    public int Correctas {get; set; }
    public int Total {get; set; }
    public double Nota {get; set; }
    public List<RespuestaExamen> Respuestas {get; set; } = new();
}
class RespuestaExamen {
    public int RespuestaExamenId {get; set; }
    public int PreguntaId {get; set; }
    public Pregunta Pregunta {get; set; } = null!;
    public string RespuestaAlumno {get; set; } = "";
    public bool EsCorrecta {get; set; }
    public int ResultadoExamenId {get; set; }
    public ResultadoExamen ResultadoExamen {get; set; } = null!;
}
class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados {get; set; }
    public DbSet<RespuestaExamen> Respuestas {get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}
class Program{
    static void Main(string[] args){
        using var db = new DatosContexto();
        db.Database.EnsureCreated();
        while (true){
            Console.WriteLine("\n--- Sistema de Exámenes ---");
            Console.WriteLine("1. Agregar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver reportes");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            string? opcion = Console.ReadLine();
            switch (opcion){
                case "1":
                    AgregarPregunta(db);
                    break;
                case "2":
                    TomarExamen(db);
                    break;
                case "3":
                    VerReportes(db);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }
    static void AgregarPregunta(DatosContexto db){
        Console.WriteLine("\n--- Agregar nueva pregunta ---");
        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine() ?? "";
        Console.Write("Opción A: ");
        string a = Console.ReadLine() ?? "";
        Console.Write("Opción B: ");
        string b = Console.ReadLine() ?? "";
        Console.Write("Opción C: ");
        string c = Console.ReadLine() ?? "";
        Console.Write("Respuesta correcta (A/B/C): ");
        string correcta = Console.ReadLine()?.ToUpper() ?? "A";
        var pregunta = new Pregunta{
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        };
        db.Preguntas.Add(pregunta);
        db.SaveChanges();
        Console.WriteLine("Pregunta guardada.");
    }
    static void TomarExamen(DatosContexto db){
        Console.Write("\nIngrese su nombre: ");
        string alumno = Console.ReadLine() ?? "Anónimo";
        int cantidad = Math.Min(5, db.Preguntas.Count());
        if (cantidad == 0){
            Console.WriteLine("No hay preguntas disponibles.");
            return;
        }
        var preguntas = db.Preguntas
            .ToList()
            .OrderBy(p => Guid.NewGuid())
            .Take(cantidad);
        var respuestas = new List<RespuestaExamen>();
        foreach (var pregunta in preguntas){
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($" A) {pregunta.RespuestaA}");
            Console.WriteLine($" B) {pregunta.RespuestaB}");
            Console.WriteLine($" C) {pregunta.RespuestaC}");
            string resp;
            do {
                Console.Write("Tu respuesta (A/B/C): ");
                resp = Console.ReadLine()?.ToUpper() ?? "";
                if (resp != "A" && resp != "B" && resp != "C") {
                    Console.WriteLine("Respuesta inválida. Ingrese solo A, B o C.");
                }
            } while (resp != "A" && resp != "B" && resp != "C");

            bool esCorrecta = resp == pregunta.Correcta;
            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.PreguntaId,
                RespuestaAlumno = resp,
                EsCorrecta = esCorrecta
            });
        }
        int correctas = respuestas.Count(r => r.EsCorrecta);
        double nota = 10.0 * correctas / cantidad;
        var resultado = new ResultadoExamen{
            Alumno = alumno,
            Correctas = correctas,
            Total = cantidad,
            Nota = nota,
            Respuestas = respuestas
        };
        db.Resultados.Add(resultado);
        db.SaveChanges();
        Console.WriteLine($"\nRespuestas correctas: {correctas} de {cantidad}");
        Console.WriteLine($"Nota final: {nota:F1}");
    }
    static void VerReportes(DatosContexto db){
        Console.WriteLine("\n--- Reportes ---");
        Console.WriteLine("1. Ver todos los exámenes");
        Console.WriteLine("2. Filtrar por alumno");
        Console.Write("Seleccione una opción: ");
        string? opcionReporte = Console.ReadLine();
        List<ResultadoExamen> examenes;
        if (opcionReporte == "2") {
            Console.Write("Ingrese el nombre del alumno: ");
            string nombreAlumno = Console.ReadLine()?.Trim().ToLower() ?? "";
            examenes = db.Resultados
                .Include(r => r.Respuestas)
                .Where(r => r.Alumno.ToLower().Contains(nombreAlumno))
                .ToList();
        }
        else {
            examenes = db.Resultados.Include(r => r.Respuestas).ToList();
        }
        foreach (var ex in examenes){
            Console.WriteLine($"{ex.Alumno} - Nota: {ex.Nota:F1} - Correctas: {ex.Correctas}/{ex.Total}");
        }
        var ranking = examenes
            .GroupBy(e => e.Alumno)
            .Select(g => new{
                Alumno = g.Key,
                MejorNota = g.Max(e => e.Nota)
            })
            .OrderByDescending(x => x.MejorNota);
        Console.WriteLine("\n--- Ranking de Mejores Notas ---");
        foreach (var r in ranking){
            Console.WriteLine($"{r.Alumno}: {r.MejorNota:F1}");
        }
        var estadisticas = db.Preguntas
            .Select(p => new{
                Pregunta = p,
                Total = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId),
                Correctas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta)
            })
            .ToList();
        Console.WriteLine("\n--- Estadísticas por Pregunta ---");
        foreach (var stat in estadisticas){
            double porcentaje = stat.Total > 0 ? 100.0 * stat.Correctas / stat.Total : 0;
            Console.WriteLine($"Pregunta: {stat.Pregunta.Enunciado}");
            Console.WriteLine($" Respondida: {stat.Total} veces, Correctas: {porcentaje:F1}%");
        }
    }
}