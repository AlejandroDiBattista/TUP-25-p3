using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

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
    static void Main(string[] args){
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
        
        using (var db = new DatosContexto()){
            db.Database.EnsureCreated();
            
            // Restaurar la configuración inicial si la base de datos está vacía
            bool baseDatosVacia = !db.Preguntas.Any();
            if (baseDatosVacia) {
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
                    .AsEnumerable() // Materializa la consulta - más eficiente que ToList() para este caso
                    .FirstOrDefault(a => NormalizarTexto(a.Nombre).Equals(nombreNormalizado));
                
                if (alumno == null)
                {
                    // Si no existe el alumno, crearlo con el nombre original (preservando formato)
                    alumno = new Alumno { Nombre = nombreAlumno };
                    db.Alumnos.Add(alumno);
                    db.SaveChanges();
                    ConsoleHelper.WriteSuccess($"Se ha creado un nuevo alumno: {nombreAlumno}");
                }
                else
                {
                    ConsoleHelper.WriteSuccess($"Bienvenido de nuevo, {alumno.Nombre}!");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
                
                // Obtener preguntas respondidas utilizando Join en vez de Include para mejor rendimiento
                var preguntasRespondidasIds = db.RespuestasExamen
                    .Join(db.ResultadosExamen,
                        r => r.ResultadoExamenId,
                        re => re.ResultadoExamenId,
                        (r, re) => new { Respuesta = r, Resultado = re })
                    .Where(x => x.Resultado.AlumnoId == alumno.AlumnoId)
                    .Select(x => x.Respuesta.PreguntaId)
                    .Distinct()
                    .ToList();
                
                var preguntasDisponibles = db.Preguntas
                    .Where(p => !preguntasRespondidasIds.Contains(p.PreguntaId))
                    .ToList();
                
                int preguntasDisponiblesCount = preguntasDisponibles.Count();
                if (preguntasDisponiblesCount == 0)
                {
                    Console.Clear();
                    ConsoleHelper.WriteTitle("TODAS LAS PREGUNTAS COMPLETADAS");
                    Console.WriteLine($"¡Felicidades {alumno.Nombre}! Has respondido todas las preguntas disponibles.");
                    Console.WriteLine("No quedan preguntas nuevas para mostrarte.");
                    
                    var respuestasAlumno = db.RespuestasExamen
                        .Join(db.ResultadosExamen,
                            r => r.ResultadoExamenId,
                            re => re.ResultadoExamenId,
                            (r, re) => new { Respuesta = r, Resultado = re })
                        .Where(x => x.Resultado.AlumnoId == alumno.AlumnoId)
                        .Select(x => x.Respuesta)
                        .ToList();
                    
                    int totalPreguntasRespondidas = respuestasAlumno.Count;
                    int respuestasCorrectas = respuestasAlumno.Count(r => r.EsCorrecta);
                    int respuestasIncorrectas = totalPreguntasRespondidas - respuestasCorrectas;
                    int puntosObtenidos = totalPreguntasRespondidas > 0 ? (respuestasCorrectas * 100) / totalPreguntasRespondidas : 0;
                    double porcentajeAciertos = totalPreguntasRespondidas > 0 ? (double)respuestasCorrectas / totalPreguntasRespondidas * 100 : 0;
                    
                    Console.WriteLine("\nResumen de tu desempeño:");
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine($"Total de preguntas respondidas: {totalPreguntasRespondidas}");
                    Console.WriteLine($"Respuestas correctas: {respuestasCorrectas}");
                    Console.WriteLine($"Respuestas incorrectas: {respuestasIncorrectas}");
                    Console.WriteLine($"Puntos obtenidos: {puntosObtenidos} de 100");
                    Console.WriteLine($"Porcentaje de aciertos: {porcentajeAciertos:F2}%");
                    
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }
                
                int preguntasExamen = Math.Min(5, preguntasDisponiblesCount);
                Console.WriteLine($"Preguntas disponibles para este examen: {preguntasDisponiblesCount}");
                
                var preguntasAleatorias = preguntasDisponibles
                    .OrderBy(p => Guid.NewGuid())
                    .Take(preguntasExamen)
                    .ToList();
                
                var resultadoExamen = new ResultadoExamen {
                    AlumnoId = alumno.AlumnoId,
                    FechaExamen = DateTime.Now,
                    TotalPreguntas = preguntasExamen,
                    CantidadCorrectas = 0
                };
                db.ResultadosExamen.Add(resultadoExamen);
                db.SaveChanges();
                
                Console.WriteLine($"\nExamen de {preguntasExamen} preguntas nuevas");
                int correctas = 0;
                foreach (var pregunta in preguntasAleatorias)
                {
                    Console.Clear();
                    ConsoleHelper.WriteTitle($"Pregunta {preguntasAleatorias.IndexOf(pregunta) + 1}/{preguntasExamen}");
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
                
                decimal notaFinal = preguntasExamen > 0 ? (decimal)correctas / preguntasExamen * 10 : 0;
                
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
        
        try 
        {
            Console.Write("Nombre del alumno: ");
            string nombreBusqueda = Console.ReadLine() ?? "";
            
            string nombreBusquedaNormalizado = NormalizarTexto(nombreBusqueda);
            
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
                    // Obtener el nombre del alumno de forma más eficiente
                    string nombreAlumno = db.Alumnos
                        .Where(a => a.AlumnoId == examen.AlumnoId)
                        .Select(a => a.Nombre)
                        .FirstOrDefault() ?? "";
                        
                    ConsoleHelper.WriteTableRow(
                        (examen.ResultadoExamenId.ToString().PadRight(4), ConsoleColor.DarkYellow),
                        (examen.FechaExamen.ToString("dd/MM/yyyy").PadRight(12), ConsoleColor.White),
                        (nombreAlumno.PadRight(20), ConsoleColor.White),
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
        var enunciadosExistentes = db.Preguntas
            .Select(p => p.Enunciado.ToLower().Trim())
            .ToHashSet();
            
        int preguntasExistentes = enunciadosExistentes.Count;
        int preguntasFaltantes = 50 - preguntasExistentes;
        
        if (preguntasFaltantes <= 0)
            return;
        
        var preguntasBanco = ObtenerBancoDePreguntas();
        
        var preguntasAInsertar = preguntasBanco
            .Where(p => !enunciadosExistentes.Contains(p.Enunciado.ToLower().Trim()))
            .Take(preguntasFaltantes)
            .ToList();
        
        if (preguntasAInsertar.Any())
        {
            db.Preguntas.AddRange(preguntasAInsertar);
            db.SaveChanges();
            ConsoleHelper.WriteSuccess($"Se agregaron {preguntasAInsertar.Count} preguntas iniciales.");
        } else {
            ConsoleHelper.WriteWarning("No se agregaron preguntas nuevas (posiblemente ya existen o el banco está vacío).");
        }
    }

    static List<Pregunta> ObtenerBancoDePreguntas()
    {
        return new List<Pregunta>
        {
            new Pregunta { Enunciado = "¿Qué palabra clave se utiliza para definir una clase en C#?", RespuestaA = "class", RespuestaB = "struct", RespuestaC = "type", Correcta = "A" },
            new Pregunta { Enunciado = "¿Cuál es el operador de asignación en C#?", RespuestaA = "==", RespuestaB = "=", RespuestaC = ":=", Correcta = "B" },
            new Pregunta { Enunciado = "¿Qué tipo de dato usarías para almacenar un número entero?", RespuestaA = "string", RespuestaB = "float", RespuestaC = "int", Correcta = "C" },
            new Pregunta { Enunciado = "En C#, ¿qué palabra clave se usa para heredar de una clase?", RespuestaA = "inherits", RespuestaB = ":", RespuestaC = "extends", Correcta = "B" },
            new Pregunta { Enunciado = "¿Cuál es el punto de entrada principal de una aplicación de consola C#?", RespuestaA = "Start()", RespuestaB = "Main()", RespuestaC = "Run()", Correcta = "B" },
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