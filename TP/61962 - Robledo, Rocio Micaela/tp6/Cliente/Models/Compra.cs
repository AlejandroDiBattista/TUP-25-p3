using System.ComponentModel.DataAnnotations;

public class Compra
{
    [Required]
    public string NombreCliente { get; set; } = string.Empty;

    [Required]
    public string ApellidoCliente { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string EmailCliente { get; set; } = string.Empty;
}
