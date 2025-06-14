using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace servidor.DTOs
{
    public class CompraDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string ApellidoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string EmailCliente { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto en la compra")]
        public List<ItemCompraDto> Items { get; set; } = new();
    }

    public class ItemCompraDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }
    }
}