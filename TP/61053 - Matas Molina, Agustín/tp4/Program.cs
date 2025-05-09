using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
    public List<RespuestaExamen> RespuestasExamen { get; set; } = new(); // Relación con RespuestaExamen
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!; 
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!; 
    public bool EsCorrecta { get; set; }
}
class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}

class Program {
    static void Main(string[] args) {
        using (var db = new DatosContexto()) {
            try {
                InicializarBaseDeDatos(db);
                MostrarMenuPrincipal(db);
            } catch (Exception ex) {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
        }
    }

   
    static void InicializarBaseDeDatos(DatosContexto db) {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        if (!db.Preguntas.Any()) {
            db.Preguntas.AddRange(new List<Pregunta> {
              new Pregunta {
    Enunciado = "¿Cuál es la consola de videojuegos desarrollada por Sony en el año 2020?",
    RespuestaA = "PlayStation 5",
    RespuestaB = "Xbox Series X",
    RespuestaC = "Nintendo Switch",
    Correcta = "A",
},

new Pregunta {
    Enunciado = "¿Qué personaje es el protagonista de la saga 'The Legend of Zelda'?",
    RespuestaA = "Zelda",
    RespuestaB = "Link",
    RespuestaC = "Ganondorf",
    Correcta = "B",
},

new Pregunta {
    Enunciado = "¿Cuál de los siguientes juegos fue desarrollado por Mojang?",
    RespuestaA = "Minecraft",
    RespuestaB = "Fortnite",
    RespuestaC = "Terraria",
    Correcta = "A",
},

new Pregunta {
    Enunciado = "¿Qué empresa desarrolló la saga 'Halo' originalmente?",
    RespuestaA = "343 Industries",
    RespuestaB = "Bungie",
    RespuestaC = "Epic Games",
    Correcta = "B",
},

new Pregunta {
    Enunciado = "¿Cuál es el título más vendido de la franquicia 'Grand Theft Auto'?",
    RespuestaA = "GTA V",
    RespuestaB = "GTA: San Andreas",
    RespuestaC = "GTA IV",
    Correcta = "A",
},


            });
            db.SaveChanges();
        }
    }

    
    static void MostrarMenuPrincipal(DatosContexto db) {
        while (true) {
            Console.Clear();
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1. Tomar examen");
            Console.WriteLine("2. Ver reportes");
            Console.WriteLine("3. Agregar preguntas");
            Console.WriteLine("4. Ver registro de preguntas");
            Console.WriteLine("5. Salir");
            Console.Write("Opción: ");
            string opcion = Console.ReadLine() ?? "";

            switch (opcion) {
                case "1":
                    TomarExamen(db);
                    break;
                case "2":
                    MostrarReportes(db);
                    break;
                case "3":
                    AgregarPregunta(db);
                    break;
                case "4":
                    RegistroPreguntas(db); 
                    break;
                case "5":
                    Console.WriteLine("Saliendo del sistema. ¡Hasta luego!");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

   
    static void TomarExamen(DatosContexto db) {
        Console.WriteLine("Ingrese su nombre:");
        string nombreAlumno = Console.ReadLine() ?? "Alumno";

       
        var preguntas = db.Preguntas.AsEnumerable().OrderBy(r => Guid.NewGuid()).Take(10).ToList();
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas registradas en el sistema. Presione una tecla para volver al menú...");
            Console.ReadKey();
            return;
        }

        int correctas = 0;
        var resultado = new ResultadoExamen {
            NombreAlumno = nombreAlumno,
            TotalPreguntas = preguntas.Count
        };

        foreach (var pregunta in preguntas) {
            Console.Clear();
            Console.WriteLine($"""
                {pregunta.Enunciado}
                A) {pregunta.RespuestaA}
                B) {pregunta.RespuestaB}
                C) {pregunta.RespuestaC}
            """);

            string respuesta;
            do {
                Console.WriteLine("Seleccione una opción (A, B, C):");
                respuesta = Console.ReadLine()?.ToUpper() ?? "";
                if (respuesta != "A" && respuesta != "B" && respuesta != "C") {
                    Console.WriteLine("Respuesta no válida. Intente nuevamente.");
                }
            } while (respuesta != "A" && respuesta != "B" && respuesta != "C");

            bool esCorrecta = respuesta == pregunta.Correcta;
            if (esCorrecta) {
                correctas++;
                Console.WriteLine("Respuesta correcta. ¡Bien hecho!");
            } else {
                Console.WriteLine($"Respuesta incorrecta. La respuesta correcta era: {pregunta.Correcta}");
            }

            resultado.Respuestas.Add(new RespuestaExamen {
                PreguntaId = pregunta.PreguntaId,
                EsCorrecta = esCorrecta
            });
        }

        resultado.RespuestasCorrectas = correctas;
        resultado.NotaFinal = (double)correctas / preguntas.Count * 10;
        db.ResultadosExamen.Add(resultado);
        db.SaveChanges();

        Console.Clear();
        Console.WriteLine($"Examen finalizado. Respuestas correctas: {correctas}/{preguntas.Count}");
        Console.WriteLine($"Nota final: {resultado.NotaFinal:F2}");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void AgregarPregunta(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("Ingrese el enunciado de la pregunta:");
        string enunciado = Console.ReadLine() ?? "";

        Console.WriteLine("Ingrese la opción A:");
        string respuestaA = Console.ReadLine() ?? "";

        Console.WriteLine("Ingrese la opción B:");
        string respuestaB = Console.ReadLine() ?? "";

        Console.WriteLine("Ingrese la opción C:");
        string respuestaC = Console.ReadLine() ?? "";

        string correcta;
        do {
            Console.WriteLine("Ingrese la respuesta correcta (A, B, C):");
            correcta = Console.ReadLine()?.ToUpper() ?? "";
            if (correcta != "A" && correcta != "B" && correcta != "C") {
                Console.WriteLine("Respuesta no válida. Intente nuevamente.");
            }
        } while (correcta != "A" && correcta != "B" && correcta != "C");

        var nuevaPregunta = new Pregunta {
            Enunciado = enunciado,
            RespuestaA = respuestaA,
            RespuestaB = respuestaB,
            RespuestaC = respuestaC,
            Correcta = correcta
        };

        db.Preguntas.Add(nuevaPregunta);
        db.SaveChanges();

        Console.WriteLine("Pregunta registrada exitosamente. Presione una tecla para continuar...");
        Console.ReadKey();
    }

  
    static void MostrarReportes(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("Seleccione un reporte:");
        Console.WriteLine("1. Listado de todos los exámenes rendidos");
        Console.WriteLine("2. Filtrar resultados por nombre de alumno");
        Console.WriteLine("3. Ranking de mejores alumnos");
        Console.WriteLine("4. Informe estadístico por pregunta");
        Console.WriteLine("5. Ver preguntas con respuestas correctas");
        Console.Write("Opción: ");
        string opcion = Console.ReadLine() ?? "";

        switch (opcion) {
            case "1":
                ListarExamenes(db);
                break;
            case "2":
                FiltrarResultadosPorAlumno(db);
                break;
            case "3":
                MostrarRanking(db);
                break;
            case "4":
                GenerarInformeEstadistico(db);
                break;
            case "5":
                RegistroPreguntas(db);
                break;
            default:
                Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                Console.ReadKey();
                break;
        }
    }

  
    static void ListarExamenes(DatosContexto db) {
        var examenes = db.ResultadosExamen.Include(r => r.Respuestas).AsNoTracking().ToList();
        if (!examenes.Any()) {
            Console.WriteLine("No hay exámenes registrados.");
        } else {
            Console.WriteLine("Listado de exámenes rendidos:");
            foreach (var examen in examenes) {
                Console.WriteLine($"Alumno: {examen.NombreAlumno}, Nota: {examen.NotaFinal:F2}, Correctas: {examen.RespuestasCorrectas}/{examen.TotalPreguntas}");
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void FiltrarResultadosPorAlumno(DatosContexto db) {
        Console.WriteLine("Ingrese el nombre del alumno:");
        string nombre = Console.ReadLine() ?? "";
        var examenes = db.ResultadosExamen.Where(r => r.NombreAlumno.Contains(nombre)).AsNoTracking().ToList();
        if (!examenes.Any()) {
            Console.WriteLine($"No se encontraron resultados para el alumno '{nombre}'.");
        } else {
            Console.WriteLine($"Resultados para el alumno '{nombre}':");
            foreach (var examen in examenes) {
                Console.WriteLine($"Nota: {examen.NotaFinal:F2}, Correctas: {examen.RespuestasCorrectas}/{examen.TotalPreguntas}");
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarRanking(DatosContexto db) {
        var ranking = db.ResultadosExamen.OrderByDescending(r => r.NotaFinal).Take(5).AsNoTracking().ToList();
        if (!ranking.Any()) {
            Console.WriteLine("No hay exámenes registrados para generar un ranking.");
        } else {
            Console.WriteLine("Ranking de mejores alumnos:");
            foreach (var examen in ranking) {
                Console.WriteLine($"Alumno: {examen.NombreAlumno}, Nota: {examen.NotaFinal:F2}");
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void GenerarInformeEstadistico(DatosContexto db) {
        var preguntas = db.Preguntas.Include(p => p.RespuestasExamen).AsNoTracking().ToList();
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas registradas para generar un informe.");
        } else {
            Console.WriteLine("Informe estadístico por pregunta:");
            foreach (var pregunta in preguntas) {
                int totalRespuestas = pregunta.RespuestasExamen.Count;
                int correctas = pregunta.RespuestasExamen.Count(r => r.EsCorrecta);
                double porcentajeCorrectas = totalRespuestas > 0 ? (double)correctas / totalRespuestas * 100 : 0;
                Console.WriteLine($"""
                    Pregunta: {pregunta.Enunciado}
                    Total respondida: {totalRespuestas}
                    Porcentaje correctas: {porcentajeCorrectas:F2}%
                """);
            }
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void RegistroPreguntas(DatosContexto db) {
        Console.Clear();
        var preguntas = db.Preguntas.AsNoTracking().ToList();

        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas registradas en el sistema.");
        } else {
            Console.WriteLine("Listado de preguntas con sus respuestas correctas:");
            foreach (var pregunta in preguntas) {
                Console.WriteLine($"""
                    Pregunta: {pregunta.Enunciado}
                    A) {pregunta.RespuestaA}
                    B) {pregunta.RespuestaB}
                    C) {pregunta.RespuestaC}
                    Respuesta Correcta: {pregunta.Correcta}
                """);
            }
        }

        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}