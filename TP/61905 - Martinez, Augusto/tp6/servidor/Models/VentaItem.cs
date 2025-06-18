using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class VentaItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; } // ✅ Clave foránea sin anotación para evitar conflictos

        [Required]
        public int VentaId { get; set; } // ✅ Clave foránea sin anotación para evitar conflictos

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // ✅ Asegura precisión en la base de datos
        public decimal PrecioUnitario { get; set; }

        [NotMapped]
        public decimal Total => Cantidad * PrecioUnitario;

        // ✅ Propiedades de navegación correctamente definidas sin `[ForeignKey]`
        public Producto Producto { get; set; } = null!;
        public Venta Venta { get; set; } = null!;
    }
}
