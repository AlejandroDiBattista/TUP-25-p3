// Representa un item del carrito en el cliente
namespace cliente.Modelos;
public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public Producto Producto { get; set; } // Para mostrar los detalles del producto en el carrito
}