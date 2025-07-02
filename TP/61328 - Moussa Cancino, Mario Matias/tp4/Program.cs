using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// Clase Pregunta (basada en el código original)
class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = ""; // 'A', 'B' o 'C'

    // Relación con RespuestaExamen
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

// Nueva clase para almacenar resultados de exámenes
class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    
    public string NombreAlumno { get; set; } = "";
    
    public int RespuestasCorrectas { get; set; }
    
    public int TotalPreguntas { get; set; }
    
    public double NotaFinal { get; set; }
    
    public DateTime FechaExamen { get; set; }
    
    // Relación con RespuestaExamen
    public List<RespuestaExamen> RespuestasExamen { get; set; } = new List<RespuestaExamen>();
}

// Nueva clase para almacenar las respuestas individuales de cada examen
class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    
    // Relación con ResultadoExamen
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    
    // Relación con Pregunta
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    
    public string RespuestaDada { get; set; } = ""; // 'A', 'B' o 'C'
    
    public bool EsCorrecta { get; set; }
}

// Contexto de datos expandido (basado en el código original)
class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Configuramos las relaciones entre entidades
        
        // Relación entre ResultadoExamen y RespuestaExamen (uno a muchos)
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.RespuestasExamen)
            .HasForeignKey(r => r.ResultadoExamenId);
        
        // Relación entre Pregunta y RespuestaExamen (uno a muchos)
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.Respuestas)
            .HasForeignKey(r => r.PreguntaId);
    }
}

// Clase Program principal
class Program {
    static void Main(string[] args) {
        ConfigurarBaseDeDatos();
        
        bool salir = false;
        while (!salir) {
            MostrarMenuPrincipal();
            var opcion = Console.ReadLine();
            
            switch (opcion) {
                case "1":
                    RegistrarPregunta();
                    break;
                case "2":
                    TomarExamen();
                    break;
                case "3":
                    MostrarReportes();
                    break;
                case "4":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida. Presione Enter para continuar...");
                    Console.ReadLine();
                    break;
            }
            
            Console.Clear();
        }
    }
    
    static void ConfigurarBaseDeDatos() {
        using (var db = new DatosContexto()) {
            db.Database.EnsureCreated();
            
            // Agregar algunas preguntas iniciales si no hay ninguna
            if (!db.Preguntas.Any()) {
                var preguntasIniciales = new List<Pregunta> {
                    new Pregunta {
                        Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                        RespuestaA = "Java",
                        RespuestaB = "C#",
                        RespuestaC = "Python",
                        Correcta = "B"
                    },
                    new Pregunta {
                        Enunciado = "¿Qué significa OOP?",
                        RespuestaA = "Object Oriented Programming",
                        RespuestaB = "Ordered Output Processing",
                        RespuestaC = "Online Operating Platform",
                        Correcta = "A"
                    },
                    new Pregunta {
                        Enunciado = "¿Cuál de los siguientes NO es un tipo de datos en C#?",
                        RespuestaA = "int",
                        RespuestaB = "char",
                        RespuestaC = "object[]",
                        Correcta = "C"
                    },
                    new Pregunta {
                        Enunciado = "¿Qué es Entity Framework?",
                        RespuestaA = "Un lenguaje de programación",
                        RespuestaB = "Un ORM para .NET",
                        RespuestaC = "Un sistema operativo",
                        Correcta = "B"
                    },
                    new Pregunta {
                        Enunciado = "¿Qué estructura de datos funciona con el principio LIFO?",
                        RespuestaA = "Cola (Queue)",
                        RespuestaB = "Pila (Stack)",
                        RespuestaC = "Lista (List)",
                        Correcta = "B"
                    }
                };
                
                db.Preguntas.AddRange(preguntasIniciales);
                db.SaveChanges();
            }
        }
    }
    
