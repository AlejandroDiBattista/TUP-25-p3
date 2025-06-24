using System.ComponentModel.DataAnnotations;

namespace servidor.ModelosServidor
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string ApellidoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string EmailCliente { get; set; } = string.Empty;
    }
}
