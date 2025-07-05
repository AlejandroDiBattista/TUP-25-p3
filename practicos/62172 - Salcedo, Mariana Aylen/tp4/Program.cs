using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP4App
{
    public class Pregunta
    {
        public int Id { get; set; }
        public string Enunciado { get; set; }
        public string OpcionA { get; set; }
        public string OpcionB { get; set; }
        public string OpcionC { get; set; }
        public char RespuestaCorrecta { get; set; }

        public ICollection<RespuestaExamen> Respuestas { get; set; }
    }

    public class ResultadoExamen
    {
        public int Id { get; set; }
        public string NombreAlumno { get; set; }
        public int CantidadCorrectas { get; set; }
        public int TotalPreguntas { get; set; }
        public double NotaFinal { get; set; }

        public ICollection<RespuestaExamen> Respuestas { get; set; }
    }

    public class RespuestaExamen
    {
        public int Id { get; set; }

        public int ResultadoExamenId { get; set; }
        public ResultadoExamen ResultadoExamen { get; set; }

        public int PreguntaId { get; set; }
        public Pregunta Pregunta { get; set; }

        public char RespuestaDada { get; set; }
        public bool EsCorrecta { get; set; }
    }

    public class ExamenDbContext : DbContext
    {
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<ResultadoExamen> Resultados { get; set; }
        public DbSet<RespuestaExamen> Respuestas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=examenes.db");
        }
    }

    class Program
    {
        static void Main()
        {
            using var db = new ExamenDbContext();
            db.Database.Migrate();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== SISTEMA DE EXAMEN ===");
                Console.WriteLine("1. Registrar pregunta");
                Console.WriteLine("2. Tomar examen");
                Console.WriteLine("3. Ver historial");
                Console.WriteLine("4. Ver ranking");
                Console.WriteLine("5. Estadísticas por pregunta");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        RegistrarPregunta(db);
                        break;
                    case "2":
                        TomarExamen(db);
                        break;
                    case "3":
                        VerHistorial(db);
                        break;
                    case "4":
                        VerRanking(db);
                        break;
                    case "5":
                        EstadisticasPreguntas(db);
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }
        }

        static void RegistrarPregunta(ExamenDbContext db)
        {
            Console.Write("Enunciado: ");
            var enunciado = Console.ReadLine();

            Console.Write("Opción A: ");
            var a = Console.ReadLine();

            Console.Write("Opción B: ");
            var b = Console.ReadLine();

            Console.Write("Opción C: ");
            var c = Console.ReadLine();

            Console.Write("Respuesta correcta (A/B/C): ");
            var correcta = Console.ReadLine().ToUpper()[0];

            var pregunta = new Pregunta
            {
                Enunciado = enunciado,
                OpcionA = a,
                OpcionB = b,
                OpcionC = c,
                RespuestaCorrecta = correcta
            };

            db.Preguntas.Add(pregunta);
            db.SaveChanges();

            Console.WriteLine("Pregunta registrada.");
        }

        static void TomarExamen(ExamenDbContext db)
        {
            Console.Write("Nombre del alumno: ");
            var nombre = Console.ReadLine();

            var preguntas = db.Preguntas.OrderBy(p => Guid.NewGuid()).Take(5).ToList();
            int correctas = 0;

            var resultado = new ResultadoExamen
            {
                NombreAlumno = nombre,
                TotalPreguntas = preguntas.Count,
                Respuestas = new List<RespuestaExamen>()
            };

            foreach (var p in preguntas)
            {
                Console.WriteLine($"\n{p.Enunciado}");
                Console.WriteLine($"A. {p.OpcionA}");
                Console.WriteLine($"B. {p.OpcionB}");
                Console.WriteLine($"C. {p.OpcionC}");
                Console.Write("Respuesta: ");
                var respuesta = Console.ReadLine().ToUpper()[0];

                bool esCorrecta = respuesta == p.RespuestaCorrecta;
                if (esCorrecta) correctas++;

                resultado.Respuestas.Add(new RespuestaExamen
                {
                    PreguntaId = p.Id,
                    RespuestaDada = respuesta,
                    EsCorrecta = esCorrecta
                });
            }

            resultado.CantidadCorrectas = correctas;
            resultado.NotaFinal = correctas;

            db.Resultados.Add(resultado);
            db.SaveChanges();

            Console.WriteLine($"\nExamen finalizado. Nota: {resultado.NotaFinal}/5");
        }

        static void VerHistorial(ExamenDbContext db)
        {
            var resultados = db.Resultados.Include(r => r.Respuestas).ToList();
            foreach (var r in resultados)
            {
                Console.WriteLine($"{r.NombreAlumno} - Nota: {r.NotaFinal} ({r.CantidadCorrectas}/{r.TotalPreguntas})");
            }
        }

        static void VerRanking(ExamenDbContext db)
        {
            var ranking = db.Resultados
                .GroupBy(r => r.NombreAlumno)
                .Select(g => new { Nombre = g.Key, MejorNota = g.Max(x => x.NotaFinal) })
                .OrderByDescending(x => x.MejorNota)
                .ToList();

            foreach (var item in ranking)
            {
                Console.WriteLine($"{item.Nombre} - Mejor Nota: {item.MejorNota}");
            }
        }

        static void EstadisticasPreguntas(ExamenDbContext db)
        {
            var preguntas = db.Preguntas.Include(p => p.Respuestas).ToList();
            foreach (var p in preguntas)
            {
                int total = p.Respuestas.Count;
                int correctas = p.Respuestas.Count(r => r.EsCorrecta);
                double porcentaje = total > 0 ? (correctas * 100.0) / total : 0;

                Console.WriteLine($"\n{p.Enunciado}");
                Console.WriteLine($"Respondida: {total} veces");
                Console.WriteLine($"Correctas: {porcentaje:0.0}%");
            }
        }
    }
}