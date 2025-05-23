using System;
using System.Collections.Generic;
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
    public List<RespuestaExamen> RespuestasExamen { get; set; } = new List<RespuestaExamen>(); // Relación con RespuestaExamen
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime FechaExamen { get; set; } = DateTime.Now;
    public List<RespuestaExamen> RespuestasExamen { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public int PreguntaId { get; set; } 
    public Pregunta Pregunta { get; set; }

    public string RespuestaAlumno { get; set; } = "";
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<ResultadoExamen>()
            .HasMany(re => re.RespuestasExamen)
            .WithOne(r => r.ResultadoExamen)
            .HasForeignKey(r => r.ResultadoExamenId);

        modelBuilder.Entity<Pregunta>()
            .HasMany(p => p.RespuestasExamen)
            .WithOne(r => r.Pregunta)
            .HasForeignKey(r => r.PreguntaId);

        base.OnModelCreating(modelBuilder);
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

            if (!db.Preguntas.Any())
            {

                db.Preguntas.AddRange(
                    new Pregunta
                    {
                        Enunciado = "¿Qué paradigma de programación se centra en la organización del código en 'objetos' que contienen datos y comportamientos?",
                        RespuestaA = "Programación Funcional",
                        RespuestaB = "Programación Orientada a Objetos",
                        RespuestaC = "Programación Estructurada",
                        Correcta = "B"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Cuál de los siguientes conceptos es fundamental en la Programación Orientada a Objetos y permite que una clase herede propiedades y métodos de otra?",
                        RespuestaA = "Polimorfismo",
                        RespuestaB = "Encapsulamiento",
                        RespuestaC = "Herencia",
                        Correcta = "C"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Qué es una 'clase' en C#?",
                        RespuestaA = "Una instancia de un objeto",
                        RespuestaB = "Un plano o plantilla para crear objetos",
                        RespuestaC = "Una variable que almacena múltiples valores",
                        Correcta = "B"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Qué es un 'objeto' en C#?",
                        RespuestaA = "Una definición de datos y comportamientos",
                        RespuestaB = "Una instancia de una clase",
                        RespuestaC = "Una palabra reservada del lenguaje",
                        Correcta = "B"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Cuál es el propósito principal del 'encapsulamiento' en POO?",
                        RespuestaA = "Ocultar la complejidad interna de un objeto y controlar el acceso a sus datos",
                        RespuestaB = "Permitir que diferentes clases respondan al mismo mensaje de diferentes maneras",
                        RespuestaC = "Crear nuevas clases basadas en clases existentes",
                        Correcta = "A"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Qué es el 'polimorfismo' en POO?",
                        RespuestaA = "La capacidad de una clase para tener múltiples constructores",
                        RespuestaB = "La capacidad de diferentes clases para responder al mismo mensaje de formas específicas",
                        RespuestaC = "La forma de organizar el código en funciones",
                        Correcta = "B"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Cuál de las siguientes palabras clave se utiliza para definir una clase en C#?",
                        RespuestaA = "struct",
                        RespuestaB = "class",
                        RespuestaC = "interface",
                        Correcta = "B"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Qué son los 'miembros' de una clase?",
                        RespuestaA = "Solo las variables dentro de la clase",
                        RespuestaB = "Las variables (campos) y las funciones (métodos) dentro de la clase",
                        RespuestaC = "Los comentarios utilizados para documentar la clase",
                        Correcta = "B"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Qué es un 'constructor' en una clase de C#?",
                        RespuestaA = "Un método especial que se ejecuta automáticamente al crear una instancia de la clase",
                        RespuestaB = "Una variable que se utiliza para inicializar objetos",
                        RespuestaC = "Una propiedad que solo se puede leer",
                        Correcta = "A"
                    },
                    new Pregunta
                    {
                        Enunciado = "¿Cuál es la diferencia principal entre una 'clase' y una 'estructura' (struct) en C#?",
                        RespuestaA = "Las clases son tipos por valor y las estructuras son tipos por referencia",
                        RespuestaB = "Las clases pueden tener herencia, mientras que las estructuras no",
                        RespuestaC = "No hay diferencia significativa entre ellas",
                        Correcta = "B"
                    }
                );
                db.SaveChanges();
                Console.WriteLine("Se han cargado 10 preguntas iniciales.");
            }
            else
            {
                Console.WriteLine("Ya existen preguntas cargadas en la base de datos.");
            }

            bool continuar = true;

            while (continuar)
            {
                Console.WriteLine("\n--- Menú Principal ---");
                Console.WriteLine("1. Tomar Examen");
                Console.WriteLine("2. Ver Reportes");
                Console.WriteLine("3. Salir");
                Console.Write("Seleccione una opción: ");
                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        TomarExamen(db);
                        break;
                    case "2":
                        MostrarMenuReportes(db);
                        break;
                    case "3":
                        continuar = false;
                        Console.WriteLine("¡Gracias por usar el Sistema de Exámenes!");
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Por favor, intente de nuevo.");
                        break;
                }
            }
        }
    }
    static void TomarExamen(DatosContexto db)
    {
        Console.WriteLine("\n¡Bienvenido al Sistema de Exámenes!");
        Console.Write("Ingrese su nombre: ");
        string nombreAlumno = Console.ReadLine() ?? "Anónimo";

        var todasLasPreguntas = db.Preguntas.ToList();
        int cantidadTotalPreguntas = todasLasPreguntas.Count;
        int numeroPreguntasExamen = Math.Min(5, cantidadTotalPreguntas);

        if (numeroPreguntasExamen == 0)
        {
            Console.WriteLine("No hay preguntas disponibles en la base de datos. El examen no puede realizarse.");
            return;
        }

        var preguntasAleatorias = todasLasPreguntas.OrderBy(q => Guid.NewGuid()).Take(numeroPreguntasExamen).ToList();
        int respuestasCorrectas = 0;
        var respuestasDelAlumno = new List<RespuestaExamen>();

        Console.WriteLine($"\n{nombreAlumno}, vas a responder {numeroPreguntasExamen} preguntas.");

        for (int i = 0; i < preguntasAleatorias.Count; i++)
        {
            var preguntaActual = preguntasAleatorias[i];
            Console.WriteLine($"\nPregunta #{i + 1}: {preguntaActual.Enunciado}");
            Console.WriteLine($"A) {preguntaActual.RespuestaA}");
            Console.WriteLine($"B) {preguntaActual.RespuestaB}");
            Console.WriteLine($"C) {preguntaActual.RespuestaC}");
            Console.Write("Tu respuesta (A, B o C): ");
            string respuestaAlumno = Console.ReadLine()?.ToUpper() ?? "";
            bool esCorrecta = respuestaAlumno == preguntaActual.Correcta.ToUpper();

            if (esCorrecta)
            {
                Console.WriteLine("¡Correcto!");
                respuestasCorrectas++;
            }
            else
            {
                Console.WriteLine($"Incorrecto. La respuesta correcta era: {preguntaActual.Correcta}");
            }

            respuestasDelAlumno.Add(new RespuestaExamen
            {
                PreguntaId = preguntaActual.PreguntaId,
                RespuestaAlumno = respuestaAlumno,
                EsCorrecta = esCorrecta
            });
        }

        double notaFinal = (double)respuestasCorrectas / numeroPreguntasExamen * 10;
        Console.WriteLine($"\n--- Resultados del Examen ---");
        Console.WriteLine($"Nombre del alumno: {nombreAlumno}");
        Console.WriteLine($"Respuestas correctas: {respuestasCorrectas} de {numeroPreguntasExamen}");
        Console.WriteLine($"Nota final: {notaFinal:F2} / 10");

        var resultadoExamen = new ResultadoExamen
        {
            NombreAlumno = nombreAlumno,
            CantidadCorrectas = respuestasCorrectas,
            TotalPreguntas = numeroPreguntasExamen,
            NotaFinal = notaFinal,
            RespuestasExamen = respuestasDelAlumno
        };

        db.ResultadosExamen.Add(resultadoExamen);
        db.SaveChanges();

        Console.WriteLine("\nResultados del examen guardados en la base de datos.");
    }

    static void MostrarMenuReportes(DatosContexto db)
    {
        bool volverAlMenuPrincipal = false;
        while (!volverAlMenuPrincipal)
        {
            Console.WriteLine("\n--- Menú de Reportes ---");
            Console.WriteLine("1. Listado de todos los exámenes rendidos");
            Console.WriteLine("2. Filtrar resultados por nombre de alumno");
            Console.WriteLine("3. Ranking de los mejores alumnos");
            Console.WriteLine("4. Informe estadístico por pregunta");
            Console.WriteLine("5. Volver al menú principal");
            Console.Write("Seleccione una opción: ");
            string opcionReporte = Console.ReadLine() ?? "";

            switch (opcionReporte)
            {
                case "1":
                    ListarTodosLosExamenes(db);
                    break;
                case "2":
                    FiltrarResultadosPorAlumno(db);
                    break;
                case "3":
                    MostrarRankingAlumnos(db);
                    break;
                case "4":
                    MostrarEstadisticasPorPregunta(db);
                    break;
                case "5":
                    volverAlMenuPrincipal = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida. Por favor, intente de nuevo.");
                    break;
            }
        }
    }

    static void ListarTodosLosExamenes(DatosContexto db)
    {
        Console.WriteLine("\n--- Listado de Todos los Exámenes Rendidos ---");
        var resultados = db.ResultadosExamen.OrderByDescending(r => r.FechaExamen).ToList();

        if (resultados.Any())
        {
            foreach (var resultado in resultados)
            {
                Console.WriteLine($"""
                    ID del Examen: {resultado.ResultadoExamenId}
                    Alumno: {resultado.NombreAlumno}
                    Fecha: {resultado.FechaExamen:dd/MM/yyyy HH:mm:ss}
                    Correctas: {resultado.CantidadCorrectas} de {resultado.TotalPreguntas}
                    Nota Final: {resultado.NotaFinal:F2} / 10
                    ------------------------------------
                """);
            }
        }
        else
        {
            Console.WriteLine("No se encontraron exámenes rendidos.");
        }
    }

    static void FiltrarResultadosPorAlumno(DatosContexto db)
    {
        Console.Write("\nIngrese el nombre del alumno a buscar: ");
        string nombreBuscar = Console.ReadLine() ?? "";

        if (!string.IsNullOrWhiteSpace(nombreBuscar))
        {
            Console.WriteLine($"\n--- Resultados de Exámenes para '{nombreBuscar}' ---");
            var resultados = db.ResultadosExamen
                .Where(r => r.NombreAlumno.ToLower().Contains(nombreBuscar.ToLower()))
                .OrderByDescending(r => r.FechaExamen)
                .ToList();

            if (resultados.Any())
            {
                foreach (var resultado in resultados)
                {
                    Console.WriteLine($"""
                        ID del Examen: {resultado.ResultadoExamenId}
                        Fecha: {resultado.FechaExamen:dd/MM/yyyy HH:mm:ss}
                        Correctas: {resultado.CantidadCorrectas} de {resultado.TotalPreguntas}
                        Nota Final: {resultado.NotaFinal:F2} / 10
                        ------------------------------------
                    """);
                }
            }
            else
            {
                Console.WriteLine($"No se encontraron exámenes para el alumno '{nombreBuscar}'.");
            }
        }
        else
        {
            Console.WriteLine("El nombre del alumno no puede estar vacío.");
        }
    }

    static void MostrarRankingAlumnos(DatosContexto db)
    {
        Console.WriteLine("\n--- Ranking de los Mejores Alumnos (Mejor Nota) ---");
        var ranking = db.ResultadosExamen
            .GroupBy(r => r.NombreAlumno)
            .Select(g => new
            {
                NombreAlumno = g.Key,
                MejorNota = g.Max(r => r.NotaFinal)
            })
            .OrderByDescending(r => r.MejorNota)
            .ToList();

        if (ranking.Any())
        {
            int puesto = 1;
            foreach (var alumno in ranking)
            {
                Console.WriteLine($"{puesto}. {alumno.NombreAlumno} - Mejor Nota: {alumno.MejorNota:F2} / 10");
                puesto++;
            }
        }
        else
        {
            Console.WriteLine("No hay resultados de exámenes para generar un ranking.");
        }
    }

    static void MostrarEstadisticasPorPregunta(DatosContexto db)
    {
        Console.WriteLine("\n--- Informe Estadístico por Pregunta ---");
        var estadisticas = db.RespuestasExamen
            .GroupBy(r => r.PreguntaId)
            .Select(g => new
            {
                PreguntaId = g.Key,
                CantidadRespondida = g.Count(),
                CantidadCorrectas = g.Count(r => r.EsCorrecta)
            })
            .OrderBy(e => e.PreguntaId)
            .ToList();

        if (estadisticas.Any())
        {
            foreach (var estadistica in estadisticas)
            {
                var pregunta = db.Preguntas.Find(estadistica.PreguntaId);
                if (pregunta != null)
                {
                    double porcentajeCorrectas = estadistica.CantidadRespondida > 0 ? (double)estadistica.CantidadCorrectas / estadistica.CantidadRespondida * 100 : 0;
                    Console.WriteLine($"""
                        Pregunta #{estadistica.PreguntaId:000}: {pregunta.Enunciado}
                        Respondida: {estadistica.CantidadRespondida} veces
                        Correctas: {estadistica.CantidadCorrectas} veces ({porcentajeCorrectas:F2}%)
                        ------------------------------------
                    """);
                }
            }
        }
        else
        {
            Console.WriteLine("No hay respuestas de exámenes para generar estadísticas por pregunta.");
        }
    }
}


