using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace servidor.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        [MaxLength(300)]
        public string ImagenUrl { get; set; } = string.Empty;

        // ✅ Agregar relación con `VentaItem`
        public List<VentaItem> VentaItems { get; set; } = new List<VentaItem>();
    }
}
