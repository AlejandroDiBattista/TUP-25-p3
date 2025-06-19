public class RespuestaExamen
{
    public int Id { get; set; }
    public int ResultadoExamenId { get; set; }
    public ResultadoExamen ResultadoExamen { get; set; }
    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; }
    public int RespuestaId { get; set; }
    public Respuesta Respuesta { get; set; }
    public bool EsCorrecta { get; set; }
}