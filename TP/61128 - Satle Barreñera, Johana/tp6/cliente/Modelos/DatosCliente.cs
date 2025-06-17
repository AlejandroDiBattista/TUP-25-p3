using System.ComponentModel.DataAnnotations;

namespace cliente.Modelos;

public class DatosCliente
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = "";

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = "";
}