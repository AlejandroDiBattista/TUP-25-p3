namespace Servidor.Models;

/// <summary>
/// Representa un carrito de compras temporal antes de confirmar la compra.
/// </summary>
public class Carrito
{
    /// <summary>
    /// Identificador único del carrito.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Lista de ítems (productos y cantidades) agregados al carrito.
    /// </summary>
    public List<ItemCarrito> Items { get; set; } = new();
}
