using System.ComponentModel.DataAnnotations;

namespace servidor.models
{
    public class ConfirmarCompra
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string ApellidoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string EmailCliente { get; set; } = string.Empty;
    }
}