    static void MostrarMenuPrincipal() {
        Console.WriteLine("=== SISTEMA DE EXÁMENES ===");
        Console.WriteLine("1. Registrar nueva pregunta");
        Console.WriteLine("2. Tomar examen");
        Console.WriteLine("3. Reportes");
        Console.WriteLine("4. Salir");
        Console.Write("Seleccione una opción: ");
    }
    
    static void RegistrarPregunta() {
        Console.Clear();
        Console.WriteLine("=== REGISTRAR NUEVA PREGUNTA ===");
        
        Console.Write("Enunciado de la pregunta: ");
        string enunciado = Console.ReadLine();
        
        Console.Write("Alternativa A: ");
        string respuestaA = Console.ReadLine();
        
        Console.Write("Alternativa B: ");
        string respuestaB = Console.ReadLine();
        
        Console.Write("Alternativa C: ");
        string respuestaC = Console.ReadLine();
        
        Console.Write("Respuesta correcta (A, B o C): ");
        string correcta = Console.ReadLine().ToUpper();
        
        // Validación simple
        if (string.IsNullOrWhiteSpace(enunciado) || 
            string.IsNullOrWhiteSpace(respuestaA) || 
            string.IsNullOrWhiteSpace(respuestaB) || 
            string.IsNullOrWhiteSpace(respuestaC) ||
            !new[] {"A", "B", "C"}.Contains(correcta)) {
            
            Console.WriteLine("Datos inválidos. La pregunta no fue guardada.");
            Console.WriteLine("Presione Enter para continuar...");
            Console.ReadLine();
            return;
        }
        
        var pregunta = new Pregunta {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta
        };
        
        using (var db = new DatosContexto()) {
            db.Preguntas.Add(pregunta);
            db.SaveChanges();
            
            Console.WriteLine("Pregunta registrada correctamente.");
            Console.WriteLine("Presione Enter para continuar...");
            Console.ReadLine();
        }
    }
    
