using System.ComponentModel.DataAnnotations;

namespace Cliente.Modelos;

public class Compra
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La dirección es obligatoria")]
    public string Direccion { get; set; } = "";
}
