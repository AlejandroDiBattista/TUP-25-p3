using Microsoft.EntityFrameworkCore;

namespace TP4_61450;

public interface IExamen
{
    public int ExamenId { get; set; }
    public int Nota { get; set; }
    public bool Aprobado { get; set; }
    public int AlumnoId { get; set; }
    public Alumno Alumno { get; set; }
    public ICollection<PreguntaExamen> Preguntas { get; set; }

    string ToString();
    void CalcularNota();
    void GuardarEstadisticasAlumno();
    void MostrarNota();
    void TomarExamen();
}

public class Examen : IExamen
{
    public int ExamenId { get; set; }
    public int Nota { get; set; } = 0;
    public bool Aprobado { get; set; } = false;
    public int AlumnoId { get; set; }
    public Alumno? Alumno { get; set; }
    public ICollection<PreguntaExamen> Preguntas { get; set; } = new List<PreguntaExamen>();

    public override string ToString()
    {
        using (DatosContexto db = new DatosContexto())
        {
            Alumno = db.Alumnos.Find(AlumnoId);
            Preguntas = db.PreguntasExamen.Where(pe => pe.ExamenId == ExamenId).ToList();
        }
        string estado = Aprobado ? "APROBADO" : "DESAPROBADO";
        return $"""

                Examen ID: {ExamenId}
                Alumno: {Alumno?.Nombre}
                Nota: {Nota}/10  -  Estado {estado}
                Preguntas Respondidas: {Preguntas.Count(p => p.Respondida)}
                Preguntas Correctas: {Preguntas.Count(p => p.Correcta)}
                Preguntas Incorrectas: {Preguntas.Count(p => !p.Correcta)}
            
            """;
    }

    public void CalcularNota()
    {
        using (DatosContexto db = new DatosContexto())
        {
            using var transaccion = db.Database.BeginTransaction();
            try
            {
                Nota = (Preguntas.Count(p => p.Correcta) * 10) / Preguntas.Count;
                Aprobado = Nota >= 6;

                db.SaveChanges();
                transaccion.Commit();
            }
            catch (Exception error)
            {
                transaccion.Rollback();
                Console.WriteLine("Error al calcular la nota.");
                Console.WriteLine(error.Message);
            }
        }
        GuardarEstadisticasAlumno();
    }

    public void GuardarEstadisticasAlumno()
    {
        using (DatosContexto db = new DatosContexto())
        {
            using var transaccion = db.Database.BeginTransaction();
            try
            {
                Alumno alumno = db.Alumnos.Find(AlumnoId);
                if (alumno == null)
                {
                    Console.WriteLine("Alumno no encontrado.");
                    return;
                }

                int preguntasRespondidas = Preguntas.Count(p => p.Respondida);
                int preguntasCorrectas = Preguntas.Count(p => p.Correcta);

                alumno.CantidadRespondidas += preguntasRespondidas;
                alumno.CantidadCorrectas += preguntasCorrectas;

                db.SaveChanges();
                transaccion.Commit();
            }
            catch (Exception error)
            {
                transaccion.Rollback();
                Console.WriteLine("Error al guardar las estadísticas del examen.");
                Console.WriteLine(error.Message);
            }
        }
    }

    public void MostrarNota()
    {
        string estado = Aprobado ? "APROBADO" : "DESAPROBADO";
        Console.WriteLine($"   Nota: {Nota}/10   -   Examen {estado}");
    }

