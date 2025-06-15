using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class VentaItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Producto")] // 🔥 Declarar explícitamente la relación
        public int ProductoId { get; set; }

        [ForeignKey("Venta")] // 🔥 Declarar explícitamente la relación
        public int VentaId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        public decimal Total => Cantidad * PrecioUnitario;

        // 🔥 Propiedades de navegación correctamente definidas
        public Producto Producto { get; set; } = null!;
        public Venta Venta { get; set; } = null!;
    }
}
