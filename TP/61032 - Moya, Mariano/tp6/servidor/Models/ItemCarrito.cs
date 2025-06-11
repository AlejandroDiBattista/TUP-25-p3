namespace Servidor.Models;

/// <summary>
/// Relaciona un producto con un carrito y la cantidad seleccionada.
/// </summary>
public class ItemCarrito
{
    public int Id { get; set; }
    public int CarritoId { get; set; }
    public Carrito Carrito { get; set; } = null!;
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int Cantidad { get; set; }
}
