using System.Collections.Generic;

public class Pregunta
{
    public int Id { get; set; }
    public string Texto { get; set; }
    public List<Respuesta> Respuestas { get; set; }
}