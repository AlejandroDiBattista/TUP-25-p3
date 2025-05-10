using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Cuestion
{
    public int Id { get; set; }
    public string Texto { get; set; } = "";
    public string Opcion1 { get; set; } = "";
    public string Opcion2 { get; set; } = "";
    public string Opcion3 { get; set; } = "";
    public string RespuestaValida { get; set; } = "";
    public List<RespuestaAlumno> HistorialRespuestas { get; set; } = new();
}

class Evaluacion
{
    public int Id { get; set; }
    public string NombreEstudiante { get; set; } = "";
    public int TotalCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double Calificacion { get; set; }
    public DateTime Momento { get; set; } = DateTime.Now;
    public List<RespuestaAlumno> DetalleRespuestas { get; set; } = new();
}

class RespuestaAlumno
{
    public int Id { get; set; }
    public int CuestionId { get; set; }
    public Cuestion Cuestion { get; set; } = null!;
    public int EvaluacionId { get; set; }
    public Evaluacion Evaluacion { get; set; } = null!;
    public bool EsCorrecta { get; set; }
}

class BaseDatos : DbContext
{
    public DbSet<Cuestion> Cuestiones { get; set; } = null!;
    public DbSet<Evaluacion> Evaluaciones { get; set; } = null!;
    public DbSet<RespuestaAlumno> Respuestas { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite("Data Source=cuestionario.db");
    }
}

class App
{
    static void Main()
    {
        using var db = new BaseDatos();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        while (true)
        {
            Console.WriteLine("\n--- Menú Principal ---");
            Console.WriteLine("1. Añadir Cuestión");
            Console.WriteLine("2. Realizar Evaluación");
            Console.WriteLine("3. Consultar Resultados");
            Console.WriteLine("4. Finalizar");
            Console.Write("Elige una opción: ");
            var eleccion = Console.ReadLine();

            switch (eleccion)
            {
                case "1": AgregarCuestion(db); break;
                case "2": RealizarEvaluacion(db); break;
                case "3": MostrarResultados(db); break;
                case "4": return;
                default: Console.WriteLine("Opción no válida."); break;
            }
        }
    }

    static void AgregarCuestion(BaseDatos db)
    {
        var nueva = new Cuestion();
        Console.Write("Escribe la pregunta: "); nueva.Texto = Console.ReadLine()!;
        Console.Write("Opción 1: "); nueva.Opcion1 = Console.ReadLine()!;
        Console.Write("Opción 2: "); nueva.Opcion2 = Console.ReadLine()!;
        Console.Write("Opción 3: "); nueva.Opcion3 = Console.ReadLine()!;
        Console.Write("Respuesta correcta (1/2/3): ");
        var correcta = Console.ReadLine()!;
        nueva.RespuestaValida = correcta;
        db.Cuestiones.Add(nueva);
        db.SaveChanges();
        Console.WriteLine($"Cuestión guardada con ID: {nueva.Id}");
    }

    static void RealizarEvaluacion(BaseDatos db)
    {
        Console.Write("Nombre del estudiante: ");
        string estudiante = Console.ReadLine()!;
        var listado = db.Cuestiones.ToList();

        if (!listado.Any())
        {
            Console.WriteLine("No hay preguntas disponibles."); return;
        }

        var seleccionadas = listado.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
        int aciertos = 0;
        var evaluacion = new Evaluacion { NombreEstudiante = estudiante, TotalPreguntas = seleccionadas.Count };

        foreach (var item in seleccionadas)
        {
            Console.WriteLine($"\nPregunta {item.Id}: {item.Texto}");
            Console.WriteLine($" 1) {item.Opcion1}");
            Console.WriteLine($" 2) {item.Opcion2}");
            Console.WriteLine($" 3) {item.Opcion3}");
            Console.Write("Tu respuesta: ");
            var entrada = Console.ReadLine()!;
            bool esValida = entrada == item.RespuestaValida;
            if (esValida) aciertos++;

            evaluacion.DetalleRespuestas.Add(new RespuestaAlumno { Cuestion = item, EsCorrecta = esValida });
        }

        evaluacion.TotalCorrectas = aciertos;
        evaluacion.Calificacion = Math.Round((double)aciertos / evaluacion.TotalPreguntas * 5, 2);
        db.Evaluaciones.Add(evaluacion);
        db.SaveChanges();

        Console.WriteLine($"\nResultado: {aciertos}/{evaluacion.TotalPreguntas} - Calificación: {evaluacion.Calificacion}");
    }

