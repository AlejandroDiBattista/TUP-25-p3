using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB {get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
    public List<Respuesta> Respuestas { get; set; } = new();
}

class Examen {
    public int Id { get; set; }
    public string Alumno { get; set; } = "";
    public int Correctas { get; set; }
    public int Total { get; set; }
    public double Nota { get; set; }
    public List<Respuesta> Respuestas { get; set; } = new();
}

class Respuesta {
    public int Id { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } = null!;
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
    public bool EsCorrecta { get; set; }
}

class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; } = null!;
    public DbSet<Examen> Examenes { get; set; } = null!;
    public DbSet<Respuesta> Respuestas { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}

class Program {
    private const int PREGUNTAS_POR_DEFECTO = 5;

    static void Main(string[] args) {
        using (var db = new DatosContexto()) {
            db.Database.EnsureCreated();
            
            while (true) {
                Console.Clear();
                Console.WriteLine("=== Sistema de Exámenes ===");
                Console.WriteLine("1) Registrar Pregunta");
                Console.WriteLine("2) Tomar Examen");
                Console.WriteLine("3) Ver Reportes");
                Console.WriteLine("4) Eliminar Pregunta");
                Console.WriteLine("0) Salir");
                Console.Write("Opción: ");
                
                var op = Console.ReadLine();
                if (op == "0") break;
                
                switch (op) {
                    case "1": RegistrarPregunta(db); break;
                    case "2": TomarExamen(db); break;
                    case "3": VerReportes(db); break;
                    case "4": EliminarPregunta(db); break;
                    default:
                        Console.WriteLine("Opción no válida");
                        Console.WriteLine("Pulsa ENTER para continuar");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }

    static void RegistrarPregunta(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Registrar Nueva Pregunta ===\n");
        var p = new Pregunta();
        
        Console.Write("Enunciado: "); 
        p.Enunciado = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(p.Enunciado)) return;
        
        Console.Write("A) "); p.RespuestaA = Console.ReadLine()?.Trim() ?? "";
        Console.Write("B) "); p.RespuestaB = Console.ReadLine()?.Trim() ?? "";
        Console.Write("C) "); p.RespuestaC = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(p.RespuestaA) || string.IsNullOrEmpty(p.RespuestaB) || string.IsNullOrEmpty(p.RespuestaC)) {
            Console.WriteLine("Las respuestas no pueden estar vacías");
            return;
        }
        
        do {
            Console.Write("Correcta (A/B/C): ");
            p.Correcta = Console.ReadLine()?.Trim().ToUpper() ?? "";
        } while (p.Correcta != "A" && p.Correcta != "B" && p.Correcta != "C");

        Console.WriteLine("\n¿Desea guardar esta pregunta? (S/N)");
        if (Console.ReadLine()?.Trim().ToUpper() != "S") return;
        
        db.Preguntas.Add(p);
        db.SaveChanges();
        
        Console.WriteLine("\n¡Pregunta registrada correctamente!");
        Console.WriteLine("Pulsa ENTER para continuar");
        Console.ReadLine();
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Nuevo Examen ===\n");
        Console.Write("Nombre del alumno: ");
        var alumno = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(alumno)) return;

        var preguntasDisponibles = db.Preguntas.ToList();
        var cantidadPreguntas = Math.Min(PREGUNTAS_POR_DEFECTO, preguntasDisponibles.Count);
        
        if (cantidadPreguntas == 0) {
            Console.WriteLine("No hay preguntas disponibles.");
            Console.WriteLine("Pulsa ENTER para continuar");
            Console.ReadLine();
            return;
        }

        var random = new Random();
        var preguntas = preguntasDisponibles
            .OrderBy(x => random.Next())
            .Take(cantidadPreguntas)
            .ToList();

        var examen = new Examen { 
            Alumno = alumno,
            Total = cantidadPreguntas
        };

        for (int i = 0; i < preguntas.Count; i++) {
            var p = preguntas[i];
            Console.Clear();
            Console.WriteLine($"=== Pregunta {i + 1}/{cantidadPreguntas} ===\n");
            Console.WriteLine(p.Enunciado);
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}\n");
            
            string respuesta;
            do {
                Console.Write("Tu respuesta (A/B/C): ");
                respuesta = Console.ReadLine()?.Trim().ToUpper() ?? "";
            } while (respuesta != "A" && respuesta != "B" && respuesta != "C");

            var esCorrecta = respuesta == p.Correcta;
            if (esCorrecta) {
                examen.Correctas++;
                Console.WriteLine("\n¡Correcto!");
            } else {
                Console.WriteLine($"\nIncorrecto. La respuesta correcta era: {p.Correcta}");
            }
            Console.WriteLine("\nPresiona ENTER para continuar...");
            Console.ReadLine();

            examen.Respuestas.Add(new Respuesta {
                Pregunta = p,
                EsCorrecta = esCorrecta
            });
        }

        examen.Nota = Math.Round((double)examen.Correctas / examen.Total * 10, 2);
        db.Examenes.Add(examen);
        db.SaveChanges();

