namespace servidor.Models;

/// <summary>
/// Modelo que representa un item individual dentro de una compra.
/// Vincula un producto específico con una compra, incluyendo cantidad y precio al momento de la compra.
/// </summary>
public class ItemCompra
{
    /// <summary>
    /// Identificador único del item de compra. Clave primaria en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto asociado a este item.
    /// Clave foránea que referencia la tabla Productos.
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Navegación hacia el producto asociado.
    /// Permite acceder a los datos completos del producto desde el item.
    /// </summary>
    public Producto Producto { get; set; } = null!;

    /// <summary>
    /// Identificador de la compra a la que pertenece este item.
    /// Clave foránea que referencia la tabla Compras.
    /// </summary>
    public int CompraId { get; set; }

    /// <summary>
    /// Navegación hacia la compra asociada.
    /// Permite acceder a los datos completos de la compra desde el item.
    /// </summary>
    public Compra Compra { get; set; } = null!;

    /// <summary>
    /// Cantidad de unidades del producto en este item de compra.
    /// Debe ser > 0. Representa cuántas unidades del producto se compraron.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del producto al momento de la compra.
    /// Se guarda para mantener el historial, ya que el precio del producto puede cambiar.
    /// Utilizamos decimal para precisión en cálculos monetarios.
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Calcula el subtotal de este item (Cantidad × PrecioUnitario).
    /// Propiedad calculada que no se almacena en la base de datos.
    /// </summary>
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
