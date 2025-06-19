using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace servidor.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ✅ Genera automáticamente `Id`
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "double")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")] // ✅ Validación para evitar valores incorrectos
        public int Stock { get; set; }

        [MaxLength(300)]
        public string ImagenUrl { get; set; } = string.Empty;

        // ✅ Relación con `VentaItem`
        public List<VentaItem> VentaItems { get; set; } = new();
    }
}
