using System.ComponentModel.DataAnnotations;

namespace Cliente.Models
{
    public class VentaItem
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }

        public decimal Total => PrecioUnitario * Cantidad;
    }
}
