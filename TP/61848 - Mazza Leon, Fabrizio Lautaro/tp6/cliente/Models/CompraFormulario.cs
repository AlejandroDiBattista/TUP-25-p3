using System.ComponentModel.DataAnnotations;

namespace cliente.Models;

public class CompraFormulario
{
    [Required]
    public string Nombre { get; set; }
    
    [Required]
    public string Apellido { get; set; }
    
    [Required, EmailAddress]
    public string Email { get; set; }
}
