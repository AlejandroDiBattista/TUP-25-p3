using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.IO;

// Modelo de datos
class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
    
    // Relaciones
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class Alumno {
    public int AlumnoId { get; set; }
    public string Nombre { get; set; } = "";
    
    // Relaciones
    public List<ResultadoExamen> Resultados { get; set; } = new List<ResultadoExamen>();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public int AlumnoId { get; set; }
    public DateTime FechaExamen { get; set; }
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public decimal NotaFinal { get; set; }
    
    // Relaciones
    public Alumno? Alumno { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public int PreguntaId { get; set; }
    public string RespuestaSeleccionada { get; set; } = "";
    public bool EsCorrecta { get; set; }
    
    // Relaciones
    public ResultadoExamen? ResultadoExamen { get; set; }
    public Pregunta? Pregunta { get; set; }
}

class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de relaciones
        modelBuilder.Entity<ResultadoExamen>()
            .HasOne(r => r.Alumno)
            .WithMany(a => a.Resultados)
            .HasForeignKey(r => r.AlumnoId);
            
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);
            
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.Respuestas)
            .HasForeignKey(r => r.PreguntaId);
    }
}

class Program{
    static void Main(string[] args)
    {
        // Configurar la codificación para mostrar caracteres acentuados
        Console.OutputEncoding = Encoding.UTF8;
        
        // Para máxima compatibilidad en Windows, también puedes usar:
        try {
            Console.InputEncoding = Encoding.UTF8;
            // En algunos sistemas Windows esto puede ser necesario
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        } catch (Exception) {
            // Ignorar errores si no se puede configurar
        }
        
        // Continuar directamente con la base de datos existente
        using (var db = new DatosContexto())
        {
            // Crear la base de datos si no existe
            db.Database.EnsureCreated();
            
            // Configurar el sistema inicial solo si la base de datos está vacía
            if (!db.Preguntas.Any())
            {
                Console.WriteLine("Base de datos vacía detectada. Realizando configuración inicial...");
                ConfigurarSistemaInicial(db);
            }
            
            // Flujo principal del programa
            bool salir = false;
            while (!salir)
            {
                MostrarMenu();
                string opcion = Console.ReadLine() ?? "";
                
                switch (opcion)
                {
                    case "1":
                        RegistrarPregunta(db);
                        break;
                    case "2":
                        TomarExamen(db);
                        break;
                    case "3":
                        MostrarExamenes(db);
                        break;
                    case "4":
                        BuscarPorAlumno(db);
                        break;
                    case "5":
                        MostrarRanking(db);
                        break;
                    case "6":
                        MostrarEstadisticasPregunta(db);
                        break;
                    case "123456789":
                        ReiniciarSistema(db);
                        break;
                    case "0":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        Console.WriteLine("Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
    
    static void MostrarMenu()
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("SISTEMA DE EXÁMENES");
        ConsoleHelper.WriteMenuOption("1", "Registrar nueva pregunta");
        ConsoleHelper.WriteMenuOption("2", "Tomar examen");
        ConsoleHelper.WriteMenuOption("3", "Ver todos los exámenes");
        ConsoleHelper.WriteMenuOption("4", "Buscar exámenes por alumno");
        ConsoleHelper.WriteMenuOption("5", "Ver ranking de alumnos");
        ConsoleHelper.WriteMenuOption("6", "Ver estadísticas por pregunta");
        ConsoleHelper.WriteMenuOption("0", "Salir");
        
        Console.WriteLine();
        ConsoleHelper.WriteColored("Seleccione una opción: ", ConsoleColor.Yellow);
    }
    
    static void RegistrarPregunta(DatosContexto db)
    {
        bool volver = false;
        while (!volver)
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRAR NUEVA PREGUNTA ===\n");
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1. Registrar una pregunta manualmente");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("\nOpción: ");
            
            string opcion = Console.ReadLine() ?? "";
            
            switch (opcion)
            {
                case "1":
                    RegistrarPreguntaManual(db);
                    break;
                case "0":
                    volver = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    
    static void RegistrarPreguntaManual(DatosContexto db)
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("REGISTRAR PREGUNTA MANUALMENTE");
        
        try
        {
            Console.Write("Enunciado: ");
            string enunciado = Console.ReadLine() ?? "";
            
            Console.Write("Respuesta A: ");
            string respuestaA = Console.ReadLine() ?? "";
            
            Console.Write("Respuesta B: ");
            string respuestaB = Console.ReadLine() ?? "";
            
            Console.Write("Respuesta C: ");
            string respuestaC = Console.ReadLine() ?? "";
            
            string correcta;
            do {
                Console.Write("Respuesta correcta (A, B o C): ");
                correcta = Console.ReadLine()?.ToUpper() ?? "";
            } while (correcta != "A" && correcta != "B" && correcta != "C");
            
            bool preguntaExiste = db.Preguntas
                .Any(p => p.Enunciado.ToLower().Trim() == enunciado.ToLower().Trim());
                
            if (preguntaExiste)
            {
                ConsoleHelper.WriteWarning("\nATENCIÓN: Ya existe una pregunta con un enunciado similar.");
                Console.Write("¿Desea registrar esta pregunta de todos modos? (S/N): ");
                string confirmar = Console.ReadLine()?.ToUpper() ?? "N";
                
                if (confirmar != "S")
                {
                    ConsoleHelper.WriteWarning("\nRegistro cancelado.");
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }
            }
            
            // Uso de transacción para garantizar la atomicidad
            using (var transaction = db.Database.BeginTransaction())
            {
                var pregunta = new Pregunta {
                    Enunciado = enunciado,
                    RespuestaA = respuestaA,
                    RespuestaB = respuestaB,
                    RespuestaC = respuestaC,
                    Correcta = correcta
                };
                
                db.Preguntas.Add(pregunta);
                db.SaveChanges();
                transaction.Commit();
                
                ConsoleHelper.WriteSuccess("\nPregunta registrada exitosamente.");
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"\nError al registrar la pregunta: {ex.Message}");
        }
        
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
    
    static void TomarExamen(DatosContexto db)
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("TOMAR EXAMEN");
        
        try 
        {
            int totalPreguntas = db.Preguntas.Count();
            if (totalPreguntas == 0)
            {
                Console.WriteLine("No hay preguntas registradas. Agregue preguntas antes de tomar un examen.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }
            
            Console.Write("Nombre del alumno: ");
            string nombreAlumno = Console.ReadLine() ?? "Anónimo";
            
            // Normalizar el nombre ingresado
            string nombreNormalizado = NormalizarTexto(nombreAlumno);
            
            // Iniciar transacción para garantizar consistencia
            using (var transaction = db.Database.BeginTransaction())
            {
                // Buscar al alumno por nombre normalizado (insensible a mayúsculas/minúsculas y espacios)
                var alumno = db.Alumnos
                    .AsEnumerable() 
                    .FirstOrDefault(a => NormalizarTexto(a.Nombre).Equals(nombreNormalizado));
                
                if (alumno == null)
                {
                    // Si no existe el alumno, crearlo con el nombre original (preservando formato)
                    alumno = new Alumno { Nombre = nombreAlumno };
                    db.Alumnos.Add(alumno);
                    db.SaveChanges();
                }
                
                var preguntasRespondidasIds = db.RespuestasExamen
                    .Include(r => r.ResultadoExamen)
                    .Where(r => r.ResultadoExamen != null && r.ResultadoExamen.AlumnoId == alumno.AlumnoId)
                    .Select(r => r.PreguntaId)
                    .Distinct()
                    .ToList();
                
                var preguntasDisponibles = db.Preguntas
                    .Where(p => !preguntasRespondidasIds.Contains(p.PreguntaId))
                    .ToList();
                    
                int preguntasDisponiblesCount = preguntasDisponibles.Count();
                
                if (preguntasDisponiblesCount == 0)
                {
                    Console.Clear();
                    ConsoleHelper.WriteTitle("¡FELICIDADES! HAS COMPLETADO TODAS LAS PREGUNTAS");
                    Console.WriteLine($"¡Felicidades {alumno.Nombre}! Has respondido todas las 50 preguntas disponibles.");
                    Console.WriteLine("Has completado el programa completo de C# básico.");
                    
                    // Mostrar estadísticas finales
                    var respuestasAlumno = db.RespuestasExamen
                        .Include(r => r.ResultadoExamen)
                        .Where(r => r.ResultadoExamen != null && r.ResultadoExamen.AlumnoId == alumno.AlumnoId)
                        .ToList();
                    
                    int totalPreguntasRespondidas = respuestasAlumno.Count;
                    int respuestasCorrectas = respuestasAlumno.Count(r => r.EsCorrecta);
                    int respuestasIncorrectas = totalPreguntasRespondidas - respuestasCorrectas;
                    double porcentajeAciertos = totalPreguntasRespondidas > 0 ? 
                        (double)respuestasCorrectas / totalPreguntasRespondidas * 100 : 0;
                    
                    ConsoleHelper.WriteTitle("RESUMEN FINAL DE RENDIMIENTO");
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine($"Total de preguntas respondidas: {totalPreguntasRespondidas}/50");
                    ConsoleHelper.WriteSuccess($"Respuestas correctas: {respuestasCorrectas} ({porcentajeAciertos:F2}%)");
                    ConsoleHelper.WriteError($"Respuestas incorrectas: {respuestasIncorrectas} ({100-porcentajeAciertos:F2}%)");
                    
                    // Calificación final
                    string calificacion;
                    ConsoleColor calificacionColor;
                    
                    if (porcentajeAciertos >= 90) {
                        calificacion = "¡EXCELENTE!";
                        calificacionColor = ConsoleColor.Green;
                    } else if (porcentajeAciertos >= 75) {
                        calificacion = "¡MUY BIEN!";
                        calificacionColor = ConsoleColor.Cyan;
                    } else if (porcentajeAciertos >= 60) {
                        calificacion = "BIEN";
                        calificacionColor = ConsoleColor.Yellow;
                    } else {
                        calificacion = "NECESITA MEJORAR";
                        calificacionColor = ConsoleColor.Red;
                    }
                    
                    ConsoleHelper.WriteColored($"\nCalificación final: ", ConsoleColor.White);
                    ConsoleHelper.WriteLineColored(calificacion, calificacionColor);
                    
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }
                
                // Ir directamente al examen con 5 preguntas máximo
                int preguntasExamen = Math.Min(5, preguntasDisponiblesCount);
                var preguntasAleatorias = preguntasDisponibles
                    .OrderBy(p => Guid.NewGuid())
                    .Take(preguntasExamen)
                    .ToList();
                
                var resultadoExamen = new ResultadoExamen {
                    AlumnoId = alumno.AlumnoId,
                    FechaExamen = DateTime.Now,
                    TotalPreguntas = preguntasAleatorias.Count,
                    CantidadCorrectas = 0
                };
                db.ResultadosExamen.Add(resultadoExamen);
                db.SaveChanges();
                
                int correctas = 0;
                foreach (var pregunta in preguntasAleatorias)
                {
                    Console.Clear();
                    ConsoleHelper.WriteTitle($"Pregunta {preguntasAleatorias.IndexOf(pregunta) + 1}/{preguntasAleatorias.Count}");
                    Console.WriteLine($"\n{pregunta.Enunciado}\n");
                    Console.WriteLine($"A) {pregunta.RespuestaA}");
                    Console.WriteLine($"B) {pregunta.RespuestaB}");
                    Console.WriteLine($"C) {pregunta.RespuestaC}");
                    string respuesta;
                    do {
                        Console.Write("\nTu respuesta (A, B o C): ");
                        respuesta = Console.ReadLine()?.ToUpper() ?? "";
                    } while (respuesta != "A" && respuesta != "B" && respuesta != "C");
                    bool esCorrecta = respuesta == pregunta.Correcta;
                    if (esCorrecta) correctas++;
                    var respuestaExamen = new RespuestaExamen {
                        ResultadoExamenId = resultadoExamen.ResultadoExamenId,
                        PreguntaId = pregunta.PreguntaId,
                        RespuestaSeleccionada = respuesta,
                        EsCorrecta = esCorrecta
                    };
                    db.RespuestasExamen.Add(respuestaExamen);
                }
                
                decimal notaFinal = preguntasAleatorias.Count > 0 ? (decimal)correctas / preguntasAleatorias.Count * 10 : 0;
                
                resultadoExamen.CantidadCorrectas = correctas;
                resultadoExamen.NotaFinal = notaFinal;
                db.SaveChanges();
                
                // Confirmar todos los cambios
                transaction.Commit();
                
                Console.Clear();
                ConsoleHelper.WriteTitle("RESULTADO DEL EXAMEN");
                Console.WriteLine($"Alumno: {alumno.Nombre}");
                Console.WriteLine($"Respuestas correctas: {correctas} de {preguntasExamen}");
                Console.WriteLine($"Nota final: {(int)notaFinal} / 10");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"Error al procesar el examen: {ex.Message}");
            Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
            Console.ReadKey();
        }
    }
    
    static void MostrarExamenes(DatosContexto db)
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("LISTADO DE EXÁMENES");
        
        var examenes = db.ResultadosExamen
            .Include(r => r.Alumno)
            .OrderBy(r => r.FechaExamen)
            .ToList();
            
        if (examenes.Count == 0)
        {
            ConsoleHelper.WriteWarning("No hay exámenes registrados.");
        }
        else
        {
            ConsoleHelper.WriteTableRow(
                ("ID".PadRight(4), ConsoleColor.Cyan),
                ("Fecha".PadRight(12), ConsoleColor.Cyan),
                ("Alumno".PadRight(20), ConsoleColor.Cyan),
                ("Correctas".PadRight(10), ConsoleColor.Cyan),
                ("Nota".PadRight(6), ConsoleColor.Cyan)
            );
            Console.WriteLine(new string('-', 56));
            
            foreach (var examen in examenes)
            {
                ConsoleHelper.WriteTableRow(
                    (examen.ResultadoExamenId.ToString().PadRight(4), ConsoleColor.DarkYellow),
                    (examen.FechaExamen.ToString("dd/MM/yyyy").PadRight(12), ConsoleColor.White),
                    ((examen.Alumno?.Nombre ?? "").PadRight(20), ConsoleColor.White),
                    ($"{examen.CantidadCorrectas}/{examen.TotalPreguntas}".PadRight(10), ConsoleColor.White),
                    (((int)examen.NotaFinal).ToString().PadRight(6), ConsoleColor.DarkYellow)
                );
            }
        }
        
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
    
    static void BuscarPorAlumno(DatosContexto db)
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("BUSCAR EXÁMENES POR ALUMNO");
        Console.Write("Nombre del alumno: ");
        string nombreBusqueda = Console.ReadLine() ?? "";
        
        string nombreBusquedaNormalizado = NormalizarTexto(nombreBusqueda);
        
        try 
        {
            // Optimizar consulta usando Join en vez de cargar todo a memoria
            var examenes = db.ResultadosExamen
                .Join(db.Alumnos,
                    r => r.AlumnoId,
                    a => a.AlumnoId,
                    (r, a) => new { Resultado = r, Alumno = a })
                .AsEnumerable() // Necesario para usar NormalizarTexto que no se puede traducir a SQL
                .Where(x => NormalizarTexto(x.Alumno?.Nombre ?? "").Contains(nombreBusquedaNormalizado))
                .OrderBy(x => x.Resultado.FechaExamen)
                .Select(x => x.Resultado)
                .ToList();
            
            Console.WriteLine();
            if (examenes.Count == 0)
            {
                ConsoleHelper.WriteWarning($"No se encontraron exámenes para '{nombreBusqueda}'.");
            }
            else
            {
                ConsoleHelper.WriteSuccess($"Exámenes encontrados para '{nombreBusqueda}':");
                ConsoleHelper.WriteTableRow(
                    ("ID".PadRight(4), ConsoleColor.Cyan),
                    ("Fecha".PadRight(12), ConsoleColor.Cyan),
                    ("Alumno".PadRight(20), ConsoleColor.Cyan),
                    ("Correctas".PadRight(10), ConsoleColor.Cyan),
                    ("Nota".PadRight(6), ConsoleColor.Cyan)
                );
                Console.WriteLine(new string('-', 56));
                
                foreach (var examen in examenes)
                {
                    ConsoleHelper.WriteTableRow(
                        (examen.ResultadoExamenId.ToString().PadRight(4), ConsoleColor.DarkYellow),
                        (examen.FechaExamen.ToString("dd/MM/yyyy").PadRight(12), ConsoleColor.White),
                        ((examen.Alumno?.Nombre ?? "").PadRight(20), ConsoleColor.White),
                        ($"{examen.CantidadCorrectas}/{examen.TotalPreguntas}".PadRight(10), ConsoleColor.White),
                        (((int)examen.NotaFinal).ToString().PadRight(6), ConsoleColor.DarkYellow)
                    );
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"Error al buscar exámenes: {ex.Message}");
        }
        
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
    
    static string NormalizarTexto(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return "";
            
        // Primero eliminamos espacios
        string sinEspacios = texto.Replace(" ", "");
        
        // Luego normalizamos (eliminamos acentos, etc.)
        string normalizedString = sinEspacios.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString().ToLower();
    }
    
    static void MostrarRanking(DatosContexto db)
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("RANKING DE ALUMNOS");
        
        var alumnos = db.Alumnos
            .Include(a => a.Resultados)
            .ToList();
        
        var ranking = alumnos
            .Select(a => new {
                Alumno = a,
                MejorNota = a.Resultados.Any() ? a.Resultados.Max(r => r.NotaFinal) : 0,
                ExamenesConMejorNota = a.Resultados
                    .Where(r => r.NotaFinal == (a.Resultados.Any() ? a.Resultados.Max(x => x.NotaFinal) : 0))
                    .Select(r => r.ResultadoExamenId)
                    .OrderBy(id => id)
                    .ToList()
            })
            .OrderByDescending(x => x.MejorNota)
            .ToList();
            
        if (ranking.Count == 0)
        {
            ConsoleHelper.WriteWarning("No hay alumnos registrados.");
        }
        else
        {
            ConsoleHelper.WriteTableRow(
                ("Posición".PadRight(8), ConsoleColor.Cyan),
                ("Alumno".PadRight(20), ConsoleColor.Cyan),
                ("Mejor Nota".PadRight(10), ConsoleColor.Cyan),
                ("Exámenes con Mejor Nota".PadRight(30), ConsoleColor.Cyan)
            );
            Console.WriteLine(new string('-', 70));
            int posicion = 1;
            foreach (var item in ranking)
            {
                string examenesTexto = item.ExamenesConMejorNota.Any() ? 
                    string.Join(", ", item.ExamenesConMejorNota.Select(id => $"Examen #{id}")) : 
                    "N/A";
                ConsoleHelper.WriteTableRow(
                    (posicion.ToString().PadRight(8), ConsoleColor.White),
                    (item.Alumno.Nombre.PadRight(20), ConsoleColor.White),
                    (((int)item.MejorNota).ToString().PadRight(10), ConsoleColor.DarkYellow),
                    (examenesTexto.PadRight(30), ConsoleColor.White)
                );
                posicion++;
            }
        }
        
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarEstadisticasPregunta(DatosContexto db)
    {
        Console.Clear();
        try
        {
            ConsoleHelper.WriteTitle("ESTADÍSTICAS POR PREGUNTA");
            
            var preguntas = db.Preguntas
                .OrderBy(p => p.PreguntaId)
                .ToList();
            
            if (preguntas.Count == 0)
            {
                ConsoleHelper.WriteWarning("No hay preguntas registradas.");
            }
            else
            {
                var respuestasExamen = db.RespuestasExamen
                    .Include(r => r.ResultadoExamen)
                    .ThenInclude(re => re.Alumno)
                    .Include(r => r.Pregunta)
                    .ToList();
                    
                if (respuestasExamen.Count == 0)
                {
                    ConsoleHelper.WriteWarning("No hay respuestas registradas para ninguna pregunta.");
                }
                else
                {
                    ConsoleHelper.WriteLineColored("DETALLE DE RESPUESTAS POR PREGUNTA:\n", ConsoleColor.Yellow);
                    
                    foreach (var pregunta in preguntas)
                    {
                        var respuestasDePregunta = respuestasExamen.Where(r => r.PreguntaId == pregunta.PreguntaId).ToList();
                        
                        if (respuestasDePregunta.Count > 0)
                        {
                            string titulo = TruncateString(pregunta.Enunciado, 50);
                            int respuestasCorrectas = respuestasDePregunta.Count(r => r.EsCorrecta);
                            double porcentajeAcierto = (double)respuestasCorrectas / respuestasDePregunta.Count * 100;
                            
                            ConsoleHelper.WriteColored($"Pregunta #{pregunta.PreguntaId:D3}", ConsoleColor.Green);
                            ConsoleHelper.WriteLineColored($" - {titulo}", ConsoleColor.White);
                            ConsoleHelper.WriteSuccess($"Veces respondida: {respuestasDePregunta.Count} (Correctas: {respuestasCorrectas}, {porcentajeAcierto:F2}%)");
                            Console.WriteLine("-----------------------------------------------------------");
                            ConsoleHelper.WriteTableRow(
                                ("ID Examen".PadRight(10), ConsoleColor.Cyan),
                                ("Fecha".PadRight(12), ConsoleColor.Cyan),
                                ("Alumno".PadRight(20), ConsoleColor.Cyan),
                                ("Respuesta".PadRight(10), ConsoleColor.Cyan),
                                ("¿Correcta?".PadRight(10), ConsoleColor.Cyan)
                            );
                            
                            foreach (var respuesta in respuestasDePregunta)
                            {
                                string fecha = respuesta.ResultadoExamen?.FechaExamen.ToString("dd/MM/yyyy") ?? "N/A";
                                string alumno = respuesta.ResultadoExamen?.Alumno?.Nombre ?? "Desconocido";
                                alumno = TruncateString(alumno, 20);
                                
                                ConsoleHelper.WriteTableRow(
                                    (respuesta.ResultadoExamenId.ToString().PadRight(10), ConsoleColor.DarkYellow),
                                    (fecha.PadRight(12), ConsoleColor.White),
                                    (TruncateString(alumno, 20).PadRight(20), ConsoleColor.White),
                                    (respuesta.RespuestaSeleccionada.PadRight(10), ConsoleColor.Yellow),
                                    (respuesta.EsCorrecta ? "✓ Sí".PadRight(10) : "✗ No".PadRight(10), respuesta.EsCorrecta ? ConsoleColor.Green : ConsoleColor.Red)
                                );
                            }
                            Console.WriteLine("-----------------------------------------------------------\n");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"Error al mostrar estadísticas: {ex.Message}");
        }
        
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey(true);
        ConsoleHelper.ResetColors();
    }
    
    static string TruncateString(string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        return str.Length <= maxLength ? str : str.Substring(0, maxLength - 3) + "...";
    }

    static void ReiniciarSistema(DatosContexto db)
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("REINICIAR SISTEMA");
        ConsoleHelper.WriteWarning("¡ATENCIÓN! Esta acción eliminará todos los exámenes, respuestas y alumnos de la base de datos.");
        ConsoleHelper.WriteWarning("Las preguntas se conservarán intactas.");
        ConsoleHelper.WriteWarning("\nEsta acción no se puede deshacer.");
        
        Console.WriteLine("\nPara confirmar, escriba 'REINICIAR': ");
        string confirmacion = Console.ReadLine() ?? "";
        if (confirmacion != "REINICIAR")
        {
            ConsoleHelper.WriteWarning("\nOperación cancelada.");
            Console.WriteLine("El sistema no ha sido reiniciado.");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }
        
        try
        {
            // Mejorar el manejo de transacciones para garantizar atomicidad
            using (var transaction = db.Database.BeginTransaction())
            {
                db.ChangeTracker.Clear();
                
                // Eliminar en el orden correcto para evitar violaciones de integridad
                db.Database.ExecuteSqlRaw("DELETE FROM RespuestasExamen");
                db.Database.ExecuteSqlRaw("DELETE FROM ResultadosExamen");
                db.Database.ExecuteSqlRaw("DELETE FROM Alumnos");
                
                // Reiniciar contadores
                db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='Alumnos'");
                db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='ResultadosExamen'");
                db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='RespuestasExamen'");
                
                transaction.Commit();
                
                Console.Clear();
                ConsoleHelper.WriteTitle("SISTEMA REINICIADO EXITOSAMENTE");
                ConsoleHelper.WriteSuccess("Todos los exámenes, respuestas y alumnos han sido eliminados.");
                ConsoleHelper.WriteSuccess($"Se han conservado {db.Preguntas.Count()} preguntas en la base de datos.");
                ConsoleHelper.WriteTitle("Estadísticas actuales");
                ConsoleHelper.WriteSuccess($"Preguntas: {db.Preguntas.Count()}");
                ConsoleHelper.WriteSuccess($"Alumnos: {db.Alumnos.Count()}");
                ConsoleHelper.WriteSuccess($"Exámenes: {db.ResultadosExamen.Count()}");
                ConsoleHelper.WriteSuccess($"Respuestas: {db.RespuestasExamen.Count()}");
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError("\nError al reiniciar el sistema:");
            ConsoleHelper.WriteError(ex.Message);
            try {
                ConsoleHelper.WriteWarning("\nIntentando limpieza de emergencia...");
                db.Database.ExecuteSqlRaw("DELETE FROM RespuestasExamen");
                db.Database.ExecuteSqlRaw("DELETE FROM ResultadosExamen");
                db.Database.ExecuteSqlRaw("DELETE FROM Alumnos");
                db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='Alumnos'");
                db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='ResultadosExamen'");
                db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='RespuestasExamen'");
                ConsoleHelper.WriteSuccess("Limpieza de emergencia completada.");
            } catch (Exception cleanupEx) {
                ConsoleHelper.WriteError($"Error en limpieza de emergencia: {cleanupEx.Message}");
            }
        }
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
    
    static void ConfigurarSistemaInicial(DatosContexto db)
    {
        Console.Clear();
        ConsoleHelper.WriteTitle("CONFIGURACIÓN INICIAL DEL SISTEMA");
        ConsoleHelper.WriteWarning("Se agregarán preguntas iniciales a la base de datos.");
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
        
        int totalPreguntas = db.Preguntas.Count();
        if (totalPreguntas < 50) {
            ConsoleHelper.WriteTitle("Configurando banco de preguntas básicas...");
            CompletarPreguntasHasta50(db);
        }
        Console.Clear();
        ConsoleHelper.WriteTitle("SISTEMA CONFIGURADO EXITOSAMENTE");
        ConsoleHelper.WriteSuccess($"Total de preguntas disponibles: {db.Preguntas.Count()}");
        ConsoleHelper.WriteSuccess("El sistema está listo para ser utilizado.");
        Console.WriteLine("\nPresione cualquier tecla para continuar al menú principal...");
        Console.ReadKey();
    }

    static void CompletarPreguntasHasta50(DatosContexto db)
    {
        // Obtener la cantidad actual de preguntas
        int preguntasActuales = db.Preguntas.Count();
        
        // Mensaje de diagnóstico
        Console.WriteLine($"Preguntas actuales en la base de datos: {preguntasActuales}");
        
        // Si ya hay suficientes preguntas, salir
        if (preguntasActuales >= 50)
        {
            Console.WriteLine("Ya hay 50 o más preguntas en la base de datos.");
            return;
        }

        // Obtener enunciados existentes para evitar duplicados
        var enunciadosExistentes = db.Preguntas
            .Select(p => p.Enunciado.ToLower().Trim())
            .ToHashSet();
        
        // Obtener banco completo de preguntas
        var preguntasBanco = ObtenerBancoDePreguntas();
        Console.WriteLine($"Banco de preguntas contiene: {preguntasBanco.Count} preguntas");
        
        // Asegurar transacción para la inserción
        using (var transaction = db.Database.BeginTransaction())
        {
            try
            {
                // Filtrar y obtener preguntas a insertar
                int preguntasAgregadas = 0;
                
                // Procesar cada pregunta individualmente para mejor control
                foreach (var pregunta in preguntasBanco)
                {
                    // Verificar si la pregunta ya existe
                    if (!enunciadosExistentes.Contains(pregunta.Enunciado.ToLower().Trim()))
                    {
                        // Agregar pregunta
                        db.Preguntas.Add(pregunta);
                        db.SaveChanges();
                        enunciadosExistentes.Add(pregunta.Enunciado.ToLower().Trim());
                        preguntasAgregadas++;
                        
                        // Si llegamos a 50 preguntas, detenemos el proceso
                        if (preguntasActuales + preguntasAgregadas >= 50)
                            break;
                    }
                }

                // Confirmar transacción
                transaction.Commit();
                Console.WriteLine($"Se agregaron {preguntasAgregadas} preguntas a la base de datos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar preguntas: {ex.Message}");
                transaction.Rollback();
            }
        }
        
        // Verificación final
        int preguntasFinales = db.Preguntas.Count();
        Console.WriteLine($"Total preguntas después de la carga: {preguntasFinales}");
    }

    static List<Pregunta> ObtenerBancoDePreguntas()
    {
        return new List<Pregunta>
        {
            // Nuevas preguntas proporcionadas
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para declarar una constante en C#?", RespuestaA = "static", RespuestaB = "const", RespuestaC = "final", Correcta = "B" },
            new Pregunta { Enunciado = "¿Cuál es el resultado de Console.WriteLine(3 + 4 * 2);?", RespuestaA = "14", RespuestaB = "11", RespuestaC = "10", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué método se utiliza para convertir una cadena en un número entero?", RespuestaA = "int.Parse()", RespuestaB = "ToString()", RespuestaC = "Convert.ToChar()", Correcta = "A" },
            new Pregunta { Enunciado = "¿Cómo defines un método en C# que no devuelve ningún valor?", RespuestaA = "void", RespuestaB = "return", RespuestaC = "static", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué operador se utiliza para la comparación de igualdad en C#?", RespuestaA = "==", RespuestaB = "=", RespuestaC = "!=", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué se imprime al ejecutar este código?\nint x = 5; \nint y = 3;\nConsole.WriteLine(x % y);", RespuestaA = "2", RespuestaB = "1", RespuestaC = "0", Correcta = "A" },
            new Pregunta { Enunciado = "¿Cuál es el valor predeterminado de una variable bool?", RespuestaA = "true", RespuestaB = "false", RespuestaC = "null", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para manejar excepciones en C#?", RespuestaA = "catch", RespuestaB = "throw", RespuestaC = "error", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué devuelve el siguiente código?\nstring name = \"C#\";\nConsole.WriteLine(name.Length);", RespuestaA = "1", RespuestaB = "2", RespuestaC = "3", Correcta = "B" },
            new Pregunta { Enunciado = "¿Cuál de las siguientes es una sentencia válida para crear un bucle for en C#?", RespuestaA = "for int i = 0; i < 10; i++ {}", RespuestaB = "for (int i = 0; i < 10; i++) {}", RespuestaC = "for i = 0 to 10 {}", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué palabra clave se usa para salir de un bucle en C#?", RespuestaA = "exit", RespuestaB = "break", RespuestaC = "continue", Correcta = "B" },
            new Pregunta { Enunciado = "¿Cuál es el resultado de Console.WriteLine(10 / 3);?", RespuestaA = "3.33", RespuestaB = "3", RespuestaC = "4", Correcta = "B" },
            new Pregunta { Enunciado = "¿En qué tipo de dato se almacena el valor 3.14 en C#?", RespuestaA = "float", RespuestaB = "double", RespuestaC = "int", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué se imprime al ejecutar este código?\nstring text = \"Hola\";\nConsole.WriteLine(text[1]);", RespuestaA = "H", RespuestaB = "o", RespuestaC = "l", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para declarar una clase en C#?", RespuestaA = "new", RespuestaB = "class", RespuestaC = "this", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué salida genera el siguiente código?\nint[] arr = {1, 2, 3};\nConsole.WriteLine(arr[1]);", RespuestaA = "1", RespuestaB = "2", RespuestaC = "3", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué operador se utiliza para concatenar cadenas en C#?", RespuestaA = "+", RespuestaB = "&", RespuestaC = "*", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué imprime este código?\nint a = 10;\nint b = 20;\nConsole.WriteLine(a > b ? \"Mayor\" : \"Menor\");", RespuestaA = "Mayor", RespuestaB = "Menor", RespuestaC = "Error", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué significa la palabra clave static en C#?", RespuestaA = "Que un método o variable pertenece a la instancia de la clase.", RespuestaB = "Que un método o variable pertenece a la clase en sí.", RespuestaC = "No tiene significado en C#.", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué imprime este código?\nint a = 5;\nConsole.WriteLine(++a);", RespuestaA = "5", RespuestaB = "6", RespuestaC = "7", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué sentencia se usa para devolver un valor desde un método?", RespuestaA = "return", RespuestaB = "break", RespuestaC = "continue", Correcta = "A" },
            new Pregunta { Enunciado = "¿Cómo defines un comentario de una sola línea en C#?", RespuestaA = "/* */", RespuestaB = "//", RespuestaC = "<!-- -->", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué hace el siguiente código?\nint[] nums = {1, 2, 3};\nforeach (int n in nums)\n{\n    Console.Write(n);\n}", RespuestaA = "Imprime 123", RespuestaB = "Imprime 321", RespuestaC = "Genera un error.", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué palabra clave ejecuta siempre un bloque de código después del try y catch?", RespuestaA = "finally", RespuestaB = "end", RespuestaC = "final", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué se imprime al ejecutar este código?\nConsole.WriteLine(Math.Pow(2, 3));", RespuestaA = "6", RespuestaB = "8", RespuestaC = "9", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué se imprime al ejecutar el siguiente código?\nConsole.WriteLine(10 == 10);", RespuestaA = "true", RespuestaB = "false", RespuestaC = "null", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para heredar en C#?", RespuestaA = "extends", RespuestaB = "inherits", RespuestaC = ":", Correcta = "C" },
            new Pregunta { Enunciado = "¿Qué salida da este código?\nConsole.WriteLine(5 / 2.0);", RespuestaA = "2", RespuestaB = "2.5", RespuestaC = "Error", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué significa la palabra clave this en C#?", RespuestaA = "Hace referencia a la clase padre.", RespuestaB = "Hace referencia a la instancia actual.", RespuestaC = "No tiene significado en C#.", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué operador se usa para acceder a los miembros de una clase?", RespuestaA = ". (punto)", RespuestaB = "->", RespuestaC = "::", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué resultado genera este código?\nint x = 10;\nx += 5;\nConsole.WriteLine(x);", RespuestaA = "10", RespuestaB = "15", RespuestaC = "5", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué palabra clave evita que una clase sea heredada en C#?", RespuestaA = "sealed", RespuestaB = "final", RespuestaC = "static", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué salida da este código?\nint[] nums = {2, 4, 6};\nConsole.WriteLine(nums.Length);", RespuestaA = "2", RespuestaB = "3", RespuestaC = "4", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para declarar una propiedad en una clase C#?", RespuestaA = "get", RespuestaB = "set", RespuestaC = "Ambas", Correcta = "C" },
            new Pregunta { Enunciado = "¿Qué resultado da este código?\nint a = 5;\nint b = 3;\nConsole.WriteLine(a * b);", RespuestaA = "8", RespuestaB = "15", RespuestaC = "5", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para definir una interfaz en C#?", RespuestaA = "interface", RespuestaB = "class", RespuestaC = "abstract", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué salida genera este código?\nint a = 10;\nif (a > 5)\n    Console.WriteLine(\"Mayor\");\nelse\n    Console.WriteLine(\"Menor\");", RespuestaA = "Mayor", RespuestaB = "Menor", RespuestaC = "Error", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué resultado genera este código?\nstring result = string.Concat(\"Hola\", \" \", \"Mundo\");\nConsole.WriteLine(result);", RespuestaA = "Hola Mundo", RespuestaB = "HolaMundo", RespuestaC = "Error", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué método se utiliza para ordenar una lista en C#?", RespuestaA = "Sort()", RespuestaB = "Order()", RespuestaC = "Arrange()", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué imprime este código?\nint x = 5;\nint y = 10;\nConsole.WriteLine(x < y && y > 5);", RespuestaA = "true", RespuestaB = "false", RespuestaC = "Error", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué tipo de dato se utiliza para almacenar texto en C#?", RespuestaA = "char", RespuestaB = "string", RespuestaC = "text", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué operador se utiliza para acceder a un miembro estático de una clase?", RespuestaA = ".", RespuestaB = "::", RespuestaC = "->", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué resultado da este código?\nint x = 7 % 2;\nConsole.WriteLine(x);", RespuestaA = "1", RespuestaB = "0", RespuestaC = "2", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué salida genera este código?\nint a = 5;\nint b = 10;\nConsole.WriteLine(a > b ? \"Mayor\" : \"Menor\");", RespuestaA = "Mayor", RespuestaB = "Menor", RespuestaC = "Error", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para declarar una enumeración en C#?", RespuestaA = "enum", RespuestaB = "list", RespuestaC = "enumerate", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué resultado genera este código?\nConsole.WriteLine(Math.Sqrt(16));", RespuestaA = "4", RespuestaB = "8", RespuestaC = "16", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué se imprime al ejecutar este código?\nstring s = \"C# es genial!\";\nConsole.WriteLine(s.ToUpper());", RespuestaA = "c# es genial!", RespuestaB = "C# ES GENIAL!", RespuestaC = "Error", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué salida genera este código?\nint a = 15;\nint b = 5;\nConsole.WriteLine(a / b);", RespuestaA = "3", RespuestaB = "5", RespuestaC = "15", Correcta = "A" },
            new Pregunta { Enunciado = "¿Cuál es el valor predeterminado de un atributo de tipo int en C#?", RespuestaA = "0", RespuestaB = "null", RespuestaC = "-1", Correcta = "A" },
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para declarar un iterador en C#?", RespuestaA = "yield", RespuestaB = "return", RespuestaC = "continue", Correcta = "A" }
        };
    }

    // Métodos para manejar los colores en la consola
    static class ConsoleHelper
    {
        // Guarda el color original
        private static readonly ConsoleColor DefaultForeground = Console.ForegroundColor;
        private static readonly ConsoleColor DefaultBackground = Console.BackgroundColor;
        
        // Método para escribir texto con un color específico
        public static void WriteColored(string text, ConsoleColor foreground)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = foreground;
            Console.Write(text);
            Console.ForegroundColor = original;
        }
        
        // Método para escribir texto con color y salto de línea
        public static void WriteLineColored(string text, ConsoleColor foreground)
        {
            WriteColored(text + "\n", foreground);
        }
        
        // Método para restaurar los colores por defecto
        public static void ResetColors()
        {
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;
        }
        
        // Método para mostrar un título
        public static void WriteTitle(string title)
        {
            Console.WriteLine();
            WriteLineColored("=".PadRight(40, '='), ConsoleColor.Cyan);
            WriteLineColored($"=== {title} ===", ConsoleColor.Cyan);
            WriteLineColored("=".PadRight(40, '='), ConsoleColor.Cyan);
            Console.WriteLine();
        }
        
        // Método para mostrar una opción del menú
        public static void WriteMenuOption(string key, string description)
        {
            WriteColored($"[{key}] ", ConsoleColor.Green);
            WriteLineColored(description, ConsoleColor.White);
        }
        
        // Método para mostrar un mensaje de éxito
        public static void WriteSuccess(string message)
        {
            WriteLineColored(message, ConsoleColor.Green);
        }
        
        // Método para mostrar un mensaje de error
        public static void WriteError(string message)
        {
            WriteLineColored(message, ConsoleColor.Red);
        }
        
        // Método para mostrar un mensaje de advertencia
        public static void WriteWarning(string message)
        {
            WriteLineColored(message, ConsoleColor.Yellow);
        }
        
        // Método para mostrar una fila de tabla con colores
        public static void WriteTableRow(params (string Text, ConsoleColor Color)[] columns)
        {
            foreach (var column in columns)
            {
                WriteColored(column.Text, column.Color);
            }
            Console.WriteLine();
        }
        
        // Método para mostrar indicadores de correcto/incorrecto
        public static void WriteCorrectStatus(bool isCorrect)
        {
            if (isCorrect)
            {
                WriteColored("✓ Sí", ConsoleColor.Green);
            }
            else
            {
                WriteColored("✗ No", ConsoleColor.Red);
            }
        }
    }
} // Final de la clase Program