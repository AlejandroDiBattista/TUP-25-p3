using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;


class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";

    public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}


class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int Correctas { get; set; }
    public int Total { get; set; }
    public double NotaFinal { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}


class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public string RespuestaDada { get; set; } = "";
    public bool EsCorrecta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen Resultado { get; set; } = null!;
}


class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<ResultadoExamen> Resultados => Set<ResultadoExamen>();
    public DbSet<RespuestaExamen> Respuestas => Set<RespuestaExamen>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program {
    static void Main() {
        using var db = new DatosContexto();
        db.Database.EnsureCreated();

        while (true) {
            Console.Clear();
            Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
            Console.WriteLine("1. Realizar examen");
            Console.WriteLine("2. Ver reportes");
            Console.WriteLine("3. Registrar nueva pregunta");
            Console.WriteLine("4. Ver respuestas dadas por los alumnos");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine() ?? "";

            switch (opcion) {
                case "1":
                    TomarExamen(db);
                    break;
                case "2":
                    MostrarMenuReportes(db);
                    break;
                case "3":
                    RegistrarPregunta(db);
                    break;
                case "4":
                    MostrarRespuestasDadas(db);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        Console.Write("\nIngrese su nombre: ");
        string alumno = Console.ReadLine() ?? "Anonimo";

        var preguntas = db.Preguntas.ToList().OrderBy(_ => Guid.NewGuid()).Take(10).ToList();

        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas disponibles. Registre preguntas primero.");
            Console.ReadKey();
            return;
        }

        int correctas = 0;
        var respuestasAlumno = new List<RespuestaExamen>();

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine($"\n{p.Enunciado}\nA) {p.RespuestaA}\nB) {p.RespuestaB}\nC) {p.RespuestaC}");
            Console.Write("Respuesta (A/B/C): ");
            string? r = Console.ReadLine()?.Trim().ToUpper();

            bool esCorrecta = (r == p.Correcta.ToUpper());
            if (esCorrecta) correctas++;

            Console.WriteLine(esCorrecta ? "¡Correcto!" : $" Incorrecto. Respuesta correcta: {p.Correcta}");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();

            respuestasAlumno.Add(new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaDada = r ?? "",
                EsCorrecta = esCorrecta
            });
        }

        var resultado = new ResultadoExamen {
            Alumno = alumno,
            Correctas = correctas,
            Total = preguntas.Count,
            NotaFinal = correctas * 10.0 / preguntas.Count,
            Respuestas = respuestasAlumno
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Nota: {resultado.NotaFinal}/10 ({resultado.Correctas} correctas de {resultado.Total})\n");
        Console.WriteLine("Presione una tecla para volver al menú...");
        Console.ReadKey();
    }

    static void RegistrarPregunta(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("--- Registrar Nueva Pregunta ---");

        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine()?.Trim() ?? "";

        if (db.Preguntas.Any(p => p.Enunciado.ToLower() == enunciado.ToLower())) {
            Console.WriteLine("Esa pregunta ya existe.");
            Console.ReadKey();
            return;
        }

        Console.Write("Respuesta A: ");
        string a = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Respuesta B: ");
        string b = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Respuesta C: ");
        string c = Console.ReadLine()?.Trim() ?? "";

        string correcta;
        do {
            Console.Write("¿Cuál es la respuesta correcta? (A/B/C): ");
            correcta = Console.ReadLine()?.Trim().ToUpper() ?? "";
        } while (!new[] { "A", "B", "C" }.Contains(correcta));

        db.Preguntas.Add(new Pregunta {
            Enunciado = enunciado,
            RespuestaA = a,
            RespuestaB = b,
            RespuestaC = c,
            Correcta = correcta
        });

        db.SaveChanges();
        Console.WriteLine("La Pregunta se guardó correctamente.");
        Console.ReadKey();
    }

    static void MostrarRespuestasDadas(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("--- Respuestas Registradas por Alumnos ---");

        var respuestas = db.Respuestas
            .Include(r => r.Pregunta)
            .Include(r => r.Resultado)
            .OrderByDescending(r => r.Resultado.Fecha);

        foreach (var r in respuestas) {
            Console.WriteLine($"\nAlumno: {r.Resultado.Alumno} | Fecha: {r.Resultado.Fecha:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Pregunta: {r.Pregunta.Enunciado}");
            Console.WriteLine($"Respuesta dada: {r.RespuestaDada} | Correcta: {r.Pregunta.Correcta} | {(r.EsCorrecta ? "Correcta" : "Incorrecta")}");
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarMenuReportes(DatosContexto db) {
        while (true) {
            Console.Clear();
            Console.WriteLine("\n--- MENÚ DE REPORTES ---");
            Console.WriteLine("1. Listado de todos los exámenes");
            Console.WriteLine("2. Filtrar resultados por nombre de alumno");
            Console.WriteLine("3. Ranking de mejores alumnos");
            Console.WriteLine("4. Informe estadístico por pregunta");
            Console.WriteLine("5. Volver al menú principal");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine() ?? "";

            switch (opcion) {
                case "1":
                    MostrarListadoExamenes(db);
                    break;
                case "2":
                    Console.Write("Ingrese el nombre a buscar: ");
                    string nombre = Console.ReadLine() ?? "";
                    MostrarFiltradoPorAlumno(db, nombre);
                    break;
                case "3":
                    MostrarRanking(db);
                    break;
                case "4":
                    MostrarEstadisticasPorPregunta(db);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }
    }

    static void MostrarListadoExamenes(DatosContexto db) {
        Console.WriteLine("\n--- Todos los exámenes ---");
        foreach (var r in db.Resultados.OrderByDescending(x => x.Fecha)) {
            Console.WriteLine($"{r.Fecha:dd/MM/yyyy HH:mm} - {r.Alumno} - Nota: {r.NotaFinal:F1}");
        }
    }

    static void MostrarFiltradoPorAlumno(DatosContexto db, string nombre) {
        Console.WriteLine($"\n--- Exámenes de '{nombre}' ---");
        var resultados = db.Resultados
            .Where(r => r.Alumno.ToLower().Contains(nombre.ToLower()))
            .OrderByDescending(r => r.Fecha)
            .ToList();

        if (!resultados.Any()) {
            Console.WriteLine("No se encontraron resultados.");
            return;
        }

        foreach (var r in resultados) {
            Console.WriteLine($"{r.Fecha:dd/MM/yyyy HH:mm} - {r.Alumno} - Nota: {r.NotaFinal:F1}");
        }
    }

    static void MostrarRanking(DatosContexto db) {
        Console.WriteLine("\n--- Ranking (mejor nota por alumno) ---");
        var ranking = db.Resultados
            .GroupBy(x => x.Alumno)
            .Select(g => new { Alumno = g.Key, MejorNota = g.Max(x => x.NotaFinal) })
            .OrderByDescending(x => x.MejorNota);

        foreach (var r in ranking) {
            Console.WriteLine($"{r.Alumno} - Nota Máxima: {r.MejorNota:F1}");
        }
    }

    static void MostrarEstadisticasPorPregunta(DatosContexto db) {
        Console.WriteLine("\n--- Estadísticas por Pregunta ---");
        foreach (var p in db.Preguntas.Include(p => p.Respuestas)) {
            int total = p.Respuestas.Count;
            int correctas = p.Respuestas.Count(r => r.EsCorrecta);
            double porcentaje = total == 0 ? 0 : 100.0 * correctas / total;

            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"Respondida: {total} veces - Correctas: {porcentaje:F1}%");
        }
    }
}



