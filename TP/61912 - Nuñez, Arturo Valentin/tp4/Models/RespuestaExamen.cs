using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaExamenes.Console.Models;

public class RespuestaExamen
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ResultadoExamenId { get; set; }
    
    [Required]
    public int PreguntaId { get; set; }
    
    [Required]
    public char RespuestaSeleccionada { get; set; } // 'A', 'B', o 'C'
    
    [Required]
    public bool EsCorrecta { get; set; }
    
    // Relaciones con otras entidades
    [ForeignKey("ResultadoExamenId")]
    public virtual ResultadoExamen ResultadoExamen { get; set; } = null!;
    
    [ForeignKey("PreguntaId")]
    public virtual Pregunta Pregunta { get; set; } = null!;
} 