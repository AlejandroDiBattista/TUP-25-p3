using System.ComponentModel.DataAnnotations;

namespace Servidor.Models;

public class Compra
{
    public int Id { get; set; }
    
    public DateTime Fecha { get; set; } = DateTime.Now;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
    public decimal Total { get; set; }
    
    [Required]
    [StringLength(100)]
    public string NombreCliente { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string ApellidoCliente { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string EmailCliente { get; set; } = string.Empty;
    
    // Navegación
    public virtual ICollection<ItemCompra> Items { get; set; } = new List<ItemCompra>();
}
