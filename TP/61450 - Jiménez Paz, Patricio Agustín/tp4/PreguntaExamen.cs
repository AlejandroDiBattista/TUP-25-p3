namespace TP4_61450;

public interface IPreguntaExamen
{
    public int PreguntaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public int ExamenId { get; set; }
    public Examen Examen { get; set; }
    public string Respuesta { get; set; }
    public bool Correcta { get; set; }
    public bool Respondida => !string.IsNullOrEmpty(Respuesta);

    public void ResponderPregunta();
    public void VerificarRespuesta();
    public void ActualizarEstadisticas();
}

public class PreguntaExamen : IPreguntaExamen
{
    public int PreguntaExamenId { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta? Pregunta { get; set; }
    public int ExamenId { get; set; }
    public Examen? Examen { get; set; }
    public string Respuesta { get; set; } = "";
    public bool Correcta { get; set; } = false;
    public bool Respondida => !string.IsNullOrEmpty(Respuesta);

    public void ResponderPregunta()
    {
        Console.WriteLine(Pregunta.ToString());
        Console.Write("\nRespuesta (A, B, C): ");
        string respuesta;
        do
        {
            respuesta = Console.ReadLine().ToUpper();
            if (respuesta != "A" && respuesta != "B" && respuesta != "C")
            {
                Console.Write("Respuesta inválida. Por favor, ingrese A, B o C: ");
            }
        } while (respuesta != "A" && respuesta != "B" && respuesta != "C");
        using (DatosContexto db = new DatosContexto())
        {
            //Pregunta pregunta = db.Preguntas.Find(PreguntaId);
            using var transaccion = db.Database.BeginTransaction();
            try
            {
                Respuesta = respuesta; // Guardar la respuesta
                VerificarRespuesta();
                db.SaveChanges();

                transaccion.Commit();
            }
            catch (Exception)
            {
                transaccion.Rollback();
                Console.WriteLine("Error al guardar la respuesta.");
            }
        }
    }

    public void VerificarRespuesta()
    {
        Correcta = Respuesta == Pregunta.RespuestaCorrecta;
    }

    public void ActualizarEstadisticas()
    {
        using (DatosContexto db = new DatosContexto())
        {
            using var transaccion = db.Database.BeginTransaction();
            try
            {
                Pregunta pregunta = db.Preguntas.Find(PreguntaId);
                if (pregunta != null)
                {
                    pregunta.CantidadRespondida = pregunta.CantidadRespondida + 1;
                    if (Correcta)
                    {
                        pregunta.CantidadCorrecta = pregunta.CantidadCorrecta + 1;
                    }
                    db.SaveChanges();
                    transaccion.Commit();
                }
                else
                {
                    Console.WriteLine("Pregunta no encontrada.");
                    transaccion.Rollback();
                }
            }
            catch (Exception)
            {
                transaccion.Rollback();
                Console.WriteLine("Error al guardar la respuesta.");
            }
        }
    }
}
