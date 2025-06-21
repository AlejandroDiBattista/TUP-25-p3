namespace servidor.Models;

/// <summary>
/// Modelo que representa un carrito de compras temporal.
/// Se usa para manejar los productos antes de confirmar la compra.
/// </summary>
public class Carrito
{
    /// <summary>
    /// Identificador único del carrito. Se genera automáticamente en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Lista de items en el carrito.
    /// Cada item representa un producto con su cantidad deseada.
    /// </summary>
    public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
}

/// <summary>
/// Modelo que representa un item dentro del carrito de compras.
/// Similar a ItemCompra pero para el estado temporal antes de confirmar.
/// </summary>
public class ItemCarrito
{
    /// <summary>
    /// Identificador único del item de carrito.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del carrito al que pertenece este item.
    /// </summary>
    public int CarritoId { get; set; }
    public Carrito Carrito { get; set; } = null!;

    /// <summary>
    /// Identificador del producto en el carrito.
    /// </summary>
    public int ProductoId { get; set; }    /// <summary>
    /// Navegación hacia el producto asociado.
    /// </summary>
    public Producto Producto { get; set; } = null!;

    /// <summary>
    /// Cantidad deseada del producto.
    /// Debe ser > 0 y <= stock disponible.
    /// </summary>
    public int Cantidad { get; set; }
}
