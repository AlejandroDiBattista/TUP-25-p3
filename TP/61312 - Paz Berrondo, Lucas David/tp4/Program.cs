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

    public List<RespuestaExamen> RespuestasExamen { get; set; } = new();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.RespuestasExamen)
            .HasForeignKey(r => r.PreguntaId);
    }
}

class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto())
        {
            db.Database.EnsureCreated();
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("--- Sistema de Exámenes Multiple Choice ---");
            Console.WriteLine("1) Registrar pregunta");
            Console.WriteLine("2) Tomar examen");
            Console.WriteLine("3) Ver reportes");
            Console.WriteLine("0) Salir");
            Console.Write("Seleccione una opción: ");
            var op = Console.ReadLine();
            switch (op)
            {
                case "1": RegistrarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": VerReportes(); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida"); Console.ReadKey(); break;
            }
        }
    }
    static void RegistrarPregunta()
    {
        Console.Clear();
        Console.WriteLine("--- Registrar nueva pregunta ---");
        Console.Write("Enunciado: ");
        var enunciado = Console.ReadLine() ?? "";
        Console.Write("Respuesta A: ");
        var a = Console.ReadLine() ?? "";
        Console.Write("Respuesta B: ");
        var b = Console.ReadLine() ?? "";
        Console.Write("Respuesta C: ");
        var c = Console.ReadLine() ?? "";
        string correcta;
        do {
            Console.Write("¿Cuál es la opción correcta? (A/B/C) ");
            correcta = (Console.ReadLine() ?? "").Trim().ToUpper();
        } while (correcta != "A" && correcta != "B" && correcta != "C");

        using (var db = new DatosContexto())
        {
            var pregunta = new Pregunta {
                Enunciado = enunciado,
                RespuestaA = a,
                RespuestaB = b,
                RespuestaC = c,
                Correcta = correcta
            };
            db.Preguntas.Add(pregunta);
            db.SaveChanges();
        }
        Console.WriteLine("Pregunta registrada correctamente.");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void TomarExamen()
    {
        Console.Clear();
        Console.WriteLine("--- Tomar Examen ---");
        Console.Write("Ingrese su nombre: ");
        var nombre = Console.ReadLine() ?? "";
        List<Pregunta> preguntas;
        using (var db = new DatosContexto())
        {
            preguntas = db.Preguntas.ToList()
                .OrderBy(x => Guid.NewGuid())
                .Take(5)
                .ToList();
        }
        if (preguntas.Count == 0)
        {
            Console.WriteLine("No hay preguntas registradas. Agregue preguntas primero.");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();
        int nro = 1;
        foreach (var pregunta in preguntas)
        {
            Console.Clear();
            Console.WriteLine($"Pregunta {nro} de {preguntas.Count}");
            Console.WriteLine(pregunta.Enunciado);
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");
            string resp;
            do {
                Console.Write("Respuesta (A/B/C): ");
                resp = (Console.ReadLine() ?? "").Trim().ToUpper();
            } while (resp != "A" && resp != "B" && resp != "C");
            bool esCorrecta = resp == pregunta.Correcta;
            if (esCorrecta) correctas++;
            respuestas.Add(new RespuestaExamen {
                PreguntaId = pregunta.PreguntaId,
                RespuestaAlumno = resp,
                EsCorrecta = esCorrecta
            });
            nro++;
        }
        double nota = (double)correctas / preguntas.Count * 10.0;

        using (var db = new DatosContexto())
        {
            var resultado = new ResultadoExamen {
                NombreAlumno = nombre,
                CantidadCorrectas = correctas,
                TotalPreguntas = preguntas.Count,
                NotaFinal = Math.Round(nota, 2),
                Fecha = DateTime.Now
            };
            db.ResultadosExamen.Add(resultado);
            db.SaveChanges();
            // Guardar respuestas
            foreach (var r in respuestas)
            {
                r.ResultadoExamenId = resultado.ResultadoExamenId;
                db.RespuestasExamen.Add(r);
            }
            db.SaveChanges();
        }
        Console.WriteLine($"\nExamen finalizado. Respuestas correctas: {correctas}/{preguntas.Count}");
        Console.WriteLine($"Nota final: {Math.Round(nota,2)}");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void VerReportes()
    {
        Console.Clear();
        Console.WriteLine("--- Reportes ---");
        Console.WriteLine("1) Listado de exámenes rendidos");
        Console.WriteLine("2) Filtrar resultados por alumno");
        Console.WriteLine("3) Ranking de mejores alumnos");
        Console.WriteLine("4) Estadísticas por pregunta");
        Console.WriteLine("0) Volver");
        Console.Write("Seleccione una opción: ");
        var op = Console.ReadLine();
        switch (op)
        {
            case "1": ListadoExamenes(); break;
            case "2": FiltrarPorAlumno(); break;
            case "3": RankingAlumnos(); break;
            case "4": EstadisticasPreguntas(); break;
            case "0": return;
            default: Console.WriteLine("Opción inválida"); Console.ReadKey(); break;
        }
    }

    static void ListadoExamenes()
    {
        Console.Clear();
        Console.WriteLine("--- Listado de exámenes rendidos ---");
        using (var db = new DatosContexto())
        {
            var examenes = db.ResultadosExamen.OrderByDescending(e => e.Fecha).ToList();
            foreach (var ex in examenes)
            {
                Console.WriteLine($"{ex.Fecha:dd/MM/yyyy HH:mm} | Alumno: {ex.NombreAlumno} | Correctas: {ex.CantidadCorrectas}/{ex.TotalPreguntas} | Nota: {ex.NotaFinal}");
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void FiltrarPorAlumno()
    {
        Console.Clear();
        Console.WriteLine("--- Filtrar resultados por alumno ---");
        Console.Write("Ingrese nombre del alumno: ");
        var nombre = Console.ReadLine() ?? "";
        using (var db = new DatosContexto())
        {
            var examenes = db.ResultadosExamen
                .Where(e => e.NombreAlumno.ToLower().Contains(nombre.ToLower()))
                .OrderByDescending(e => e.Fecha)
                .ToList();
            foreach (var ex in examenes)
            {
                Console.WriteLine($"{ex.Fecha:dd/MM/yyyy HH:mm} | Alumno: {ex.NombreAlumno} | Correctas: {ex.CantidadCorrectas}/{ex.TotalPreguntas} | Nota: {ex.NotaFinal}");
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void RankingAlumnos()
    {
        Console.Clear();
        Console.WriteLine("--- Ranking de mejores alumnos (mejor nota) ---");
        using (var db = new DatosContexto())
        {
            var ranking = db.ResultadosExamen
                .GroupBy(e => e.NombreAlumno)
                .Select(g => new {
                    Alumno = g.Key,
                    MejorNota = g.Max(x => x.NotaFinal)
                })
                .OrderByDescending(x => x.MejorNota)
                .Take(10)
                .ToList();
            int pos = 1;
            foreach (var r in ranking)
            {
                Console.WriteLine($"{pos}) {r.Alumno} - Mejor nota: {r.MejorNota}");
                pos++;
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void EstadisticasPreguntas()
    {
        Console.Clear();
        Console.WriteLine("--- Estadísticas por pregunta ---");
        using (var db = new DatosContexto())
        {
            var preguntas = db.Preguntas.Include(p => p.RespuestasExamen).ToList();
            foreach (var p in preguntas)
            {
                int total = p.RespuestasExamen.Count;
                int correctas = p.RespuestasExamen.Count(r => r.EsCorrecta);
                double porcentaje = total > 0 ? (double)correctas / total * 100.0 : 0;
                Console.WriteLine($"\nPregunta #{p.PreguntaId:000}: {p.Enunciado}");
                Console.WriteLine($"Respondida: {total} veces");
                Console.WriteLine($"% de respuestas correctas: {porcentaje:F2}%");
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}