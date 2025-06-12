using System.ComponentModel.DataAnnotations;

namespace servidor.Modulos
{
    public class RequisitosCompra
    {
        [Required]
        public string NombreCliente { get; set; }
        [Required]
        public string ApellidoCliente { get; set; }
        [Required, EmailAddress]
        public string EmailCliente { get; set; }
    }
}