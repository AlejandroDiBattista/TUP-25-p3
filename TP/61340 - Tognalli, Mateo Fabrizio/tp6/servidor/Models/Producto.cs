using System.ComponentModel.DataAnnotations;

namespace Servidor.Models;

public class Producto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
    
    [StringLength(500)]
    public string ImagenUrl { get; set; } = string.Empty;
    
    // Navegación
    public virtual ICollection<ItemCompra> ItemsCompra { get; set; } = new List<ItemCompra>();
}
