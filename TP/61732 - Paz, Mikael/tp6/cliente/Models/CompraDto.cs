using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace cliente.Models
{
    public class CompraDto
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCliente { get; set; } = string.Empty;
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string ApellidoCliente { get; set; } = string.Empty;
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string EmailCliente { get; set; } = string.Empty;
        public List<ItemCompraDto> Items { get; set; } = new();
    }
    public class ItemCompraDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
