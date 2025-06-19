using System.ComponentModel.DataAnnotations;

namespace Cliente.Models
{
    public class FormularioCompra
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }
    }
}
