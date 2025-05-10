using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// Clase Pregunta
class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

// Clase Examen
class Examen {
    public int ExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; } = 5;
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

// Clase RespuestaExamen
class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public bool EsCorrecta { get; set; }
}

// Contexto de Base de Datos
class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

// Programa Principal
class Program {
    static void Main(string[] args) {
        using (var db = new DatosContexto()) {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            MostrarMenu(db);
        }
    }

    static void MostrarMenu(DatosContexto db) {
        while (true) {
            Console.Clear();
            Console.WriteLine("Sistema de Exámenes Multiple Choice");
            Console.WriteLine("1. Registrar Pregunta");
            Console.WriteLine("2. Tomar Examen");
            Console.WriteLine("3. Ver Reportes");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion) {
                case "1":
                    RegistrarPregunta(db);
                    break;
                case "2":
                    TomarExamen(db);
                    break;
                case "3":
                    GenerarReportes(db);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione Enter para continuar...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("Registro de Pregunta");
        Console.Write("Enunciado: ");
        var enunciado = Console.ReadLine() ?? "";
        Console.Write("Opción A: ");
        var opcionA = Console.ReadLine() ?? "";
        Console.Write("Opción B: ");
        var opcionB = Console.ReadLine() ?? "";
        Console.Write("Opción C: ");
        var opcionC = Console.ReadLine() ?? "";
        Console.Write("Respuesta Correcta (A/B/C): ");
        var correcta = Console.ReadLine()?.ToUpper() ?? "";

        var pregunta = new Pregunta {
            Enunciado = enunciado,
            RespuestaA = opcionA,
            RespuestaB = opcionB,
            RespuestaC = opcionC,
            Correcta = correcta
        };

        db.Preguntas.Add(pregunta);
        db.SaveChanges();
        Console.WriteLine("Pregunta registrada correctamente. Presione Enter para continuar...");
        Console.ReadLine();
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("Toma de Examen");
        Console.Write("Nombre del Alumno: ");
        var nombreAlumno = Console.ReadLine() ?? "";

        // Traer las preguntas de la base de datos y luego barajarlas en memoria
        var preguntas = db.Preguntas.ToList();
        var preguntasAleatorias = preguntas.OrderBy(p => Guid.NewGuid()).Take(5).ToList(); // Barajar en memoria

        int correctas = 0;
        var examen = new Examen {
            NombreAlumno = nombreAlumno,
            TotalPreguntas = preguntasAleatorias.Count
        };
        db.Examenes.Add(examen);
        db.SaveChanges();

        foreach (var pregunta in preguntasAleatorias) {
            Console.Clear();
            Console.WriteLine($"{pregunta.Enunciado}\nA) {pregunta.RespuestaA}\nB) {pregunta.RespuestaB}\nC) {pregunta.RespuestaC}\n");
            Console.Write("Respuesta (A/B/C): ");
            var respuesta = Console.ReadLine()?.ToUpper() ?? "";
            bool esCorrecta = respuesta == pregunta.Correcta;
            if (esCorrecta) correctas++;

            var respuestaExamen = new RespuestaExamen {
                ExamenId = examen.ExamenId,
                PreguntaId = pregunta.PreguntaId,
                EsCorrecta = esCorrecta
            };
            db.RespuestasExamen.Add(respuestaExamen);
            db.SaveChanges();
        }

        examen.RespuestasCorrectas = correctas;
        examen.NotaFinal = (double)correctas / examen.TotalPreguntas * 5.0;
        db.SaveChanges();

        Console.WriteLine($"Examen finalizado. Nota: {examen.NotaFinal:0.00} / 5.00");
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }

    static void GenerarReportes(DatosContexto db) {
        // Reporte básico de exámenes realizados
        var examenes = db.Examenes.Include(e => e.Respuestas)
            .OrderBy(e => e.Fecha)
            .ToList();

        Console.Clear();
        Console.WriteLine("Reporte de Exámenes Realizados:");
        foreach (var examen in examenes) {
            Console.WriteLine($"Alumno: {examen.NombreAlumno} | Fecha: {examen.Fecha.ToShortDateString()} | Nota: {examen.NotaFinal:0.00}/5.00");
        }

        Console.WriteLine("\nFiltrar resultados por nombre de alumno:");
        var nombreFiltro = Console.ReadLine()?.ToLower();
        if (!string.IsNullOrEmpty(nombreFiltro)) {
            var examenesFiltrados = examenes.Where(e => e.NombreAlumno.ToLower().Contains(nombreFiltro)).ToList();
            Console.WriteLine($"Exámenes filtrados para '{nombreFiltro}':");
            foreach (var examen in examenesFiltrados) {
                Console.WriteLine($"Alumno: {examen.NombreAlumno} | Fecha: {examen.Fecha.ToShortDateString()} | Nota: {examen.NotaFinal:0.00}/5.00");
            }
        }

        // Ranking de los mejores alumnos basado en la mejor nota
        var ranking = examenes.OrderByDescending(e => e.NotaFinal).Take(10).ToList();
        Console.WriteLine("\nRanking de los mejores alumnos:");
        foreach (var examen in ranking) {
            Console.WriteLine($"Alumno: {examen.NombreAlumno} | Nota: {examen.NotaFinal:0.00}/5.00");
        }

        // Informe estadístico por pregunta
        Console.WriteLine("\nInforme estadístico por pregunta:");
        var preguntas = db.Preguntas.Include(p => p.Respuestas).ToList();
        foreach (var pregunta in preguntas) {
            var respuestasCorrectas = pregunta.Respuestas.Count(r => r.EsCorrecta);
            var totalRespuestas = pregunta.Respuestas.Count();
            var porcentajeCorrectas = totalRespuestas == 0 ? 0 : (double)respuestasCorrectas / totalRespuestas * 100;

            Console.WriteLine($"{pregunta.Enunciado}: {totalRespuestas} respuestas, {porcentajeCorrectas:0.00}% correctas.");
        }

        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }
}
