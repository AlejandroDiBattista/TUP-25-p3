using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// MODELOS
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
    public string NombreAlumno { get; set; } = "";
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
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

// DB CONTEXT
public class ExamenDbContext : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; } = null!;
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; } = null!;
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.RespuestasExamen)
            .HasForeignKey(r => r.PreguntaId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);
    }
}

// APLICACIÓN
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
                case "1": RegistrarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": VerReportes(); break;
                case "4": MostrarEstadisticas(); break;
                case "5": return;
                default: MostrarMensaje("Opción no válida."); break;
            }
        }
    }

    public void RegistrarPregunta()
    {
        LimpiarPantalla();
        Console.WriteLine("Registrar nueva pregunta");
        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine()!;
        Console.Write("Opción A: ");
        string opcionA = Console.ReadLine()!;
        Console.Write("Opción B: ");
        string opcionB = Console.ReadLine()!;
        Console.Write("Opción C: ");
        string opcionC = Console.ReadLine()!;
        string respuestaCorrecta;
        do
        {
            Console.Write("Respuesta Correcta (A/B/C): ");
            respuestaCorrecta = Console.ReadLine()!.ToUpper();
        } while (respuestaCorrecta != "A" && respuestaCorrecta != "B" && respuestaCorrecta != "C");

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
        LimpiarPantalla();
        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine()!;

        var preguntas = _db.Preguntas.OrderBy(r => Guid.NewGuid()).Take(5).ToList();

        if (preguntas.Count == 0)
        {
            MostrarMensaje("No hay preguntas registradas.");
            return;
        }

        int respuestasCorrectas = 0;
        List<RespuestaExamen> respuestasExamen = new();

        foreach (var pregunta in preguntas)
        {
            LimpiarPantalla();
            MostrarPregunta(pregunta);
            string respuestaAlumno;
            do
            {
                respuestaAlumno = LeerOpcion().ToUpper();
            } while (respuestaAlumno != "A" && respuestaAlumno != "B" && respuestaAlumno != "C");

            bool esCorrecta = respuestaAlumno == pregunta.Correcta;
            if (esCorrecta) respuestasCorrectas++;

            respuestasExamen.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.PreguntaId,
                RespuestaAlumno = respuestaAlumno,
                EsCorrecta = esCorrecta
            });
        }

        decimal notaFinal = (decimal)respuestasCorrectas / preguntas.Count * 10;

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            CantidadCorrectas = respuestasCorrectas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = notaFinal,
            Respuestas = respuestasExamen
        };

        _db.ResultadosExamen.Add(resultado);
        _db.SaveChanges();

        MostrarMensaje($"¡Examen finalizado! {nombre}, tu nota es: {notaFinal:F2}/10");
    }

    public void VerReportes()
    {
        LimpiarPantalla();
        Console.WriteLine("Exámenes rendidos:");
        var resultados = _db.ResultadosExamen.ToList();
        foreach (var resultado in resultados)
        {
            Console.WriteLine($"{resultado.NombreAlumno} - Nota: {resultado.NotaFinal:F2}");
        }

        Console.Write("\nFiltrar por nombre (dejar vacío para ver todos): ");
        string filtro = Console.ReadLine()!;
        var filtrados = string.IsNullOrEmpty(filtro)
            ? resultados
            : resultados.Where(r => r.NombreAlumno.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var resultado in filtrados)
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
            int total = pregunta.RespuestasExamen.Count;
            int correctas = pregunta.RespuestasExamen.Count(r => r.EsCorrecta);
            decimal porcentaje = total > 0 ? (decimal)correctas / total * 100 : 0;

            Console.WriteLine($"\"{pregunta.Enunciado}\"");
            Console.WriteLine($"Total: {total}, Correctas: {correctas}, Acierto: {porcentaje:F2}%");
            Console.WriteLine();
        }

        Console.ReadKey();
    }

    // MÉTODOS AUXILIARES
    private void LimpiarPantalla() => Console.Clear();
    private void MostrarMenu()
    {
        Console.WriteLine("----- MENÚ -----");
        Console.WriteLine("1. Registrar pregunta");
        Console.WriteLine("2. Tomar examen");
        Console.WriteLine("3. Ver reportes");
        Console.WriteLine("4. Ver estadísticas");
        Console.WriteLine("5. Salir");
        Console.Write("Seleccione una opción: ");
    }
    private string LeerOpcion() => Console.ReadLine() ?? "";
    private void MostrarMensaje(string mensaje)
    {
        Console.WriteLine(mensaje);
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
    private void MostrarPregunta(Pregunta pregunta)
    {
        Console.WriteLine($"Pregunta: {pregunta.Enunciado}");
        Console.WriteLine($"A) {pregunta.RespuestaA}");
        Console.WriteLine($"B) {pregunta.RespuestaB}");
        Console.WriteLine($"C) {pregunta.RespuestaC}");
        Console.Write("Tu respuesta (A/B/C): ");
    }
}

// PUNTO DE ENTRADA
class Program
{
    static void Main(string[] args)
    {
        new ExamenApp().Iniciar();
    }
}