    public static Examen? GenerarExamen()
    {
        Console.Clear();
        Console.WriteLine("--- Nuevo Examen ---");

        // Obtener o crear un alumno
        Alumno alumno = Alumno.ObtenerOCrear();

        // Crear un nuevo examen
        using (DatosContexto db = new DatosContexto())
        {
            var preguntasDisponibles = db
                .Preguntas.AsEnumerable()
                .OrderBy(p => Guid.NewGuid())
                .Take(10)
                .ToList();

            if (!preguntasDisponibles.Any())
            {
                Console.WriteLine("No hay preguntas disponibles para tomar el examen.");
                Utilidades.Esperar();
                return null;
            }

            Examen nuevoExamen = new Examen { AlumnoId = alumno.AlumnoId };

            using var transaccion = db.Database.BeginTransaction();
            try
            {
                // Guardar el examen en la base de datos
                db.Examenes.Add(nuevoExamen);
                db.SaveChanges();

                // Agregar las preguntas al examen
                foreach (Pregunta pregunta in preguntasDisponibles)
                {
                    PreguntaExamen preguntaExamen = new PreguntaExamen
                    {
                        PreguntaId = pregunta.PreguntaId,
                        Pregunta = pregunta,
                        ExamenId = nuevoExamen.ExamenId,
                        Examen = nuevoExamen,
                    };
                    db.PreguntasExamen.Add(preguntaExamen);
                }
                db.SaveChanges();

                transaccion.Commit();
            }
            catch (Exception error)
            {
                transaccion.Rollback();
                Console.WriteLine("Error al generar el examen.");
                Console.WriteLine(error.Message);
                Utilidades.Esperar();
                return null;
            }

            // Mostrar el examen al alumno
            Console.Clear();
            Console.WriteLine($"--- Examen para {alumno.Nombre} ---");
            Console.WriteLine($"Legajo: {alumno.Legajo}");
            Console.WriteLine($"Preguntas: {preguntasDisponibles.Count}");
            Console.WriteLine($"Examen ID: {nuevoExamen.ExamenId}");
            Console.WriteLine("--------------------------------------------------");
            /* foreach (PreguntaExamen preguntaExamen in nuevoExamen.Preguntas)
            {
                Console.WriteLine(preguntaExamen.Pregunta.ToString());
                Console.WriteLine("--------------------------------------------------");
            } */
            Utilidades.Esperar();
            return nuevoExamen;
        }
    }

    public void TomarExamen()
    {
        Console.Clear();
        Console.WriteLine("--- Tomar Examen ---");
        Console.WriteLine($"Examen ID: {ExamenId}");
        Console.WriteLine($"Alumno: {Alumno?.Nombre}");
        Console.WriteLine("--------------------------------------------------");

        foreach (PreguntaExamen pregunta in Preguntas)
        {
            Console.Clear();
            pregunta.ResponderPregunta();
        }

        Console.Clear();
        Console.WriteLine("--------------------------------------------------");
        if (Preguntas.All(p => p.Respondida))
        {
            Console.WriteLine("Todas las preguntas han sido respondidas.");
        }
        else
        {
            Console.WriteLine("No todas las preguntas han sido respondidas.");
        }
        // Actualizar estadísticas de cada pregunta
        foreach (PreguntaExamen pregunta in Preguntas)
        {
            pregunta.ActualizarEstadisticas();
        }
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("Examen finalizado");
        Console.WriteLine("--------------------------------------------------");
        CalcularNota();
        MostrarNota();
    }

    public static void MostrarUltimosExamenes(int cantidad)
    {
        Console.Clear();
        using (DatosContexto db = new DatosContexto())
        {
            var examenes = db.Examenes.OrderByDescending(e => e.ExamenId).Take(cantidad).ToList();

            if (!examenes.Any())
            {
                Console.WriteLine("No se encontraron exámenes para mostrar.");
                return;
            }
            Console.WriteLine($"--- Resultados de los últimos {cantidad} examenes ---");
            Console.WriteLine("--------------------------------------------------");
            foreach (var examen in examenes)
            {
                Console.WriteLine(examen.ToString());
            }
            Console.WriteLine("--------------------------------------------------");
        }
    }

    public static void MostrarExamenesDeAlumno(int legajo, int cantidad)
    {
        using (DatosContexto db = new DatosContexto())
        {
            var examenes = db
                .Examenes.Include(e => e.Alumno)
                .Where(e => e.Alumno.Legajo == legajo)
                .OrderByDescending(e => e.ExamenId)
                .Take(cantidad)
                .ToList();

            if (!examenes.Any())
            {
                Console.WriteLine(
                    $"No se encontraron exámenes para el alumno con legajo {legajo}."
                );
                return;
            }

            Console.WriteLine($"-- Exámenes del alumno con legajo {legajo}:");
            Console.WriteLine("--------------------------------------------------");
            foreach (var examen in examenes)
            {
                Console.WriteLine(examen.ToString());
            }
            Console.WriteLine("--------------------------------------------------");
        }
    }

    public static void MostrarRankingAlumnosPorNota()
    {
        using (DatosContexto db = new DatosContexto())
        {
            var alumnos = db.Alumnos.Include(a => a.Examenes).Take(5).ToList();

            var ranking = alumnos.OrderByDescending(a => a.Promedio).ToList();

            Console.WriteLine("--- Ranking de Alumnos por Nota ---");
            Console.WriteLine("--------------------------------------------------");
            int posicion = 1;
            foreach (var alumno in ranking)
            {
                Console.WriteLine(
                    $"""

                        {posicion++}) Alumno: {alumno.Nombre}
                        Legajo: {alumno.Legajo}
                        Exámenes Tomados: {alumno.Examenes.Count}
                        Promedio: {alumno.Promedio:F2}
                    
                    """
                );
            }
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
