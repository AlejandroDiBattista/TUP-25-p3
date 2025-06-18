using System.ComponentModel.DataAnnotations;

namespace SistemaExamenes.Console.Models;

public class ResultadoExamen
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string NombreAlumno { get; set; } = string.Empty;
    
    [Required]
    public int CantidadRespuestasCorrectas { get; set; }
    
    [Required]
    public int TotalPreguntas { get; set; }
    
    [Required]
    public decimal NotaFinal { get; set; } // Sobre 5 puntos
    
    [Required]
    public DateTime FechaExamen { get; set; } = DateTime.Now;
    
    // Relación con RespuestaExamen (1 a muchos)
    public virtual ICollection<RespuestaExamen> RespuestasExamen { get; set; } = new List<RespuestaExamen>();
} 