using System.ComponentModel.DataAnnotations;

namespace cliente.Modelos
{
    public class CompraConfirmacionDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Ingrese el gmail por favor o inténtelo más tarde")]
        [EmailAddress(ErrorMessage = "Ingrese un gmail válido")]
        public string Email { get; set; }
    }
}