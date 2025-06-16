namespace servidor.Models;

/// <summary>
/// Modelo que representa un carrito de compras temporal.
/// Se usa para manejar los productos antes de confirmar la compra.
/// </summary>
public class Carrito
{
    /// <summary>
    /// Identificador único del carrito. Se genera como GUID para sesiones temporales.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Fecha de creación del carrito.
    /// Se usa para limpiar carritos abandonados.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Lista de items en el carrito.
    /// Cada item representa un producto con su cantidad deseada.
    /// </summary>
    public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();

    /// <summary>
    /// Calcula el total del carrito (suma de todos los subtotales).
    /// Propiedad calculada que no se almacena en la base de datos.
    /// </summary>
    public decimal Total => Items.Sum(item => item.Subtotal);

    /// <summary>
    /// Calcula la cantidad total de items en el carrito.
    /// Útil para mostrar el contador en el ícono del carrito.
    /// </summary>
    public int TotalItems => Items.Sum(item => item.Cantidad);
}

/// <summary>
/// Modelo que representa un item dentro del carrito de compras.
/// Similar a ItemCompra pero para el estado temporal antes de confirmar.
/// </summary>
public class ItemCarrito
{
    /// <summary>
    /// Identificador del producto en el carrito.
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Navegación hacia el producto asociado.
    /// </summary>
    public Producto Producto { get; set; } = null!;

    /// <summary>
    /// Cantidad deseada del producto.
    /// Debe ser > 0 y <= stock disponible.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario actual del producto.
    /// Se toma del producto al agregarlo al carrito.
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Calcula el subtotal del item (Cantidad × PrecioUnitario).
    /// </summary>
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
