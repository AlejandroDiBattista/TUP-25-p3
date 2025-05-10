using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

// Modelos
public class Pregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
    public List<RespuestaExamen> RespuestasExamen { get; set; } = new();
}

public class ResultadoExamen
{
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; }
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public decimal NotaFinal { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

public class RespuestaExamen
{
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

// DbContext
public class ExamenDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

// Clase lógica
public class ExamenApp
{
    private readonly ExamenDbContext _db;

    public ExamenApp()
    {
        _db = new ExamenDbContext();
        _db.Database.EnsureCreated();
    }

    public void Iniciar()
    {
        while (true)
        {
            LimpiarPantalla();
            MostrarMenu();

            string opcion = LeerOpcion();

            switch (opcion)
            {
                case "1":
                    RegistrarPregunta();
                    break;
                case "2":
                    TomarExamen();
                    break;
                case "3":
                    VerReportes();
                    break;
                case "4":
                    MostrarEstadisticas();
                    break;
                case "5":
                    return;
                default:
                    MostrarMensaje("Opción no válida, por favor intente nuevamente.");
                    break;
            }
        }
    }

    public void RegistrarPregunta()
    {
        LimpiarPantalla();
        Console.WriteLine("Registrar nueva pregunta");

        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine();

        Console.Write("Opción A: ");
        string opcionA = Console.ReadLine();

        Console.Write("Opción B: ");
        string opcionB = Console.ReadLine();

        Console.Write("Opción C: ");
        string opcionC = Console.ReadLine();

        Console.Write("Respuesta Correcta (A/B/C): ");
        string respuestaCorrecta = Console.ReadLine().ToUpper();

        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            RespuestaA = opcionA,
            RespuestaB = opcionB,
            RespuestaC = opcionC,
            Correcta = respuestaCorrecta
        };

        _db.Preguntas.Add(pregunta);
        _db.SaveChanges();

        MostrarMensaje("Pregunta registrada con éxito.");
    }

    public void TomarExamen()
    {
        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine();

        var preguntas = _db.Preguntas.OrderBy(r => Guid.NewGuid()).Take(5).ToList();

        int respuestasCorrectas = 0;
        List<RespuestaExamen> respuestasExamen = new();

        foreach (var pregunta in preguntas)
        {
            LimpiarPantalla();
            MostrarPregunta(pregunta);

            string respuestaAlumno = LeerOpcion().ToUpper();
            bool esCorrecta = respuestaAlumno == pregunta.Correcta;

            if (esCorrecta) respuestasCorrectas++;

            respuestasExamen.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.PreguntaId,
                RespuestaAlumno = respuestaAlumno,
                EsCorrecta = esCorrecta
            });
        }

        decimal notaFinal = (decimal)respuestasCorrectas / preguntas.Count() * 10;

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            CantidadCorrectas = respuestasCorrectas,
            TotalPreguntas = preguntas.Count(),
            NotaFinal = notaFinal,
            Respuestas = respuestasExamen
        };

        _db.ResultadosExamen.Add(resultado);
        _db.SaveChanges();

        MostrarMensaje($"¡Examen finalizado! {nombre}, tu nota es: {notaFinal:F2} sobre 10.");
    }
public void VerReportes()
{
    LimpiarPantalla();
    Console.WriteLine("Exámenes rendidos:");

    var resultados = _db.ResultadosExamen.Include(r => r.Respuestas).ThenInclude(r => r.Pregunta).ToList();

    foreach (var resultado in resultados)
    {
        Console.WriteLine($"{resultado.NombreAlumno} - Nota: {resultado.NotaFinal:F2}");
    }

    Console.WriteLine("\nFiltrar por nombre de alumno:");
    string nombreFiltro = Console.ReadLine();

    var resultadoFiltrado = resultados.Where(r => r.NombreAlumno.Contains(nombreFiltro)).ToList();

    foreach (var resultado in resultadoFiltrado)
    {
        Console.WriteLine($"{resultado.NombreAlumno} - Nota: {resultado.NotaFinal:F2}");
    }
    Console.ReadKey();
}


    public void MostrarEstadisticas()
    {
        LimpiarPantalla();
        Console.WriteLine("Estadísticas por pregunta:");

        var preguntas = _db.Preguntas.Include(p => p.RespuestasExamen).ToList();

        foreach (var pregunta in preguntas)
        {
            int totalRespuestas = pregunta.RespuestasExamen.Count();
            int respuestasCorrectas = pregunta.RespuestasExamen.Count(r => r.EsCorrecta);
            decimal porcentajeCorrectas = totalRespuestas > 0 ? (decimal)respuestasCorrectas / totalRespuestas * 100 : 0;

            Console.WriteLine($"Pregunta: {pregunta.Enunciado}");
            Console.WriteLine($"Total respuestas: {totalRespuestas}, Correctas: {respuestasCorrectas}");
            Console.WriteLine($"Porcentaje de respuestas correctas: {porcentajeCorrectas:F2}%");
            Console.WriteLine();
        }

        Console.ReadKey();
    }

    // Métodos auxiliares

    private void LimpiarPantalla() => Console.Clear();

    private void MostrarMenu()
    {
        Console.WriteLine("1. Registrar pregunta");
        Console.WriteLine("2. Tomar examen");
        Console.WriteLine("3. Ver reportes");
        Console.WriteLine("4. Ver estadísticas");
        Console.WriteLine("5. Salir");
        Console.Write("Seleccione una opción: ");
    }

    private string LeerOpcion() => Console.ReadLine();

    private void MostrarMensaje(string mensaje)
    {
        Console.WriteLine(mensaje);
        Console.ReadKey();
    }

    private void MostrarPregunta(Pregunta pregunta)
    {
        Console.WriteLine($"Pregunta: {pregunta.Enunciado}");
        Console.WriteLine($"A) {pregunta.RespuestaA}");
        Console.WriteLine($"B) {pregunta.RespuestaB}");
        Console.WriteLine($"C) {pregunta.RespuestaC}");
        Console.Write("Selecciona una respuesta (A/B/C): ");
    }
}
