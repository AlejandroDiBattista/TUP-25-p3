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
}

class ResultadoExamen{
    public int Id { get; set; }
    public string Alumno { get; set; }
    public int RespuestasCorrectas { get; set; }
    public int TotalPreguntas { get; set; } = 10; // Defecto: 10 preguntas por examen
    public double NotaFinal { get; set; }
}

public class RespuestaExamen
{
    public int Id { get; set; }
    public int ExamenId { get; set; }
    public int PreguntaId { get; set; }
    public bool Correcta { get; set; }
}

class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> ResultadosExamen { get; set; }
    public DbSet<RespuestaExamen> RespuestasExamen { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}

class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto()){
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            var p = new Pregunta {
                Enunciado  = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                RespuestaA = "Java",
                RespuestaB = "C#",
                RespuestaC = "Python",
                Correcta   = "B"
            };
            db.Preguntas.Add(p);
            db.SaveChanges();

            Console.WriteLine("Pregunta registrada correctamente.");
            
            Console.Clear();
            foreach(var pregunta in db.Preguntas){
                Console.WriteLine($"""

                    #{pregunta.PreguntaId:000}
                
                    {p.Enunciado}

                     A) {p.RespuestaA}
                     B) {p.RespuestaB}
                     C) {p.RespuestaC}

                """);
            }

            static void TomarExamen() 
            {
            using (var db = new DatosContexto())
            {
                    Console.Write("Ingrese su nombre: ");
                    string nombreAlumno = Console.ReadLine();

                    int cantidadPreguntas = Math.Min(10, db.Preguntas.Count());
                    var preguntas = db.Preguntas.OrderBy(p => Guid.NewGuid()).Take(cantidadPreguntas).ToList();
                    
                    int respuestasCorrectas = 0;

                    foreach (var pregunta in preguntas)
                    {
                        Console.WriteLine($"\n{pregunta.Enunciado}");
                        Console.WriteLine($"A) {pregunta.OpcionA}");
                        Console.WriteLine($"B) {pregunta.OpcionB}");
                        Console.WriteLine($"C) {pregunta.OpcionC}");
                        Console.Write("Tu respuesta (A/B/C): ");
                        char respuesta = Console.ReadKey().KeyChar;
                        Console.WriteLine("\n");

                        bool correcta = respuesta == pregunta.RespuestaCorrecta;
                        if (correcta) respuestasCorrectas++;

                        db.RespuestasExamen.Add(new RespuestaExamen
                        {
                            PreguntaId = pregunta.Id,
                            Correcta = correcta
                        });
                    }

                    double notaFinal = (respuestasCorrectas / (double)cantidadPreguntas) * 10;

                    db.ResultadosExamen.Add(new ResultadoExamen
                    {
                        Alumno = nombreAlumno,
                        RespuestasCorrectas = respuestasCorrectas,
                        TotalPreguntas = cantidadPreguntas,
                        NotaFinal = notaFinal
                    });

                    db.SaveChanges();

                    Console.WriteLine($"\nExamen finalizado. Nota obtenida: {notaFinal}/10.");
                }
            }

            static void MostrarRanking()
            {
                using (var db = new DatosContexto())
                {
                    var ranking = db.ResultadosExamen
                        .OrderByDescending(r => r.NotaFinal)
                        .Take(5)
                        .ToList();

                    Console.WriteLine("\n Ranking de Mejores Alumnos:");
                    foreach (var resultado in ranking)
                    {
                        Console.WriteLine($"{resultado.Alumno}: {resultado.NotaFinal}/10");
                    }
                }
            }
            static void EstadisticasPorPregunta()
            {
                using (var db = new DatosContexto())
                {
                    var estadisticas = db.RespuestasExamen
                        .GroupBy(r => r.PreguntaId)
                        .Select(g => new
                        {
                            PreguntaId = g.Key,
                            TotalRespuestas = g.Count(),
                            Correctas = g.Count(r => r.Correcta),
                            PorcentajeCorrectas = (g.Count(r => r.Correcta) / (double)g.Count()) * 100
                        })
                        .ToList();

                    Console.WriteLine("\n📊 Estadísticas por Pregunta:");
                    foreach (var stat in estadisticas)
                    {
                        Console.WriteLine($"Pregunta ID: {stat.PreguntaId}");
                        Console.WriteLine($"Total de Respuestas: {stat.TotalRespuestas}");
                        Console.WriteLine($"Correctas: {stat.Correctas} ({stat.PorcentajeCorrectas:F2}%)\n");
                    }
                }
            }
        }
    }
}