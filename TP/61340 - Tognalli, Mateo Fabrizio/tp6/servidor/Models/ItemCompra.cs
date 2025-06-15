using System.ComponentModel.DataAnnotations;

namespace Servidor.Models;

public class ItemCompra
{
    public int Id { get; set; }
    
    public int ProductoId { get; set; }
    
    public int CompraId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0")]
    public decimal PrecioUnitario { get; set; }
    
    // Navegación
    public virtual Producto Producto { get; set; } = null!;
    public virtual Compra Compra { get; set; } = null!;
}
