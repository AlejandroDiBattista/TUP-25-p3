using System.ComponentModel.DataAnnotations;

namespace cliente.Modelos
{
    public class DatosCliente
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string NombreCliente { get; set; } 

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string ApellidoCliente { get; set; } 

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string EmailCliente { get; set; } 
    }
}