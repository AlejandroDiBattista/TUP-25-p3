using System.ComponentModel.DataAnnotations;

namespace cliente.Models;

public class CompraFormulario
{

    public int CarritoId { get; set; }

    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "El campo Apellido es obligatorio.")]
    public string Apellido { get; set; }
    
    [Required(ErrorMessage = "El campo Email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del Email es inválido.")]
    public string Email { get; set; }
}
