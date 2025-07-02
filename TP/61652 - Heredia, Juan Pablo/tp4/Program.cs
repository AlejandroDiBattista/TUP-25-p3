using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static System.Console;
class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
}
class ResultadoExamen{
    public int ResultadoExamenId {get; set; }
    public string Alumno {get; set;} ="";
    public int Correctas {get; set;}
    public int Total{get; set;}
    public double Nota {get; set;}
    public List<RespuestasExamen> Respuestas{get; set;}=new ();
}
class RespuestasExamen{
    [Key]
    public int RespuestaExamenId {get; set; }
    public int PreguntaId {get; set;}
    public Pregunta Pregunta {get; set;}=null!;
    public string RespuestaAlumno {get; set;} = "";
    public bool EsCorrecta {get; set;}
    public int ResultadoExamenId {get; set;}
    public ResultadoExamen ResultadoExamen {get; set;}=null!;
}
class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados {get; set;}
    public DbSet<RespuestasExamen> Respuestas {get; set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
}
class Program{
    static void Main(string[] args){
        using var db = new DatosContexto();
        db.Database.EnsureCreated();
        while(true){
            Clear();
            WriteLine("\n===Sistema de Examen===");
            WriteLine("1.Ingresar Preguntas");
            WriteLine("2.Tomar Examen");
            WriteLine("3.Reporte Examenes");
            WriteLine("0.Salir");
            Write("Ingrese una opcion: ");
            string? opcion=ReadLine();
            switch(opcion){
                case "1":AgregarPregunta(db); break;
                case "2":TomarExamen(db); break;
                case "3":VerReportes(db); break;
                case "0": return;
                default: WriteLine("Opcion invalida "); break;
            }
        }
    }
    static void AgregarPregunta(DatosContexto db){
        WriteLine("\nAgregar nueva pregunta");
        Write("Ingrese Enunciado: ");
        string enunciado=ReadLine()??"";
        Write("Opcion A: ");
        string A=ReadLine()??"";
        Write("Opcion B: ");
        string B=ReadLine()??"";
        Write("Opcion C: ");
        string C=ReadLine()??"";
        Write("Respuesta correcta: ");
        string correcta=ReadLine()?.ToUpper()??"A";
        var pregunta= new Pregunta{
            Enunciado=enunciado,
            RespuestaA=A,
            RespuestaB=B,
            RespuestaC=C,
            Correcta=correcta
        };
        db.Preguntas.Add(pregunta);
        db.SaveChanges();
        Clear();
        WriteLine("Preguntas guardadas");
    }
    static void TomarExamen(DatosContexto db){
        Clear();
        Write("\nIngresar su Nombre: ");
        string alumno=ReadLine()??"Anonimo";
        int cantidad=Math.Min(5,db.Preguntas.Count());
        if (cantidad==0){
            Clear();
            WriteLine("No hay preguntas disponibles");
            ReadKey();
            return;
        }
        var preguntas=db.Preguntas.ToList().OrderBy(p => Guid.NewGuid()).Take(cantidad);
        var respuestas=new List<RespuestasExamen>();
        foreach(var pregunta in preguntas){
            Clear();
            WriteLine($"\n{pregunta.Enunciado}");
            WriteLine($"{pregunta.RespuestaA}");
            WriteLine($"{pregunta.RespuestaB}");
            WriteLine($"{pregunta.RespuestaC}");
            string resp;
            do{
                Write("Ingrese respuesta (A/B/C): ");
                resp=ReadLine()?.ToUpper()??"";
                if (resp != "A" && resp != "B" && resp != "C") {
                    WriteLine("Respuesta inválida. Ingrese solo A, B o C.");
                }
            } while(resp!="A"&&resp!="B"&&resp!="C");
            bool esCorrecta=resp==pregunta.Correcta;
            respuestas.Add(new RespuestasExamen
            {
                PreguntaId=pregunta.PreguntaId,
                RespuestaAlumno=resp,
                EsCorrecta=esCorrecta
            });
        }
        int correctas=respuestas.Count(r => r.EsCorrecta);
        double nota =10.0 * correctas/cantidad;
        var resultado= new ResultadoExamen{
            Alumno=alumno,
            Correctas=correctas,
            Total=cantidad,
            Nota=nota,
            Respuestas=respuestas
        };
        db.Resultados.Add(resultado);
        db.SaveChanges();
        WriteLine($"\nRespuestas correctas {correctas} de {cantidad}");
        WriteLine($"Nota final: {nota:F1}");
    }
    static void VerReportes(DatosContexto db){
        Clear();
        WriteLine("\n==Reportes==");
        WriteLine("1. Ver todos los exámenes");
        WriteLine("2. Filtrar por Alumno");
        Write("Ingrese una opcion: ");
        string? opReporte=ReadLine();
        Clear();
        List<ResultadoExamen> examenes;
        if(opReporte=="2"){
            Write("\nIngrese el nombre del Alumno: ");
            string nombreAlumno=ReadLine()?.Trim().ToLower()??"";
            examenes = db.Resultados
            .Include(r =>r.Respuestas)
            .Where(r=>r.Alumno.ToLower().Contains(nombreAlumno))
            .ToList();
        }
        else{
            examenes= db.Resultados.Include(r=>r.Respuestas).ToList();
        }
        foreach(var ex in examenes){
            WriteLine($"{ex.Alumno} - Nota: {ex.Nota:F1} - Correctas: {ex.Correctas}/{ex.Total}");
        }
        var ranking=examenes
        .GroupBy(e => e.Alumno)
        .Select(g=> new{
            Alumno=g.Key,
            MejorNota=g.Max(e=>e.Nota)
        })
        .OrderByDescending(x =>x.MejorNota);
        WriteLine("\n=== Ranking de Mejores Notas ===");
        foreach(var r in ranking){
            WriteLine($"{r.Alumno}:{r.MejorNota:F1}");
        }
        var estadisticas =db.Preguntas
        .Select(p=>new{
            Pregunta=p,
            Total=db.Respuestas.Count(r=>r.PreguntaId==p.PreguntaId),
            Correctas=db.Respuestas.Count(r=>r.PreguntaId==p.PreguntaId && r.EsCorrecta)
        }).ToList();
        WriteLine("\n=== Estadisticas por Pregunta ===");
        foreach(var stat in estadisticas){
            double porcentaje =stat.Total>0?100.0*stat.Correctas/stat.Total:0;
            WriteLine($"Pregunta:{stat.Pregunta.Enunciado}");
            WriteLine($"Respondida: {stat.Total} veces, Correctas: {porcentaje:F1}%");
        }
        ReadKey();
    }
}
