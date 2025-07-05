using System.ComponentModel.DataAnnotations;

namespace cliente.Models
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string ApellidoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un email válido")]
        public string EmailCliente { get; set; } = string.Empty;
    }
}
