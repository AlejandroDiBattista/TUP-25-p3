namespace Examenes.Models;

public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = null!;
    public string AlternativaA { get; set; } = null!;
    public string AlternativaB { get; set; } = null!;
    public string AlternativaC { get; set; } = null!;
    public char RespuestaCorrecta { get; set; }
    
    public List<RespuestaExamen> Respuestas { get; set; } = new();
}