        Console.Clear();
        Console.WriteLine($"=== Resultado Final ===\n");
        Console.WriteLine($"Correctas: {examen.Correctas}/{examen.Total}");
        Console.WriteLine($"Nota final: {examen.Nota}/10");
        Console.WriteLine("\nPulsa ENTER para continuar");
        Console.ReadLine();
    }

    static void VerReportes(DatosContexto db) {
        while (true) {
            Console.Clear();
            Console.WriteLine("=== Menú de Reportes ===");
            Console.WriteLine("1) Ver todos los exámenes");
            Console.WriteLine("2) Buscar por alumno");
            Console.WriteLine("3) Ranking de mejores notas");
            Console.WriteLine("4) Estadísticas por pregunta");
            Console.WriteLine("0) Volver al menú principal");
            Console.Write("Opción: ");
            
            var op = Console.ReadLine();
            if (op == "0") break;
            
            switch (op) {
                case "1": MostrarTodosExamenes(db); break;
                case "2": BuscarPorAlumno(db); break;
                case "3": MostrarRanking(db); break;
                case "4": MostrarEstadisticas(db); break;
                default:
                    Console.WriteLine("Opción no válida");
                    Console.WriteLine("Pulsa ENTER para continuar");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void MostrarTodosExamenes(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Todos los Exámenes ===\n");
        
        var examenes = db.Examenes
            .OrderByDescending(e => e.Id)
            .ToList();
            
        if (!examenes.Any()) {
            Console.WriteLine("No hay exámenes registrados.");
        } else {
            foreach (var e in examenes) {
                Console.WriteLine($"{e.Alumno}: {e.Nota}/10 ({e.Correctas}/{e.Total})");
            }
        }
        
        Console.WriteLine("\nPulsa ENTER para continuar");
        Console.ReadLine();
    }

    static void BuscarPorAlumno(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Buscar Exámenes por Alumno ===\n");
        Console.Write("Nombre del alumno: ");
        var nombre = Console.ReadLine()?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(nombre)) {
            Console.WriteLine("\nDebe ingresar un nombre");
            Console.WriteLine("Pulsa ENTER para continuar");
            Console.ReadLine();
            return;
        }
        
        var examenes = db.Examenes
            .Where(e => e.Alumno.Contains(nombre))
            .OrderByDescending(e => e.Id)
            .ToList();
        
        Console.WriteLine();
        if (!examenes.Any()) {
            Console.WriteLine("No se encontraron exámenes para ese alumno.");
        } else {
            foreach (var e in examenes) {
                Console.WriteLine($"{e.Alumno}: {e.Nota}/10 ({e.Correctas}/{e.Total})");
            }
        }
        
        Console.WriteLine("\nPulsa ENTER para continuar");
        Console.ReadLine();
    }

    static void MostrarRanking(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Ranking de Mejores Notas ===\n");
        
        var ranking = db.Examenes
            .GroupBy(e => e.Alumno)
            .Select(g => new {
                Alumno = g.Key,
                MejorNota = g.Max(e => e.Nota)
            })
            .OrderByDescending(x => x.MejorNota)
            .ToList();

        if (!ranking.Any()) {
            Console.WriteLine("No hay exámenes registrados.");
        } else {
            foreach (var r in ranking) {
                Console.WriteLine($"{r.Alumno}: {r.MejorNota}/10");
            }
        }
        
        Console.WriteLine("\nPulsa ENTER para continuar");
        Console.ReadLine();
    }

    static void MostrarEstadisticas(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Estadísticas por Pregunta ===\n");
        
        var preguntas = db.Preguntas
            .Include(p => p.Respuestas)
            .ToList();
        
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas registradas.");
        } else {
            foreach (var p in preguntas) {
                var total = p.Respuestas.Count;
                var correctas = p.Respuestas.Count(r => r.EsCorrecta);
                var porcentaje = total > 0 ? (double)correctas / total * 100 : 0;
                
                Console.WriteLine($"""
                    #{p.PreguntaId:000}

                    {p.Enunciado}

                    A) {p.RespuestaA}
                    B) {p.RespuestaB}
                    C) {p.RespuestaC}

                    Respondida {total} veces
                    Porcentaje de aciertos: {porcentaje:F2}%
                    """);
                Console.WriteLine();
            }
        }
        
        Console.WriteLine("\nPulsa ENTER para continuar");
        Console.ReadLine();
    }

    static void EliminarPregunta(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Eliminar Pregunta ===\n");
        
        var preguntas = db.Preguntas.ToList();
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas registradas.");
            Console.WriteLine("\nPulsa ENTER para continuar");
            Console.ReadLine();
            return;
        }

        foreach (var p in preguntas) {
            Console.WriteLine($"""
                #{p.PreguntaId:000}
                {p.Enunciado}
                A) {p.RespuestaA}
                B) {p.RespuestaB}
                C) {p.RespuestaC}
                """);
            Console.WriteLine();
        }

        Console.Write("\nIngrese el ID de la pregunta a eliminar (0 para cancelar): ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id == 0) return;

        var pregunta = db.Preguntas.Find(id);
        if (pregunta == null) {
            Console.WriteLine("Pregunta no encontrada.");
            Console.WriteLine("\nPulsa ENTER para continuar");
            Console.ReadLine();
            return;
        }

        Console.Write("¿Está seguro que desea eliminar esta pregunta? (S/N): ");
        if (Console.ReadLine()?.Trim().ToUpper() != "S") return;

        db.Preguntas.Remove(pregunta);
        db.SaveChanges();

        Console.WriteLine("\nPregunta eliminada correctamente");
        Console.WriteLine("Pulsa ENTER para continuar");
        Console.ReadLine();
    }
}