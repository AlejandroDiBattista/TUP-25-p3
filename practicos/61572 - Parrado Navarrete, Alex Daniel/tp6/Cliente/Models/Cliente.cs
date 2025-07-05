using System.ComponentModel.DataAnnotations;

namespace cliente.Models;

public class Cliente
{
    [Required(ErrorMessage = "Nombre obligatorio")]
    public string Nombre   { get; set; } = string.Empty;

    [Required(ErrorMessage = "Apellido obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email obligatorio")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email    { get; set; } = string.Empty;
}