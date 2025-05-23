using System;
using System.Data.Common;
using System.Linq;

namespace TP4_61450;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        bool seguirEjecutando = true;

        Inicializar();

        while (seguirEjecutando)
        {
            int cantPreguntas = ObtenerCantidadPreguntas();
            Console.Clear();
            Console.WriteLine("====== Examenes ======");
            Console.WriteLine($"\nFecha: {DateTime.Now} - Preguntas guardadas: {cantPreguntas} \n");
            Console.WriteLine("1. Agregar una nueva pregunta");
            Console.WriteLine("2. Tomar un examen");
            Console.WriteLine("3. Mostrar resultados de los últimos examenes");
            Console.WriteLine("4. Mostrar examenes de un alumno");
            Console.WriteLine("5. Mostrar ranking de los alumnos por nota");
            Console.WriteLine("6. Mostrar las preguntas guardadas");
            Console.WriteLine("7. Mostrar estadísticas de las preguntas");
            Console.WriteLine("0. Salir");
            Console.WriteLine("\nSeleccione una opción:");
            Console.WriteLine("=====================================");

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    AgregarPregunta();
                    break;
                case ConsoleKey.D2:
                    TomarExamen();
                    break;
                case ConsoleKey.D3:
                    MostrarUltimosExamenes();
                    break;
                case ConsoleKey.D4:
                    MostrarExamenesDeAlumno();
                    break;
                case ConsoleKey.D5:
                    RankingMejoresAlumnos();
                    break;
                case ConsoleKey.D6:
                    MostrarPreguntas();
                    break;
                case ConsoleKey.D7:
                    MostrarEstadisticas();
                    break;
                case ConsoleKey.D0:
                    seguirEjecutando = false;
                    break;
            }
        }

        void Inicializar()
        {
            using (DatosContexto db = new DatosContexto())
            {
                if (db.Database.CanConnect()) // Si la base de datos existe, no inicializar
                {
                    Console.WriteLine("La base de datos ya existe. No hace falta inicialización.");
                    return;
                }

                Console.WriteLine("Inicializando la base de datos...");
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                Pregunta p = new Pregunta
                {
                    Enunciado =
                        "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                    RespuestaA = "Java",
                    RespuestaB = "C#",
                    RespuestaC = "Python",
                    RespuestaCorrecta = "B",
                };
                db.Preguntas.Add(p);
                db.SaveChanges();
            }
        }

        int ObtenerCantidadPreguntas()
        {
            using (DatosContexto db = new DatosContexto())
            {
                return db.Preguntas.Count();
            }
        }

        void AgregarPregunta()
        {
            Console.Clear();
            Console.WriteLine("--- Agregar Pregunta ---");
            string enunciado = Utilidades.ObtenerEntrada("\nIngrese el enunciado completo: ");
            string respuestaA = Utilidades.ObtenerEntrada("\nIngrese la respuesta A: ");
            string respuestaB = Utilidades.ObtenerEntrada("\nIngrese la respuesta B: ");
            string respuestaC = Utilidades.ObtenerEntrada("\nIngrese la respuesta C: ");
            Console.Write("\nIngrese la letra de la respuesta correcta: ");
            string respuestaCorrecta;
            do
            {
                respuestaCorrecta = Console.ReadLine()?.ToUpper();
                if (
                    respuestaCorrecta != "A"
                    && respuestaCorrecta != "B"
                    && respuestaCorrecta != "C"
                )
                {
                    Console.WriteLine("Respuesta incorrecta. Por favor, ingrese A, B o C:");
                }
            } while (
                respuestaCorrecta != "A" && respuestaCorrecta != "B" && respuestaCorrecta != "C"
            );

            using (DatosContexto db = new DatosContexto())
            {
                using var transaccion = db.Database.BeginTransaction();
                try
                {
                    Pregunta nuevaPregunta = new Pregunta
                    {
                        Enunciado = enunciado,
                        RespuestaA = respuestaA,
                        RespuestaB = respuestaB,
                        RespuestaC = respuestaC,
                        RespuestaCorrecta = respuestaCorrecta,
                    };
                    db.Preguntas.Add(nuevaPregunta);
                    db.SaveChanges();
                    transaccion.Commit();
                    Console.WriteLine("Pregunta agregada correctamente.");
                }
                catch (Exception)
                {
                    transaccion.Rollback();
                }
            }

            Utilidades.Esperar();
        }

        void TomarExamen()
        {
            Console.Clear();
            Console.WriteLine("--- Tomar Examen ---");
            Examen examen = Examen.GenerarExamen();
            if (examen == null)
            {
                Console.WriteLine("No se pudo generar el examen.");
                Utilidades.Esperar();
                return;
            }

            examen.TomarExamen();
        }

        void MostrarUltimosExamenes()
        {
            Console.Clear();
            Console.WriteLine("--- Resultados de los últimos examenes ---");
            int cantidad = int.Parse(
                Utilidades.ObtenerEntrada("Ingrese la cantidad de examenes a mostrar: ")
            );
            if (cantidad <= 0)
            {
                Console.WriteLine("La cantidad de examenes debe ser mayor a 0.");
                Utilidades.Esperar();
                return;
            }
            Examen.MostrarUltimosExamenes(cantidad);
            Utilidades.Esperar();
        }

        void MostrarExamenesDeAlumno()
        {
            Console.Clear();
            Console.WriteLine("--- Mostrar examenes de un alumno ---");
            int legajo = int.Parse(Utilidades.ObtenerEntrada("Ingrese el legajo del alumno: "));
            if (legajo <= 0)
            {
                Console.WriteLine("El legajo debe ser mayor a 0.");
                Utilidades.Esperar();
                return;
            }
            int cantidad = int.Parse(
                Utilidades.ObtenerEntrada("Ingrese la cantidad de examenes a mostrar: ")
            );
            if (cantidad <= 0)
            {
                Console.WriteLine("La cantidad de examenes debe ser mayor a 0.");
                Utilidades.Esperar();
                return;
            }
            Examen.MostrarExamenesDeAlumno(legajo, cantidad);
            Utilidades.Esperar();
        }

        void RankingMejoresAlumnos()
        {
            Console.Clear();
            Examen.MostrarRankingAlumnosPorNota();
            Utilidades.Esperar();
        }

        void MostrarPreguntas()
        {
            Console.Clear();
            using (DatosContexto db = new DatosContexto())
            {
                foreach (var pregunta in db.Preguntas)
                {
                    Console.WriteLine(pregunta);
                }
            }
            Utilidades.Esperar();
        }

        void MostrarEstadisticas()
        {
            Console.Clear();
            using (DatosContexto db = new DatosContexto())
            {
                foreach (Pregunta pregunta in db.Preguntas)
                {
                    pregunta.MostrarEstadisticas();
                }
            }
            Utilidades.Esperar();
        }
    }
}
