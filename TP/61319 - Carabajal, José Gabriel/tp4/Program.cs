using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; }
    public string OpcionA { get; set; }
    public string OpcionB { get; set; }
    public string OpcionC { get; set; }
    public string OpcionCorrecta { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class ResultadoExamen
{
    public int Id { get; set; }
    public string Alumno { get; set; }
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double Nota { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class RespuestaExamen
{
    public int Id { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public string OpcionSeleccionada { get; set; }
    public bool EsCorrecta { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
}

public class ExamContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=examenes.db");
}

class Program
{
    static void Main(string[] args)
    {
        
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();

        using var context = new ExamContext();
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var p = new List<Pregunta>
        {
            new Pregunta {
                Enunciado  = "- ¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                OpcionA    = "Java",
                OpcionB    = "C#",
                OpcionC    = "Python",
                OpcionCorrecta = "B"
            },
            new Pregunta { 
                Enunciado = "- ¿Qué palabra clave se usa en C# para definir una clase?",    
                OpcionA = "class",     
                OpcionB = "struct",   
                OpcionC = "interface", 
                OpcionCorrecta = "A" 
            },
            new Pregunta { 
                Enunciado = "- ¿Qué método es el punto de entrada de una aplicación .NET?", 
                OpcionA = "Start()",    
                OpcionB = "Main()",     
                OpcionC = "Init()",     
                OpcionCorrecta = "B" 
            },
            new Pregunta { 
                Enunciado = "- ¿Qué operador se usa para la concatenación de strings en C#?",  
                OpcionA = "+",         
                OpcionB = "&",         
                OpcionC = ".",          
                OpcionCorrecta = "A" 
            },
            new Pregunta { 
                Enunciado = "- ¿Cuál de los siguientes no es un tipo de valor en C#?",     
                OpcionA = "int",       
                OpcionB = "string",    
                OpcionC = "bool",       
                OpcionCorrecta = "B" 
            }
        };
        context.Preguntas.AddRange(p);
        context.SaveChanges();

        // Menú principal
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--------- MENÚ DE OPCIONES --------\n");
            Console.WriteLine("1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Listar todos los exámenes");
            Console.WriteLine("4. Filtrar exámenes por alumno");
            Console.WriteLine("5. Ranking de alumnos (mejor nota)");
            Console.WriteLine("6. Estadística por pregunta");
            Console.WriteLine("7. Salir");
            Console.ResetColor();
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": RegistrarPregunta(context); break;
                case "2": TomarExamen(context);   break;
                case "3": ListarExamenes(context); break;
                case "4": FiltrarPorAlumno(context); break;
                case "5": RankingAlumnos(context);  break;
                case "6": EstadisticaPorPregunta(context); break;
                case "7": return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Opción inválida.");
                    Console.ResetColor();
                    break;
            }
        }
    }

    static void RegistrarPregunta(ExamContext context)
    {
        Console.Write("Enunciado: "); string enunciado = Console.ReadLine();
        Console.Write("Opción A: ");    string a = Console.ReadLine();
        Console.Write("Opción B: ");    string b = Console.ReadLine();
        Console.Write("Opción C: ");    string c = Console.ReadLine();
        Console.Write("Opción correcta (A/B/C): "); string correcta = Console.ReadLine().ToUpper();

        var pregunta = new Pregunta {
            Enunciado = enunciado,
            OpcionA = a,
            OpcionB = b,
            OpcionC = c,
            OpcionCorrecta = correcta
        };
        context.Preguntas.Add(pregunta);
        context.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Pregunta registrada con ID = " + pregunta.Id);
        Console.ResetColor();
    }

    static void TomarExamen(ExamContext context)
    {
        Console.Write("Nombre del alumno: ");
        string alumno = Console.ReadLine();

        int totalPreguntas = context.Preguntas.Count();
        if (totalPreguntas == 0)
        {
            Console.WriteLine("No hay preguntas disponibles.");
            return;
        }

        int numPreguntas = Math.Min(5, totalPreguntas);
        var preguntas = context.Preguntas
                            .OrderBy(p => EF.Functions.Random())
                            .Take(numPreguntas)
                            .ToList();

        int aciertos = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var pregunta in preguntas)
        {
            Console.WriteLine(pregunta.Enunciado);
            Console.WriteLine("A) " + pregunta.OpcionA);
            Console.WriteLine("B) " + pregunta.OpcionB);
            Console.WriteLine("C) " + pregunta.OpcionC);

            string r;
            do
            {
                Console.Write("Respuesta (A/B/C): ");
                r = Console.ReadLine().ToUpper();
                if (r != "A" && r != "B" && r != "C")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Opción inválida. Seleccione A, B o C.");
                    Console.ResetColor();
                }
            } while (r != "A" && r != "B" && r != "C");

            bool esCorrecta = (r == pregunta.OpcionCorrecta);
            if (esCorrecta) aciertos++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.Id,
                OpcionSeleccionada = r,
                EsCorrecta = esCorrecta
            });

            Console.WriteLine(); 
        }

        double nota = Math.Round(aciertos * 10.0 / numPreguntas, 2);
        var resultado = new ResultadoExamen
        {
            Alumno = alumno,
            RespuestasCorrectas = aciertos,
            TotalPreguntas = numPreguntas,
            Nota = nota,
            Respuestas = respuestas
        };
        context.Resultados.Add(resultado);
        context.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Examen finalizado! Aciertos: {aciertos}/{numPreguntas}.\n Nota: {nota}/10");
        Console.ResetColor();
    }

    static void ListarExamenes(ExamContext context)
    {
        var lista = context.Resultados.ToList();
        Console.WriteLine("Listado de todos los exámenes:");
        foreach (var res in lista)
            Console.WriteLine($"ID={res.Id} Alumno={res.Alumno} Aciertos={res.RespuestasCorrectas}/{res.TotalPreguntas} Nota={res.Nota}/10");
    }

    static void FiltrarPorAlumno(ExamContext context)
    {
        Console.Write("Nombre del alumno a filtrar: "); 
        string nombre = Console.ReadLine();
        var lista = context.Resultados.Where(r => r.Alumno == nombre).ToList();

        Console.WriteLine($"Exámenes del alumno \"{nombre}\":");
        foreach (var res in lista)
            Console.WriteLine($"ID={res.Id} Aciertos={res.RespuestasCorrectas}/{res.TotalPreguntas} Nota={res.Nota}/10");
    }

    static void RankingAlumnos(ExamContext context)
    {
        var ranking = context.Resultados
                            .OrderByDescending(r => r.Nota)
                            .ToList();
        Console.WriteLine("Ranking de alumnos por mejor nota:");
        foreach (var res in ranking)
            Console.WriteLine($"Alumno={res.Alumno} Nota={res.Nota}/10 ({res.RespuestasCorrectas}/{res.TotalPreguntas})");
    }

    static void EstadisticaPorPregunta(ExamContext context)
    {
        Console.WriteLine("Estadísticas por pregunta:");
        var preguntas = context.Preguntas.ToList();
        foreach (var preg in preguntas)
        {
            int total    = context.Respuestas.Count(r => r.PreguntaId == preg.Id);
            int correctas= context.Respuestas.Count(r => r.PreguntaId == preg.Id && r.EsCorrecta);
            double pct   = total > 0 ? Math.Round(100.0 * correctas / total, 2) : 0.0;
            Console.WriteLine($"Pregunta ID={preg.Id}: \"{preg.Enunciado}\" - Respondida {total} veces, Acertadas {correctas} ({pct}%)");
        }
    }
}
