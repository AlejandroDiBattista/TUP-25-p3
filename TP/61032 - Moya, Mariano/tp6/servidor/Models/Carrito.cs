namespace Servidor.Models;

/// <summary>
/// Representa un carrito de compras temporal antes de confirmar la compra.
/// </summary>
public class Carrito
{
    public int Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new();
}