    static void TomarExamen() {
        using (var db = new DatosContexto()) {
            // Verificar si hay preguntas suficientes
            int totalPreguntas = db.Preguntas.Count();
            if (totalPreguntas == 0) {
                Console.WriteLine("No hay preguntas registradas en el sistema.");
                Console.WriteLine("Presione Enter para continuar...");
                Console.ReadLine();
                return;
            }
            
            Console.Clear();
            Console.WriteLine("=== TOMAR EXAMEN ===");
            
            Console.Write("Ingrese su nombre: ");
            string nombreAlumno = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(nombreAlumno)) {
                Console.WriteLine("Debe ingresar un nombre válido.");
                Console.WriteLine("Presione Enter para continuar...");
                Console.ReadLine();
                return;
            }
            
            // Seleccionar preguntas aleatorias (máximo 5)
            int cantidadPreguntas = Math.Min(5, totalPreguntas);
            var preguntasAleatorias = db.Preguntas
            .AsEnumerable() // fuerza que todo lo siguiente se ejecute en memoria
            .OrderBy(p => Guid.NewGuid())
            .Take(cantidadPreguntas)
            .ToList();

            
            // Crear resultado de examen
            var resultadoExamen = new ResultadoExamen {
                NombreAlumno = nombreAlumno,
                FechaExamen = DateTime.Now,
                TotalPreguntas = cantidadPreguntas,
                RespuestasCorrectas = 0,
                NotaFinal = 0
            };
            
            db.ResultadosExamen.Add(resultadoExamen);
            db.SaveChanges(); // Guardar para obtener el ID
            
            Console.Clear();
            Console.WriteLine($"Examen para: {nombreAlumno}");
            Console.WriteLine($"Total de preguntas: {cantidadPreguntas}");
            Console.WriteLine("Responda cada pregunta seleccionando A, B o C");
            Console.WriteLine();
            
            // Proceso de tomar el examen
            for (int i = 0; i < preguntasAleatorias.Count; i++) {
                var pregunta = preguntasAleatorias[i];
                
                Console.WriteLine($"Pregunta {i+1} de {cantidadPreguntas}:");
                Console.WriteLine(pregunta.Enunciado);
                Console.WriteLine($"A) {pregunta.RespuestaA}");
                Console.WriteLine($"B) {pregunta.RespuestaB}");
                Console.WriteLine($"C) {pregunta.RespuestaC}");
                
                Console.Write("Su respuesta: ");
                string respuestaAlumno = Console.ReadLine().ToUpper();
                
                // Validar respuesta
                if (!new[] {"A", "B", "C"}.Contains(respuestaAlumno)) {
                    Console.WriteLine("Respuesta inválida. Debe ser A, B o C.");
                    i--; // Repetir la misma pregunta
                    continue;
                }
                
                // Verificar si es correcta
                bool esCorrecta = respuestaAlumno == pregunta.Correcta;
                if (esCorrecta) {
                    resultadoExamen.RespuestasCorrectas++;
                }
                
                // Guardar la respuesta
                var respuestaExamen = new RespuestaExamen {
                    ResultadoExamenId = resultadoExamen.ResultadoExamenId,
                    PreguntaId = pregunta.PreguntaId,
                    RespuestaDada = respuestaAlumno,
                    EsCorrecta = esCorrecta
                };
                
                db.RespuestasExamen.Add(respuestaExamen);
                
                Console.WriteLine(esCorrecta ? "¡Correcto!" : $"Incorrecto. La respuesta correcta era {pregunta.Correcta}");
                Console.WriteLine();
            }
            
            // Calcular nota final (sobre 5 puntos)
            resultadoExamen.NotaFinal = (double)resultadoExamen.RespuestasCorrectas * 5 / cantidadPreguntas;
            
            // Guardar los resultados
            db.SaveChanges();
            
            // Mostrar resultado final
            Console.WriteLine($"===== RESULTADO DEL EXAMEN =====");
            Console.WriteLine($"Alumno: {resultadoExamen.NombreAlumno}");
            Console.WriteLine($"Respuestas correctas: {resultadoExamen.RespuestasCorrectas} de {resultadoExamen.TotalPreguntas}");
            Console.WriteLine($"Nota final: {resultadoExamen.NotaFinal:F2} / 5.00");
            
            Console.WriteLine("Presione Enter para continuar...");
            Console.ReadLine();
        }
    }
    
    static void MostrarReportes() {
        bool salir = false;
        while (!salir) {
            Console.Clear();
            Console.WriteLine("=== REPORTES ===");
            Console.WriteLine("1. Listar todos los exámenes");
            Console.WriteLine("2. Filtrar exámenes por alumno");
            Console.WriteLine("3. Ranking de mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("5. Volver al menú principal");
            Console.Write("Seleccione una opción: ");
            
            var opcion = Console.ReadLine();
            
            switch (opcion) {
                case "1":
                    ListarTodosLosExamenes();
                    break;
                case "2":
                    FiltrarExamenesPorAlumno();
                    break;
                case "3":
                    MostrarRankingMejoresAlumnos();
                    break;
                case "4":
                    MostrarEstadisticasPorPregunta();
                    break;
                case "5":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida. Presione Enter para continuar...");
                    Console.ReadLine();
                    break;
            }
        }
    }
    
    static void ListarTodosLosExamenes() {
        Console.Clear();
        Console.WriteLine("=== LISTADO DE TODOS LOS EXÁMENES ===");
        
        using (var db = new DatosContexto()) {
            var examenes = db.ResultadosExamen
                .OrderByDescending(r => r.FechaExamen)
                .ToList();
            
            if (examenes.Count == 0) {
                Console.WriteLine("No hay exámenes registrados.");
            } else {
                Console.WriteLine("ID | Alumno | Fecha | Correctas | Total | Nota");
                Console.WriteLine("--------------------------------------------------");
                
                foreach (var examen in examenes) {
                    Console.WriteLine($"{examen.ResultadoExamenId} | {examen.NombreAlumno} | {examen.FechaExamen:dd/MM/yyyy HH:mm} | {examen.RespuestasCorrectas} | {examen.TotalPreguntas} | {examen.NotaFinal:F2}");
                }
            }
        }
        
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }
    
    static void FiltrarExamenesPorAlumno() {
        Console.Clear();
        Console.WriteLine("=== FILTRAR EXÁMENES POR ALUMNO ===");
        
        Console.Write("Ingrese el nombre del alumno: ");
        string nombreAlumno = Console.ReadLine();
        
        using (var db = new DatosContexto()) {
            var examenes = db.ResultadosExamen
                .Where(r => r.NombreAlumno.Contains(nombreAlumno))
                .OrderByDescending(r => r.FechaExamen)
                .ToList();
            
            if (examenes.Count == 0) {
                Console.WriteLine($"No se encontraron exámenes para el alumno '{nombreAlumno}'.");
            } else {
                Console.WriteLine($"Exámenes de {nombreAlumno}:");
                Console.WriteLine("ID | Fecha | Correctas | Total | Nota");
                Console.WriteLine("--------------------------------------------------");
                
                foreach (var examen in examenes) {
                    Console.WriteLine($"{examen.ResultadoExamenId} | {examen.FechaExamen:dd/MM/yyyy HH:mm} | {examen.RespuestasCorrectas} | {examen.TotalPreguntas} | {examen.NotaFinal:F2}");
                }
            }
        }
        
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }
    
    static void MostrarRankingMejoresAlumnos() {
        Console.Clear();
        Console.WriteLine("=== RANKING DE MEJORES ALUMNOS ===");
        
        using (var db = new DatosContexto()) {
            // Obtener la mejor nota de cada alumno
            var mejoresNotas = db.ResultadosExamen
                .GroupBy(r => r.NombreAlumno)
                .Select(g => new {
                    Alumno = g.Key,
                    MejorNota = g.Max(r => r.NotaFinal)
                })
                .OrderByDescending(r => r.MejorNota)
                .Take(10) // Mostrar los 10 mejores
                .ToList();
            
            if (mejoresNotas.Count == 0) {
                Console.WriteLine("No hay exámenes registrados.");
            } else {
                Console.WriteLine("Posición | Alumno | Mejor Nota");
                Console.WriteLine("----------------------------------");
                
                for (int i = 0; i < mejoresNotas.Count; i++) {
                    var alumno = mejoresNotas[i];
                    Console.WriteLine($"{i+1} | {alumno.Alumno} | {alumno.MejorNota:F2}");
                }
            }
        }
        
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }
    
    static void MostrarEstadisticasPorPregunta() {
        Console.Clear();
        Console.WriteLine("=== ESTADÍSTICAS POR PREGUNTA ===");
        
        using (var db = new DatosContexto()) {
            // Obtener todas las preguntas con sus estadísticas
            var preguntas = db.Preguntas.ToList();
            
            foreach (var pregunta in preguntas) {
                Console.WriteLine($"Pregunta #{pregunta.PreguntaId}: {pregunta.Enunciado}");
                
                // Contar cuántas veces fue respondida
                var respuestas = db.RespuestasExamen
                    .Where(r => r.PreguntaId == pregunta.PreguntaId)
                    .ToList();
                
                int totalRespuestas = respuestas.Count;
                int respuestasCorrectas = respuestas.Count(r => r.EsCorrecta);
                
                Console.WriteLine($"Total de veces respondida: {totalRespuestas}");
                
                double porcentajeCorrectas = totalRespuestas > 0 
                    ? (double)respuestasCorrectas * 100 / totalRespuestas 
                    : 0;
                
                Console.WriteLine($"Porcentaje de respuestas correctas: {porcentajeCorrectas:F2}%");
                Console.WriteLine();
            }
        }
        
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }
}