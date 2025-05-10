using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TP4.Data;
using TP4.Models;

namespace TP4
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new ExamenContext();
            db.Database.EnsureCreated();

            while (true)
            {
                Console.WriteLine("\n--- Menú Principal ---");
                Console.WriteLine("1. Registrar pregunta");
                Console.WriteLine("2. Tomar examen");
                Console.WriteLine("3. Ver todos los resultados");
                Console.WriteLine("4. Filtrar por nombre de alumno");
                Console.WriteLine("5. Ranking de mejores alumnos");
                Console.WriteLine("6. Informe estadístico por pregunta");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");
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
                        VerResultados(db);
                        break;
                    case "4":
                        FiltrarPorNombre(db);
                        break;
                    case "5":
                        MostrarRanking(db);
                        break;
                    case "6":
                        InformeEstadistico(db);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
        }

        static void RegistrarPregunta(ExamenContext db)
        {
            Console.Write("Enunciado: ");
            string enunciado = Console.ReadLine();
            Console.Write("Opción A: ");
            string a = Console.ReadLine();
            Console.Write("Opción B: ");
            string b = Console.ReadLine();
            Console.Write("Opción C: ");
            string c = Console.ReadLine();
            Console.Write("Respuesta correcta (A/B/C): ");
            string correcta = Console.ReadLine().ToUpper();

            var opcionesValidas = new[] { "A", "B", "C" };
            if (!opcionesValidas.Contains(correcta))
            {
                Console.WriteLine("Respuesta correcta inválida. Debe ser A, B o C.");
                return;
            }

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
            Console.WriteLine("Pregunta registrada con éxito.");
        }

        static void TomarExamen(ExamenContext db)
        {
            Console.Write("Nombre del alumno: ");
            string nombre = Console.ReadLine();

            var preguntas = db.Preguntas
                .AsEnumerable()
                .OrderBy(x => Guid.NewGuid())
                .Take(5)
                .ToList();

            if (preguntas.Count == 0)
            {
                Console.WriteLine("No hay preguntas registradas.");
                return;
            }

            int correctas = 0;
            var resultado = new ResultadoExamen
            {
                NombreAlumno = nombre,
                TotalPreguntas = preguntas.Count,
                Respuestas = new List<RespuestaExamen>()
            };

            foreach (var pregunta in preguntas)
            {
                Console.WriteLine($"\n{pregunta.Enunciado}");
                Console.WriteLine($"A: {pregunta.OpcionA}");
                Console.WriteLine($"B: {pregunta.OpcionB}");
                Console.WriteLine($"C: {pregunta.OpcionC}");

                string respuesta;
                do
                {
                    Console.Write("Tu respuesta (A/B/C): ");
                    respuesta = Console.ReadLine().Trim().ToUpper();
                    if (!new[] { "A", "B", "C" }.Contains(respuesta))
                        Console.WriteLine("Entrada inválida. Por favor, ingrese solo A, B o C.");
                } while (!new[] { "A", "B", "C" }.Contains(respuesta));

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
            resultado.NotaFinal = (correctas / (double)preguntas.Count) * 10;

            db.Resultados.Add(resultado);
            db.SaveChanges();

            Console.WriteLine($"\nExamen finalizado. Nota: {resultado.NotaFinal:F1}/10");
        }

        static void VerResultados(ExamenContext db)
        {
            var resultados = db.Resultados.OrderByDescending(r => r.NotaFinal).ToList();
            foreach (var r in resultados)
            {
                Console.WriteLine($"{r.NombreAlumno} - {r.NotaFinal:F1} puntos ({r.CantidadCorrectas}/{r.TotalPreguntas} correctas)");
            }
        }

        static void FiltrarPorNombre(ExamenContext db)
        {
            Console.Write("Ingrese el nombre del alumno: ");
            string nombre = Console.ReadLine();
            var resultados = db.Resultados
                .Where(r => r.NombreAlumno.ToLower().Contains(nombre.ToLower()))
                .OrderByDescending(r => r.NotaFinal)
                .ToList();

            if (resultados.Count == 0)
            {
                Console.WriteLine("No se encontraron resultados.");
                return;
            }

            foreach (var r in resultados)
            {
                Console.WriteLine($"{r.NombreAlumno} - {r.NotaFinal:F1} puntos ({r.CantidadCorrectas}/{r.TotalPreguntas})");
            }
        }

        static void MostrarRanking(ExamenContext db)
        {
            var ranking = db.Resultados
                .GroupBy(r => r.NombreAlumno)
                .Select(g => new
                {
                    Alumno = g.Key,
                    MejorNota = g.Max(r => r.NotaFinal)
                })
                .OrderByDescending(x => x.MejorNota)
                .ToList();

            Console.WriteLine("\n--- Ranking de Mejores Alumnos ---");
            foreach (var alumno in ranking)
            {
                Console.WriteLine($"{alumno.Alumno} - Mejor Nota: {alumno.MejorNota:F1}");
            }
        }

        static void InformeEstadistico(ExamenContext db)
        {
            var preguntas = db.Preguntas.Include(p => p.Respuestas).ToList();

            Console.WriteLine("\n--- Informe por Pregunta ---");
            foreach (var p in preguntas)
            {
                int total = p.Respuestas?.Count ?? 0;
                int correctas = p.Respuestas?.Count(r => r.EsCorrecta) ?? 0;
                double porcentaje = total > 0 ? (correctas / (double)total) * 100 : 0;

                Console.WriteLine($"ID {p.Id} - \"{p.Enunciado}\"");
                Console.WriteLine($"  Respondida: {total} veces");
                Console.WriteLine($"  Correctas: {correctas} ({porcentaje:F1}%)\n");
            }
        }
    }
}