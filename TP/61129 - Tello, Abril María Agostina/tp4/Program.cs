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

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class ResultadoExamen {
    public int ResultadoExamenId { get; set; }
    public string NombreAlumno { get; set; } = "";
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public double NotaFinal { get; set; }

    public List<RespuestaExamen> Respuestas { get; set; } = new();
}

class RespuestaExamen {
    public int RespuestaExamenId { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }

    public string RespuestaAlumno { get; set; } = "";
    public bool EsCorrecta { get; set; }
}
class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<ResultadoExamen> Resultados { get; set; }
    public DbSet<RespuestaExamen> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}

class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto()){
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            if(!db.Preguntas.Any()){
                var preguntas = new List<Pregunta> {
                    new Pregunta {
                        Enunciado  = "¿Cuál es el lenguaje de programación más utilizado en el desarrollo web?",
                        RespuestaA = "JavaScript",
                        RespuestaB = "C#",
                        RespuestaC = "Python",
                        Correcta   = "A"
                    },
                    new Pregunta {
                        Enunciado  = "¿Qué es un ORM?",
                        RespuestaA = "Un framework de desarrollo web",
                        RespuestaB = "Un patrón de diseño",
                        RespuestaC = "Una herramienta para mapear objetos a bases de datos",
                        Correcta   = "C"
                    },
                    new Pregunta {
                        Enunciado  = "¿Qué es una API REST?",
                        RespuestaA = "Un tipo de base de datos",
                        RespuestaB = "Un protocolo de comunicación",
                        RespuestaC = "Una arquitectura para construir servicios web",
                        Correcta   = "C"
                    },
                    new Pregunta {
                        Enunciado  = "¿Qué es un contenedor en Docker?",
                        RespuestaA = "Un tipo de base de datos",
                        RespuestaB = "Una unidad de almacenamiento",
                        RespuestaC = "Una instancia aislada de una aplicación",
                        Correcta   = "C"
                    },
                    new Pregunta {
                        Enunciado  = "¿Qué es un microservicio?",
                        RespuestaA = "Un tipo de base de datos",
                        RespuestaB = "Una arquitectura de software",
                        RespuestaC = "Un patrón de diseño",
                        Correcta   = "B"
                    },
                };
            }
            
            db.Preguntas.AddRange(preguntas);
            db.SaveChanges();
            
            Console.Clear();
            Console.Write("Ingrese su nombre: ");
            string nombre = Console.ReadLine();

            // Tomar examen (seleccionar 10 preguntas aleatorias si hay suficientes)
            var preguntasAleatorias = db.Preguntas.OrderBy(x => Guid.NewGuid()).Take(10).ToList();
            if (preguntasAleatorias.Count == 0)
            {
                Console.WriteLine("No hay preguntas disponibles.");
                return;
            }

            int correctas = 0;
            var respuestasExamen = new List<RespuestaExamen>();

            foreach (var pregunta in preguntasAleatorias)
            {
                Console.WriteLine($"\n{pregunta.Enunciado}");
                Console.WriteLine($"A) {pregunta.RespuestaA}");
                Console.WriteLine($"B) {pregunta.RespuestaB}");
                Console.WriteLine($"C) {pregunta.RespuestaC}");
                Console.Write("Tu respuesta: ");
                string respuesta = Console.ReadLine()?.ToUpper();

                bool esCorrecta = respuesta == pregunta.Correcta;
                if (esCorrecta) correctas++;

                respuestasExamen.Add(new RespuestaExamen
                {
                    PreguntaId = pregunta.PreguntaId,
                    RespuestaAlumno = respuesta,
                    EsCorrecta = esCorrecta
                });
            }

            // Registrar resultado
            var resultadoExamen = new ResultadoExamen
            {
                NombreAlumno = nombre,
                CantidadCorrectas = correctas,
                TotalPreguntas = preguntasAleatorias.Count,
                NotaFinal = correctas * 10.0 / preguntasAleatorias.Count,
                Respuestas = respuestasExamen
            };

            db.Resultados.Add(resultadoExamen);
            db.SaveChanges();

            Console.WriteLine($"\nExamen finalizado. Nota: {resultadoExamen.NotaFinal:F1}/10");

            // Reportes

            // Mostrar listado de exámenes rendidos
            Console.WriteLine("\nListado de exámenes rendidos:");
            foreach (var resultado in db.Resultados)
            {
                Console.WriteLine($"{resultado.NombreAlumno}: {resultado.NotaFinal:F1}/10");
            }

            // Filtrar resultados por nombre de alumno
            Console.Write("\nIngrese el nombre del alumno para filtrar los resultados: ");
            string nombreFiltro = Console.ReadLine();
            var resultadosFiltrados = db.Resultados.Where(r => r.NombreAlumno.Contains(nombreFiltro)).ToList();

            Console.WriteLine("\nResultados filtrados:");
            foreach (var resultado in resultadosFiltrados)
            {
                Console.WriteLine($"{resultado.NombreAlumno}: {resultado.NotaFinal:F1}/10");
            }

            // Ranking de mejores alumnos
            Console.WriteLine("\nRanking de mejores alumnos:");
            var ranking = db.Resultados.OrderByDescending(r => r.NotaFinal).ToList();
            foreach (var resultado in ranking)
            {
                Console.WriteLine($"{resultado.NombreAlumno}: {resultado.NotaFinal:F1}/10");
            }

            // Informe estadístico por pregunta
            Console.WriteLine("\nInforme estadístico por pregunta:");
            var preguntasEstadisticas = db.Preguntas.ToList();
            foreach (var pregunta in preguntasEstadisticas)
            {
                var totalRespuestas = db.Respuestas.Count(r => r.PreguntaId == pregunta.PreguntaId);
                var respuestasCorrectas = db.Respuestas.Count(r => r.PreguntaId == pregunta.PreguntaId && r.EsCorrecta);
                double porcentajeCorrectas = totalRespuestas > 0 ? (double)respuestasCorrectas / totalRespuestas * 100 : 0;

                Console.WriteLine($"Pregunta #{pregunta.PreguntaId:000}: {pregunta.Enunciado}");
                Console.WriteLine($"  Respuestas dadas: {totalRespuestas}");
                Console.WriteLine($"  Porcentaje de respuestas correctas: {porcentajeCorrectas:F2}%");
            }
        }
    }
            
}

