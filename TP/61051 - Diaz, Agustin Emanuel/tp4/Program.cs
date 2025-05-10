using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// Definición de los modelos
class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
    
    // Relación con RespuestaExamen
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime FechaExamen { get; set; }
    
    // Relación con RespuestaExamen
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    
    // Relación con Pregunta
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    
    // Relación con ResultadoExamen
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    
    public bool EsCorrecta { get; set; }
    public string RespuestaSeleccionada { get; set; } = "";
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; } = null!;
    public DbSet<ResultadoExamen> ResultadosExamenes { get; set; } = null!;
    public DbSet<RespuestaExamen> RespuestasExamenes { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Configuración de relaciones
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.Respuestas)
            .HasForeignKey(r => r.PreguntaId);
            
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);
    }
}

class Program {
    static void Main(string[] args) {
        // Inicializar la base de datos y cargar datos iniciales
        InicializarBaseDeDatos();
        
        bool salir = false;
        while (!salir) {
            MostrarMenuPrincipal();
            string opcion = Console.ReadLine() ?? "";
            
            switch (opcion) {
                case "1":
                    RegistrarPregunta();
                    break;
                case "2":
                    TomarExamen();
                    break;
                case "3":
                    MostrarResultados();
                    break;
                case "4":
                    MostrarEstadisticasPorPregunta();
                    break;
                case "5":
                    MostrarRanking();
                    break;
                case "6":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    
    static void InicializarBaseDeDatos() {
        using (var db = new DatosContexto()) {
            db.Database.EnsureCreated();
            
            // Verificar si ya hay preguntas registradas
            if (!db.Preguntas.Any()) {
                // Cargar algunas preguntas iniciales
                var preguntas = new List<Pregunta> {
                    new Pregunta {
                        Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                        RespuestaA = "Java",
                        RespuestaB = "C#",
                        RespuestaC = "Python",
                        Correcta = "B"
                    },
                    new Pregunta {
                        Enunciado = "¿Qué significa ORM en el contexto de bases de datos?",
                        RespuestaA = "Object-Relational Mapping",
                        RespuestaB = "Object-Related Model",
                        RespuestaC = "Operational Resource Management",
                        Correcta = "A"
                    },
                    new Pregunta {
                        Enunciado = "¿Qué es Entity Framework?",
                        RespuestaA = "Un lenguaje de programación",
                        RespuestaB = "Un sistema operativo",
                        RespuestaC = "Un ORM para .NET",
                        Correcta = "C"
                    },
                    new Pregunta {
                        Enunciado = "¿Qué método se usa para asegurar que una base de datos existe en Entity Framework Core?",
                        RespuestaA = "CreateDatabase()",
                        RespuestaB = "EnsureCreated()",
                        RespuestaC = "InitializeDatabase()",
                        Correcta = "B"
                    },
                    new Pregunta {
                        Enunciado = "¿Cuál de estos NO es un tipo de base de datos compatible con Entity Framework Core?",
                        RespuestaA = "SQLite",
                        RespuestaB = "SQL Server",
                        RespuestaC = "MongoDB",
                        Correcta = "C"
                    }
                };
                
                db.Preguntas.AddRange(preguntas);
                db.SaveChanges();
            }
        }
    }
    
    static void MostrarMenuPrincipal() {
        Console.Clear();
        Console.WriteLine("===============================================");
        Console.WriteLine("      SISTEMA DE EVALUACIÓN DE ALUMNOS        ");
        Console.WriteLine("===============================================");
        Console.WriteLine("1. Registrar nueva pregunta");
        Console.WriteLine("2. Tomar examen");
        Console.WriteLine("3. Ver resultados de exámenes");
        Console.WriteLine("4. Ver estadísticas por pregunta");
        Console.WriteLine("5. Ver ranking de alumnos");
        Console.WriteLine("6. Salir");
        Console.WriteLine("===============================================");
        Console.Write("Seleccione una opción: ");
    }
    
    static void RegistrarPregunta() {
        Console.Clear();
        Console.WriteLine("===============================================");
        Console.WriteLine("         REGISTRO DE NUEVA PREGUNTA           ");
        Console.WriteLine("===============================================");
        
        Console.Write("Enunciado de la pregunta: ");
        string enunciado = Console.ReadLine() ?? "";
        
        Console.Write("Respuesta A: ");
        string respuestaA = Console.ReadLine() ?? "";
        
        Console.Write("Respuesta B: ");
        string respuestaB = Console.ReadLine() ?? "";
        
        Console.Write("Respuesta C: ");
        string respuestaC = Console.ReadLine() ?? "";
        
        string correcta = "";
        bool respuestaValida = false;
        
        while (!respuestaValida) {
            Console.Write("Respuesta correcta (A, B o C): ");
            correcta = (Console.ReadLine() ?? "").ToUpper();
            
            if (correcta == "A" || correcta == "B" || correcta == "C") {
                respuestaValida = true;
            } else {
                Console.WriteLine("Por favor, ingrese una opción válida (A, B o C).");
            }
        }
        
        var nuevaPregunta = new Pregunta {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta
        };
        
        using (var db = new DatosContexto()) {
            db.Preguntas.Add(nuevaPregunta);
            db.SaveChanges();
        }
        
        Console.WriteLine("\nPregunta registrada correctamente. Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
    
    static void TomarExamen() {
        Console.Clear();
        Console.WriteLine("===============================================");
        Console.WriteLine("                 TOMAR EXAMEN                 ");
        Console.WriteLine("===============================================");
        
        Console.Write("Nombre del alumno: ");
        string nombreAlumno = Console.ReadLine() ?? "";
        
        // Validar que se haya ingresado un nombre
        if (string.IsNullOrWhiteSpace(nombreAlumno)) {
            Console.WriteLine("Debe ingresar un nombre válido. Presione cualquier tecla para volver al menú principal...");
            Console.ReadKey();
            return;
        }
        
        using (var db = new DatosContexto()) {
            // Obtener todas las preguntas disponibles
            var todasLasPreguntas = db.Preguntas.ToList();
            
            if (todasLasPreguntas.Count == 0) {
                Console.WriteLine("No hay preguntas registradas en el sistema. Agregue preguntas antes de tomar un examen.");
                Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
                Console.ReadKey();
                return;
            }
            
            // Determinar cuántas preguntas usar (máximo 5)
            int totalPreguntas = Math.Min(5, todasLasPreguntas.Count);
            
            // Seleccionar preguntas aleatorias
            var random = new Random();
            var preguntasExamen = todasLasPreguntas
                .OrderBy(p => random.Next())
                .Take(totalPreguntas)
                .ToList();
            
            // Crear el resultado del examen
            var resultadoExamen = new ResultadoExamen {
                NombreAlumno = nombreAlumno,
                TotalPreguntas = totalPreguntas,
                RespuestasCorrectas = 0,
                FechaExamen = DateTime.Now,
                NotaFinal = 0
            };
            
            db.ResultadosExamenes.Add(resultadoExamen);
            db.SaveChanges();
            
            // Tomar el examen pregunta por pregunta
            foreach (var pregunta in preguntasExamen) {
                Console.Clear();
                Console.WriteLine($"Pregunta {preguntasExamen.IndexOf(pregunta) + 1} de {totalPreguntas}");
                Console.WriteLine("===============================================");
                Console.WriteLine($"\n{pregunta.Enunciado}\n");
                Console.WriteLine($" A) {pregunta.RespuestaA}");
                Console.WriteLine($" B) {pregunta.RespuestaB}");
                Console.WriteLine($" C) {pregunta.RespuestaC}");
                Console.WriteLine("===============================================");
                
                // Solicitar respuesta del alumno
                string respuesta = "";
                bool respuestaValida = false;
                
                while (!respuestaValida) {
                    Console.Write("\nSeleccione su respuesta (A, B o C): ");
                    respuesta = (Console.ReadLine() ?? "").ToUpper();
                    
                    if (respuesta == "A" || respuesta == "B" || respuesta == "C") {
                        respuestaValida = true;
                    } else {
                        Console.WriteLine("Por favor, ingrese una opción válida (A, B o C).");
                    }
                }
                
                // Verificar si la respuesta es correcta
                bool esCorrecta = respuesta == pregunta.Correcta;
                
                // Si es correcta, incrementar el contador
                if (esCorrecta) {
                    resultadoExamen.RespuestasCorrectas++;
                }
                
                // Registrar la respuesta
                var respuestaExamen = new RespuestaExamen {
                    PreguntaId = pregunta.PreguntaId,
                    ResultadoExamenId = resultadoExamen.ResultadoExamenId,
                    EsCorrecta = esCorrecta,
                    RespuestaSeleccionada = respuesta
                };
                
                db.RespuestasExamenes.Add(respuestaExamen);
            }
            
            // Calcular nota final (sobre 5)
            resultadoExamen.NotaFinal = (double)resultadoExamen.RespuestasCorrectas * 5 / totalPreguntas;
            
            // Guardar los cambios
            db.SaveChanges();
            
            // Mostrar resultado final
            Console.Clear();
            Console.WriteLine("===============================================");
            Console.WriteLine("             RESULTADO DEL EXAMEN             ");
            Console.WriteLine("===============================================");
            Console.WriteLine($"Alumno: {resultadoExamen.NombreAlumno}");
            Console.WriteLine($"Respuestas correctas: {resultadoExamen.RespuestasCorrectas} de {totalPreguntas}");
            Console.WriteLine($"Nota final: {resultadoExamen.NotaFinal:F1} de 5.0");
            Console.WriteLine("===============================================");
            Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
            Console.ReadKey();
        }
    }
    
    static void MostrarResultados() {
        Console.Clear();
        Console.WriteLine("===============================================");
        Console.WriteLine("          RESULTADOS DE EXÁMENES              ");
        Console.WriteLine("===============================================");
        
        Console.WriteLine("Filtrar por nombre (deje en blanco para ver todos): ");
        string filtro = Console.ReadLine() ?? "";
        
        using (var db = new DatosContexto()) {
            var query = db.ResultadosExamenes.AsQueryable();
            
            // Aplicar filtro si se especificó
            if (!string.IsNullOrWhiteSpace(filtro)) {
                query = query.Where(r => r.NombreAlumno.Contains(filtro));
            }
            
            // Obtener resultados ordenados por fecha (más reciente primero)
            var resultados = query.OrderByDescending(r => r.FechaExamen).ToList();
            
            if (resultados.Count == 0) {
                Console.WriteLine("\nNo se encontraron resultados para el filtro especificado.");
            } else {
                Console.WriteLine("\nID  | Fecha            | Alumno          | Correctas | Total | Nota");
                Console.WriteLine("-----------------------------------------------------------------------");
                
                foreach (var resultado in resultados) {
                    Console.WriteLine($"{resultado.ResultadoExamenId:D3} | {resultado.FechaExamen:dd/MM/yyyy HH:mm} | {resultado.NombreAlumno.PadRight(15)} | {resultado.RespuestasCorrectas}/{resultado.TotalPreguntas}        | {resultado.TotalPreguntas}     | {resultado.NotaFinal:F1}");
                }
            }
        }
        
        Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
        Console.ReadKey();
    }
    
    static void MostrarEstadisticasPorPregunta() {
        Console.Clear();
        Console.WriteLine("===============================================");
        Console.WriteLine("       ESTADÍSTICAS POR PREGUNTA              ");
        Console.WriteLine("===============================================");
        
        using (var db = new DatosContexto()) {
            // Cargar todas las preguntas con sus respuestas relacionadas
            var preguntas = db.Preguntas
                .Include(p => p.Respuestas)
                .ToList();
            
            if (preguntas.Count == 0) {
                Console.WriteLine("\nNo hay preguntas registradas en el sistema.");
            } else {
                foreach (var pregunta in preguntas) {
                    int totalRespuestas = pregunta.Respuestas.Count;
                    int respuestasCorrectas = pregunta.Respuestas.Count(r => r.EsCorrecta);
                    double porcentajeAcierto = totalRespuestas > 0 ? (double)respuestasCorrectas / totalRespuestas * 100 : 0;
                    
                    Console.WriteLine($"\nPregunta ID: {pregunta.PreguntaId}");
                    Console.WriteLine($"Enunciado: {pregunta.Enunciado}");
                    Console.WriteLine($"Veces respondida: {totalRespuestas}");
                    Console.WriteLine($"Respuestas correctas: {respuestasCorrectas}");
                    Console.WriteLine($"Porcentaje de acierto: {porcentajeAcierto:F1}%");
                    Console.WriteLine("-----------------------------------------------");
                }
            }
        }
        
        Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
        Console.ReadKey();
    }
    
    static void MostrarRanking() {
        Console.Clear();
        Console.WriteLine("===============================================");
        Console.WriteLine("             RANKING DE ALUMNOS                ");
        Console.WriteLine("===============================================");
        
        using (var db = new DatosContexto()) {
            // Obtener los mejores resultados por alumno (la mejor nota de cada uno)
            var mejoresResultados = db.ResultadosExamenes
                .GroupBy(r => r.NombreAlumno)
                .Select(g => g.OrderByDescending(r => r.NotaFinal).First())
                .OrderByDescending(r => r.NotaFinal)
                .ToList();
            
            if (mejoresResultados.Count == 0) {
                Console.WriteLine("\nNo hay resultados de exámenes registrados en el sistema.");
            } else {
                Console.WriteLine("\nPosición | Alumno          | Mejor Nota | Fecha");
                Console.WriteLine("--------------------------------------------------");
                
                for (int i = 0; i < mejoresResultados.Count; i++) {
                    var resultado = mejoresResultados[i];
                    Console.WriteLine($"{i + 1,8} | {resultado.NombreAlumno.PadRight(15)} | {resultado.NotaFinal:F1}/5.0    | {resultado.FechaExamen:dd/MM/yyyy}");
                }
            }
        }
        
        Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
        Console.ReadKey();
    }
}