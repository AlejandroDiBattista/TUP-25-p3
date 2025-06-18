using System.Collections.Generic;

namespace tp4.Modelos
{
    public class Pregunta
    {
        public int Id { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public string OpcionA { get; set; } = string.Empty;
        public string OpcionB { get; set; } = string.Empty;
        public string OpcionC { get; set; } = string.Empty;
        public char RespuestaCorrecta { get; set; }

        public ICollection<RespuestaExamen> Respuestas { get; set; } = new List<RespuestaExamen>();
    }
}
