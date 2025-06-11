namespace Servidor.Models;

/// <summary>
/// Representa un producto disponible en la tienda online.
/// </summary>
public class Producto
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Precio del producto.
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Stock disponible del producto.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// URL de la imagen representativa del producto.
    /// </summary>
    public string ImagenUrl { get; set; } = string.Empty;

    /// <summary>
    /// Relación con los ítems de compra donde aparece este producto.
    /// </summary>
    public List<ItemCompra> ItemsCompra { get; set; } = new();
}
