using System;
using System.Data.Common;
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
    public string Correcta { get; set; } = "";
}

class RespuestaExamen {
    public int Id { get; set; }
    public int ResultadoExamenId { get; set; }
    public int PreguntaId { get; set; }
    public string RespuestaDada { get; set; } = "";
    public bool EsCorrecta { get; set; }
}

class ResultadoExamen {
    public int Id { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
}

class DatosContexto : DbContext{
class DatosContexto : DbContext {
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}

class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto()){
            db.Database.EnsureDeleted();
class Program {
    static void Main() {
        using (var db = new DatosContexto()) {
            db.Database.EnsureCreated();

            var p = new Pregunta {
                Enunciado  = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                RespuestaA = "Java",
                RespuestaB = "C#",
                RespuestaC = "Python",
                Correcta   = "B"
            };
            db.Preguntas.Add(p);

            while (true) {
                Console.Clear();
                Console.WriteLine("===== Examen de Fútbol =====");
                Console.WriteLine("1. Agregar preguntas");
                Console.WriteLine("2. Eliminar pregunta");
                Console.WriteLine("3. Tomar examen");
                Console.WriteLine("4. Ver resultados");
                Console.WriteLine("5. Ver ranking");
                Console.WriteLine("6. Ver estadísticas por pregunta");
                Console.WriteLine("7. Salir");
                Console.Write("\nSeleccione una opción: ");

                var opcion = Console.ReadLine();

                switch (opcion) {
                    case "1":
                        AgregarPreguntas(db);
                        break;
                    case "2":
                        EliminarPregunta(db);
                        break;
                    case "3":
                        TomarExamen(db);
                        break;
                    case "4":
                        VerResultados(db);
                        break;
                    case "5":
                        VerRanking(db);
                        break;
                    case "6":
                        VerEstadisticas(db);
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }
    }

    static void AgregarPreguntas(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("=== Agregar preguntas ===");
        while (true) {
            Console.Write("Enunciado (o enter para salir): ");
            var enunciado = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(enunciado)) break;

            Console.Write("Opción A: ");
            var a = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Opción B: ");
            var b = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Opción C: ");
            var c = Console.ReadLine()?.Trim() ?? "";

            string correcta;
            do {
                Console.Write("¿Cuál es la opción correcta? (A/B/C): ");
                correcta = Console.ReadLine()?.ToUpper().Trim();
            } while (correcta != "A" && correcta != "B" && correcta != "C");

            db.Preguntas.Add(new Pregunta {
                Enunciado = enunciado,
                RespuestaA = a,
                RespuestaB = b,
                RespuestaC = c,
                Correcta = correcta
            });
            db.SaveChanges();


            Console.WriteLine("Pregunta agregada.\n");
        }
    }

    static void EliminarPregunta(DatosContexto db) {
        Console.Clear();
        var preguntas = db.Preguntas.OrderBy(p => p.Id).ToList();
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas para eliminar.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("=== Preguntas registradas ===");
        foreach (var p in preguntas) {
            Console.WriteLine($"{p.Id}. {p.Enunciado}");
        }

        Console.Write("Ingrese el ID de la pregunta a eliminar: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            var pregunta = db.Preguntas.Find(id);
            if (pregunta != null) {
                db.Preguntas.Remove(pregunta);
                db.SaveChanges();

                Console.WriteLine("Pregunta eliminada.");
            } else {
                Console.WriteLine("ID no encontrado.");
            }
        } else {
            Console.WriteLine("ID inválido.");
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void TomarExamen(DatosContexto db) {
        Console.Clear();
        string nombreAlumno;

        do {
            Console.Write("Ingrese su nombre (solo letras): ");
            nombreAlumno = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(nombreAlumno)) {
                Console.WriteLine("El nombre no puede estar vacío. Intente nuevamente.");
            } else if (!EsNombreValido(nombreAlumno)) {
                Console.WriteLine("El nombre solo puede contener letras y espacios. Intente nuevamente.");
                nombreAlumno = null;
            }
        } while (string.IsNullOrEmpty(nombreAlumno));

        var preguntas = db.Preguntas.ToList();

        if (preguntas.Count == 0) {
            Console.WriteLine("No hay preguntas registradas. Presione una tecla para continuar...");
            Console.ReadKey();
            return;
        }

        var seleccionadas = preguntas.OrderBy(p => Guid.NewGuid()).Take(Math.Min(10, preguntas.Count)).ToList();

        int correctas = 0;
        var respuestasExamen = new List<RespuestaExamen>();

        foreach (var p in seleccionadas) {
            Console.Clear();
            foreach(var pregunta in db.Preguntas){
                Console.WriteLine($"""
            Console.WriteLine(p.Enunciado);
            Console.WriteLine($"A) {p.RespuestaA}");
            Console.WriteLine($"B) {p.RespuestaB}");
            Console.WriteLine($"C) {p.RespuestaC}");

                    #{pregunta.PreguntaId:000}
                
                    {p.Enunciado}
            string respuesta;
            do {
                Console.Write("\nSeleccione una opción (A, B, C): ");
                respuesta = Console.ReadLine()?.ToUpper().Trim();
            } while (respuesta != "A" && respuesta != "B" && respuesta != "C");

            bool esCorrecta = respuesta == p.Correcta;
            if (esCorrecta) correctas++;

            respuestasExamen.Add(new RespuestaExamen {
                PreguntaId = p.Id,
                RespuestaDada = respuesta,
                EsCorrecta = esCorrecta
            });
        }

        double nota = (correctas / 10.0) * 10;

        var resultado = new ResultadoExamen {
            NombreAlumno = nombreAlumno,
            CantidadCorrectas = correctas,
            TotalPreguntas = seleccionadas.Count,
            NotaFinal = nota
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        foreach (var r in respuestasExamen) {
            r.ResultadoExamenId = resultado.Id;
            db.Respuestas.Add(r);
        }
        db.SaveChanges();

        Console.Clear();
        Console.WriteLine($"Examen finalizado. Nota: {nota} / 10");
        Console.WriteLine($"Respuestas correctas: {correctas} de {seleccionadas.Count}");

        Console.Write("\n¿Ver respuestas correctas? (S/N): ");
        if (Console.ReadLine()?.ToUpper().Trim() == "S") {
            foreach (var r in respuestasExamen) {
                var p = seleccionadas.First(x => x.Id == r.PreguntaId);
                Console.WriteLine($"\n{p.Enunciado}");
                Console.WriteLine($"Correcta: {p.Correcta}, Tu respuesta: {r.RespuestaDada} - {(r.EsCorrecta ? "Correcta" : "Incorrecta")}");
            }
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static bool EsNombreValido(string nombre) {
        foreach (char c in nombre) {
            if (!char.IsLetter(c) && !char.IsWhiteSpace(c)) {
                return false;
            }
        }
        return true;
    }

    static void VerResultados(DatosContexto db) {
        Console.Clear();
        Console.Write("Ingrese el nombre del alumno (o deje vacío para todos): ");
        string nombre = Console.ReadLine()?.Trim();

                     A) {p.RespuestaA}
                     B) {p.RespuestaB}
                     C) {p.RespuestaC}
        var resultados = db.Resultados
            .Where(r => string.IsNullOrEmpty(nombre) || r.NombreAlumno.Contains(nombre))
            .Include(r => r.Respuestas)
            .ToList();

                """);
        if (!resultados.Any()) {
            Console.WriteLine("No se encontraron resultados.");
        } else {
            foreach (var r in resultados) {
                Console.WriteLine($"\nAlumno: {r.NombreAlumno} - Fecha: {r.Fecha}");
                Console.WriteLine($"Nota: {r.NotaFinal} / 10 - Correctas: {r.CantidadCorrectas} de {r.TotalPreguntas}");
            }
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }
}

    static void VerRanking(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("===== Ranking de Mejores Alumnos =====");

        var ranking = db.Resultados
            .OrderByDescending(r => r.NotaFinal)
            .ThenByDescending(r => r.Fecha)
            .Take(10)
            .ToList();

        if (!ranking.Any()) {
            Console.WriteLine("No hay resultados.");
        } else {
            foreach (var r in ranking) {
                Console.WriteLine($"Alumno: {r.NombreAlumno} - Nota: {r.NotaFinal} - Fecha: {r.Fecha:dd/MM/yyyy HH:mm}");
            }
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void VerEstadisticas(DatosContexto db) {
        Console.Clear();
        Console.WriteLine("===== Estadísticas por Pregunta =====");

        var preguntas = db.Preguntas.ToList();
        if (!preguntas.Any()) {
            Console.WriteLine("No hay preguntas registradas.");
        } else {
            foreach (var p in preguntas) {
                var respuestas = db.Respuestas.Where(r => r.PreguntaId == p.Id).ToList();
                int total = respuestas.Count;
                int correctas = respuestas.Count(r => r.EsCorrecta);
                double porcentaje = total == 0 ? 0 : (correctas / (double)total) * 100;

                Console.WriteLine($"\nPregunta: {p.Enunciado}");
                Console.WriteLine($"Total respuestas: {total}, Correctas: {correctas}, Porcentaje: {porcentaje:F2}%");
            }
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }
}