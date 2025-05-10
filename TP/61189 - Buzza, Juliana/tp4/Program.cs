using Examenes.Data;
using Examenes.Models;
using Microsoft.EntityFrameworkCore;

var context = new AppDbContext();
context.Database.EnsureCreated();

while (true)
{
    Console.Clear();
    Console.WriteLine("Sistema de Exámenes");
    Console.WriteLine("1. Tomar examen");
    Console.WriteLine("2. Reportes");
    Console.WriteLine("3. Salir");
    
    var opcion = Console.ReadLine();
    
    switch (opcion)
    {
        case "1":
            TomarExamen(context);
            break;
        case "2":
            MostrarReportes(context);
            break;
        case "3":
            return;
        default:
            Console.WriteLine("Opción inválida!");
            Console.ReadKey();
            break;
    }
}

static void TomarExamen(AppDbContext db)
{
    try
    {
        Console.Clear();
        
        if (!db.Preguntas.Any())
        {
            Console.WriteLine("¡No hay preguntas disponibles!");
            Console.ReadKey();
            return;
        }

        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine()?.Trim();
        
        if (string.IsNullOrEmpty(nombre))
        {
            Console.WriteLine("¡Nombre inválido!");
            Console.ReadKey();
            return;
        }

        var preguntas = db.Preguntas
            .OrderBy(r => EF.Functions.Random())
            .Take(5)
            .ToList();

        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombre,
            TotalPreguntas = preguntas.Count,
            Fecha = DateTime.Now
        };

        using var transaction = db.Database.BeginTransaction();
        
        try
        {
            foreach (var pregunta in preguntas)
            {
                Console.Clear();
                Console.WriteLine($"Pregunta {preguntas.IndexOf(pregunta) + 1}/{preguntas.Count}");
                Console.WriteLine(new string('-', 40));
                Console.WriteLine(pregunta.Enunciado);
                Console.WriteLine($"A) {pregunta.AlternativaA}");
                Console.WriteLine($"B) {pregunta.AlternativaB}");
                Console.WriteLine($"C) {pregunta.AlternativaC}");

                char respuesta;
                do
                {
                    Console.Write("\nRespuesta (A/B/C): ");
                    respuesta = char.ToUpper(Console.ReadKey().KeyChar);
                } while (!"ABC".Contains(respuesta));

                bool esCorrecta = respuesta == pregunta.RespuestaCorrecta;
                if (esCorrecta) resultado.Correctas++;
                
                resultado.Respuestas.Add(new RespuestaExamen
                {
                    PreguntaId = pregunta.Id,
                    Respuesta = respuesta,
                    EsCorrecta = esCorrecta
                });
            }

            resultado.Nota = resultado.TotalPreguntas > 0 
                ? (decimal)resultado.Correctas / resultado.TotalPreguntas * 10 
                : 0;

            db.ResultadosExamenes.Add(resultado);
            db.SaveChanges();
            transaction.Commit();

            // Bloque de resultados corregido
            Console.Clear();
            Console.WriteLine(new string('═', 40));
            Console.WriteLine(" RESULTADO DEL EXAMEN ");
            Console.WriteLine(new string('═', 40));
            Console.WriteLine($"Alumno: {resultado.NombreAlumno}");
            Console.WriteLine($"Fecha: {resultado.Fecha:g}");
            Console.WriteLine($"Preguntas: {resultado.Correctas}/{resultado.TotalPreguntas}");
            Console.WriteLine($"Nota: {resultado.Nota:0.00}/10");
            Console.WriteLine(new string('═', 40));
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"\nError: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("\nPresione cualquier tecla...");
            Console.ReadKey();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error crítico: {ex.Message}");
        Console.ReadKey();
    }
}

static void MostrarReportes(AppDbContext db)
{
    Console.Clear();
    Console.WriteLine("1. Listado de exámenes");
    Console.WriteLine("2. Filtrar por nombre");
    Console.WriteLine("3. Ranking de mejores alumnos");
    Console.WriteLine("4. Estadísticas por pregunta");
    
    switch (Console.ReadLine())
    {
        case "1":
            ListarExamenes(db);
            break;
        case "2":
            FiltrarPorNombre(db);
            break;
        case "3":
            MostrarRanking(db);
            break;
        case "4":
            EstadisticasPreguntas(db);
            break;
    }
    Console.ReadKey();
}

static void ListarExamenes(AppDbContext db)
{
    var examenes = db.ResultadosExamenes
        .Include(e => e.Respuestas)
        .ToList();
    
    foreach (var e in examenes)
    {
        Console.WriteLine($"{e.Fecha:g} - {e.NombreAlumno}: {e.Nota:0.00}");
    }
}

static void FiltrarPorNombre(AppDbContext db)
{
    Console.Write("Nombre a buscar: ");
    var nombre = Console.ReadLine()!;
    
    var resultados = db.ResultadosExamenes
        .Where(e => e.NombreAlumno.Contains(nombre))
        .ToList();
    
    foreach (var r in resultados)
    {
        Console.WriteLine($"[{r.Fecha:dd/MM/yy HH:mm}] {r.NombreAlumno} - Nota: {r.Nota:0.00}/10");
    }
}
static void MostrarRanking(AppDbContext db)
{
    try
    {
        Console.Clear();
        Console.WriteLine("=== RANKING DE MEJORES ALUMNOS ===");
        
        // 1. Validar existencia de datos
        if (!db.ResultadosExamenes.Any())
        {
            Console.WriteLine("\nNo hay exámenes registrados!");
            return;
        }

        // 2. Consulta optimizada
        var ranking = db.ResultadosExamenes
            .AsEnumerable()
            .GroupBy(e => e.NombreAlumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(e => e.Nota),
                Intentos = g.Count()
            })
            .OrderByDescending(x => x.MejorNota)
            .ThenBy(x => x.Alumno)
            .ToList();

        Console.WriteLine("\n{0,-20} {1,-10} {2}", "ALUMNOS ", "     NOTAS ", "       INTENTOS");
        Console.WriteLine(new string('-', 50));
        
        foreach (var item in ranking)
        {
            Console.WriteLine($"{item.Alumno,-20} {item.MejorNota,10:0.00}/10 {item.Intentos,10}");
        }
        
        // 4. Estadística adicional
        Console.WriteLine($"\nTotal de alumnos en ranking: {ranking.Count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError generando ranking: {ex.Message}");
    }
}

static void EstadisticasPreguntas(AppDbContext db)
{
    var stats = db.RespuestasExamen
        .Include(r => r.Pregunta)  
        .GroupBy(r => r.Pregunta)
        .Select(g => new {
            Pregunta = g.Key.Enunciado,
            Total = g.Count(),
            Correctas = g.Count(r => r.EsCorrecta),
            Porcentaje = g.Any() ? 
                (decimal)g.Count(r => r.EsCorrecta) / g.Count() * 100 : 0
        })
        .ToList();
    
    foreach (var s in stats)
    {
        Console.WriteLine($"{s.Pregunta}");
        Console.WriteLine($"  Veces respondida: {s.Total}");
        Console.WriteLine($"  Correctas: {s.Correctas} ({s.Porcentaje:0}%)");
    }
}