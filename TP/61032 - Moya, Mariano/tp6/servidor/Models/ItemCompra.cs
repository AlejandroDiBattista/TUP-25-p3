namespace Servidor.Models;

/// <summary>
/// Representa un ítem dentro de una compra (producto, cantidad y precio unitario).
/// </summary>
public class ItemCompra
{
    /// <summary>
    /// Identificador único del ítem de compra.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto comprado.
    /// </summary>
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;

    /// <summary>
    /// Identificador de la compra a la que pertenece este ítem.
    /// </summary>
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;

    /// <summary>
    /// Cantidad de unidades compradas de este producto.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del producto al momento de la compra.
    /// </summary>
    public decimal PrecioUnitario { get; set; }
}
