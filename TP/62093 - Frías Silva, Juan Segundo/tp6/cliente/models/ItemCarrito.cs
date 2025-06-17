using cliente.Models;

namespace cliente.Models;

public class ItemCarrito
{
    public Producto Producto { get; set; } = null!;
    public int Cantidad { get; set; }
}