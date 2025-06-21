using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
}

class ContextoDatosTP : DbContext {
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Examen> Examenes => Set<Examen>();
    public DbSet<Respuesta> Respuestas => Set<Respuesta>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Examen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ExamenId);

        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Pregunta)
            .WithMany()
            .HasForeignKey(r => r.PreguntaId);
    }
}

class Program {
    static void Main() {
        using var db = new ContextoDatosTP();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        if (!db.Preguntas.Any()) {
            db.Preguntas.AddRange(
                new Pregunta {
                    Enunciado = "¿Cuál es el rango de valores que puede almacenar una variable de tipo byte en C#?",
                    RespuestaA = "-128 a 127",
                    RespuestaB = "0 a 255",
                    RespuestaC = "-255 a 255",
                    Correcta   = "B"
                },
                new Pregunta {
                    Enunciado = "¿Qué tipo entero utilizarías para almacenar un valor de 4.000 millones (4.000.000.000)?",
                    RespuestaA = "int",
                    RespuestaB = "short",
                    RespuestaC = "long",
                    Correcta   = "C"
                },
                new Pregunta {
                    Enunciado = "¿Qué resultado se obtiene al ejecutar la expresión 5 + 2 * 3?",
                    RespuestaA = "21",
                    RespuestaB = "11",
                    RespuestaC = "17",
                    Correcta   = "B"
                },
                new Pregunta {
                    Enunciado = "¿Cuál es el valor final de x después de esta operación?\nint x = 10;\nx = x / 2 + 3;",
                    RespuestaA = "8",
                    RespuestaB = "10",
                    RespuestaC = "5",
                    Correcta   = "A"
                },
                new Pregunta {
                    Enunciado = "¿Qué operador tiene mayor precedencia en C#?",
                    RespuestaA = "+ (suma)",
                    RespuestaB = "/ (división)",
                    RespuestaC = "= (asignación)",
                    Correcta   = "B"
                }
            );
            db.SaveChanges();
            Console.WriteLine("Preguntas cargadas.");
        }

        while (true) {
            Console.Clear();
            Console.WriteLine("""
                1. Cargar pregunta
                2. Tomar examen
                3. Ver todos los exámenes
                4. Buscar exámenes por alumno
                5. Ranking de mejores alumnos
                6. Estadísticas por pregunta
                0. Salir
            """);

            Console.Write("Opción: ");
            var op = Console.ReadLine();
            if (string.IsNullOrEmpty(op)) continue;

            switch (op) {
                case "1": CargarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": ListarExamenes(); break;
                case "4": BuscarPorAlumno(); break;
                case "5": Ranking(); break;
                case "6": Estadisticas(); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida."); break;
            }

            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }
    }

    static void CargarPregunta() {
        using var db = new ContextoDatosTP();

        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine()!;
        Console.Write("Respuesta A: ");
        string a = Console.ReadLine()!;
        Console.Write("Respuesta B: ");
        string b = Console.ReadLine()!;
        Console.Write("Respuesta C: ");
        string c = Console.ReadLine()!;
        Console.Write("Correcta (A/B/C): ");
        string correcta = Console.ReadLine()!.ToUpper();

        db.Preguntas.Add(new Pregunta {
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        });

        db.SaveChanges();
        Console.WriteLine("Pregunta guardada.");
    }

    static void TomarExamen() {
        using var db = new ContextoDatosTP();

        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine()!;
        var preguntas = db.Preguntas.ToList().OrderBy(p => Guid.NewGuid()).Take(5).ToList();

        if (preguntas.Count == 0) {
            Console.WriteLine("No hay preguntas disponibles en la base de datos.");
            return;
        }

        int correctas = 0;
        var respuestas = new List<Respuesta>();

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine($"{p.Enunciado}\n");
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");

            string resp;
            do {
                Console.Write("\nRespuesta (A/B/C): ");
                resp = Console.ReadLine()!.ToUpper();
            } while (resp != "A" && resp != "B" && resp != "C");

            bool esCorrecta = resp == p.Correcta;
            if (esCorrecta) correctas++;

            respuestas.Add(new Respuesta {
                PreguntaId = p.PreguntaId,
                EsCorrecta = esCorrecta
            });
        }

        var examen = new Examen {
            NombreAlumno = nombre,
            Correctas = correctas,
            TotalPreguntas = preguntas.Count,
            Puntuacion = correctas,
            Respuestas = respuestas
        };

        db.Examenes.Add(examen);
        db.SaveChanges();

        Console.WriteLine($"\n¡Examen finalizado! Respuestas correctas: {correctas} de {preguntas.Count}");
        Console.WriteLine($"Nota final: {examen.Puntuacion}/5");
        Console.WriteLine($"Nombre del alumno: {examen.NombreAlumno}");
    }

    static void ListarExamenes() {
        using var db = new ContextoDatosTP();
        var examenes = db.Examenes.ToList();
        foreach (var e in examenes) {
            Console.WriteLine($"{e.ExamenId:000} - {e.NombreAlumno} | Nota: {e.Puntuacion} | Correctas: {e.Correctas} | Fecha: {e.Fecha:g}");
        }
    }

    static void BuscarPorAlumno() {
        using var db = new ContextoDatosTP();
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine()!;
        var examenes = db.Examenes.Where(e => e.NombreAlumno.Contains(nombre)).ToList();
        foreach (var e in examenes) {
            Console.WriteLine($"{e.ExamenId:000} - {e.NombreAlumno} | Nota: {e.Puntuacion} | Fecha: {e.Fecha:g}");
        }
    }

    static void Ranking() {
        using var db = new ContextoDatosTP();
        var ranking = db.Examenes
            .GroupBy(e => e.NombreAlumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(e => e.Puntuacion)
            })
            .OrderByDescending(r => r.MejorNota)
            .ToList();

        int pos = 1;
        foreach (var r in ranking) {
            Console.WriteLine($"{pos++}. {r.Alumno} - Mejor nota: {r.MejorNota}");
        }
    }

    static void Estadisticas() {
        using var db = new ContextoDatosTP();
        var estadisticas = db.Respuestas
            .Include(r => r.Pregunta)
            .GroupBy(r => r.Pregunta.Enunciado)
            .Select(g => new {
                Enunciado = g.Key,
                Total = g.Count(),
                Correctas = g.Count(r => r.EsCorrecta),
                Porcentaje = g.Count(r => r.EsCorrecta) * 100.0 / g.Count()
            })
            .ToList();

        foreach (var e in estadisticas) {
            Console.WriteLine($"\n{e.Enunciado}");
            Console.WriteLine($"Respondida: {e.Total} veces | Correctas: {e.Correctas} | Efectividad: {e.Porcentaje:F1}%");
        }
    }
}