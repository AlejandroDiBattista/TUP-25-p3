using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Examen
{
    [Key]
    public int IdExamen { get; set; }

    [ForeignKey("Alumno")]
    public int IdAlumnoFK { get; set; }

    public string Materia { get; set; }

    public List<Pregunta> ListaPreguntas { get; set; } = new List<Pregunta>();

    public Alumno Alumno { get; set; } // Propiedad de navegación
}

public class Respuestas
{
    [Key]
    public int IdResultado { get; set; }

    [ForeignKey("Examen")]
    public int IdExamenFK { get; set; }

    [ForeignKey("Pregunta")]
    public int PreguntaIdFk { get; set; }
    public bool RespondidaCorrectamente { get; set; }

    public Examen Examen { get; set; }
    public Pregunta Pregunta { get; set; }

}

public class Alumno
{
    [Key]
    public int IdAlumno { get; set; }
    public string nombre { get; set; }
    public List<Examen> Examenes { get; set; } = new List<Examen>();
}
public class Pregunta
{
    [Key]
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";

}

public class Resultados
{
    [Key]
    public int IdResultado { get; set; }

    [ForeignKey("Examen")]
    public int IdExamenFK { get; set; }
    public string nombreAlumno { get; set; }
    public int cantidadPreguntas { get; set; }
    public int NotaFinal { get; set; }
    public Examen Examen { get; set; }

}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<Resultados> Resultados { get; set; }
    public DbSet<Respuestas> Respuestas { get; set; }

    public DbSet<Alumno> Alumnos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Pregunta>().HasData(BDPreguntas.CargarListaPreguntas());

    }

}

class Program
{
    static void Main(string[] args)
    {

        using (var db = new DatosContexto())
        {
            // db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            int optMenu = 0;
            Console.Clear();



            do
            {
                Console.Clear();
                Console.WriteLine("Ingrese una opcion");
                Console.WriteLine($"""
                1) Mostrar listado de todos los exámenes rendidos.
                2) Filtrar resultados por nombre de alumno.
                3) Mostrar un ranking de los mejores alumnos basado en la mejor nota obtenida.
                4) Mostrar un informe estadístico por pregunta
                5) Tomar Examen
                0) Salir
                """);

                optMenu = Convert.ToInt32(Console.ReadLine());

                switch (optMenu)
                {
                    case 1:
                        foreach (var item in db.Examenes)
                        {
                            Console.WriteLine($"Examen: {item.IdExamen} | Materia: {item.Materia}");
                        }
                        PresKeyContinue();
                        break;
                    case 2:
                        Console.WriteLine("Inrgese el nombre");
                        string nombreBuscado = Console.ReadLine();

                        foreach (var item in db.Resultados.Where(x => x.nombreAlumno == nombreBuscado))
                        {
                            Console.WriteLine($"Nombre: {item.nombreAlumno} | Preguntas: {item.cantidadPreguntas} | Correctas {item.NotaFinal}");
                        }
                        PresKeyContinue();

                        break;
                    case 3:
                        foreach (var item in db.Resultados.OrderByDescending(x => x.NotaFinal))
                        {
                            Console.WriteLine($"Nombre: {item.nombreAlumno} | Preguntas: {item.cantidadPreguntas} | Correctas {item.NotaFinal}");
                        }
                        PresKeyContinue();
                        break;
                    case 4:
                        var estadisticasPorPregunta = db.Respuestas
                            .GroupBy(r => new { r.PreguntaIdFk, r.Pregunta.Enunciado })
                            .Select(g => new
                            {
                                Enunciado = g.Key.Enunciado,
                                TotalRespondidas = g.Count(),
                                TotalCorrectas = g.Count(r => r.RespondidaCorrectamente),
                                PorcentajeCorrectas = g.Count(r => r.RespondidaCorrectamente) * 100.0 / g.Count()
                            })
                            .ToList();

                        foreach (var item in estadisticasPorPregunta)
                        {
                            Console.WriteLine("-----------------------------------------------------------------------")

                            Console.WriteLine($"Enunciado: {item.Enunciado}");
                            Console.WriteLine($"Total de veces respondidas: {item.TotalRespondidas} ");
                            Console.WriteLine($"Total de veces respondidas Correctamente: {item.TotalCorrectas}");
                            Console.WriteLine($"Porcentaje de respuestas respondidas Correctamente: {item.PorcentajeCorrectas}\n");
                            Console.WriteLine("-----------------------------------------------------------------------")
                        }
                        PresKeyContinue();

                        break;
                    case 5:
                        TomarExamen.Rendir();
                        PresKeyContinue();

                        break;
                    default:
                        break;
                }
            } while (optMenu != 0);
        }
    }

    public static void PresKeyContinue()
    {
        Console.Clear();
        Console.WriteLine("presine cualquier letras para continuar");
        Console.ReadKey();
    }

}

