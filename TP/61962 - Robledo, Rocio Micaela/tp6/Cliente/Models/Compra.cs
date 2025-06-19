using System.ComponentModel.DataAnnotations; // Importa atributos para validar datos

public class Compra
{
    [Required] // Indica que este campo es obligatorio (no puede estar vacío)
    public string NombreCliente { get; set; } = string.Empty;

    [Required] // También obligatorio
    public string ApellidoCliente { get; set; } = string.Empty;

    [Required]  // Obligatorio
    [EmailAddress] // Valida que el valor sea un email con formato correcto
    public string EmailCliente { get; set; } = string.Empty;
}
