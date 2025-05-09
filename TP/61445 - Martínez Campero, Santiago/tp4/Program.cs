using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "Texto predeterminado";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
}

class Examen {
    public int ExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int Correctas { get; set; }
    public int TotalPreguntas { get; set; }
    public float NotaFinal { get; set; }
    public List<Respuesta> Respuestas { get; set; } = new();
}

class Respuesta {
    public int RespuestaId { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public string OpcionElegida { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }
  
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Examen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ExamenId);
    }
}

class Program {
    static void Main(string[] args) {
        while (true) {
            Console.Clear();
            Console.WriteLine("=== Sistema de Exámenes Multiple Choice ===");
            Console.WriteLine("1. Registrar nueva pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Reportes");
            Console.WriteLine("4. Borrar todas las preguntas");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion) {
                case "1":
                    RegistrarPregunta();
                    break;
                case "2":
                    TomarExamen();
                    break;
                case "3":
                    MenuReportes();
                    break;
                case "4":
                    BorrarPreguntas();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void RegistrarPregunta() {
        Console.Clear();
        Console.WriteLine("=== Registrar Nueva Pregunta ===");
        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine();
        Console.Write("Respuesta A: ");
        string a = Console.ReadLine();
        Console.Write("Respuesta B: ");
        string b = Console.ReadLine();
        Console.Write("Respuesta C: ");
        string c = Console.ReadLine();
        string correcta;
        do {
            Console.Write("Letra de la respuesta correcta (A, B o C): ");
            correcta = Console.ReadLine().ToUpper();
        } while (correcta != "A" && correcta != "B" && correcta != "C");

        using (var db = new DatosContexto()) {
            db.Preguntas.Add(new Pregunta {
                Enunciado = enunciado,
                RespuestaA = a,
                RespuestaB = b,
                RespuestaC = c,
                Correcta = correcta
            });
            db.SaveChanges();
        }
        Console.WriteLine("Pregunta registrada correctamente. Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void TomarExamen() {
        Console.Clear();
        Console.WriteLine("=== Tomar Examen ===");
        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine() ?? "";
        List<Pregunta> preguntas;
        using (var db = new DatosContexto()) {
            preguntas = db.Preguntas.AsEnumerable().OrderBy(p => Guid.NewGuid()).Take(5).ToList();
        }
        if (preguntas.Count < 1) {
            Console.WriteLine("No hay preguntas registradas. Registre al menos una pregunta.");
            Console.ReadKey();
            return;
        }
        int correctas = 0;
        var respuestas = new List<Respuesta>();
        foreach (var pregunta in preguntas) {
            if (pregunta.Enunciado != null) {
                Console.WriteLine(pregunta.Enunciado.Length);
            }
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");
            string opcion;
            do {
                Console.Write("Seleccione una opción (A, B, C): ");
                opcion = Console.ReadLine().ToUpper();
            } while (opcion != "A" && opcion != "B" && opcion != "C");
            bool esCorrecta = opcion == pregunta.Correcta;
            if (esCorrecta) correctas++;
            respuestas.Add(new Respuesta {
                PreguntaId = pregunta.PreguntaId,
                OpcionElegida = opcion,
                EsCorrecta = esCorrecta
            });
        }
        float nota = correctas;
        using (var db = new DatosContexto()) {
            var examen = new Examen {
                NombreAlumno = nombre,
                Correctas = correctas,
                TotalPreguntas = preguntas.Count,
                NotaFinal = nota
            };
            db.Examenes.Add(examen);
            db.SaveChanges();
            foreach (var r in respuestas) {
                r.ExamenId = examen.ExamenId;
                db.Respuestas.Add(r);
            }
            db.SaveChanges();
        }
        Console.WriteLine($"\nExamen finalizado. Correctas: {correctas}/{preguntas.Count}. Nota: {nota}");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void MenuReportes() {
        while (true) {
            Console.Clear();
            Console.WriteLine("=== Reportes ===");
            Console.WriteLine("1. Listar todos los exámenes");
            Console.WriteLine("2. Filtrar exámenes por alumno");
            Console.WriteLine("3. Ranking de mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("5. Volver");
            Console.Write("Seleccione una opción: ");
            var op = Console.ReadLine();
            switch (op) {
                case "1":
                    ReporteExamenes();
                    break;
                case "2":
                    ReportePorAlumno();
                    break;
                case "3":
                    ReporteRanking();
                    break;
                case "4":
                    ReporteEstadisticas();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void ReporteExamenes() {
        Console.Clear();
        Console.WriteLine("=== Exámenes Rendidos ===");
        using (var db = new DatosContexto()) {
            var examenes = db.Examenes.ToList();
            foreach (var ex in examenes) {
                Console.WriteLine($"Alumno: {ex.NombreAlumno} | Correctas: {ex.Correctas}/{ex.TotalPreguntas} | Nota: {ex.NotaFinal}");
            }
        }
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void ReportePorAlumno() {
        Console.Clear();
        Console.Write("Ingrese el nombre del alumno: ");
        string nombre = Console.ReadLine();
        using (var db = new DatosContexto()) {
            var examenes = db.Examenes.Where(e => e.NombreAlumno.Contains(nombre ?? "")).ToList();
            foreach (var ex in examenes) {
                Console.WriteLine($"Alumno: {ex.NombreAlumno} | Correctas: {ex.Correctas}/{ex.TotalPreguntas} | Nota: {ex.NotaFinal}");
            }
        }
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void ReporteRanking() {
        Console.Clear();
        Console.WriteLine("=== Ranking de Mejores Alumnos ===");
        using (var db = new DatosContexto()) {
            var ranking = db.Examenes
                .GroupBy(e => e.NombreAlumno)
                .Select(g => new { Alumno = g.Key, MejorNota = g.Max(e => e.NotaFinal) })
                .OrderByDescending(x => x.MejorNota)
                .Take(10)
                .ToList();
            foreach (var r in ranking) {
                Console.WriteLine($"Alumno: {r.Alumno} | Mejor Nota: {r.MejorNota}");
            }
        }
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void ReporteEstadisticas() {
        Console.Clear();
        Console.WriteLine("=== Estadísticas por Pregunta ===");
        using (var db = new DatosContexto()) {
            var estadisticas = db.Preguntas
                .Select(p => new {
                    p.Enunciado,
                    Total = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId),
                    Correctas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta)
                })
                .ToList();
            foreach (var e in estadisticas) {
                double porcentaje = e.Total > 0 ? (e.Correctas * 100.0 / e.Total) : 0;
                Console.WriteLine($"Pregunta: {e.Enunciado}\n  Total respuestas: {e.Total} | Correctas: {e.Correctas} | % Correctas: {porcentaje:F2}%");
            }
        }
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void BorrarPreguntas() {
        Console.Clear();
        Console.WriteLine("=== Borrar Todas las Preguntas ===");
        Console.Write("¿Está seguro de que desea borrar todas las preguntas? (S/N): ");
        string confirmacion = Console.ReadLine()?.ToUpper();

        if (confirmacion == "S") {
            using (var db = new DatosContexto()) {
                db.Preguntas.RemoveRange(db.Preguntas);
                db.SaveChanges();

            }
            Console.WriteLine("Todas las preguntas han sido eliminadas.");
        } else {
            Console.WriteLine("Operación cancelada.");
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}