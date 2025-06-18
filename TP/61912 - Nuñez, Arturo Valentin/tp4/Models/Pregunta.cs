using System.ComponentModel.DataAnnotations;

namespace SistemaExamenes.Console.Models;

public class Pregunta
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Enunciado { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string AlternativaA { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string AlternativaB { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string AlternativaC { get; set; } = string.Empty;
    
    [Required]
    public char RespuestaCorrecta { get; set; } // 'A', 'B', o 'C'
    
    // Relación con RespuestaExamen (1 a muchos)
    public virtual ICollection<RespuestaExamen> RespuestasExamen { get; set; } = new List<RespuestaExamen>();
} 