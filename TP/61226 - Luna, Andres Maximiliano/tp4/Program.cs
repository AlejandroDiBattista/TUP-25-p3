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
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
    }

    class RespuestaExamen {
    public int RespuestaExamenId { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen? ResultadoExamen { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta? Pregunta { get; set; }

    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
    }
    

class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}

class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto()){
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            var p = new Pregunta {
                Enunciado  = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                RespuestaA = "Java",
                RespuestaB = "C#",
                RespuestaC = "Python",
                Correcta   = "B"
            };
            db.Preguntas.Add(p);
            db.SaveChanges();
            
            Console.Clear();
            foreach(var pregunta in db.Preguntas){
                Console.WriteLine($"""

                    #{pregunta.PreguntaId:000}
                
                    {p.Enunciado}

                     A) {p.RespuestaA}
                     B) {p.RespuestaB}
                     C) {p.RespuestaC}

                """);
            }
        bool salir = false;
            while (!salir)
            {
                Console.WriteLine("=== Menú Principal ===");
                Console.WriteLine("1. Registrar nueva pregunta");
                Console.WriteLine("2. Tomar examen");
                Console.WriteLine("3. Ver reportes");
                Console.WriteLine("4. Salir");
                Console.Write("Opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        RegistrarPregunta(db);
                        break;
                    case "2":
                        TomarExamen(db);
                        break;
                    case "3":
                        VerReportes(db);
                        break;
                    case "4":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("Presione una tecla para continuar...");
                    Console.ReadKey();
                }
        }

}
}

static void RegistrarPregunta(DatosContexto db)
{
    Console.Clear();
    Console.WriteLine("=== Registrar nueva pregunta ===");

    Console.Write("Enunciado: ");
    var enunciado = Console.ReadLine() ?? "";

    Console.Write("Respuesta A: ");
    var a = Console.ReadLine() ?? "";

    Console.Write("Respuesta B: ");
    var b = Console.ReadLine() ?? "";

    Console.Write("Respuesta C: ");
    var c = Console.ReadLine() ?? "";

    string correcta = "";
    while (correcta != "A" && correcta != "B" && correcta != "C")
    {
        Console.Write("Respuesta correcta (A/B/C): ");
        correcta = Console.ReadLine()?.ToUpper() ?? "";
        if (correcta != "A" && correcta != "B" && correcta != "C")
        {
            Console.WriteLine("Respuesta inválida. Debe ser A, B o C.");
        }
    }

    var nueva = new Pregunta
    {
        Enunciado = enunciado,
        RespuestaA = a,
        RespuestaB = b,
        RespuestaC = c,
        Correcta = correcta
    };

    db.Preguntas.Add(nueva);
    db.SaveChanges();

    Console.WriteLine("Pregunta registrada con éxito.");


}

static void TomarExamen(DatosContexto db)
{
    var preguntas = db.Preguntas
    .ToList()
    .OrderBy(r => Guid.NewGuid())
    .Take(5).
    ToList();

    if (preguntas.Count == 0)
    {
        Console.WriteLine("No hay preguntas disponibles.");
        return;
    }

    Console.Clear();
    Console.Write("Ingrese su nombre: ");
    var nombre = Console.ReadLine() ?? "Alumno";

    var resultado = new ResultadoExamen
    {
        NombreAlumno = nombre,
        TotalPreguntas = preguntas.Count
    };

    foreach (var pregunta in preguntas)
    {
        Console.Clear();
        Console.WriteLine($"Pregunta #{pregunta.PreguntaId}:");
        Console.WriteLine(pregunta.Enunciado);
        Console.WriteLine($"A) {pregunta.RespuestaA}");
        Console.WriteLine($"B) {pregunta.RespuestaB}");
        Console.WriteLine($"C) {pregunta.RespuestaC}");

        string respuestaAlumno = "";
        while (respuestaAlumno != "A" && respuestaAlumno != "B" && respuestaAlumno != "C")
        {
            Console.Write("Respuesta (A/B/C): ");
            respuestaAlumno = Console.ReadLine()?.ToUpper() ?? "";
        }

        bool esCorrecta = respuestaAlumno == pregunta.Correcta;
        if (esCorrecta) resultado.CantidadCorrectas++;

        resultado.Respuestas.Add(new RespuestaExamen
        {
            PreguntaId = pregunta.PreguntaId,
            RespuestaAlumno = respuestaAlumno,
            EsCorrecta = esCorrecta
        });
    }

    resultado.NotaFinal = (double)resultado.CantidadCorrectas / resultado.TotalPreguntas * 10;
    db.Resultados.Add(resultado);
    db.SaveChanges();

    Console.WriteLine($"\nExamen finalizado. Obtuviste {resultado.CantidadCorrectas}/{resultado.TotalPreguntas} respuestas correctas.");
    Console.WriteLine($"Nota final: {resultado.NotaFinal:F1}");
}

  static void VerReportes(DatosContexto db)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("""
                MENÚ DE REPORTES

                1. Ver todos los exámenes
                2. Buscar exámenes por alumno
                3. Ver ranking de mejores alumnos
                4. Ver estadísticas por pregunta
                5. Volver al menú principal
            """);

            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            if (opcion == "1") MostrarExamenes();
            else if (opcion == "2") BuscarExamenesPorAlumno();
            else if (opcion == "3") MostrarRankingAlumnos();
            else if (opcion == "4") MostrarEstadisticasPorPregunta();
            else if (opcion == "5") break;
            else
            {
                Console.WriteLine("Opción inválida.");
                Console.ReadKey();
            }
        }
    }

    static void MostrarExamenes()
{
    using var db = new DatosContexto();
    var examenes = db.Resultados
        .OrderByDescending(r => r.ResultadoExamenId)
        .ToList();

    Console.Clear();
    Console.WriteLine("Listado de examenes rendidos:");

    if (examenes.Count == 0)
    {
        Console.WriteLine("No hay exámenes registrados.");
        return;
    }

    foreach (var examen in examenes)
    {
        Console.WriteLine($"Alumno: {examen.NombreAlumno}");
        Console.WriteLine($" - Correctas: {examen.CantidadCorrectas} / {examen.TotalPreguntas}");
        Console.WriteLine($" - Nota final: {examen.NotaFinal:F1}\n");
    }

    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}

