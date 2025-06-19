using System.ComponentModel.DataAnnotations;

namespace cliente.DTOs
{
    public class DatosClienteDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(1, ErrorMessage = "El nombre no puede estar vacío.")]
        public string NombreSolicitante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MinLength(1, ErrorMessage = "El apellido no puede estar vacío.")]
        public string ApellidoSolicitante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string CorreoElectronicoContacto { get; set; } = string.Empty;
    }
}
