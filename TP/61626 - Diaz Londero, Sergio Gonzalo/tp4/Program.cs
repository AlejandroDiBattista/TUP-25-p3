using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// MODELOS
class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

// CONTEXTO
class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.Pregunta)
            .WithMany()
            .HasForeignKey(r => r.PreguntaId);

        modelBuilder.Entity<RespuestaExamen>()
            .HasOne(r => r.ResultadoExamen)
            .WithMany(e => e.Respuestas)
            .HasForeignKey(r => r.ResultadoExamenId);
    }
}

class Program {
    static void Main(string[] args) {
        using (var db = new DatosContexto()) {
            db.Database.EnsureCreated();
        }

        int opcion;
        do
        {
            Console.Clear();
            Console.WriteLine("EXAMEN DE C#");
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1. Cargar Preguntas");
            Console.WriteLine("2. Tomar Examen");
            Console.WriteLine("3. Ver Exámenes");
            Console.WriteLine("4. Buscar por Alumno");
            Console.WriteLine("5. Ver Ranking");
            Console.WriteLine("6. Ver Estadísticas por Pregunta");
            Console.WriteLine("0. Salir");
            Console.Write("\nElija una opción: ");
            opcion = int.Parse(Console.ReadLine() ?? "0");

            switch (opcion)
            {
                case 1:
                    CargarPreguntas();
                    break;
                case 2:
                    TomarExamen();
                    break;
                case 3:
                    VerExamenes();
                    break;
                case 4:
                    BuscarPorAlumno();
                    break;
                case 5:
                    VerRanking();
                    break;
                case 6:
                    VerEstadisticas();
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    Console.WriteLine("Presione una tecla para continuar");
                    Console.ReadKey();
                    break;
            }
        } while (opcion != 0);
    }

      static void CargarPreguntas() {
        using (var db = new DatosContexto()) {
            Console.Clear();
            Console.WriteLine("CARGAR PREGUNTAS");

            var preguntas = new List<Pregunta> {
                new Pregunta {
                    Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                    RespuestaA = "Java",
                    RespuestaB = "C#",
                    RespuestaC = "Python",
                    Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Qué palabra clave se usa para declarar una clase en C#?",
                    RespuestaA = "class",
                    RespuestaB = "struct",
                    RespuestaC = "interface",
                    Correcta = "A"
                },
                new Pregunta {
                    Enunciado = "¿Cuál es el tipo de dato correcto para números enteros en C#?",
                    RespuestaA = "float",
                    RespuestaB = "int",
                    RespuestaC = "string",
                    Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Qué estructura se utiliza para repetir un bloque de código varias veces?",
                    RespuestaA = "if",
                    RespuestaB = "switch",
                    RespuestaC = "for",
                    Correcta = "C"
                },
                new Pregunta {
                    Enunciado = "¿Qué palabra se utiliza para definir un método que no devuelve valor?",
                    RespuestaA = "void",
                    RespuestaB = "return",
                    RespuestaC = "null",
                    Correcta = "A"
                },
                new Pregunta {
                    Enunciado = "¿Qué operador se usa para comparar si dos valores son iguales?",
                    RespuestaA = "=",
                    RespuestaB = "==",
                    RespuestaC = "!=",
                    Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Cómo se declara una variable de texto en C#?",
                    RespuestaA = "text nombre = \"Hola\";",
                    RespuestaB = "char nombre = \"Hola\";",
                    RespuestaC = "string nombre = \"Hola\";",
                    Correcta = "C"
                },
                new Pregunta {
                   Enunciado  = "¿Qué sentencia se usa para manejar errores en C#?",
                   RespuestaA = "catch-try",
                   RespuestaB = "try-catch",
                   RespuestaC = "if-else",
                   Correcta   = "B"

                },
                new Pregunta {
                    Enunciado  = "¿Cuál de las siguientes declaraciones crea un arreglo de 5 enteros en C#?",
                    RespuestaA = "int[] numeros = new int(5);",
                    RespuestaB = "int numeros = [5];",
                    RespuestaC = "int[] numeros = new int[5];",
                    Correcta   = "C"

                },
                new Pregunta {
                    Enunciado  = "¿Qué tipo de valor devuelve un método que no retorna ningún dato en C#?",
                    RespuestaA = "null",
                    RespuestaB = "void",
                    RespuestaC = "empty",
                    Correcta   = "B"

                },
                
            };

            db.Preguntas.AddRange(preguntas);
            db.SaveChanges();

            Console.WriteLine("Preguntas cargadas correctamente. Presione una tecla...");
            Console.ReadKey();
        }
    }

