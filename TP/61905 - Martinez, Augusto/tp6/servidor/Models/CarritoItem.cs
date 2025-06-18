using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class CarritoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Producto))]
        public int ProductoId { get; set; }

        [Required]
        [ForeignKey(nameof(Carrito))]
        public Guid CarritoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // 🔥 Especifica precisión decimal en la base de datos
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0.")]
        public decimal PrecioUnitario { get; set; }

        [NotMapped] 
        public decimal Total => Cantidad * PrecioUnitario;

        // ✅ Propiedades de navegación correctamente definidas
        public Producto Producto { get; set; } = null!;
        public Carrito Carrito { get; set; } = null!;
    }
}
