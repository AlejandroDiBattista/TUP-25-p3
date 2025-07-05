namespace Examenes.Models;

public class ResultadoExamen
{
    public int Id { get; set; }
    public string NombreAlumno { get; set; } = null!;
    public int Correctas { get; set; }
    public int TotalPreguntas { get; set; }
    public decimal Nota { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}