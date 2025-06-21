using System;
using System.Collections.Generic;
using System.Linq;
using tp4.Models;
using tp4;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        using (var db = new ExamenDbContext())
        {
            db.Database.Migrate(); // Aplica migraciones automáticas
        }

        while (true)
        {
            Console.WriteLine("\n--- Sistema de Exámenes Multiple Choice ---");
            Console.WriteLine("1. Registrar pregunta");
            Console.WriteLine("2. Tomar examen");
            Console.WriteLine("3. Ver reportes");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();
            switch (opcion)
            {
                case "1": RegistrarPregunta(); break;
                case "2": TomarExamen(); break;
                case "3": VerReportes(); break;
                case "0": return;
                default: Console.WriteLine("Opción inválida"); break;
            }
        }
    }

    static void RegistrarPregunta()
    {
        using (var db = new ExamenDbContext())
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
            var correcta = Console.ReadLine().ToUpper();
            if (correcta != "A" && correcta != "B" && correcta != "C")
            {
                Console.WriteLine("Respuesta inválida. Debe ser A, B o C.");
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
            Console.WriteLine("Pregunta registrada correctamente.");
        }
    }

    static void TomarExamen()
    {
        using (var db = new ExamenDbContext())
        {
            Console.Write("Nombre del alumno: ");
            var nombre = Console.ReadLine();
            var preguntas = db.Preguntas.ToList()
                .OrderBy(x => Guid.NewGuid())
                .Take(5)
                .ToList();
            if (preguntas.Count == 0)
            {
                Console.WriteLine("No hay preguntas registradas.");
                return;
            }
            int correctas = 0;
            var respuestas = new List<RespuestaExamen>();
            for (int i = 0; i < preguntas.Count; i++)
            {
                var p = preguntas[i];
                Console.WriteLine($"\nPregunta {i + 1}: {p.Enunciado}");
                Console.WriteLine($"A) {p.OpcionA}");
                Console.WriteLine($"B) {p.OpcionB}");
                Console.WriteLine($"C) {p.OpcionC}");
                Console.Write("Respuesta (A/B/C): ");
                var resp = Console.ReadLine().ToUpper();
                bool esCorrecta = resp == p.RespuestaCorrecta;
                if (esCorrecta) correctas++;
                respuestas.Add(new RespuestaExamen
                {
                    PreguntaId = p.Id,
                    OpcionElegida = resp,
                    EsCorrecta = esCorrecta
                });
            }
            double nota = (double)correctas / preguntas.Count * 5;
            var resultado = new ResultadoExamen
            {
                NombreAlumno = nombre,
                CantidadCorrectas = correctas,
                TotalPreguntas = preguntas.Count,
                NotaFinal = Math.Round(nota, 2),
                Respuestas = respuestas
            };
            db.ResultadosExamen.Add(resultado);
            db.SaveChanges();
            Console.WriteLine($"\nExamen finalizado. Correctas: {correctas}/{preguntas.Count}. Nota: {resultado.NotaFinal}");
        }
    }

    static void VerReportes()
    {
        using (var db = new ExamenDbContext())
        {
            Console.WriteLine("\n1. Listado de exámenes");
            Console.WriteLine("2. Filtrar por alumno");
            Console.WriteLine("3. Ranking de mejores alumnos");
            Console.WriteLine("4. Estadísticas por pregunta");
            Console.WriteLine("0. Volver");
            Console.Write("Seleccione una opción: ");
            var op = Console.ReadLine();
            switch (op)
            {
                case "1":
                    var examenes = db.ResultadosExamen.ToList();
                    foreach (var ex in examenes)
                        Console.WriteLine($"Alumno: {ex.NombreAlumno}, Nota: {ex.NotaFinal}, Correctas: {ex.CantidadCorrectas}/{ex.TotalPreguntas}");
                    break;
                case "2":
                    Console.Write("Nombre del alumno: ");
                    var nombre = Console.ReadLine();
                    var examenesAlumno = db.ResultadosExamen.Where(x => x.NombreAlumno == nombre).ToList();
                    foreach (var ex in examenesAlumno)
                        Console.WriteLine($"Nota: {ex.NotaFinal}, Correctas: {ex.CantidadCorrectas}/{ex.TotalPreguntas}");
                    break;
                case "3":
                    var ranking = db.ResultadosExamen
                        .GroupBy(x => x.NombreAlumno)
                        .Select(g => new { Alumno = g.Key, MejorNota = g.Max(x => x.NotaFinal) })
                        .OrderByDescending(x => x.MejorNota)
                        .Take(10)
                        .ToList();
                    foreach (var r in ranking)
                        Console.WriteLine($"Alumno: {r.Alumno}, Mejor Nota: {r.MejorNota}");
                    break;
                case "4":
                    var preguntas = db.Preguntas.Include(p => p.RespuestasExamen).ToList();
                    foreach (var p in preguntas)
                    {
                        int total = p.RespuestasExamen?.Count ?? 0;
                        int correctas = p.RespuestasExamen?.Count(r => r.EsCorrecta) ?? 0;
                        double porcentaje = total > 0 ? (double)correctas / total * 100 : 0;
                        Console.WriteLine($"Pregunta: {p.Enunciado}\n  Respondida: {total} veces\n  % Correctas: {porcentaje:F2}%");
                    }
                    break;
                case "0": return;
                default: Console.WriteLine("Opción inválida"); break;
            }
        }
    }
}