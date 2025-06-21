using System.Collections.Generic;

namespace tp4.Models
{
    public class Pregunta
    {
        public int Id { get; set; }
        public string Enunciado { get; set; }
        public string OpcionA { get; set; }
        public string OpcionB { get; set; }
        public string OpcionC { get; set; }
        public string RespuestaCorrecta { get; set; } // "A", "B" o "C"
        public List<RespuestaExamen> RespuestasExamen { get; set; } = new();
    }
}
