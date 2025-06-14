using System.ComponentModel.DataAnnotations;

namespace cliente.Models
{
    public class ClienteDatos
    {
        [Required(ErrorMessage = "El nombre no es opcional")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido no es opcional")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El email no es opcional")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido")]
        public string Email { get; set; }
    }
}