namespace Examenes.Models;

public class RespuestaExamen
{
    public int Id { get; set; }
    public char Respuesta { get; set; }
    public bool EsCorrecta { get; set; }
    
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; } = null!;
    
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = null!;
}