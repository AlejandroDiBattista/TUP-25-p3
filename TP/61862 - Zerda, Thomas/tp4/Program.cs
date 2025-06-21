using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
    public float NotaFinal { get; set; }
    public List<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
}

class Respuesta {
    public int RespuestaId { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
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

        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Pregunta)
            .WithMany()
            .HasForeignKey(r => r.PreguntaId);
    }
}

class Program {
    static void Main(string[] args) {
        using (var db = new DatosContexto()) {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Console.WriteLine("Base de datos creada correctamente.");

            // Poblar la tabla de preguntas
            db.Preguntas.AddRange(
                new Pregunta {
                    Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                    RespuestaA = "Java",
                    RespuestaB = "C#",
                    RespuestaC = "Python",
                    Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Qué significa SQL?",
                    RespuestaA = "Structured Query Language",
                    RespuestaB = "Simple Query Language",
                    RespuestaC = "Standard Query Language",
                    Correcta = "A"
                },
                new Pregunta {
                    Enunciado = "¿Qué estructura de datos usa LIFO (Last In First Out)?",
                    RespuestaA = "Cola",
                    RespuestaB = "Lista",
                    RespuestaC = "Pila",
                    Correcta = "C"
                },
                new Pregunta {
                    Enunciado = "¿Cuál de estos NO es un tipo de dato primitivo en C#?",
                    RespuestaA = "int",
                    RespuestaB = "string",
                    RespuestaC = "double",
                    Correcta = "B"
                },
                new Pregunta {
                    Enunciado = "¿Qué patrón de diseño se usa para crear objetos?",
                    RespuestaA = "Singleton",
                    RespuestaB = "Factory",
                    RespuestaC = "Observer",
                    Correcta = "B"
                }
            );
            db.SaveChanges();

            Console.WriteLine("Preguntas iniciales agregadas.");

            bool salir = false;
            while (!salir) {
                Console.WriteLine("\nMENU PRINCIPAL");
                Console.WriteLine("1. Agregar nueva pregunta");
                Console.WriteLine("2. Tomar examen");
                Console.WriteLine("3. Ver reportes");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");
                
                string opcion = Console.ReadLine() ?? "";

                switch (opcion) {
                    case "1":
                        AgregarPregunta(db);
                        break;
                    case "2":
                        TomarExamen(db);
                        break;
                    case "3":
                        MostrarReportes(db);
                        break;
                    case "4":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }
            }
        }
    }

    static void AgregarPregunta(DatosContexto db) {
        Console.WriteLine("\nAGREGAR NUEVA PREGUNTA");
        
        Console.Write("Enunciado: ");
        string enunciado = Console.ReadLine() ?? "";
        
        Console.Write("Respuesta A: ");
        string respuestaA = Console.ReadLine() ?? "";
        
        Console.Write("Respuesta B: ");
        string respuestaB = Console.ReadLine() ?? "";
        
        Console.Write("Respuesta C: ");
        string respuestaC = Console.ReadLine() ?? "";
        
        string correcta;
        do {
            Console.Write("Respuesta correcta (A, B o C): ");
            correcta = (Console.ReadLine() ?? "").ToUpper();
        } while (correcta != "A" && correcta != "B" && correcta != "C");

        db.Preguntas.Add(new Pregunta {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta
        });
        
        db.SaveChanges();
        Console.WriteLine("Pregunta agregada correctamente!");
    }

    static void TomarExamen(DatosContexto db) {
        Console.WriteLine("\nTOMAR EXAMEN");
        
        Console.Write("Ingrese su nombre: ");
        string nombre = Console.ReadLine() ?? "Anónimo";
        
        var preguntas = db.Preguntas.ToList();
        if (preguntas.Count == 0) {
            Console.WriteLine("No hay preguntas disponibles para el examen.");
            return;
        }
        
        // Seleccionar 5 preguntas aleatorias (o menos si no hay suficientes)
        var random = new Random();
        var examenPreguntas = preguntas.OrderBy(x => random.Next()).Take(5).ToList();
        
        var examen = new Examen {
            NombreAlumno = nombre,
            TotalPreguntas = examenPreguntas.Count
        };
        
        int correctas = 0;
        foreach (var pregunta in examenPreguntas) {
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"A) {pregunta.RespuestaA}");
            Console.WriteLine($"B) {pregunta.RespuestaB}");
            Console.WriteLine($"C) {pregunta.RespuestaC}");
            
            string respuesta;
            do {
                Console.Write("Su respuesta (A, B o C): ");
                respuesta = (Console.ReadLine() ?? "").ToUpper();
            } while (respuesta != "A" && respuesta != "B" && respuesta != "C");
            
            bool esCorrecta = respuesta == pregunta.Correcta;
            if (esCorrecta) correctas++;
            
            examen.Respuestas.Add(new Respuesta {
                PreguntaId = pregunta.PreguntaId,
                EsCorrecta = esCorrecta
            });
        }
        
        examen.Correctas = correctas;
        examen.NotaFinal = (float)correctas / examen.TotalPreguntas * 5f;
        
