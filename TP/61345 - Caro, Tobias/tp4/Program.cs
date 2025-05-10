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
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int CantCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double Nota { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public bool Correcta { get; set; }
}

class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program
{
    static void Main(string[] args)
    {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        // ¡Llamada a la función para cargar las preguntas!
        if (!db.Preguntas.Any()) // Verifica si ya existen preguntas para no duplicarlas
        {
            CargarPreguntas(db);
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("--- MENÚ PRINCIPAL ---");
            Console.WriteLine("1. Rendir examen");
            Console.WriteLine("2. Ver todos los exámenes rendidos");
            Console.WriteLine("3. Buscar resultados por nombre de alumno");
            Console.WriteLine("4. Mostrar ranking de mejores alumnos");
            Console.WriteLine("5. Informe estadístico por pregunta");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");

            var opcion = Console.ReadLine();
            switch (opcion)
            {
                case "1":
                    TomarExamen(db);
                    break;
                case "2":
                    MostrarTodosLosExamenes(db);
                    break;
                case "3":
                    FiltrarPorAlumno(db);
                    break;
                case "4":
                    MostrarRanking(db);
                    break;
                case "5":
                    InformeEstadistico(db);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione Enter para continuar...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void CargarPreguntas(DatosContexto db)
    {
        var preguntas = new List<Pregunta> {
            new Pregunta { Enunciado = "¿Qué espacio de nombres proporciona clases base y tipos de datos fundamentales en C#?", RespuestaA = "System.Collections", RespuestaB = "System.Linq", RespuestaC = "System", Correcta = "C" },
            new Pregunta { Enunciado = "¿Cuál de las siguientes palabras clave se utiliza para declarar un bloque de código que puede generar excepciones en C#?", RespuestaA = "catch", RespuestaB = "finally", RespuestaC = "try", Correcta = "C" },
            new Pregunta { Enunciado = "¿Qué clase en System se utiliza comúnmente para realizar operaciones de entrada y salida en la consola?", RespuestaA = "File", RespuestaB = "Console", RespuestaC = "Stream", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué método de la clase Console se utiliza para leer una línea de texto desde la entrada estándar?", RespuestaA = "WriteLine()", RespuestaB = "ReadLine()", RespuestaC = "ReadKey()", Correcta = "B" },
            new Pregunta { Enunciado = "¿Cuál de los siguientes tipos de datos primitivos no es directamente parte del espacio de nombres System en C#?", RespuestaA = "int", RespuestaB = "string", RespuestaC = "DateTime", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué estructura en System representa un valor de fecha y hora?", RespuestaA = "TimeSpan", RespuestaB = "DateTime", RespuestaC = "DateOnly", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué método de la clase String en System se utiliza para comparar dos cadenas de texto, ignorando las diferencias de mayúsculas y minúsculas?", RespuestaA = "Equals()", RespuestaB = "Compare()", RespuestaC = "Equals(string, StringComparison.OrdinalIgnoreCase)", Correcta = "C" },
            new Pregunta { Enunciado = "¿Qué clase en System proporciona métodos para generar números aleatorios?", RespuestaA = "Math", RespuestaB = "Random", RespuestaC = "Guid", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué tipo en System representa un valor booleano (verdadero o falso)?", RespuestaA = "Bool", RespuestaB = "Boolean", RespuestaC = "bool", Correcta = "C" },
            new Pregunta { Enunciado = "¿Qué método de la clase Convert en System se utiliza para convertir un valor a un entero de 32 bits?", RespuestaA = "ToInt()", RespuestaB = "ToInt32()", RespuestaC = "Convert.Int32()", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué estructura en System se utiliza para representar un intervalo de tiempo?", RespuestaA = "DateTime", RespuestaB = "TimeSpan", RespuestaC = "Interval", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué método de la clase Console se utiliza para escribir información en la salida estándar, seguido de un terminador de línea?", RespuestaA = "Write()", RespuestaB = "WriteLine()", RespuestaC = "Print()", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué interfaz en System.Collections representa una colección de objetos a los que se puede acceder individualmente por índice?", RespuestaA = "IEnumerable", RespuestaB = "IList", RespuestaC = "ICollection", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué clase en System proporciona métodos estáticos para operaciones matemáticas?", RespuestaA = "Math", RespuestaB = "Numeric", RespuestaC = "Arithmetic", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué tipo en System representa una secuencia inmutable de caracteres Unicode?", RespuestaA = "Char[]", RespuestaB = "string", RespuestaC = "StringBuilder", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué método de la clase Environment en System se utiliza para obtener variables de entorno del sistema operativo?", RespuestaA = "GetCommandLineArgs()", RespuestaB = "GetEnvironmentVariables()", RespuestaC = "GetFolderPath()", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué estructura en System representa un identificador único global (GUID)?", RespuestaA = "UUID", RespuestaB = "Guid", RespuestaC = "UniqueId", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué método de la clase String en System se utiliza para dividir una cadena en subcadenas basadas en un delimitador especificado?", RespuestaA = "Substring()", RespuestaB = "Split()", RespuestaC = "Join()", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué atributo en System se puede utilizar para marcar un método como el punto de entrada de una aplicación?", RespuestaA = "[EntryPoint]", RespuestaB = "[Main]", RespuestaC = "[STAThread]", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué clase abstracta en System es la clase base para todos los tipos de valor?", RespuestaA = "Object", RespuestaB = "ValueType", RespuestaC = "DataType", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué método de la clase Array en System se utiliza para ordenar los elementos de una matriz?", RespuestaA = "Order()", RespuestaB = "Sort()", RespuestaC = "Arrange()", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué interfaz en System representa una colección de pares clave/valor?", RespuestaA = "IList", RespuestaB = "IDictionary", RespuestaC = "ISet", Correcta = "B" }
        };

        db.Preguntas.AddRange(preguntas);
        db.SaveChanges();
        Console.WriteLine("Preguntas cargadas exitosamente.");
    }


    static void TomarExamen(DatosContexto db)
    {
        Console.Clear();
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine() ?? "";

        var preguntas = db.Preguntas.AsEnumerable().OrderBy(p => Guid.NewGuid()).Take(5).ToList();

        if (preguntas.Count == 0)
        {
            Console.WriteLine("No hay preguntas cargadas en la base de datos. Presione Enter para continuar...");
            Console.ReadLine();
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();



        foreach (var p in preguntas)
        {
            Console.Clear();
            Console.WriteLine($"{p.Enunciado}\nA) {p.RespuestaA}\nB) {p.RespuestaB}\nC) {p.RespuestaC}");
            Console.Write("Respuesta (A, B, C): ");
            var resp = Console.ReadLine()?.ToUpper();
            bool esCorrecta = resp == p.Correcta.ToUpper();
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = p.PreguntaId,
                Correcta = esCorrecta
            });
        }

        double nota = correctas * 10.0 / preguntas.Count;
        var resultado = new ResultadoExamen
        {
            Alumno = nombre,
            CantCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            Nota = nota,
            Respuestas = respuestas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Nota: {nota:F1} (Correctas: {correctas})\nPresione Enter para continuar...");
        Console.ReadLine();
    }

    static void MostrarTodosLosExamenes(DatosContexto db)
    {
        Console.Clear();
        var resultados = db.Resultados.ToList();
        foreach (var r in resultados)
        {
            Console.WriteLine($"Alumno: {r.Alumno} - Nota: {r.Nota:F1} ({r.CantCorrectas}/{r.TotalPreguntas})");
        }
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }

    static void FiltrarPorAlumno(DatosContexto db)
    {
        Console.Clear();
        Console.Write("Nombre del alumno a buscar: ");
        var nombre = Console.ReadLine() ?? "";
        var resultados = db.Resultados.Where(r => r.Alumno.Contains(nombre)).ToList();

        foreach (var r in resultados)
        {
            Console.WriteLine($"Alumno: {r.Alumno} - Nota: {r.Nota:F1} ({r.CantCorrectas}/{r.TotalPreguntas})");
        }
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }

    static void MostrarRanking(DatosContexto db)
    {
        Console.Clear();
        var ranking = db.Resultados
            .GroupBy(r => r.Alumno)
            .Select(g => new { Alumno = g.Key, MejorNota = g.Max(r => r.Nota) })
            .OrderByDescending(x => x.MejorNota)
            .ToList();

        int pos = 1;
        foreach (var r in ranking)
        {
            Console.WriteLine($"{pos++}. {r.Alumno} - Mejor Nota: {r.MejorNota:F1}");
        }
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }

    static void InformeEstadistico(DatosContexto db)
    {
        Console.Clear();
        var preguntas = db.Preguntas.Include(p => p.Respuestas).ToList();

        foreach (var p in preguntas)
        {
            int total = p.Respuestas.Count;
            int correctas = p.Respuestas.Count(r => r.Correcta);
            double porcentaje = total > 0 ? correctas * 100.0 / total : 0;
            Console.WriteLine($"Pregunta #{p.PreguntaId:000}: {p.Enunciado}");
            Console.WriteLine($"  Respondida: {total} veces, Correctas: {porcentaje:F1}%\n");
        }

        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }
}