namespace TP4.Models;

public class RespuestaExamen
{
    public int Id { get; set; }
    public string RespuestaDada { get; set; } 
    public bool EsCorrecta { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }

    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
}