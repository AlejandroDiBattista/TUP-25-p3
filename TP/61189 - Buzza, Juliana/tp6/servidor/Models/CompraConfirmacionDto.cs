using System.ComponentModel.DataAnnotations;

namespace servidor.Models 
{
    public class CompraConfirmationDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
        public string ApellidoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres.")]
        public string EmailCliente { get; set; } = string.Empty;
    }
}
