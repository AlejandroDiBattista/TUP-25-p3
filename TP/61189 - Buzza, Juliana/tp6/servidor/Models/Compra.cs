using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace servidor.Models
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        [MaxLength(100)]
        public string? NombreCliente { get; set; }
        [MaxLength(100)]
        public string? ApellidoCliente { get; set; }
        [MaxLength(100)]
        [EmailAddress]
        public string? EmailCliente { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Confirmed"

        public ICollection<ItemCompra> Items { get; set; } = new List<ItemCompra>();
    }
}