using System;
using System.Collections.Generic;
using System.Linq;
using tp4.Datos;
using tp4.Modelos;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main()
    {
        using var db = new AppDbContext();
        db.Database.Migrate();

        InicializarPreguntasSiNoExisten(db);

        Console.WriteLine("Ingrese su nombre:");
        string nombreAlumno = Console.ReadLine() ?? "";

       var preguntas = db.Preguntas.AsEnumerable().OrderBy(p => Guid.NewGuid()).Take(5).ToList();
        int correctas = 0;
        var respuestas = new List<RespuestaExamen>();

        foreach (var pregunta in preguntas)
        {
            Console.WriteLine($"\n{pregunta.Enunciado}");
            Console.WriteLine($"A. {pregunta.OpcionA}");
            Console.WriteLine($"B. {pregunta.OpcionB}");
            Console.WriteLine($"C. {pregunta.OpcionC}");

            char respuestaAlumno;
            while (true)
            {
                Console.Write("Tu respuesta (A/B/C): ");
                var respuestaAlumnoStr = Console.ReadLine()?.Trim().ToUpper();

                if (respuestaAlumnoStr != null && respuestaAlumnoStr.Length == 1 &&
                    (respuestaAlumnoStr[0] == 'A' || respuestaAlumnoStr[0] == 'B' || respuestaAlumnoStr[0] == 'C'))
                {
                    respuestaAlumno = respuestaAlumnoStr[0];
                    break;
                }
                else
                {
                    Console.WriteLine("Respuesta inválida. Por favor ingrese A, B o C.");
                }
            }

            bool esCorrecta = respuestaAlumno == pregunta.RespuestaCorrecta;
            if (esCorrecta) correctas++;

            respuestas.Add(new RespuestaExamen
            {
                PreguntaId = pregunta.Id,
                RespuestaAlumno = respuestaAlumno,
                Correcta = esCorrecta
            });
        }

        double notaFinal = 5.0 * correctas / preguntas.Count;
        var resultado = new ResultadoExamen
        {
            NombreAlumno = nombreAlumno,
            Correctas = correctas,
            Total = preguntas.Count,
            NotaFinal = notaFinal,
            Respuestas = respuestas
        };

        db.ResultadosExamen.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nExamen finalizado. Nota: {notaFinal:F2}/5. Respuestas correctas: {correctas}/{preguntas.Count}");
    }

    static void InicializarPreguntasSiNoExisten(AppDbContext db)
    {
        if (db.Preguntas.Any()) return;

       var preguntas = new List<Pregunta>
{
    new Pregunta { Enunciado = "¿Cuál es la capital de Francia?", OpcionA = "Madrid", OpcionB = "París", OpcionC = "Roma", RespuestaCorrecta = 'B' },
    new Pregunta { Enunciado = "¿Quién escribió 'Cien años de soledad'?", OpcionA = "Gabriel García Márquez", OpcionB = "Pablo Neruda", OpcionC = "Mario Vargas Llosa", RespuestaCorrecta = 'A' },
    new Pregunta { Enunciado = "¿Qué planeta es el más cercano al sol?", OpcionA = "Venus", OpcionB = "Tierra", OpcionC = "Mercurio", RespuestaCorrecta = 'C' },
    new Pregunta { Enunciado = "¿En qué año comenzó la Segunda Guerra Mundial?", OpcionA = "1939", OpcionB = "1945", OpcionC = "1914", RespuestaCorrecta = 'A' },
    new Pregunta { Enunciado = "¿Cuál es el río más largo del mundo?", OpcionA = "Amazonas", OpcionB = "Nilo", OpcionC = "Yangtsé", RespuestaCorrecta = 'B' },
    new Pregunta { Enunciado = "¿Cuál es el símbolo químico del oro?", OpcionA = "Au", OpcionB = "Ag", OpcionC = "O", RespuestaCorrecta = 'A' },
    new Pregunta { Enunciado = "¿Cuántos planetas hay en el sistema solar?", OpcionA = "8", OpcionB = "9", OpcionC = "7", RespuestaCorrecta = 'A' },
    new Pregunta { Enunciado = "¿Quién pintó la Mona Lisa?", OpcionA = "Vincent Van Gogh", OpcionB = "Leonardo da Vinci", OpcionC = "Pablo Picasso", RespuestaCorrecta = 'B' },
    new Pregunta { Enunciado = "¿Cuál es la lengua más hablada del mundo?", OpcionA = "Inglés", OpcionB = "Español", OpcionC = "Chino mandarín", RespuestaCorrecta = 'C' },
    new Pregunta { Enunciado = "¿Cuál es la capital de Australia?", OpcionA = "Sydney", OpcionB = "Melbourne", OpcionC = "Canberra", RespuestaCorrecta = 'C' }
};


        db.Preguntas.AddRange(preguntas);
        db.SaveChanges();
    }
}
