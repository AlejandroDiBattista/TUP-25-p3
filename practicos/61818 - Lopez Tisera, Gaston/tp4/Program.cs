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

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen Resultado { get; set; }

    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.Respuestas)
            .HasForeignKey(r => r.PreguntaId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Resultado)
            .WithMany(res => res.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);
    }
}

class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto()){


/*             db.Database.EnsureDeleted();
            db.Database.EnsureCreated(); */
            
                    while (true)
        {
            Console.Clear();
            Console.WriteLine("""
            === SISTEMA DE EXÁMENES MULTIPLE CHOICE ===

            1. Registrar nueva pregunta
            2. Tomar examen
            3. Ver reportes
            4. Eliminar preguntas
            0. Salir
            """);

            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": RegistrarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": VerReportes(); break;
                case "4": EliminarPregunta(); break;
                case "0": return;
                default:
                    Console.WriteLine("Opción inválida.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void RegistrarPregunta()
    {
        using var db = new DatosContexto();
        Console.Clear();
        Console.WriteLine("== Registrar nueva pregunta ==");

        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine() ?? "";

        Console.Write("Respuesta A: ");
        string a = Console.ReadLine() ?? "";

        Console.Write("Respuesta B: ");
        string b = Console.ReadLine() ?? "";

        Console.Write("Respuesta C: ");
        string c = Console.ReadLine() ?? "";

        string correcta;
        do
        {
            Console.Write("Respuesta correcta (A/B/C): ");
            correcta = Console.ReadLine()?.Trim().ToUpper();
        } while (correcta != "A" && correcta != "B" && correcta != "C");

        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        };
            
        db.Preguntas.Add(pregunta);      
        db.SaveChanges();               

        Console.WriteLine("Pregunta registrada correctamente.");
        Console.WriteLine("\nPresione una tecla para volver al menú...");
        Console.ReadKey();

        }

       static void EliminarPregunta()
{
    using var db = new DatosContexto();
    Console.Clear();

    var preguntas = db.Preguntas.ToList();

    if (preguntas.Count == 0)
    {
        Console.WriteLine("No hay preguntas para eliminar.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("== Eliminar Pregunta ==");
    for (int i = 0; i < preguntas.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {preguntas[i].Enunciado}");
    }

    Console.Write("\nIngrese el número de la pregunta a eliminar: ");
    if (int.TryParse(Console.ReadLine(), out int opcion) && opcion >= 1 && opcion <= preguntas.Count)
    {
        var seleccionada = preguntas[opcion - 1];
        db.Preguntas.Remove(seleccionada);
        db.SaveChanges();
        Console.WriteLine("Pregunta eliminada correctamente.");
    }
    else
    {
        Console.WriteLine("Opción inválida.");
    }

    Console.WriteLine("\nPresione una tecla para volver al menú...");
    Console.ReadKey();
}

        static void TomarExamen()
    {
        using var db = new DatosContexto();
        Console.Clear();
        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine()?.Trim() ?? "";

        var preguntas = db.Preguntas
                  .ToList()  
                  .OrderBy(p => Guid.NewGuid())  
                  .Take(5)  
                  .ToList();

        if (preguntas.Count == 0)
        {
            Console.WriteLine("No hay preguntas registradas.");
            Console.ReadKey();
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var p in preguntas)
        {
            Console.Clear();
            Console.WriteLine($"""
            {p.Enunciado}

             A) {p.RespuestaA}
             B) {p.RespuestaB}
             C) {p.RespuestaC}
            """);

            string respuesta;
            do
            {
                Console.Write("Respuesta (A/B/C): ");
                respuesta = Console.ReadLine()?.Trim().ToUpper();
            } while (respuesta != "A" && respuesta != "B" && respuesta != "C");

            bool esCorrecta = respuesta == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = p.PreguntaId,
                EsCorrecta = esCorrecta
            });
        }
    

           int total = preguntas.Count;
        double nota = (correctas * 10.0) / total;

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            CantidadCorrectas = correctas,
            TotalPreguntas = total,
            NotaFinal = nota,
            Respuestas = respuestas
        };
          
            db.Resultados.Add(resultado);
            db.SaveChanges();

            Console.Clear();
            Console.WriteLine($"""
        Examen finalizado.
        Alumno: {nombre}
        Correctas: {correctas}/{total}
        Nota final: {nota:F1}
        """);
 
         Console.WriteLine("\nPresione una tecla para volver al menú...");
        Console.ReadKey();
    }

         static void VerReportes()
    {
        using var db = new DatosContexto();

        Console.Clear();
        Console.WriteLine("""
        == REPORTES ==
        1. Ver todos los exámenes
        2. Buscar exámenes por alumno
        3. Ranking de mejores alumnos
        4. Estadísticas por pregunta
        """);

        Console.Write("Seleccione una opción: ");
        var opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                Console.Clear();
                foreach (var r in db.Resultados)
                {
                    Console.WriteLine($"{r.NombreAlumno} | {r.CantidadCorrectas}/{r.TotalPreguntas} | Nota: {r.NotaFinal:F1}");
                }
                break;

            case "2":
                Console.Write("Nombre del alumno: ");
                string nombre = Console.ReadLine() ?? "";
                Console.Clear();
                var resultados = db.Resultados
                    .Where(r => r.NombreAlumno.ToLower().Contains(nombre.ToLower()))
                    .ToList();

                foreach (var r in resultados)
                {
                    Console.WriteLine($"{r.NombreAlumno} | {r.CantidadCorrectas}/{r.TotalPreguntas} | Nota: {r.NotaFinal:F1}");
                }
                break;

            case "3":
                Console.Clear();
                var ranking = db.Resultados
                    .GroupBy(r => r.NombreAlumno)
                    .Select(g => new
                    {
                        Alumno = g.Key,
                        MejorNota = g.Max(x => x.NotaFinal)
                    })
                    .OrderByDescending(x => x.MejorNota)
                    .ToList();

                Console.WriteLine("Ranking de mejores alumnos:");
                foreach (var r in ranking)
                {
                    Console.WriteLine($"{r.Alumno}: {r.MejorNota:F1}");
                }
                break;

            case "4":
                Console.Clear();
                var estadisticas = db.Preguntas
                    .Select(p => new
                    {
                        Pregunta = p.Enunciado,
                        Total = p.Respuestas.Count,
                        Correctas = p.Respuestas.Count(r => r.EsCorrecta)
                    })
                    .ToList();

            foreach (var e in estadisticas)
                {
                    double porcentaje = e.Total == 0 ? 0 : (e.Correctas * 100.0) / e.Total;
                    Console.WriteLine($"\nPregunta: {e.Pregunta}");
                    Console.WriteLine($"Respondida: {e.Total} veces");
                    Console.WriteLine($"Correctas: {porcentaje:F1}%");
                }
                break;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }

        Console.WriteLine("\nPresione una tecla para volver al menú...");
        Console.ReadKey();
            }
        }
    }