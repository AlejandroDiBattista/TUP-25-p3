using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AplicacionExamen
{
    public class Pregunta
    {
        public int Id { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public string OpcionUno { get; set; } = string.Empty;
        public string OpcionDos { get; set; } = string.Empty;
        public string OpcionTres { get; set; } = string.Empty;
        public string OpcionCorrecta { get; set; } = string.Empty;
        public ICollection<RespuestaAlumno> Respuestas { get; set; } = new List<RespuestaAlumno>();
    }

    public class Resultado
    {
        public int Id { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public int CantidadCorrectas { get; set; }
        public int CantidadPreguntas { get; set; }
        public double Nota { get; set; }
        public ICollection<RespuestaAlumno> Respuestas { get; set; } = new List<RespuestaAlumno>();
    }

    public class RespuestaAlumno
    {
        public int Id { get; set; }
        public int PreguntaId { get; set; }
        public Pregunta Pregunta { get; set; } = null!;
        public int ResultadoId { get; set; }
        public Resultado Resultado { get; set; } = null!;
        public string RespuestaSeleccionada { get; set; } = string.Empty;
        public bool EsCorrecta { get; set; }
    }

    public class ContextoExamen : DbContext
    {
        public DbSet<Pregunta> Preguntas { get; set; } = null!;
        public DbSet<Resultado> Resultados { get; set; } = null!;
        public DbSet<RespuestaAlumno> Respuestas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder opciones)
        {
            opciones.UseSqlite("Data Source=examen.db");
        }
    }

    class Programa
    {
        private static readonly Dictionary<string, Action<ContextoExamen>> AccionesMenu = new()
        {
            ["1"] = RegistrarPregunta,
            ["2"] = CargarPreguntasEjemplo,
            ["3"] = RealizarExamen,
            ["4"] = MostrarResultados,
            ["5"] = MostrarRanking,
            ["6"] = MostrarEstadisticas
        };

        static void Main()
        {
            using var contexto = new ContextoExamen();
            contexto.Database.EnsureCreated();

            string opcion;
            do
            {
                MostrarMenu();
                opcion = Console.ReadLine() ?? string.Empty;
                Console.Clear();

                if (opcion == "0") break;
                if (AccionesMenu.TryGetValue(opcion, out var accion))
                {
                    accion(contexto);
                }
            } while (true);
        }

        static void MostrarMenu()
        {
            Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
            Console.WriteLine("1. Agregar pregunta");
            Console.WriteLine("2. Cargar preguntas de ejemplo");
            Console.WriteLine("3. Realizar examen");
            Console.WriteLine("4. Mostrar resultados");
            Console.WriteLine("5. Ranking de estudiantes");
            Console.WriteLine("6. Estadísticas de preguntas");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
        }

        static void RegistrarPregunta(ContextoExamen ctx)
        {
            Console.Write("Enunciado de la pregunta: ");
            var enunciado = Console.ReadLine() ?? string.Empty;
            Console.Write("Opción 1: "); var op1 = Console.ReadLine() ?? string.Empty;
            Console.Write("Opción 2: "); var op2 = Console.ReadLine() ?? string.Empty;
            Console.Write("Opción 3: "); var op3 = Console.ReadLine() ?? string.Empty;
            Console.Write("Opción correcta (1/2/3): "); var correcta = Console.ReadLine() ?? string.Empty;

            ctx.Preguntas.Add(new Pregunta
            {
                Enunciado = enunciado,
                OpcionUno = op1,
                OpcionDos = op2,
                OpcionTres = op3,
                OpcionCorrecta = correcta
            });
            ctx.SaveChanges();
            Console.WriteLine("Pregunta guardada exitosamente.");
        }

        static void CargarPreguntasEjemplo(ContextoExamen ctx)
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
            var ejemplo = new Pregunta
            {
                Enunciado = "¿Quién desarrolló .NET?",
                OpcionUno = "Oracle",
                OpcionDos = "Microsoft",
                OpcionTres = "Google",
                OpcionCorrecta = "2"
            };
            ctx.Preguntas.Add(ejemplo);
            ctx.SaveChanges();

            foreach (var p in ctx.Preguntas)
            {
                Console.WriteLine($"ID {p.Id:000} - {p.Enunciado}\n1) {p.OpcionUno} 2) {p.OpcionDos} 3) {p.OpcionTres}\n");
            }
        }

        static void RealizarExamen(ContextoExamen ctx)
        {
            if (!ctx.Preguntas.Any())
            {
                Console.WriteLine("No hay preguntas disponibles.");
                return;
            }
            Console.Write("Nombre del estudiante: ");
            var nombre = Console.ReadLine() ?? string.Empty;

            var seleccionadas = ctx.Preguntas.OrderBy(_ => Guid.NewGuid()).Take(5).ToList();
            var respuestas = new List<RespuestaAlumno>();
            int correctas = 0;

            foreach (var p in seleccionadas)
            {
                Console.WriteLine($"{p.Enunciado}\n1) {p.OpcionUno} 2) {p.OpcionDos} 3) {p.OpcionTres}");
                Console.Write("Respuesta: ");
                var sel = Console.ReadLine() ?? string.Empty;
                bool acierto = sel == p.OpcionCorrecta;
                if (acierto) correctas++;
                respuestas.Add(new RespuestaAlumno { Pregunta = p, RespuestaSeleccionada = sel, EsCorrecta = acierto });
            }

            var resultado = new Resultado
            {
                NombreEstudiante = nombre,
                CantidadCorrectas = correctas,
                CantidadPreguntas = seleccionadas.Count,
                Nota = Math.Round((double)correctas / seleccionadas.Count * 10, 1),
                Respuestas = respuestas
            };
            ctx.Resultados.Add(resultado);
            ctx.SaveChanges();
            Console.WriteLine($"\nResultado: {resultado.Nota}/10");
        }

        static void MostrarResultados(ContextoExamen ctx)
        {
            var todos = ctx.Resultados.ToList();
            if (!todos.Any()) { Console.WriteLine("Sin resultados registrados."); return; }
            todos.ForEach(r => Console.WriteLine($"Alumno: {r.NombreEstudiante}, Nota: {r.Nota}/10, Correctas: {r.CantidadCorrectas}/{r.CantidadPreguntas}"));
        }

        static void MostrarRanking(ContextoExamen ctx)
        {
            var ranking = ctx.Resultados
                .GroupBy(r => r.NombreEstudiante)
                .Select(g => new { Nombre = g.Key, NotaMaxima = g.Max(r => r.Nota) })
                .OrderByDescending(x => x.NotaMaxima)
                .ToList();

            Console.WriteLine("\n--- RANKING ---");
            ranking.ForEach(r => Console.WriteLine($"{r.Nombre}: {r.NotaMaxima}/10"));
        }

        static void MostrarEstadisticas(ContextoExamen ctx)
        {
            var preguntas = ctx.Preguntas.Include(p => p.Respuestas).ToList();
            preguntas.ForEach(p =>
            {
                var total = p.Respuestas.Count;
                var correctas = p.Respuestas.Count(r => r.EsCorrecta);
                var porcentaje = total > 0 ? Math.Round((double)correctas / total * 100, 1) : 0;
                Console.WriteLine($"Pregunta: {p.Enunciado}\nContestada: {total} veces, Aciertos: {porcentaje}%\n");
            });
        }
    }
}
