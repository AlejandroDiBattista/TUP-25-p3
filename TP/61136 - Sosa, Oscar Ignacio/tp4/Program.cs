using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

// Modelos
public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; }
    public string OpcionA { get; set; }
    public string OpcionB { get; set; }
    public string OpcionC { get; set; }
    public string RespuestaCorrecta { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class ResultadoExamen
{
    public int Id { get; set; }
    public string NombreAlumno { get; set; }
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class RespuestaExamen
{
    public int Id { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public string RespuestaDada { get; set; }
    public bool EsCorrecta { get; set; }
}

// DbContext
public class ExamenDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=examenes.db");
}

// Programa principal
class Program
{
    static ExamenDbContext context = new ExamenDbContext();

    static void Main()
    {
        context.Database.Migrate();
        CargarPreguntasIniciales();

        while (true)
        {
            Console.WriteLine("\nMenú:");
            Console.WriteLine("1. Registrar Pregunta");
            Console.WriteLine("2. Tomar Examen");
            Console.WriteLine("3. Ver Reportes");
            Console.WriteLine("0. Salir");

            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();
            if (opcion == "1") RegistrarPregunta();
            else if (opcion == "2") TomarExamen();
            else if (opcion == "3") MostrarReportes();
            else if (opcion == "0") break;
        }
    }

    static void CargarPreguntasIniciales()
    {
        if (context.Preguntas.Any()) return;

        var preguntas = new List<Pregunta>
        {
            new() { Enunciado = "¿Qué tipo de dato se usa para representar números enteros en C#?", OpcionA = "string", OpcionB = "int", OpcionC = "bool", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Cuál es el operador de igualdad en C#?", OpcionA = "==", OpcionB = "=", OpcionC = "!=", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué palabra clave se usa para declarar una clase en C#?", OpcionA = "function", OpcionB = "define", OpcionC = "class", RespuestaCorrecta = "C" },
            new() { Enunciado = "¿Cuál es el resultado de 3 + 2 * 2 en C#?", OpcionA = "10", OpcionB = "7", OpcionC = "12", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Qué estructura se usa para tomar decisiones en C#?", OpcionA = "while", OpcionB = "if", OpcionC = "for", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Qué palabra se usa para heredar una clase en C#?", OpcionA = "inherits", OpcionB = "base", OpcionC = ":", RespuestaCorrecta = "C" },
            new() { Enunciado = "¿Qué valor devuelve un método `void`?", OpcionA = "null", OpcionB = "ninguno", OpcionC = "0", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Qué bucle se ejecuta al menos una vez siempre?", OpcionA = "for", OpcionB = "while", OpcionC = "do-while", RespuestaCorrecta = "C" },
            new() { Enunciado = "¿Qué palabra clave se usa para definir una interfaz?", OpcionA = "interface", OpcionB = "abstract", OpcionC = "implements", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué hace el operador `&&` en C#?", OpcionA = "OR lógico", OpcionB = "AND lógico", OpcionC = "Negación", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Cuál es el tipo base de todas las clases en C#?", OpcionA = "System", OpcionB = "Object", OpcionC = "Base", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Qué es una excepción en programación?", OpcionA = "Un tipo de variable", OpcionB = "Un error en tiempo de compilación", OpcionC = "Un error en tiempo de ejecución", RespuestaCorrecta = "C" },
            new() { Enunciado = "¿Qué palabra clave atrapa excepciones en C#?", OpcionA = "catch", OpcionB = "handle", OpcionC = "error", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué estructura permite iterar una colección?", OpcionA = "foreach", OpcionB = "if", OpcionC = "switch", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Cuál es el tipo para números con decimales en C#?", OpcionA = "float", OpcionB = "int", OpcionC = "char", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué representa una clase estática en C#?", OpcionA = "Clase que no se puede instanciar", OpcionB = "Clase heredable", OpcionC = "Clase abstracta", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué palabra se usa para definir una constante?", OpcionA = "const", OpcionB = "let", OpcionC = "readonly", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué palabra clave evita herencia en C#?", OpcionA = "sealed", OpcionB = "private", OpcionC = "static", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué palabra se usa para implementar interfaces?", OpcionA = "override", OpcionB = "interface", OpcionC = "implements", RespuestaCorrecta = "C" },
            new() { Enunciado = "¿Cuál es la extensión de un archivo de código fuente en C#?", OpcionA = ".cs", OpcionB = ".cpp", OpcionC = ".java", RespuestaCorrecta = "A" },
            new() { Enunciado = "¿Qué palabra clave se usa para métodos que pueden redefinirse?", OpcionA = "override", OpcionB = "virtual", OpcionC = "abstract", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Qué operador incrementa una variable?", OpcionA = "--", OpcionB = "++", OpcionC = "**", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Qué comando compila un proyecto .NET?", OpcionA = "dotnet compile", OpcionB = "dotnet run", OpcionC = "dotnet build", RespuestaCorrecta = "C" },
            new() { Enunciado = "¿Qué palabra se usa para declarar una variable local?", OpcionA = "dim", OpcionB = "var", OpcionC = "new", RespuestaCorrecta = "B" },
            new() { Enunciado = "¿Qué estructura permite elegir entre múltiples opciones?", OpcionA = "if", OpcionB = "switch", OpcionC = "for", RespuestaCorrecta = "B" },
        };

        context.Preguntas.AddRange(preguntas);
        context.SaveChanges();
        Console.WriteLine("Preguntas cargadas automáticamente.");
    }

    static void RegistrarPregunta()
    {
        Console.Write("Enunciado: ");
        var enunciado = Console.ReadLine();
        Console.Write("Opción A: ");
        var a = Console.ReadLine();
        Console.Write("Opción B: ");
        var b = Console.ReadLine();
        Console.Write("Opción C: ");
        var c = Console.ReadLine();
        Console.Write("Respuesta correcta (A/B/C): ");
        var correcta = Console.ReadLine().ToUpper();

        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            OpcionA = a,
            OpcionB = b,
            OpcionC = c,
            RespuestaCorrecta = correcta
        };

        context.Preguntas.Add(pregunta);
        context.SaveChanges();
        Console.WriteLine("Pregunta registrada con éxito.");
    }

    static void TomarExamen()
    {
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine();

        var preguntas = context.Preguntas.OrderBy(p => Guid.NewGuid()).Take(5).ToList();
        if (preguntas.Count == 0)
        {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }

        var respuestas = new List<RespuestaExamen>();
        int correctas = 0;

        foreach (var p in preguntas)
        {
            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"A) {p.OpcionA}");
            Console.WriteLine($"B) {p.OpcionB}");
            Console.WriteLine($"C) {p.OpcionC}");
            Console.Write("Respuesta: ");
            var resp = Console.ReadLine().ToUpper();
            bool esCorrecta = resp == p.RespuestaCorrecta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = p.Id,
                RespuestaDada = resp,
                EsCorrecta = esCorrecta
            });
        }

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            CantidadCorrectas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = correctas,
            Respuestas = respuestas
        };

        context.Resultados.Add(resultado);
        context.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Nota: {resultado.NotaFinal}/5");
    }

    static void MostrarReportes()
    {
        Console.WriteLine("\n1. Listado de Exámenes");
        Console.WriteLine("2. Buscar por Alumno");
        Console.WriteLine("3. Ranking de Mejores Alumnos");
        Console.WriteLine("4. Estadística por Pregunta");

        Console.Write("Seleccione una opción: ");
        var opcion = Console.ReadLine();

        if (opcion == "1")
        {
            var examenes = context.Resultados.ToList();
            foreach (var e in examenes)
                Console.WriteLine($"{e.NombreAlumno} - Nota: {e.NotaFinal}/5 - Fecha: {e.Fecha}");
        }
        else if (opcion == "2")
        {
            Console.Write("Nombre del alumno: ");
            var nombre = Console.ReadLine();
            var examenes = context.Resultados.Where(r => r.NombreAlumno == nombre).ToList();
            foreach (var e in examenes)
                Console.WriteLine($"{e.NombreAlumno} - Nota: {e.NotaFinal}/5 - Fecha: {e.Fecha}");
        }
        else if (opcion == "3")
        {
            var ranking = context.Resultados
                .GroupBy(r => r.NombreAlumno)
                .Select(g => new { Alumno = g.Key, MejorNota = g.Max(r => r.NotaFinal) })
                .OrderByDescending(r => r.MejorNota)
                .ToList();

            foreach (var item in ranking)
                Console.WriteLine($"{item.Alumno}: {item.MejorNota}/5");
        }
        else if (opcion == "4")
        {
            var estadisticas = context.Preguntas
                .Select(p => new
                {
                    p.Enunciado,
                    Total = p.Respuestas.Count,
                    Correctas = p.Respuestas.Count(r => r.EsCorrecta)
                })
                .ToList();

            foreach (var p in estadisticas)
            {
                double porcentaje = p.Total > 0 ? (p.Correctas * 100.0 / p.Total) : 0;
                Console.WriteLine($"\n{p.Enunciado}\n - Respondida: {p.Total} veces\n - % Correctas: {porcentaje:F2}%");
            }
        }
    }
}
