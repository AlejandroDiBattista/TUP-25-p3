using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class CarritoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        // Foreign key
        [Required]
        public Guid CarritoId { get; set; }

        [ForeignKey("CarritoId")]
        public Carrito Carrito { get; set; } = null!;
    }
}
