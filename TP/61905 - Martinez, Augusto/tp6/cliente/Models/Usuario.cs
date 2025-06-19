using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Cliente.Models;


namespace Cliente.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // ✅ Relación con carritos y ventas
        public List<Carrito> Carritos { get; set; } = new();
        public List<Venta> Ventas { get; set; } = new();
    }
}
