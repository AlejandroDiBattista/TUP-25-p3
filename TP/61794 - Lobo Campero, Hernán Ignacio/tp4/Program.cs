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
}

class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
}

class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
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
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Inicializar preguntas
            InicializarPreguntas(db);

            // Menú principal
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Sistema de Exámenes Multiple Choice");
                Console.WriteLine("1. Tomar examen");
                Console.WriteLine("2. Ver resultados");
                Console.WriteLine("3. Salir");
                Console.Write("Seleccione una opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        TomarExamen(db);
                        break;
                    case "2":
                        VerResultados(db);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    static void InicializarPreguntas(DatosContexto db)
    {
        if (!db.Preguntas.Any())
        {
            // Generador de números aleatorios
            var random = new Random();

            // Lista de preguntas seleccionadas
            var preguntas = new List<(string Enunciado, string RespuestaCorrecta, string RespuestaIncorrecta1, string RespuestaIncorrecta2)>
            {
                ("¿Qué es un algoritmo?", "Un conjunto de instrucciones para resolver un problema", "Un lenguaje de programación", "Un tipo de hardware"),
                ("¿Qué significa HTML?", "HyperText Markup Language", "HighText Machine Language", "Hyperlink and Text Markup Language"),
                ("¿Qué es un framework?", "Un entorno de trabajo que facilita el desarrollo de software", "Un lenguaje de programación", "Un sistema operativo"),
                ("¿Qué es una base de datos relacional?", "Un sistema para almacenar datos en tablas relacionadas", "Un sistema para almacenar datos en archivos planos", "Un sistema para almacenar datos en la nube"),
                ("¿Qué es un compilador?", "Un programa que traduce código fuente a código máquina", "Un editor de texto", "Un sistema operativo"),
                ("¿Qué es un sistema operativo?", "Un software que gestiona los recursos del hardware", "Un lenguaje de programación", "Un compilador"),
                ("¿Qué es la programación orientada a objetos?", "Un paradigma de programación basado en objetos", "Un lenguaje de programación", "Un sistema operativo"),
                ("¿Qué es una variable?", "Un espacio en memoria para almacenar datos", "Un tipo de dato", "Un operador matemático"),
                ("¿Qué es un bucle?", "Una estructura que permite repetir código", "Un tipo de dato", "Un operador lógico"),
                ("¿Qué es una función?", "Un bloque de código reutilizable", "Un tipo de dato", "Un operador matemático")
            };

            // Lista para almacenar las preguntas con respuestas aleatorias
            var preguntasFinales = new List<Pregunta>();

            foreach (var (enunciado, correcta, incorrecta1, incorrecta2) in preguntas)
            {
                // Generar un orden aleatorio para las respuestas
                var respuestas = new List<(string Texto, string Letra)>
                {
                    (correcta, "A"),
                    (incorrecta1, "B"),
                    (incorrecta2, "C")
                }.OrderBy(_ => random.Next()).ToList();

                // Crear la pregunta con las respuestas en orden aleatorio
                preguntasFinales.Add(new Pregunta
                {
                    Enunciado = enunciado,
                    RespuestaA = respuestas[0].Texto,
                    RespuestaB = respuestas[1].Texto,
                    RespuestaC = respuestas[2].Texto,
                    Correcta = respuestas.First(r => r.Texto == correcta).Letra // Identificar la letra correcta
                });
            }

            // Agregar preguntas a la base de datos
            db.Preguntas.AddRange(preguntasFinales);
            db.SaveChanges();
        }
    }

    static void TomarExamen(DatosContexto db)
    {
        Console.Clear();
        Console.Write("Ingrese su nombre: ");
        var alumno = Console.ReadLine();

        // Seleccionar 10 preguntas aleatorias
        var preguntas = db.Preguntas
            .AsEnumerable() // Evalúa en memoria
            .OrderBy(p => Guid.NewGuid()) // Orden aleatorio
            .Take(10)
            .ToList();

        int correctas = 0;

        // Crear un nuevo resultado de examen
        var resultado = new ResultadoExamen
        {
            Alumno = alumno,
            RespuestasCorrectas = 0, // Se actualizará después
            TotalPreguntas = preguntas.Count,
            NotaFinal = 0 // Se actualizará después
        };

        db.ResultadosExamen.Add(resultado);
        db.SaveChanges(); // Guardar para obtener el ResultadoExamenId

        foreach (var pregunta in preguntas)
        {
            Console.Clear();
            Console.WriteLine(pregunta.Enunciado);
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");
            Console.Write("Seleccione una opción (A, B, C): ");
            var respuesta = Console.ReadLine()?.ToUpper();

            bool esCorrecta = respuesta == pregunta.Correcta;
            if (esCorrecta) correctas++;

            // Agregar la respuesta asociada al resultado
            db.RespuestasExamen.Add(new RespuestaExamen
            {
                ResultadoExamenId = resultado.ResultadoExamenId, // Asociar al resultado
                PreguntaId = pregunta.PreguntaId,
                EsCorrecta = esCorrecta
            });
        }

        // Actualizar el resultado del examen
        resultado.RespuestasCorrectas = correctas;
        resultado.NotaFinal = (correctas / (double)preguntas.Count) * 10;

        db.SaveChanges(); // Guardar los cambios finales

        Console.WriteLine($"\nExamen finalizado. Respuestas correctas: {correctas}/{preguntas.Count}. Nota final: {resultado.NotaFinal:F2}");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void VerResultados(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("Resultados de Exámenes:");
        var resultados = db.ResultadosExamen
            .OrderByDescending(r => r.NotaFinal)
            .ToList();

        foreach (var resultado in resultados)
        {
            Console.WriteLine($"Alumno: {resultado.Alumno}, Nota: {resultado.NotaFinal:F2}, Correctas: {resultado.RespuestasCorrectas}/{resultado.TotalPreguntas}");
        }

        Console.WriteLine("\n1. Filtrar por nombre de alumno");
        Console.WriteLine("2. Mostrar ranking de mejores alumnos");
        Console.WriteLine("3. Informe estadístico por pregunta");
        Console.WriteLine("4. Volver al menú principal");
        Console.Write("Seleccione una opción: ");
        var opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                FiltrarPorAlumno(db);
                break;
            case "2":
                MostrarRanking(db);
                break;
            case "3":
                InformeEstadistico(db);
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                Console.ReadKey();
                break;
        }
    }

    static void FiltrarPorAlumno(DatosContexto db)
    {
        Console.Clear();
        Console.Write("Ingrese el nombre del alumno: ");
        var nombre = Console.ReadLine();

        var resultados = db.ResultadosExamen
            .Where(r => r.Alumno.Contains(nombre))
            .OrderByDescending(r => r.NotaFinal)
            .ToList();

        if (resultados.Any())
        {
            Console.WriteLine($"Resultados para el alumno: {nombre}");
            foreach (var resultado in resultados)
            {
                Console.WriteLine($"Nota: {resultado.NotaFinal:F2}, Correctas: {resultado.RespuestasCorrectas}/{resultado.TotalPreguntas}");
            }
        }
        else
        {
            Console.WriteLine("No se encontraron resultados para el alumno especificado.");
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarRanking(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("Ranking de los mejores alumnos:");

        var ranking = db.ResultadosExamen
            .GroupBy(r => r.Alumno)
            .Select(g => new
            {
                Alumno = g.Key,
                MejorNota = g.Max(r => r.NotaFinal)
            })
            .OrderByDescending(r => r.MejorNota)
            .Take(10) // Mostrar los 10 mejores
            .ToList();

        foreach (var entrada in ranking)
        {
            Console.WriteLine($"Alumno: {entrada.Alumno}, Mejor Nota: {entrada.MejorNota:F2}");
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void InformeEstadistico(DatosContexto db)
    {
        Console.Clear();
        Console.WriteLine("Informe estadístico por pregunta:");

        var estadisticas = db.Preguntas
            .Select(p => new
            {
                Pregunta = p.Enunciado,
                TotalRespuestas = db.RespuestasExamen.Count(r => r.PreguntaId == p.PreguntaId),
                RespuestasCorrectas = db.RespuestasExamen.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta)
            })
            .ToList();

        foreach (var estadistica in estadisticas)
        {
            var porcentajeCorrectas = estadistica.TotalRespuestas > 0
                ? (estadistica.RespuestasCorrectas / (double)estadistica.TotalRespuestas) * 100
                : 0;

            Console.WriteLine($"Pregunta: {estadistica.Pregunta}");
            Console.WriteLine($"Total de respuestas: {estadistica.TotalRespuestas}");
            Console.WriteLine($"Respuestas correctas: {estadistica.RespuestasCorrectas} ({porcentajeCorrectas:F2}%)");
            Console.WriteLine();
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }
}