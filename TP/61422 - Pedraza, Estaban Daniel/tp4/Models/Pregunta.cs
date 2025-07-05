namespace TP4.Models;

public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; }
    public string OpcionA { get; set; }
    public string OpcionB { get; set; }
    public string OpcionC { get; set; }
    public string RespuestaCorrecta { get; set; } 
    public ICollection<RespuestaExamen> Respuestas { get; set; } 
}