static void BuscarExamenesPorAlumno()
{
    Console.Clear();
    Console.Write("Ingrese el nombre del alumno a buscar: ");
    string nombre = Console.ReadLine() ?? "";

    using var db = new DatosContexto();

    var examenes = db.Resultados
        .Where(r => r.NombreAlumno.ToLower().Contains(nombre.ToLower()))
        .OrderByDescending(r => r.ResultadoExamenId)
        .ToList();

    Console.Clear();
    Console.WriteLine($"Examenes de: {nombre}\n");

    if (examenes.Count == 0)
    {
        Console.WriteLine("No se encontraron exámenes para ese nombre.");
    }
    else
    {
        foreach (var examen in examenes)
        {
            Console.WriteLine($"Alumno: {examen.NombreAlumno}");
            Console.WriteLine($" - Correctas: {examen.CantidadCorrectas} / {examen.TotalPreguntas}");
            Console.WriteLine($" - Nota final: {examen.NotaFinal:F1}\n");
        }
    }

    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}

static void MostrarRankingAlumnos()
{
    Console.Clear();
    using var db = new DatosContexto();

    var ranking = db.Resultados
        .GroupBy(r => r.NombreAlumno.ToLower())
        .Select(grupo => new
        {
            Nombre = grupo.First().NombreAlumno,
            MejorNota = grupo.Max(r => r.NotaFinal)
        })
        .OrderByDescending(r => r.MejorNota)
        .ToList();

    Console.WriteLine("Mejores alumnos");

    if (ranking.Count == 0)
    {
        Console.WriteLine("No hay exámenes registrados.");
    }
    else
    {
        int posicion = 1;
        foreach (var alumno in ranking)
        {
            Console.WriteLine($"{posicion}. {alumno.Nombre} - Nota: {alumno.MejorNota:F1}");
            posicion++;
        }
    }

    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

static void MostrarEstadisticasPorPregunta()
{
    Console.Clear();
    using var db = new DatosContexto();

    var estadisticas = db.Preguntas
        .Select(p => new
        {
            p.PreguntaId,
            p.Enunciado,
            TotalRespuestas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId),
            Correctas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta)
        })
        .ToList();

    Console.WriteLine("Estadisticas por pregunta");

    if (estadisticas.Count == 0)
    {
        Console.WriteLine("No hay preguntas registradas.");
    }
    else
    {
        foreach (var e in estadisticas)
        {
            double porcentaje = e.TotalRespuestas > 0
                ? (double)e.Correctas / e.TotalRespuestas * 100
                : 0;

            Console.WriteLine($"""
                Pregunta #{e.PreguntaId:000}:
                {e.Enunciado}
                Respuestas totales: {e.TotalRespuestas}
                Porcentaje correctas: {porcentaje:F2}%
                --------------------------------------
            """);
        }
    }

    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}
}