    static void MostrarResultados(BaseDatos db)
    {
        Console.WriteLine("\n--- Reportes ---");
        Console.WriteLine("1. Ver todos los exámenes");
        Console.WriteLine("2. Buscar por nombre");
        Console.WriteLine("3. Top 10 Calificaciones");
        Console.WriteLine("4. Estadísticas por pregunta");
        Console.WriteLine("5. Volver al menú");
        Console.Write("Elige una opción: ");
        var eleccion = Console.ReadLine();

        switch (eleccion)
        {
            case "1": VerExamenes(db); break;
            case "2": BuscarEstudiante(db); break;
            case "3": RankingNotas(db); break;
            case "4": Estadisticas(db); break;
            case "5": return;
            default: Console.WriteLine("Opción inválida."); break;
        }
    }

    static void VerExamenes(BaseDatos db)
    {
        var registros = db.Evaluaciones.OrderByDescending(x => x.Momento).ToList();
        foreach (var ev in registros)
        {
            Console.WriteLine($"{ev.Momento:dd/MM/yyyy HH:mm} - {ev.NombreEstudiante}: {ev.TotalCorrectas}/{ev.TotalPreguntas} - Nota: {ev.Calificacion}");
        }
    }

    static void BuscarEstudiante(BaseDatos db)
    {
        Console.Write("Nombre del estudiante: ");
        string nombre = Console.ReadLine()!;
        var evaluaciones = db.Evaluaciones
                             .Where(x => x.NombreEstudiante.ToLower() == nombre.ToLower())
                             .OrderByDescending(x => x.Momento)
                             .ToList();

        if (!evaluaciones.Any())
        {
            Console.WriteLine("Sin registros para ese estudiante.");
            return;
        }

        foreach (var ev in evaluaciones)
        {
            Console.WriteLine($"{ev.Momento:dd/MM/yyyy HH:mm} - {ev.NombreEstudiante}: {ev.TotalCorrectas}/{ev.TotalPreguntas} - Nota: {ev.Calificacion}");
        }
    }

    static void RankingNotas(BaseDatos db)
    {
        var ranking = db.Evaluaciones
                        .GroupBy(x => x.NombreEstudiante)
                        .Select(g => new { Nombre = g.Key, NotaMax = g.Max(x => x.Calificacion) })
                        .OrderByDescending(x => x.NotaMax)
                        .Take(10)
                        .ToList();

        Console.WriteLine("\n--- Top Estudiantes ---");
        foreach (var est in ranking)
        {
            Console.WriteLine($"{est.Nombre} - Nota más alta: {est.NotaMax}");
        }
    }

    static void Estadisticas(BaseDatos db)
    {
        var cuestiones = db.Cuestiones.Include(c => c.HistorialRespuestas).ToList();
        Console.WriteLine("\n--- Estadísticas por Cuestión ---");

        foreach (var c in cuestiones)
        {
            int total = c.HistorialRespuestas.Count;
            int buenas = c.HistorialRespuestas.Count(r => r.EsCorrecta);
            double porcentaje = total > 0 ? Math.Round((double)buenas / total * 100, 2) : 0;

            Console.WriteLine($"[{c.Id}] {c.Texto}");
            Console.WriteLine($" Respuesta total: {total} - Correctas: {porcentaje}%");
        }
    }
}
