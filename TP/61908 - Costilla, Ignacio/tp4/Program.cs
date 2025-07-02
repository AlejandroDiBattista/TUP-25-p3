using System;
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
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ExamenId { get; set; }  // ID de examen, aunque por ahora lo dejamos como 0 si no usamos exámenes específicos.
    public int PreguntaId { get; set; }
    public string RespuestaDada { get; set; } = ""; // Aseguramos que siempre se tenga un valor, puede ser "A", "B" o "C"
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

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
            db.Database.EnsureCreated();  // Asegura que la base de datos se haya creado

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Registrar pregunta");
                Console.WriteLine("2. Tomar examen");
                Console.WriteLine("3. Ver resultados");
                Console.WriteLine("4. Ver reportes");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");
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
                        VerResultados(db);
                        break;
                    case "4":
                        MostrarReportes(db);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db)
    {
        Console.Clear();
        Console.Write("Enunciado de la pregunta: ");
        string enunciado = Console.ReadLine();
        Console.Write("Respuesta A: ");
        string respuestaA = Console.ReadLine();
        Console.Write("Respuesta B: ");
        string respuestaB = Console.ReadLine();
        Console.Write("Respuesta C: ");
        string respuestaC = Console.ReadLine();
        Console.Write("Respuesta correcta (A/B/C): ");
        string correcta = Console.ReadLine();

        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta.ToUpper()
        };

        db.Preguntas.Add(pregunta);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada con éxito.");
        Console.ReadKey();
    }

    static void TomarExamen(DatosContexto db)
    {
        // Solicitar el nombre del alumno
        Console.Clear();
        Console.Write("Ingrese su nombre: ");
        string nombreAlumno = Console.ReadLine();

       
        var preguntasExamen = db.Preguntas
            .OrderBy(p => EF.Functions.Random())  // Esto hace que las preguntas sean aleatorias
            .Take(10)  
            .ToList();  

        
        if (preguntasExamen.Count < 10)
        {
            preguntasExamen = db.Preguntas.ToList();  // Trae todas las preguntas si son menos de 10
        }

        // Mostrar las preguntas y permitir al alumno responder
        int respuestasCorrectas = 0;
        var respuestasExamen = new System.Collections.Generic.List<RespuestaExamen>();

        foreach (var pregunta in preguntasExamen)
        {
            Console.Clear();
            Console.WriteLine($"Pregunta: {pregunta.Enunciado}");
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");

            Console.Write("Seleccione una respuesta (A, B, C): ");
            string respuestaAlumno = Console.ReadLine().ToUpper();

            // Asignar la respuesta dada y verificar si es correcta
            bool esCorrecta = respuestaAlumno == pregunta.Correcta;
            if (esCorrecta)
            {
                respuestasCorrectas++;
            }

            
            respuestasExamen.Add(new RespuestaExamen
            {
                ExamenId = 0,  
                PreguntaId = pregunta.PreguntaId,
                RespuestaDada = respuestaAlumno,  
                EsCorrecta = esCorrecta  
            });
        }

        
        double notaFinal = (double)respuestasCorrectas / preguntasExamen.Count * 10;
        Console.WriteLine($"Examen completado. Su nota es: {notaFinal}/10");

       
        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombreAlumno,
            RespuestasCorrectas = respuestasCorrectas,
            TotalPreguntas = preguntasExamen.Count,
            NotaFinal = notaFinal
        };

        db.ResultadosExamen.Add(resultado);
        db.SaveChanges();

        // Registrar las respuestas del examen
        foreach (var respuesta in respuestasExamen)
        {
            db.RespuestasExamen.Add(respuesta);
        }
        db.SaveChanges();
    }

    static void VerResultados(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("1. Ver todos los resultados");
        Console.WriteLine("2. Ver resultados por alumno");
        Console.Write("Seleccione una opción: ");
        string opcion = Console.ReadLine();

        if (opcion == "1")
        {
            var resultados = db.ResultadosExamen.ToList();
            foreach (var resultado in resultados)
            {
                Console.WriteLine($"Alumno: {resultado.NombreAlumno}, Nota: {resultado.NotaFinal}/10");
            }
        }
        else if (opcion == "2")
        {
            Console.Write("Ingrese el nombre del alumno: ");
            string nombreAlumno = Console.ReadLine();
            var resultados = db.ResultadosExamen.Where(r => r.NombreAlumno.Contains(nombreAlumno)).ToList();
            foreach (var resultado in resultados)
            {
                Console.WriteLine($"Alumno: {resultado.NombreAlumno}, Nota: {resultado.NotaFinal}/10");
            }
        }
        else
        {
            Console.WriteLine("Opción no válida.");
        }
        Console.ReadKey();
    }

    static void MostrarReportes(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("1. Ver ranking de los mejores alumnos");
        Console.WriteLine("2. Ver informe estadístico por pregunta");
        Console.Write("Seleccione una opción: ");
        string opcion = Console.ReadLine();

        if (opcion == "1")
        {
            
            var ranking = db.ResultadosExamen
                .OrderByDescending(r => r.NotaFinal)
                .ToList();

            Console.WriteLine("Ranking de los mejores alumnos:");
            foreach (var alumno in ranking)
            {
                Console.WriteLine($"Alumno: {alumno.NombreAlumno}, Nota: {alumno.NotaFinal}/10");
            }
        }
        else if (opcion == "2")
        {
           
            var estadisticasPorPregunta = db.Preguntas.Select(p => new
            {
                PreguntaId = p.PreguntaId,
                Enunciado = p.Enunciado,
                VecesRespondida = db.RespuestasExamen.Count(r => r.PreguntaId == p.PreguntaId),
                RespuestasCorrectas = db.RespuestasExamen.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta),
            }).ToList();

            Console.WriteLine("Informe estadístico por pregunta:");
            foreach (var estadistica in estadisticasPorPregunta)
            {
                double porcentajeCorrectas = estadistica.VecesRespondida == 0 ? 0 : (double)estadistica.RespuestasCorrectas / estadistica.VecesRespondida * 100;
                Console.WriteLine($"Pregunta: {estadistica.Enunciado}");
                Console.WriteLine($"Veces respondida: {estadistica.VecesRespondida}");
                Console.WriteLine($"Porcentaje de respuestas correctas: {porcentajeCorrectas:F2}%");
            }
        }
        else
        {
            Console.WriteLine("Opción no válida.");
        }
        Console.ReadKey();
    }
}