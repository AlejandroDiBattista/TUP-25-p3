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

    Console.WriteLine("✅ Pregunta registrada con éxito.");
}
}