public static class TomarExamen
{
    public static void Rendir()
    {
        using (var db = new DatosContexto())
        {
            // db.Database.EnsureCreated();

            List<string> RespuestasCorrectas = new List<string> { "a", "b", "c" };
            string opcion;
            Alumno alumno = new Alumno();

            Resultados resutaldo = new Resultados();
            Examen examen = new Examen();
            int correcta = 0, cantidad = 0;

            Console.Clear();

            Console.WriteLine("Ingrese el nombre del alumno");
            alumno.nombre = Console.ReadLine();
            db.Alumnos.Add(alumno);
            db.SaveChanges();

            Console.WriteLine("\nIngrese el nombre de la materia");
            examen.Materia = Console.ReadLine();
            examen.IdAlumnoFK = db.Alumnos.OrderByDescending(a => a.IdAlumno).FirstOrDefault()?.IdAlumno ?? 0;
            db.Examenes.Add(examen);
            db.SaveChanges();


            Console.Clear();
            foreach (var pregunta in db.Preguntas)
            {
                opcion = ""; // Inicializar opción antes del bucle

                while (!RespuestasCorrectas.Contains(opcion.ToLower()))
                {
                    Console.Clear();
                    Console.WriteLine("Elije una de las siguientes opciones");

                    Console.WriteLine($"""
                    #{pregunta.PreguntaId:000}

                    {pregunta.Enunciado}

                    A) {pregunta.RespuestaA}
                    B) {pregunta.RespuestaB}
                    C) {pregunta.RespuestaC}
                    """);

                    opcion = Console.ReadKey().KeyChar.ToString().ToLower();
                }

                Respuestas respuestas = new Respuestas();

                respuestas.RespondidaCorrectamente = pregunta.Correcta.ToLower() == opcion;
                respuestas.PreguntaIdFk = pregunta.PreguntaId;
                respuestas.IdExamenFK = db.Examenes.OrderByDescending(a => a.IdExamen).FirstOrDefault()?.IdExamen ?? 0;

                db.Respuestas.Add(respuestas);
                db.SaveChanges();


                correcta += (pregunta.Correcta.ToLower() == opcion) ? 1 : 0;
                cantidad++;

            }
            resutaldo.NotaFinal = correcta;
            resutaldo.nombreAlumno = alumno.nombre;
            resutaldo.cantidadPreguntas = cantidad;
            resutaldo.IdExamenFK = db.Examenes.OrderByDescending(a => a.IdExamen).FirstOrDefault()?.IdExamen ?? 0;

            db.Resultados.Add(resutaldo);
            db.SaveChanges();

            Console.Clear();
            Console.WriteLine($"Respuestas Correctas {correcta}/{cantidad}");

        }
    }
}

public static class BDPreguntas
{

    public static List<Pregunta> CargarListaPreguntas()
    {


        List<Pregunta> preguntas = new List<Pregunta>
        {
            new Pregunta
            {
                PreguntaId = 1,
                Enunciado = "¿Cuál de las siguientes palabras clave se utiliza para definir una clase en C#?",
                RespuestaA = "struct",
                RespuestaB = "class",
                RespuestaC = "interface",
                Correcta = "b"
            },
            new Pregunta
            {
                PreguntaId = 2,
                Enunciado = "¿Cuál es el operador de acceso a miembros en C#?",
                RespuestaA = ".",
                RespuestaB = "::",
                RespuestaC = "->",
                Correcta = "a"
            },
            new Pregunta
            {
                PreguntaId = 3,
                Enunciado = "¿Cuál de los siguientes tipos de datos en C# es un valor por referencia?",
                RespuestaA = "int",
                RespuestaB = "string",
                RespuestaC = "double",
                Correcta = "b"
            },
            new Pregunta
            {
                PreguntaId = 4,
                Enunciado = "¿Cómo se declara una variable constante en C#?",
                RespuestaA = "const int x = 10;",
                RespuestaB = "readonly int x = 10;",
                RespuestaC = "static int x = 10;",
                Correcta = "a;"
            },
            new Pregunta
            {
                PreguntaId = 5,
                Enunciado = "¿Cuál de los siguientes tipos en C# es una estructura?",
                RespuestaA = "List<T>",
                RespuestaB = "Dictionary<T, K>",
                RespuestaC = "DateTime",
                Correcta = "c"
            },
            new Pregunta
            {
                PreguntaId = 6,
                Enunciado = "¿Cuál de las siguientes afirmaciones sobre los métodos en C# es correcta?",
                RespuestaA = "Todos los métodos deben devolver un valor.",
                RespuestaB = "Un método puede tener múltiples valores de retorno.",
                RespuestaC = "Los métodos en C# deben definirse dentro de una clase.",
                Correcta = "c."
            },
            new Pregunta
            {
                PreguntaId = 7,
                Enunciado = "¿Qué palabra clave se usa para definir una variable que no puede cambiar su valor?",
                RespuestaA = "var",
                RespuestaB = "const",
                RespuestaC = "dynamic",
                Correcta = "b"
            },
            new Pregunta
            {
                PreguntaId = 8,
                Enunciado = "¿Cuál de los siguientes modificadores de acceso permite que una variable solo sea accesible dentro de la misma clase?",
                RespuestaA = "public",
                RespuestaB = "private",
                RespuestaC = "protected",
                Correcta = "b"
            },
            new Pregunta
            {
                PreguntaId = 9,
                Enunciado = "¿Qué tipo de datos se usa para trabajar con valores booleanos en C#?",
                RespuestaA = "int",
                RespuestaB = "bool",
                RespuestaC = "char",
                Correcta = "b"
            },
            new Pregunta
            {
                PreguntaId = 10,
                Enunciado = "¿Cuál de las siguientes estructuras de control se usa para iterar una cantidad fija de veces?",
                RespuestaA = "while",
                RespuestaB = "do-while",
                RespuestaC = "for",
                Correcta = "c"
            }

        };

        return preguntas;

    }
}