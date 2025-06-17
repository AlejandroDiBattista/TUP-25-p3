namespace Servidor.Models;

/// <summary>
/// Relaciona un producto con un carrito y la cantidad seleccionada.
/// </summary>
public class ItemCarrito
{
    /// <summary>
    /// Identificador único del ítem de carrito.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del carrito al que pertenece este ítem.
    /// </summary>
    public int CarritoId { get; set; }
    public Carrito Carrito { get; set; } = null!;

    /// <summary>
    /// Identificador del producto agregado al carrito.
    /// </summary>
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;

    /// <summary>
    /// Cantidad seleccionada de este producto en el carrito.
    /// </summary>
    public int Cantidad { get; set; }
}
