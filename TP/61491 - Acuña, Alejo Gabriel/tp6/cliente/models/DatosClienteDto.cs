using System.ComponentModel.DataAnnotations;
using cliente.Models;
namespace cliente.Models;

public class DatosClienteDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
