using System;
using System.Data.Common;
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
            db.Database.EnsureDeleted(); 
            db.Database.EnsureCreated(); 

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

        var preguntas = db.Preguntas.ToList(); 
        if (!preguntas.Any())
        {
            Console.WriteLine("No hay preguntas en el sistema."); return;
        }

        var seleccion = preguntas.OrderBy(x => Guid.NewGuid()).Take(5).ToList(); 

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
        Console.WriteLine("\n---Reportes---");
        Console.WriteLine("1. Listar examenes rendidos");
        Console.WriteLine("2. Filtrar por alumno");
        Console.WriteLine("3. Ranking de mejores notas");
        Console.WriteLine("4. Estadistica de preguntas");
        Console.WriteLine("5. Regresar al menu principal");
        Console.Write("Seleccione una opcion: ");
        var op = Console.ReadLine();
        switch(op){
           case "1": ListarExamenes(db); break;
          case "2": FiltrarPorAlumno(db); break;
          case "3": MostrarRanking(db); break;
          case "4": EstadisticaPreguntas(db); break;
          case "5": return;
         default: Console.WriteLine("Opción inválida."); break;

        }
        
           
    }   
    static void ListarExamenes(DatosContexto db)
{
    var examenes = db.Resultados.OrderByDescending(x => x.Fecha).ToList();
    foreach (var ex in examenes)
    {
        Console.WriteLine($"{ex.Fecha:dd/MM/yyyy HH:mm} - {ex.Alumno}: {ex.Correctas}/{ex.Total} - Nota: {ex.Nota}");
    }
}
 static void FiltrarPorAlumno(DatosContexto db)
{
    Console.Write("Nombre del alumno: ");
    var nombre = Console.ReadLine()!;
    var examenes = db.Resultados
                     .Where(x => x.Alumno.ToLower() == nombre.ToLower())
                     .OrderByDescending(x => x.Fecha)
                     .ToList();

    if (!examenes.Any())
    {
        Console.WriteLine("No se encontraron exámenes para ese alumno.");
        return;
    }

    foreach (var ex in examenes)
    {
        Console.WriteLine($"{ex.Fecha:dd/MM/yyyy HH:mm} - {ex.Alumno}: {ex.Correctas}/{ex.Total} - Nota: {ex.Nota}");
    }
}
static void MostrarRanking(DatosContexto db)
{
    var ranking = db.Resultados
                    .GroupBy(x => x.Alumno)
                    .Select(g => new {
                        Alumno = g.Key,
                        MejorNota = g.Max(e => e.Nota)
                    })
                    .OrderByDescending(x => x.MejorNota)
                    .Take(10)
                    .ToList();

    Console.WriteLine("\n--- Ranking de mejores alumnos ---");
    foreach (var r in ranking)
    {
        Console.WriteLine($"{r.Alumno} - Mejor Nota: {r.MejorNota}");
    }
}
static void EstadisticaPreguntas(DatosContexto db)
{
    var preguntas = db.Preguntas.Include(p => p.Respuetas).ToList();
    Console.WriteLine("\n--- Estadísticas por Pregunta ---");

    foreach (var p in preguntas)
    {
        int total = p.Respuetas.Count;
        int correctas = p.Respuetas.Count(r => r.Correcto);
        double porcentaje = total > 0 ? Math.Round((double)correctas / total * 100, 2) : 0;

        Console.WriteLine($"[{p.PreguntaId}] {p.Enunciado}");
        Console.WriteLine($" Respondida: {total} veces - Correctas: {porcentaje}%");
    }
}

}