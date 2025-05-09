using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// Entidades (Tablas)
class Pregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
    public List<RespuestaExamen> Respuetas { get; set; } = new List<RespuestaExamen>();
}

class ResultadoExamen
{
    public int ResultadoExamenID { get; set; }
    public string Alumno { get; set; } = string.Empty;
    public int Correctas { get; set; }
    public int Total { get; set; }
    public double Nota { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int PreguntaID { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    public bool Correcto { get; set; }
}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; } = null!;
    public DbSet<ResultadoExamen> Resultados { get; set; } = null!;
    public DbSet<RespuestaExamen> Respuestas { get; set; } = null!;


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
            db.Database.EnsureDeleted(); // Borra las tablas si existen
            db.Database.EnsureCreated(); // Si no existen crea las tablas que estan en el dbContext

            while (true)
            {
                Console.WriteLine("\n---Menu---");
                Console.WriteLine("1. Registrar Pregunta");
                Console.WriteLine("2. Tomar Examen");
                Console.WriteLine("3. Ver Reportes");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opcion: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1": RegistrarPregunta(db); break;
                    case "2": TomarExamen(db); break;
                    case "3": VerReportes(db); break;
                    case "4": return;
                    default: Console.WriteLine("Opcion Invalida."); break;
                }
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db)
    {
        var p = new Pregunta();
        Console.Write("Enunciado: "); p.Enunciado = Console.ReadLine()!;
        Console.Write("A) "); p.RespuestaA = Console.ReadLine()!;
        Console.Write("B) "); p.RespuestaB = Console.ReadLine()!;
        Console.Write("C) "); p.RespuestaC = Console.ReadLine()!;
        Console.Write("Respuesta Correcta (A/B/C): "); p.Correcta = Console.ReadLine()!.ToUpper();
        db.Preguntas.Add(p);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada con ID: " + p.PreguntaId);
    }

    static void TomarExamen(DatosContexto db)
    {
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine()!;

        var preguntas = db.Preguntas.ToList(); // LINQ
        if (!preguntas.Any())
        {
            Console.WriteLine("No hay preguntas en el sistema."); return;
        }

        var seleccion = preguntas.OrderBy(x => Guid.NewGuid()).Take(5).ToList(); // LINQ

        int correctas = 0;
        var resultado = new ResultadoExamen { Alumno = nombre, Total = seleccion.Count };

        foreach (var preg in seleccion)
        {
            Console.WriteLine($"\n{preg.PreguntaId}. {preg.Enunciado}");
            Console.WriteLine($" A) {preg.RespuestaA}");
            Console.WriteLine($" B) {preg.RespuestaB}");
            Console.WriteLine($" C) {preg.RespuestaC}");
            Console.Write("Tu respuesta: ");
            var resp = Console.ReadLine()!.ToUpper();
            bool ok = resp == preg.Correcta;
            if (ok) correctas++;

            var respEx = new RespuestaExamen { Pregunta = preg, Correcto = ok };
            resultado.Respuestas.Add(respEx);
        }

        resultado.Correctas = correctas;
        resultado.Nota = Math.Round((double)correctas / resultado.Total * 5, 2);
        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nResultado: {correctas}/{resultado.Total} - Nota: {resultado.Nota}");
    }

    static void VerReportes(DatosContexto db)
        {
            while (true)
            {
                Console.WriteLine("\n--- Reportes ---");
                Console.WriteLine("1. Listar todos los exámenes");
                Console.WriteLine("2. Filtrar por nombre de alumno");
                Console.WriteLine("3. Ranking de mejores alumnos");
                Console.WriteLine("4. Estadísticas por pregunta");
                Console.WriteLine("5. Volver al menú principal");
                Console.Write("Seleccione una opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        var examenes = db.Resultados
                            .Include(r => r.Respuestas)
                            .ToList();

                        Console.WriteLine("\n--- Todos los Exámenes ---");
                        foreach (var ex in examenes)
                        {
                            Console.WriteLine($"Alumno: {ex.Alumno} | Fecha: {ex.Fecha.ToShortDateString()} | Nota: {ex.Nota} ({ex.Correctas}/{ex.Total})");
                        }
                        break;

                    case "2":
                        Console.Write("Nombre del alumno: ");
                        var nombre = Console.ReadLine()!;
                        var filtrados = db.Resultados
                            .Where(r => r.Alumno.Contains(nombre))
                            .ToList();

                        Console.WriteLine($"\n--- Exámenes de '{nombre}' ---");
                        foreach (var ex in filtrados)
                        {
                            Console.WriteLine($"Fecha: {ex.Fecha.ToShortDateString()} | Nota: {ex.Nota} ({ex.Correctas}/{ex.Total})");
                        }
                        break;

                    case "3":
                        var ranking = db.Resultados
                            .GroupBy(r => r.Alumno)
                            .Select(g => new
                            {
                                Alumno = g.Key,
                                MejorNota = g.Max(r => r.Nota)
                            })
                            .OrderByDescending(r => r.MejorNota)
                            .ToList();

                        Console.WriteLine("\n--- Ranking de Mejores Alumnos ---");
                        foreach (var r in ranking)
                        {
                            Console.WriteLine($"Alumno: {r.Alumno} | Mejor Nota: {r.MejorNota}");
                        }
                        break;

                    case "4":
                        var estadisticas = db.Preguntas
                            .Include(p => p.Respuetas)
                            .Select(p => new
                            {
                                Enunciado = p.Enunciado,
                                Total = p.Respuetas.Count(),
                                Correctas = p.Respuetas.Count(r => r.Correcto)
                            })
                            .ToList();

                        Console.WriteLine("\n--- Estadísticas por Pregunta ---");
                        foreach (var e in estadisticas)
                        {
                            double porcentaje = e.Total == 0 ? 0 : (double)e.Correctas / e.Total * 100;
                            Console.WriteLine($"\nPregunta: {e.Enunciado}");
                            Console.WriteLine($" - Veces respondida: {e.Total}");
                            Console.WriteLine($" - Porcentaje correctas: {porcentaje:F2}%");
                        }
                        break;

                    case "5": return;
                    default: Console.WriteLine("Opción inválida."); break;
                }
            }
        }


}

