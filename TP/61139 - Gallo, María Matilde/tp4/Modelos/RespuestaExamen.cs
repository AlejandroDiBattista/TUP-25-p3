namespace tp4.Modelos
{
    public class RespuestaExamen
    {
        public int Id { get; set; }

        public int ResultadoExamenId { get; set; }
        public ResultadoExamen ResultadoExamen { get; set; } = null!;

        public int PreguntaId { get; set; }
        public Pregunta Pregunta { get; set; } = null!;

        public char RespuestaAlumno { get; set; }
        public bool Correcta { get; set; }
    }
}
