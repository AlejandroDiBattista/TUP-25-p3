using Microsoft.EntityFrameworkCore;
using SistemaExamenes.Console.Data;
using SistemaExamenes.Console.Services;

namespace SistemaExamenes.Console;

class Program
{
    private static ExamenDbContext _context = null!;
    private static PreguntaService _preguntaService = null!;
    private static ExamenService _examenService = null!;
    
    static async Task Main(string[] args)
    {
        // Inicializar la base de datos
        _context = new ExamenDbContext();
        await _context.Database.EnsureCreatedAsync();
        
        // Inicializar servicios
        _preguntaService = new PreguntaService(_context);
        _examenService = new ExamenService(_context, _preguntaService);
        
        // Cargar datos de ejemplo si la base de datos está vacía
        if (await _preguntaService.ObtenerCantidadPreguntasAsync() == 0)
        {
            await CargarDatosDeEjemplo();
        }
        
        bool continuar = true;
        while (continuar)
        {
            MostrarMenuPrincipal();
            var opcion = LeerOpcion();
            
            switch (opcion)
            {
                case 1:
                    await RegistrarPregunta();
                    break;
                case 2:
                    await RealizarExamen();
                    break;
                case 3:
                    await MostrarResultados();
                    break;
                case 4:
                    await MostrarRanking();
                    break;
                case 5:
                    await MostrarEstadisticas();
                    break;
                case 6:
                    await ListarPreguntas();
                    break;
                case 0:
                    continuar = false;
                    System.Console.WriteLine("¡Gracias por usar el Sistema de Exámenes!");
                    break;
                default:
                    System.Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
            
            if (continuar)
            {
                System.Console.WriteLine("\nPresione cualquier tecla para continuar...");
                System.Console.ReadKey();
                System.Console.Clear();
            }
        }
    }
    
    static void MostrarMenuPrincipal()
    {
        System.Console.WriteLine("=== SISTEMA DE EXÁMENES MULTIPLE CHOICE ===");
        System.Console.WriteLine("1. Registrar nueva pregunta");
        System.Console.WriteLine("2. Realizar examen");
        System.Console.WriteLine("3. Ver resultados de exámenes");
        System.Console.WriteLine("4. Ver ranking de mejores alumnos");
        System.Console.WriteLine("5. Ver estadísticas por pregunta");
        System.Console.WriteLine("6. Listar todas las preguntas");
        System.Console.WriteLine("0. Salir");
        System.Console.Write("\nSeleccione una opción: ");
    }
    
    static int LeerOpcion()
    {
        if (int.TryParse(System.Console.ReadLine(), out int opcion))
            return opcion;
        return -1;
    }
    
    static async Task CargarDatosDeEjemplo()
    {
        System.Console.WriteLine("Cargando datos de ejemplo...");
        
        var preguntas = new[]
        {
            ("¿Cuál es la capital de Francia?", "París", "Londres", "Madrid", 'A'),
            ("¿Cuántos planetas hay en el sistema solar?", "7", "8", "9", 'B'),
            ("¿En qué año comenzó la Primera Guerra Mundial?", "1914", "1915", "1916", 'A'),
            ("¿Cuál es el elemento químico más abundante en el universo?", "Hidrógeno", "Helio", "Oxígeno", 'A'),
            ("¿Quién escribió 'Don Quijote de la Mancha'?", "Miguel de Cervantes", "William Shakespeare", "Dante Alighieri", 'A'),
            ("¿Cuál es el río más largo del mundo?", "Nilo", "Amazonas", "Misisipi", 'A'),
            ("¿En qué continente se encuentra el desierto del Sahara?", "África", "Asia", "Europa", 'A'),
            ("¿Cuál es la moneda oficial de Japón?", "Yen", "Dólar", "Euro", 'A'),
            ("¿Cuántos huesos tiene el cuerpo humano adulto?", "206", "200", "212", 'A'),
            ("¿Quién pintó la Mona Lisa?", "Leonardo da Vinci", "Miguel Ángel", "Rafael", 'A')
        };
        
        foreach (var (enunciado, altA, altB, altC, correcta) in preguntas)
        {
            await _preguntaService.CrearPreguntaAsync(enunciado, altA, altB, altC, correcta);
        }
        
        System.Console.WriteLine("Datos de ejemplo cargados exitosamente.");
    }
    
    static async Task RegistrarPregunta()
    {
        System.Console.WriteLine("\n=== REGISTRAR NUEVA PREGUNTA ===");
        
        System.Console.Write("Enunciado de la pregunta: ");
        var enunciado = System.Console.ReadLine() ?? "";
        
        if (string.IsNullOrWhiteSpace(enunciado))
        {
            System.Console.WriteLine("El enunciado no puede estar vacío.");
            return;
        }
        
        System.Console.Write("Alternativa A: ");
        var alternativaA = System.Console.ReadLine() ?? "";
        
        System.Console.Write("Alternativa B: ");
        var alternativaB = System.Console.ReadLine() ?? "";
        
        System.Console.Write("Alternativa C: ");
        var alternativaC = System.Console.ReadLine() ?? "";
        
        System.Console.Write("Respuesta correcta (A, B o C): ");
        var respuestaCorrecta = System.Console.ReadLine()?.ToUpper() ?? "";
        
        if (respuestaCorrecta != "A" && respuestaCorrecta != "B" && respuestaCorrecta != "C")
        {
            System.Console.WriteLine("La respuesta correcta debe ser A, B o C.");
            return;
        }
        
        try
        {
            var pregunta = await _preguntaService.CrearPreguntaAsync(
                enunciado, alternativaA, alternativaB, alternativaC, respuestaCorrecta[0]);
            
            System.Console.WriteLine($"Pregunta registrada exitosamente con ID: {pregunta.Id}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error al registrar la pregunta: {ex.Message}");
        }
    }
    
    static async Task RealizarExamen()
    {
        System.Console.WriteLine("\n=== REALIZAR EXAMEN ===");
        
        var cantidadPreguntas = await _preguntaService.ObtenerCantidadPreguntasAsync();
        if (cantidadPreguntas == 0)
        {
            System.Console.WriteLine("No hay preguntas disponibles para realizar el examen.");
            return;
        }
        
        System.Console.Write("Ingrese su nombre: ");
        var nombreAlumno = System.Console.ReadLine() ?? "";
        
        if (string.IsNullOrWhiteSpace(nombreAlumno))
        {
            System.Console.WriteLine("El nombre no puede estar vacío.");
            return;
        }
        
        var preguntas = await _preguntaService.ObtenerPreguntasAleatoriasAsync(5);
        var respuestas = new List<char>();
        
        System.Console.WriteLine($"\nExamen de {preguntas.Count} preguntas. ¡Buena suerte!");
        
        for (int i = 0; i < preguntas.Count; i++)
        {
            var pregunta = preguntas[i];
            System.Console.WriteLine($"\nPregunta {i + 1}:");
            System.Console.WriteLine(pregunta.Enunciado);
            System.Console.WriteLine($"A) {pregunta.AlternativaA}");
            System.Console.WriteLine($"B) {pregunta.AlternativaB}");
            System.Console.WriteLine($"C) {pregunta.AlternativaC}");
            
            char respuesta;
            do
            {
                System.Console.Write("Su respuesta (A, B o C): ");
                var input = System.Console.ReadLine()?.ToUpper() ?? "";
                respuesta = input.Length > 0 ? input[0] : ' ';
            } while (respuesta != 'A' && respuesta != 'B' && respuesta != 'C');
            
            respuestas.Add(respuesta);
        }
        
        try
        {
            var resultado = await _examenService.RealizarExamenAsync(nombreAlumno, preguntas, respuestas);
            
            System.Console.WriteLine("\n=== RESULTADO DEL EXAMEN ===");
            System.Console.WriteLine($"Alumno: {resultado.NombreAlumno}");
            System.Console.WriteLine($"Respuestas correctas: {resultado.CantidadRespuestasCorrectas}/{resultado.TotalPreguntas}");
            System.Console.WriteLine($"Nota final: {resultado.NotaFinal}/5");
            System.Console.WriteLine($"Fecha: {resultado.FechaExamen:dd/MM/yyyy HH:mm}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error al procesar el examen: {ex.Message}");
        }
    }
    
    static async Task MostrarResultados()
    {
        System.Console.WriteLine("\n=== RESULTADOS DE EXÁMENES ===");
        
        System.Console.WriteLine("1. Ver todos los resultados");
        System.Console.WriteLine("2. Filtrar por nombre de alumno");
        System.Console.Write("Seleccione una opción: ");
        
        var opcion = LeerOpcion();
        
        List<Models.ResultadoExamen> resultados;
        
        switch (opcion)
        {
            case 1:
                resultados = await _examenService.ObtenerTodosLosResultadosAsync();
                break;
            case 2:
                System.Console.Write("Ingrese el nombre del alumno: ");
                var nombre = System.Console.ReadLine() ?? "";
                resultados = await _examenService.ObtenerResultadosPorAlumnoAsync(nombre);
                break;
            default:
                System.Console.WriteLine("Opción no válida.");
                return;
        }
        
        if (resultados.Count == 0)
        {
            System.Console.WriteLine("No se encontraron resultados.");
            return;
        }
        
        System.Console.WriteLine("\nResultados:");
        System.Console.WriteLine("ID\tAlumno\t\tCorrectas\tTotal\tNota\tFecha");
        System.Console.WriteLine(new string('-', 80));
        
        foreach (var resultado in resultados)
        {
            System.Console.WriteLine($"{resultado.Id}\t{resultado.NombreAlumno,-15}\t{resultado.CantidadRespuestasCorrectas}\t\t{resultado.TotalPreguntas}\t{resultado.NotaFinal}\t{resultado.FechaExamen:dd/MM/yyyy}");
        }
    }
    
    static async Task MostrarRanking()
    {
        System.Console.WriteLine("\n=== RANKING DE MEJORES ALUMNOS ===");
        
        var ranking = await _examenService.ObtenerRankingMejoresAlumnosAsync(10);
        
        if (ranking.Count == 0)
        {
            System.Console.WriteLine("No hay resultados para mostrar el ranking.");
            return;
        }
        
        System.Console.WriteLine("Posición\tAlumno\t\tMejor Nota\tFecha");
        System.Console.WriteLine(new string('-', 60));
        
        for (int i = 0; i < ranking.Count; i++)
        {
            var resultado = ranking[i];
            System.Console.WriteLine($"{i + 1}\t\t{resultado.NombreAlumno,-15}\t{resultado.NotaFinal}\t\t{resultado.FechaExamen:dd/MM/yyyy}");
        }
    }
    
    static async Task MostrarEstadisticas()
    {
        System.Console.WriteLine("\n=== ESTADÍSTICAS POR PREGUNTA ===");
        
        var estadisticas = await _examenService.ObtenerEstadisticasPorPreguntaAsync();
        
        if (estadisticas.Count == 0)
        {
            System.Console.WriteLine("No hay estadísticas disponibles.");
            return;
        }
        
        System.Console.WriteLine("ID\tVeces\tCorrectas\tPorcentaje\tEnunciado");
        System.Console.WriteLine(new string('-', 100));
        
        foreach (dynamic stat in estadisticas)
        {
            var enunciado = stat.Enunciado.ToString();
            if (enunciado.Length > 50)
                enunciado = enunciado.Substring(0, 47) + "...";
                
            System.Console.WriteLine($"{stat.PreguntaId}\t{stat.VecesRespondida}\t{stat.RespuestasCorrectas}\t\t{stat.PorcentajeCorrecto:F1}%\t\t{enunciado}");
        }
    }
    
    static async Task ListarPreguntas()
    {
        System.Console.WriteLine("\n=== LISTA DE PREGUNTAS ===");
        
        var preguntas = await _preguntaService.ObtenerTodasLasPreguntasAsync();
        
        if (preguntas.Count == 0)
        {
            System.Console.WriteLine("No hay preguntas registradas.");
            return;
        }
        
        foreach (var pregunta in preguntas)
        {
            System.Console.WriteLine($"\nID: {pregunta.Id}");
            System.Console.WriteLine($"Enunciado: {pregunta.Enunciado}");
            System.Console.WriteLine($"A) {pregunta.AlternativaA}");
            System.Console.WriteLine($"B) {pregunta.AlternativaB}");
            System.Console.WriteLine($"C) {pregunta.AlternativaC}");
            System.Console.WriteLine($"Respuesta correcta: {pregunta.RespuestaCorrecta}");
            System.Console.WriteLine(new string('-', 50));
        }
    }
}
