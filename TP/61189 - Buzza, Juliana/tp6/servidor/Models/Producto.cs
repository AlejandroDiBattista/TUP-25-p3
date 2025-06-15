// Servidor/Models/Producto.cs
using System.ComponentModel.DataAnnotations;

namespace servidor.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public decimal Precio { get; set; }
        [Required]
        public int Stock { get; set; }
        [MaxLength(255)]
        public string ImagenUrl { get; set; } = string.Empty;
    }
}