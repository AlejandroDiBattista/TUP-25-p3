public class ResultadoExamen
{
    public int Id { get; set; }
    public string Alumno { get; set; }
    public int CantidadCorrectas { get; set; }
    public int TotalPreguntas { get; set; }
    public float NotaFinal { get; set; }

    public ICollection<RespuestaExamen> Respuestas { get; set; }
}
