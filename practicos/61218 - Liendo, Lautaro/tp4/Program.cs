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
    public string Correcta { get; set; } = ""; // A, B o C
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
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext { // Contexto de la base de datos 
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Examen> Examenes => Set<Examen>();
    public DbSet<Respuesta> Respuestas => Set<Respuesta>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");// Cambia la ruta según sea necesario
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {// Configuración de las entidades
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
        using var db = new DatosContexto();// Crear la base de datos y las tablas
        db.Database.EnsureDeleted();    // Eliminar la base de datos existente (opcional)

        db.Database.EnsureCreated();// Crear la base de datos y las tablas si no existen
        if (!db.Preguntas.Any())// Verificar si ya hay preguntas en la base de datos
{
    db.Preguntas.AddRange(// Agregar preguntas de ejemplo
        new Pregunta {
            Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
            RespuestaA = "Java",
            RespuestaB = "C#",
            RespuestaC = "Python",
            Correcta   = "B"
        },
        new Pregunta {
            Enunciado = "¿Qué significa SQL?",
            RespuestaA = "Structured Query Language",
            RespuestaB = "Simple Query Language",
            RespuestaC = "Standard Query Language",
            Correcta   = "A"
        },
        new Pregunta {
            Enunciado = "¿Qué tipo de lenguaje es C#?",
            RespuestaA = "Compilado y orientado a objetos",
            RespuestaB = "Interpretado y funcional",
            RespuestaC = "Markup y estructurado",
            Correcta   = "A"
        },
        new Pregunta {
            Enunciado = "¿Qué operador se usa para comparar igualdad en C#?",
            RespuestaA = "=",
            RespuestaB = "==",
            RespuestaC = "===",
            Correcta   = "B"
        },
        new Pregunta {
            Enunciado = "¿Cuál es el método principal de entrada en una app de consola en C#?",
            RespuestaA = "main()",
            RespuestaB = "Start()",
            RespuestaC = "Main()",
            Correcta   = "C"
        }
    );
    db.SaveChanges();
    Console.WriteLine("Preguntas cargadas automáticamente.");
}

        

        while (true) {// Bucle principal del programa
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

            Console.Write("Opción: ");// Solicitar opción al usuario
            var op = Console.ReadLine();// Leer la opción ingresada
            if (string.IsNullOrEmpty(op)) continue;// Si la opción está vacía, continuar al siguiente ciclo
            switch (op) {
                case "1": CargarPregunta(); break;// Cargar una nueva pregunta
                case "2": TomarExamen(); break;// Tomar un examen
                case "3": ListarExamenes(); break;// Listar todos los exámenes
                case "4": BuscarPorAlumno(); break;// Buscar exámenes por alumno
                case "5": Ranking(); break;// Mostrar el ranking de mejores alumnos
                case "6": Estadisticas(); break;// Mostrar estadísticas por pregunta
                case "0": return;
                default: Console.WriteLine("Opción inválida."); break;// Opción inválida
            }

            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }
    }

    static void CargarPregunta() {
        using var db = new DatosContexto();

        Console.Write("Enunciado: ");//
        string enunciado = Console.ReadLine()!;// Solicitar el enunciado de la pregunta
        Console.Write("Respuesta A: ");//
        string a = Console.ReadLine()!;
        Console.Write("Respuesta B: ");
        string b = Console.ReadLine()!;
        Console.Write("Respuesta C: ");
        string c = Console.ReadLine()!;
        Console.Write("Correcta (A/B/C): ");
        string correcta = Console.ReadLine()!.ToUpper();// Solicitar la respuesta correcta (A, B o C)

        db.Preguntas.Add(new Pregunta {// Crear una nueva pregunta
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        });

        db.SaveChanges();
        Console.WriteLine("Pregunta guardada.");
    }

    static void TomarExamen() {// Método para tomar el examen
        using var db = new DatosContexto();

        Console.Write("Ingrese su nombre: ");// Solicitar el nombre del alumno
        string nombre = Console.ReadLine()!;
        var preguntas = db.Preguntas.ToList().OrderBy(p => Guid.NewGuid()).Take(5).ToList();// Seleccionar 5 preguntas aleatorias de la base de datos
        if (preguntas.Count < 5) {// Verificar si hay suficientes preguntas
            Console.WriteLine("No hay suficientes preguntas en la base de datos.");
            return;
        }

        int correctas = 0;// Contador de respuestas correctas
        var respuestas = new List<Respuesta>();

        foreach (var p in preguntas) {// Iterar sobre cada pregunta
            Console.Clear();
            Console.WriteLine($"{p.Enunciado}\n");
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("\nRespuesta (A/B/C): ");
            string resp = Console.ReadLine()!.ToUpper();
            bool esCorrecta = resp == p.Correcta;
            if (esCorrecta) correctas++;// Incrementar el contador de respuestas correctas

            respuestas.Add(new Respuesta {
                PreguntaId = p.PreguntaId,
                EsCorrecta = esCorrecta
            });
        }

        var examen = new Examen {// Crear un nuevo examen
            NombreAlumno = nombre,
            Correctas = correctas,
            TotalPreguntas = preguntas.Count,
            NotaFinal = correctas,
            Respuestas = respuestas
        };

        db.Examenes.Add(examen);
        db.SaveChanges();

        Console.WriteLine($"\n¡Examen finalizado! Respuestas correctas: {correctas} de {preguntas.Count}");// Mostrar el resultado del examen
        Console.WriteLine($"Nota final: {examen.NotaFinal}/5");// Mostrar la nota final
        Console.WriteLine($"Nombre del alumno: {examen.NombreAlumno}");// Mostrar el nombre del alumno
    }

    static void ListarExamenes() {// Método para listar todos los exámenes
        using var db = new DatosContexto();
        var examenes = db.Examenes.ToList();
        foreach (var e in examenes) {
            Console.WriteLine($"{e.ExamenId:000} - {e.NombreAlumno} | Nota: {e.NotaFinal}");// Mostrar el ID del examen, el nombre del alumno y la nota final
        }
    }

    static void BuscarPorAlumno() {// Método para buscar exámenes por alumno
        using var db = new DatosContexto();
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine()!;
        var examenes = db.Examenes.Where(e => e.NombreAlumno.Contains(nombre)).ToList();
        foreach (var e in examenes) {
            Console.WriteLine($"{e.ExamenId:000} - {e.NombreAlumno} | Nota: {e.NotaFinal}");
        }
    }

    static void Ranking() {// Método para mostrar el ranking de mejores alumnos
        using var db = new DatosContexto();
        var ranking = db.Examenes
            .GroupBy(e => e.NombreAlumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(e => e.NotaFinal)//g.Max(e => e.NotaFinal) // Obtener la mejor nota del alumno
            })
            .OrderByDescending(r => r.MejorNota)// Ordenar por mejor nota
            .ToList();

        foreach (var r in ranking) {
            Console.WriteLine($"{r.Alumno} - Mejor nota: {r.MejorNota}");// Mostrar el nombre del alumno y su mejor nota
        }
    }

    static void Estadisticas() {
        using var db = new DatosContexto();
        var estadisticas = db.Respuestas
            .Include(r => r.Pregunta)// Incluir la pregunta relacionada
            .GroupBy(r => r.Pregunta.Enunciado) // Agrupar por enunciado de la pregunta
            .Select(g => new {  // Crear un objeto anónimo con las estadísticas
                Enunciado = g.Key,  // Enunciado de la pregunta
                Total = g.Count(),  // Total de respuestas
                Correctas = g.Count(r => r.EsCorrecta),     // Total de respuestas correctas
                Porcentaje = g.Count(r => r.EsCorrecta) * 100.0 / g.Count() // Porcentaje de respuestas correctas
            })
            .ToList();

        foreach (var e in estadisticas) {
            Console.WriteLine($"\n{e.Enunciado}");  // Mostrar el enunciado de la pregunta
            Console.WriteLine($"Respondida: {e.Total} veces | Correctas: {e.Correctas} | Efectividad: {e.Porcentaje:F1}%");     // Mostrar el total de respuestas, respuestas correctas y porcentaje de efectividad
        }
    }
}