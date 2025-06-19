using System.ComponentModel.DataAnnotations;

namespace cliente.DTOs
{
    public class CompraDTO
    {
        [Required]
        public string NombreCliente { get; set; }
        [Required]
        public string ApellidoCliente { get; set; }
        [Required, EmailAddress]
        public string EmailCliente { get; set; }
        public List<ItemCompraDTO> Items { get; set; }
    }
}