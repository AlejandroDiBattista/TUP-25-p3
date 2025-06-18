using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Usuario))] // ✅ Relación con Usuario
        public int UsuarioId { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0.")]
        public decimal Total { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ApellidoCliente { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string EmailCliente { get; set; } = string.Empty;

        // ✅ Relación con los productos de la venta
        public List<VentaItem> VentaItems { get; set; } = new List<VentaItem>();

        // ✅ Propiedad de navegación con Usuario
        public Usuario Usuario { get; set; } = null!;
    }
}
