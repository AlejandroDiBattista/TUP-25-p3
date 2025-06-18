namespace Cliente.Models;

public class ItemCompra
{
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = new();
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
