using System.ComponentModel.DataAnnotations;

namespace Cliente.Models2
{
    public class ClienteInfo
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un email válido.")]
        public string Email { get; set; } = string.Empty;
    }
}
