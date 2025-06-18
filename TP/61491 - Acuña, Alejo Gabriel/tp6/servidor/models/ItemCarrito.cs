namespace servidor.Models;

public class ItemCarrito
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int Cantidad { get; set; }

    public Guid CarritoId { get; set; }
    public Carrito Carrito { get; set; } = null!;
}
