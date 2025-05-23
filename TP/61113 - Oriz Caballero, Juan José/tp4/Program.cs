using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = ""; 
}

class Examen {
    public int ExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int Correctas { get; set; }
    public int TotalPreguntas { get; set; }
    public float Puntuacion { get; set; }
    public List<Respuesta> Respuestas { get; set; } = new();
}

class Respuesta {
    public int RespuestaId { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public bool EsCorrecta { get; set; }
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
        if (!db.Preguntas.Any())
        {
            db.Preguntas.AddRange(
                new Pregunta {
                    Enunciado = "¿Quién creó nuestra bandera?",
                    RespuestaA = "José de San Martín",
                    RespuestaB = "Manuel Belgrano",
                    RespuestaC = "Julio Argentino Roca",
                    Correcta   = "B"
                },
                new Pregunta {
                    Enunciado = "¿En que año se firma el Acta de Independencía?",
                    RespuestaA = "1816",
                    RespuestaB = "1810",
                    RespuestaC = "1910",
                    Correcta   = "A"
                },
                new Pregunta {
                    Enunciado = "¿Qué tipo de gobierno tenemos?",
                    RespuestaA = "Representativo, Republicano y Federal",
                    RespuestaB = "Aristocratico, Parlamentario",
                    RespuestaC = "Monarquía Absolutista",
                    Correcta   = "A"
                },
                new Pregunta {
                    Enunciado = "¿Quiénes gobiernan las provincias?",
                    RespuestaA = "Presidentes",
                    RespuestaB = "Gobernadores",
                    RespuestaC = "Condes",
                    Correcta   = "B"
                },
                new Pregunta {
                    Enunciado = "¿Cada cuántos años hay elecciones presidenciales?",
                    RespuestaA = "Cuando fallece el presidente actual",
                    RespuestaB = "Cada ocho años",
                    RespuestaC = "Cada cuatro años",
                    Correcta   = "C"
                }
            );
            db.SaveChanges();
            Console.WriteLine("Preguntas cargadas .");
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
        if (preguntas.Count < 5) {
            Console.WriteLine("No hay suficientes preguntas en la base de datos.");
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
            Console.Write("\nRespuesta (A/B/C): ");
            string resp = Console.ReadLine()!.ToUpper();
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
            Console.WriteLine($"{e.ExamenId:000} - {e.NombreAlumno} | Nota: {e.Puntuacion}");
        }
    }

    static void BuscarPorAlumno() {
        using var db = new ContextoDatosTP();
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine()!;
        var examenes = db.Examenes.Where(e => e.NombreAlumno.Contains(nombre)).ToList();
        foreach (var e in examenes) {
            Console.WriteLine($"{e.ExamenId:000} - {e.NombreAlumno} | Nota: {e.Puntuacion}");
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

        foreach (var r in ranking) {
            Console.WriteLine($"{r.Alumno} - Mejor nota: {r.MejorNota}");
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