        db.Examenes.Add(examen);
        db.SaveChanges();
        
        Console.WriteLine($"\nExamen completado! Resultados:");
        Console.WriteLine($"Respuestas correctas: {correctas} de {examen.TotalPreguntas}");
        Console.WriteLine($"Nota final: {examen.NotaFinal.ToString("0.00")}/5.00");
    }

    static void MostrarReportes(DatosContexto db) {
        bool volver = false;
        while (!volver) {
            Console.WriteLine("\nREPORTES");
            Console.WriteLine("1. Listado de todos los exámenes");
            Console.WriteLine("2. Filtrar por nombre de alumno");
            Console.WriteLine("3. Ranking de mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("5. Volver al menú principal");
            Console.Write("Seleccione una opción: ");
            
            string opcion = Console.ReadLine() ?? "";

            switch (opcion) {
                case "1":
                    ListarExamenes(db);
                    break;
                case "2":
                    FiltrarPorNombre(db);
                    break;
                case "3":
                    MostrarRanking(db);
                    break;
                case "4":
                    EstadisticasPreguntas(db);
                    break;
                case "5":
                    volver = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
        }
    }

    static void ListarExamenes(DatosContexto db) {
        Console.WriteLine("\nLISTADO DE TODOS LOS EXÁMENES");
        var examenes = db.Examenes.OrderByDescending(e => e.ExamenId).ToList();
        
        if (examenes.Count == 0) {
            Console.WriteLine("No hay exámenes registrados.");
            return;
        }
        
        foreach (var examen in examenes) {
            Console.WriteLine($"ID: {examen.ExamenId} - Alumno: {examen.NombreAlumno}");
            Console.WriteLine($"Correctas: {examen.Correctas}/{examen.TotalPreguntas} - Nota: {examen.NotaFinal.ToString("0.00")}/5.00");
            Console.WriteLine("------------------------");
        }
    }

    static void FiltrarPorNombre(DatosContexto db) {
        Console.Write("\nIngrese nombre o parte del nombre a buscar: ");
        string busqueda = Console.ReadLine() ?? "";
        
        var examenes = db.Examenes
            .Where(e => e.NombreAlumno.Contains(busqueda))
            .OrderByDescending(e => e.NotaFinal)
            .ToList();
        
        if (examenes.Count == 0) {
            Console.WriteLine("No se encontraron exámenes con ese nombre.");
            return;
        }
        
        Console.WriteLine($"\nRESULTADOS PARA '{busqueda}':");
        foreach (var examen in examenes) {
            Console.WriteLine($"Alumno: {examen.NombreAlumno}");
            Console.WriteLine($"Correctas: {examen.Correctas}/{examen.TotalPreguntas} - Nota: {examen.NotaFinal.ToString("0.00")}/5.00");
            Console.WriteLine("------------------------");
        }
    }

    static void MostrarRanking(DatosContexto db) {
        Console.WriteLine("\nRANKING DE MEJORES ALUMNOS");
        
        var ranking = db.Examenes
            .GroupBy(e => e.NombreAlumno)
            .Select(g => new {
                Nombre = g.Key,
                MejorNota = g.Max(e => e.NotaFinal),
                Promedio = g.Average(e => e.NotaFinal)
            })
            .OrderByDescending(x => x.MejorNota)
            .ToList();
        
        if (ranking.Count == 0) {
            Console.WriteLine("No hay datos para mostrar el ranking.");
            return;
        }
        
        int posicion = 1;
        foreach (var alumno in ranking) {
            Console.WriteLine($"{posicion}. {alumno.Nombre}");
            Console.WriteLine($"   Mejor nota: {alumno.MejorNota.ToString("0.00")}/5.00");
            Console.WriteLine($"   Promedio: {alumno.Promedio.ToString("0.00")}/5.00");
            posicion++;
        }
    }

    static void EstadisticasPreguntas(DatosContexto db) {
        Console.WriteLine("\nESTADÍSTICAS POR PREGUNTA");
        
        var estadisticas = db.Preguntas
            .Select(p => new {
                Pregunta = p,
                TotalRespuestas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId),
                Correctas = db.Respuestas.Count(r => r.PreguntaId == p.PreguntaId && r.EsCorrecta)
            })
            .ToList();
        
        if (estadisticas.Count == 0) {
            Console.WriteLine("No hay preguntas registradas.");
            return;
        }
        
        foreach (var stat in estadisticas) {
            Console.WriteLine($"\nPregunta: {stat.Pregunta.Enunciado}");
            Console.WriteLine($"Total veces respondida: {stat.TotalRespuestas}");
            
            if (stat.TotalRespuestas > 0) {
                double porcentaje = (double)stat.Correctas / stat.TotalRespuestas * 100;
                Console.WriteLine($"Porcentaje de aciertos: {porcentaje.ToString("0.00")}%");
            } else {
                Console.WriteLine("Nunca ha sido respondida en un examen.");
            }
        }
    }
}