    static void TomarExamen() {
    Console.Clear();
    Console.WriteLine("=== TOMAR EXAMEN ===");
    Console.Write("Nombre del alumno: ");
    string nombre = Console.ReadLine() ?? "";

    using (var db = new DatosContexto()) {
        
        var preguntas = db.Preguntas.ToList(); 

      
        if (preguntas.Count < 5) {
            Console.WriteLine("No hay suficientes preguntas (mínimo 5). Presione una tecla...");
            Console.ReadKey();
            return;
        }

       
        var random = new Random();
        preguntas = preguntas.OrderBy(p => random.Next()).Take(5).ToList(); 
        int correctas = 0;
        var resultado = new ResultadoExamen {
            NombreAlumno = nombre,
            TotalPreguntas = 5
        };

        foreach (var p in preguntas) {
            Console.Clear();
            Console.WriteLine(p.Enunciado);
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");
            Console.Write("Respuesta (A/B/C): ");
            string resp = Console.ReadLine()?.ToUpper() ?? "A";
            bool esCorrecta = resp == p.Correcta;

            if (esCorrecta) correctas++;

            var r = new RespuestaExamen {
                PreguntaId = p.PreguntaId,
                RespuestaAlumno = resp,
                EsCorrecta = esCorrecta
            };
            resultado.Respuestas.Add(r);
        }

        resultado.CantCorrectas = correctas;
        resultado.NotaFinal = correctas;

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Nota: {resultado.NotaFinal}/5");
        Console.WriteLine("Presione una tecla...");
        Console.ReadKey();
    }
}


    static void VerExamenes() {
        Console.Clear();
        Console.WriteLine("EXÁMENES RENDIDOS ");
        using (var db = new DatosContexto()) {
            var examenes = db.Resultados.ToList();
            foreach (var e in examenes) {
                Console.WriteLine($"{e.NombreAlumno} - Nota: {e.NotaFinal}/10 - Correctas: {e.CantCorrectas}/{e.TotalPreguntas}");
            }
        }
        Console.WriteLine("Presione una tecla...");
        Console.ReadKey();
    }

    static void BuscarPorAlumno() {
        Console.Clear();
        Console.WriteLine("BUSCAR POR ALUMNO");
        Console.Write("Ingrese nombre: ");
        string nombre = Console.ReadLine() ?? "";

        using (var db = new DatosContexto()) {
            var resultados = db.Resultados
                .Where(r => r.NombreAlumno.ToLower().Contains(nombre.ToLower()))
                .ToList();

            foreach (var r in resultados) {
                Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal}/10");
            }
        }

        Console.WriteLine("Presione una tecla...");
        Console.ReadKey();
    }

    static void VerRanking() {
        Console.Clear();
        Console.WriteLine("RANKING DE MEJORES NOTAS");

        using (var db = new DatosContexto()) {
            var ranking = db.Resultados
                .OrderByDescending(r => r.NotaFinal)
                .Take(10)
                .ToList();

            foreach (var r in ranking) {
                Console.WriteLine($"{r.NombreAlumno} - Mejor Nota: {r.NotaFinal}/10");
            }
        }

        Console.WriteLine("Presione una tecla...");
        Console.ReadKey();
    }

    static void VerEstadisticas() {
        Console.Clear();
        Console.WriteLine("ESTADÍSTICAS POR PREGUNTA");

        using (var db = new DatosContexto()) {
            var estadisticas = db.Preguntas
                .Select(p => new {
                    Pregunta = p,
                    Total = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId),
                    Correctas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta)
                }).ToList();

            foreach (var e in estadisticas) {
                string porcentaje = e.Total == 0 ? "0%" : $"{(e.Correctas * 100 / e.Total)}%";
                Console.WriteLine($"#{e.Pregunta.PreguntaId:000} - {e.Pregunta.Enunciado}");
                Console.WriteLine($"  Respondida: {e.Total} veces - Correctas: {porcentaje}\n");
            }
        }

        Console.WriteLine("Presione una tecla...");
        Console.ReadKey();
    }
}
