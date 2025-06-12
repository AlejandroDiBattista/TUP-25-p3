using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class ItemCompra
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        // Esta propiedad es nullable, y con #nullable enable es válido
        public Producto? Producto { get; set; } // Propiedad de navegación
        [Required]
        public int CompraId { get; set; }
        [ForeignKey("CompraId")]
        public Compra? Compra { get; set; } // Propiedad de navegación
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public decimal PrecioUnitario { get; set; }
    }
}
