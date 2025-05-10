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
    public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}
class Examen {
    public int ExamenId { get; set; }
    public string Alumno { get; set; } = "";
    public int Correctas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime FechaRendida { get; set; }
    public ICollection<RespuestaExamen> Respuestas { get; set; } =new List<RespuestaExamen>();
}
class RespuestaExamen {
    public int RespuestaExamenId { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; } 
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public string RespuestaElegida { get; set; } = ""; 
    public bool EsCorrecta { get; set; } 
}
class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}

class Program{
    const int MAX_PREGUNTAS = 5;
    static void Main(string[] args){
        using (var db = new DatosContexto()){
            db.Database.EnsureCreated();
            }
         bool salida = true;
     while(salida){
            Console.Clear();
            Console.WriteLine("=== Sistema de Exámenes Multiple Choice ===");
            Console.WriteLine("1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Reportes");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            var op = Console.ReadLine();
            switch(op){
                case "1": RegistrarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": MostrarReportes(); break;
                case "0": salida = false; break;
                default:
                    Console.WriteLine("Opción no válida.");
                    Pausa();
                    break;
            }
        }
 
        }
    static void RegistrarPregunta(){
                Console.Clear();
                Console.WriteLine("=== Registrar Pregunta ===");
                var pregunta = new Pregunta();
                Console.Write("Enunciado: ");
                pregunta.Enunciado = Console.ReadLine();
                Console.Write("Respuesta A: ");
                pregunta.RespuestaA = Console.ReadLine();
                Console.Write("Respuesta B: ");
                pregunta.RespuestaB = Console.ReadLine();
                Console.Write("Respuesta C: ");
                pregunta.RespuestaC = Console.ReadLine();
                Console.Write("Respuesta Correcta (A/B/C): ");
                var Correcta = Console.ReadLine().ToUpper();
                if (Correcta is "A" or "B" or "C"){
                    pregunta.Correcta = Correcta;
                } 
                else {
                    Console.WriteLine("Respuesta correcta inválida. Debe ser A, B o C.");
                    Pausa();
                    return;
                }
                using var db = new DatosContexto();
                db.Preguntas.Add(pregunta);
                db.SaveChanges();
                Console.WriteLine("Pregunta registrada correctamente.");
                Pausa();
            }
    static void TomarExamen(){
                Console.Clear();
                Console.WriteLine("=== Tomar Examen ===");
                Console.WriteLine("Nombre del alumno: ");
                var nombre = Console.ReadLine();
                if (string.IsNullOrEmpty(nombre)){
                    Console.WriteLine("Nombre inválido.");
                    Pausa();
                    return;
                }
                using var db = new DatosContexto();
                var preguntas = db.Preguntas.ToList().OrderBy(r => Guid.NewGuid()).Take(MAX_PREGUNTAS).ToList();
                var examen = new Examen { TotalPreguntas = preguntas.Count,
                 Alumno = nombre, Correctas = 0, FechaRendida = DateTime.Now };
                foreach (var pregunta in preguntas){
                    Console.Clear();
                    Console.WriteLine(pregunta.Enunciado);
                    Console.WriteLine($"A) {pregunta.RespuestaA}");
                    Console.WriteLine($"B) {pregunta.RespuestaB}");
                    Console.WriteLine($"C) {pregunta.RespuestaC}");
                    Console.Write("Respuesta: ");
                    var respuesta = Console.ReadLine().ToUpper();
                    while (respuesta != "A" && respuesta != "B" && respuesta != "C"){
                        Console.WriteLine("Respuesta inválida. Debe ser A, B o C.");
                        Console.Write("Respuesta: ");
                        respuesta = Console.ReadLine().ToUpper();
                    }
                    if (respuesta == pregunta.Correcta){
                        examen.Correctas++;
                        examen.Respuestas.Add(new RespuestaExamen { PreguntaId = pregunta.PreguntaId,
                         RespuestaElegida = respuesta, EsCorrecta = true });
                    } else {
                        examen.Respuestas.Add(new RespuestaExamen { PreguntaId = pregunta.PreguntaId, RespuestaElegida = respuesta, EsCorrecta = false });
                    }
                }
                examen.NotaFinal = (double)examen.Correctas / examen.TotalPreguntas * 5;
                db.Examenes.Add(examen);
                db.SaveChanges();
                Console.Clear();
                 Console.WriteLine($"Examen completado. Correctas: {examen.Correctas}/{examen.TotalPreguntas}");
                Console.WriteLine($" Nota Final: {examen.NotaFinal*10/MAX_PREGUNTAS}");
                Pausa();
            }
    static void MostrarReportes(){
            Console.Clear();
            Console.WriteLine("=== Reportes ===");
            Console.WriteLine("1. Listar exámenes");
            Console.WriteLine("2. Filtrar por alumno");
            Console.WriteLine("3. Ranking mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("0. Volver");
            Console.Write("Opción: ");
             var op = Console.ReadLine();
        using var db = new DatosContexto();
        switch(op){
            case "1":
                var todos = db.Examenes.Include(r => r.Respuestas).ToList();
                foreach(var r in todos)
                    Console.WriteLine($"ID de examen:{r.ExamenId}. Alumno:{r.Alumno} - Nota: {r.NotaFinal*10/MAX_PREGUNTAS} - Fecha: {r.FechaRendida}");
                break;
            case "2":
                Console.Write("Nombre alumno: ");
                 var nombre = Console.ReadLine();
                var filtro = db.Examenes.Where(r => r.Alumno.Contains(nombre)).ToList();
                foreach(var r in filtro)
                    Console.WriteLine($"ID de Examen:{r.ExamenId}. Alumno:{r.Alumno} - Nota: {r.NotaFinal*10/MAX_PREGUNTAS}");
                break;
            case "3":
                var ranking = db.Examenes
                    .GroupBy(r => r.Alumno)
                    .Select(g => new { Alumno = g.Key, MejorNota = g.Max(x => x.NotaFinal) })
                    .OrderByDescending(x => x.MejorNota)
                    .ToList();
                foreach(var x in ranking)
                    Console.WriteLine($"{x.Alumno} - Mejor Nota: {x.MejorNota*10/MAX_PREGUNTAS}");
                break;
            case "4":
                var estado = db.Preguntas
                    .Select(p => new {
                        p.PreguntaId,
                        p.Enunciado,
                        Total = p.Respuestas.Count,
                        PorcCorrectas = p.Respuestas.Count > 0
                            ? Math.Round((double)p.Respuestas.Count(r => r.EsCorrecta) / p.Respuestas.Count * 100, 2)
                            : 0
                    })
                    .ToList();
                foreach(var s in estado)
                    Console.WriteLine($"#{s.PreguntaId}: {s.Enunciado}\n  Respondida: {s.Total} veces, {s.PorcCorrectas}% correctas\n");
                break;
            default: break;
        }
        Pausa();
    }

    static void Pausa(){
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();}
    }

