using System.ComponentModel.DataAnnotations;

namespace shared.DTOs;

public class DatosCliente
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string NombreCliente { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string ApellidoCliente { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string EmailCliente { get; set; } = string.Empty;
}