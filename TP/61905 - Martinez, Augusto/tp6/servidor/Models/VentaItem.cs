using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class VentaItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Producto")] // ✅ Definición clara de clave foránea
        public int ProductoId { get; set; } 

        [Required]
        [ForeignKey("Venta")] // ✅ Definición clara de clave foránea
        public int VentaId { get; set; } 

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal PrecioUnitario { get; set; }

        [NotMapped]
        public decimal Total => Cantidad * PrecioUnitario;

        // ✅ Relaciones correctamente referenciadas
        public Producto Producto { get; set; } = null!;
        public Venta Venta { get; set; } = null!;
    }
}
