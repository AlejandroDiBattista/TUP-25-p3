using System.ComponentModel.DataAnnotations;

namespace cliente.Modulos
{
    public class RequisitosCompra
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? NombreCliente { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string? ApellidoCliente { get; set; }
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? EmailCliente { get; set; }
    }
}