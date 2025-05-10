using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

class Pregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
}
class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string Nombre { get; set; } = "";
    public int CantidadPreguntas { get; set; } = 0;
    public int RespuestasCorrectas { get; set; } = 0;
    public int Nota { get; set; } = 0;
}
class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ExamenId { get; set; } = 0;
    public ResultadoExamen Examen { get; set; } = new ResultadoExamen();
    public int PreguntaId { get; set; } = 0;
    public Pregunta Pregunta { get; set; } = new Pregunta();
    public bool Correcta { get; set; } = false;
}
class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamenes { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamenes { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}
class Program
{
    static void Main(string[] args)
    {
        bool continuar = true;
        while (continuar)
        {
            continuar = MenuPrincipal();
        }
    }
    static bool MenuPrincipal()
    {
        Console.Clear();
        Console.WriteLine("-- Menú Principal --");
        Console.WriteLine("1. Registrar Pregunta");
        Console.WriteLine("2. Tomar Examen");
        Console.WriteLine("3. Ver Reportes");
        Console.WriteLine("4. Salir");
        Console.Write("Seleccione una opción: ");
        var opcion = Console.ReadLine();
        switch (opcion)
        {
            case "1":
                RegistrarPregunta();
                break;
            case "2":
                TomarExamen();
                break;
            case "3":
                MenuReportes();
                break;
            case "4":
                return false;
            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
        return true;
    }
    static void RegistrarPregunta()
    {
        Console.Clear();
        Console.WriteLine("-- Registro de Pregunta --");
        string enunciado = ValidarNoVacio("Ingrese el enunciado de la pregunta: ");
        string respuestaA = ValidarNoVacio("Ingrese la respuesta A: ");
        string respuestaB = ValidarNoVacio("Ingrese la respuesta B: ");
        string respuestaC = ValidarNoVacio("Ingrese la respuesta C: ");
        Console.Clear();
        string correcta = ValidarRespuesta("Ingrese la respuesta correcta (A, B o C): ");
        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta
        };
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
            db.Preguntas.Add(pregunta);
            db.SaveChanges();
            Console.WriteLine("Pregunta registrada con éxito.");
        }
        Console.Clear();
        Continuar();
    }
    static void TomarExamen()
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
            var preguntas = db.Preguntas.OrderBy(p => EF.Functions.Random()).Take(5).ToList();
            if (preguntas.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("No hay preguntas registradas.");
                Continuar();
                return;
            }
            string nombre = ValidarNoVacio("Ingrese su nombre: ");
            var examen = new ResultadoExamen
            {
                Nombre = nombre,
                CantidadPreguntas = preguntas.Count,
            };
            Console.Clear();
            Console.WriteLine("-- Examen --");
            foreach (var pregunta in preguntas)
            {
                Console.WriteLine($"""

                #{pregunta.PreguntaId:000}
            
                {pregunta.Enunciado}

                    A) {pregunta.RespuestaA}
                    B) {pregunta.RespuestaB}
                    C) {pregunta.RespuestaC}

            """);
                string respuesta = ValidarRespuesta("Seleccione una respuesta (A, B o C): ");
                Console.Clear();
                if (respuesta == pregunta.Correcta)
                {
                    Console.WriteLine("Respuesta correcta.");
                    examen.RespuestasCorrectas++;
                }
                else
                {
                    Console.WriteLine("Respuesta incorrecta.");
                }
                var respuestaExamen = new RespuestaExamen
                {
                    Examen = examen,
                    Pregunta = pregunta,
                    Correcta = respuesta == pregunta.Correcta
                };
                db.Add(respuestaExamen);
            }
            examen.Nota = (examen.RespuestasCorrectas * 10) / examen.CantidadPreguntas;
            Console.Clear();
            Console.WriteLine($"Examen finalizado. Respuestas correctas: {examen.RespuestasCorrectas}/{examen.CantidadPreguntas}. Nota: {examen.Nota}");
            db.Add(examen);
            db.SaveChanges();
            Console.WriteLine("Examen guardado con éxito.");
            Continuar();
        }
    }
    static void MenuReportes()
    {
        Console.Clear();
        Console.WriteLine("-- Reportes --");
        Console.WriteLine("1. Listado de Exámenes");
        Console.WriteLine("2. Resultados por Alumno");
        Console.WriteLine("3. Listado de Mejores Alumnos");
        Console.WriteLine("4. Informe estadístico por pregunta");
        Console.WriteLine("5. Volver al menú principal");
        Console.Write("Seleccione una opción: ");
        var opcion = Console.ReadLine();
        switch (opcion)
        {
            case "1":
                ListadoExamenes();
                break;
            case "2":
                ResultadosPorAlumno();
                break;
            case "3":
                MejoresAlumnos();
                break;
            case "4":
                InformeEstadisticoPorPregunta();
                break;
            case "5":
                Main(Array.Empty<string>());
                break;
            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }
    static void ListadoExamenes()
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
            var examenes = db.Set<ResultadoExamen>().ToList();
            Console.Clear();
            if (examenes.Count == 0)
            {
                Console.WriteLine("No hay examenes registrados.");
                Continuar();
                return;
            }
            Console.WriteLine("-- Listado de Exámenes --");
            foreach (var examen in examenes)
            {
                Console.WriteLine($"Nombre: {examen.Nombre}, Respuestas Correctas: {examen.RespuestasCorrectas}/{examen.CantidadPreguntas}, Nota: {examen.Nota}");
            }
        }
        Continuar();
    }
    static void ResultadosPorAlumno()
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
            Console.Clear();
            string nombre = ValidarNoVacio("Ingrese el nombre del alumno: ");
            var examenes = db.Set<ResultadoExamen>().Where(e => e.Nombre == nombre).ToList();
            Console.Clear();
            if (examenes.Count == 0)
            {
                Console.WriteLine("No se encontraron exámenes para el alumno.");
                Continuar();
                return;
            }
            Console.WriteLine($"Resultados para {nombre}:");
            foreach (var examen in examenes)
            {
                Console.WriteLine($"Respuestas Correctas: {examen.RespuestasCorrectas}/{examen.CantidadPreguntas}, Nota: {examen.Nota}");
            }
        }
        Continuar();
    }
    static void MejoresAlumnos()
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
            var resultados = db.Set<ResultadoExamen>().OrderByDescending(e => e.Nota).Take(10).ToList();
            Console.Clear();
            if (resultados.Count == 0)
            {
                Console.WriteLine("No hay resultados registrados.");
                Continuar();
                return;
            }
            Console.WriteLine("-- Mejores Alumnos --");
            var nombresUnicos = new HashSet<string>();
            foreach (var resultado in resultados)
            {
                if (nombresUnicos.Add(resultado.Nombre))
                {
                    Console.WriteLine($"Nombre: {resultado.Nombre}, Nota: {resultado.Nota}");
                }
            }
        }
        Continuar();
    }
    static void InformeEstadisticoPorPregunta()
    {
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
            var preguntas = db.Set<Pregunta>().ToList();
            Console.Clear();
            if (preguntas.Count == 0)
            {
                Console.WriteLine("No hay preguntas registradas.");
                Continuar();
                return;
            }
            Console.WriteLine("-- Informe Estadístico por Pregunta --");
            foreach (var pregunta in preguntas)
            {
                var respuestas = db.Set<RespuestaExamen>().Where(r => r.PreguntaId == pregunta.PreguntaId).ToList();
                int correctas = respuestas.Count(r => r.Correcta);
                int incorrectas = respuestas.Count - correctas;
                Console.WriteLine($"Pregunta: {pregunta.Enunciado}");
                Console.WriteLine($"Respuestas Correctas: {correctas}, Respuestas Incorrectas: {incorrectas}");
            }
        }
        Continuar();
    }
    static string ValidarNoVacio(string mensaje)
    {
        string valor;
        do
        {
            Console.Clear();
            Console.Write(mensaje);
            valor = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(valor))
            {
                Console.WriteLine("El valor no puede estar vacío.");
            }
        } while (string.IsNullOrEmpty(valor));
        return valor;
    }
    static string ValidarRespuesta(string mensaje)
    {
        string respuesta;
        do
        {
            Console.Write(mensaje);
            respuesta = Console.ReadLine()?.ToUpper() ?? "";
            if (string.IsNullOrEmpty(respuesta) || (respuesta != "A" && respuesta != "B" && respuesta != "C"))
            {
                Console.WriteLine("Respuesta no válida. Debe ser A, B o C.");
            }
        } while (string.IsNullOrEmpty(respuesta) || (respuesta != "A" && respuesta != "B" && respuesta != "C"));
        return respuesta;
    }
    static void Continuar()
    {
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}