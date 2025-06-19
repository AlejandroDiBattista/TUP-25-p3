using System.Collections.Generic;

namespace tp4.Modelos
{
    public class ResultadoExamen
    {
        public int Id { get; set; }
        public string NombreAlumno { get; set; } = string.Empty;
        public int Correctas { get; set; }
        public int Total { get; set; }
        public double NotaFinal { get; set; }

        public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
    }
}
