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
class Program
{
    static void Main()
    {
        using var context = new AppDbContext();
        context.Database.EnsureCreated();

class DatosContexto : DbContext{
        if (!context.Preguntas.Any())
        {
            Console.WriteLine("Insertando preguntas iniciales...");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }
            context.Preguntas.AddRange(
                new Pregunta { Enunciado = "¿Cuál es la capital de Francia?", OpcionA = "París", OpcionB = "Madrid", OpcionC = "Roma", RespuestaCorrecta = 'A' },
                new Pregunta { Enunciado = "¿Cuál es el resultado de 2 + 2?", OpcionA = "3", OpcionB = "4", OpcionC = "5", RespuestaCorrecta = 'B' },
                new Pregunta { Enunciado = "¿Qué color resulta de mezclar rojo y azul?", OpcionA = "Verde", OpcionB = "Naranja", OpcionC = "Violeta", RespuestaCorrecta = 'C' },
                new Pregunta { Enunciado = "¿Quién escribió Don Quijote?", OpcionA = "Miguel de Cervantes", OpcionB = "Gabriel García Márquez", OpcionC = "Pablo Neruda", RespuestaCorrecta = 'A' },
                new Pregunta { Enunciado = "¿Cuál es el planeta más grande del sistema solar?", OpcionA = "Tierra", OpcionB = "Júpiter", OpcionC = "Saturno", RespuestaCorrecta = 'B' }
            );

}
            context.SaveChanges();
        }

        Console.WriteLine("\n--- Examen ---");
        Console.Write("Ingrese su nombre: ");
        var nombreAlumno = Console.ReadLine();

        var preguntas = context.Preguntas.OrderBy(p => Guid.NewGuid()).Take(5).ToList();

        int correctas = 0;
        var resultado = new ResultadoExamen
        {
            Alumno = nombreAlumno,
            TotalPreguntas = preguntas.Count,
            Respuestas = new List<RespuestaExamen>()
        };

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
        foreach (var pregunta in preguntas)
        {
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"A. {pregunta.OpcionA}");
            Console.WriteLine($"B. {pregunta.OpcionB}");
            Console.WriteLine($"C. {pregunta.OpcionC}");
            Console.Write("Tu respuesta (A/B/C): ");
            char respuesta = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            bool esCorrecta = respuesta == pregunta.RespuestaCorrecta;
            if (esCorrecta) correctas++;

            resultado.Respuestas.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.Id,
                RespuestaDada = respuesta,
                EsCorrecta = esCorrecta
            });
        }

        resultado.CantidadCorrectas = correctas;
        resultado.NotaFinal = correctas;

        context.Resultados.Add(resultado);
        context.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. {nombreAlumno}, obtuviste {correctas}/{preguntas.Count}. Nota: {resultado.NotaFinal}/5");
    }
}}