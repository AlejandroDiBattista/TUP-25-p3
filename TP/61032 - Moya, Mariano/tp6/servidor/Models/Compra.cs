namespace Servidor.Models;

/// <summary>
/// Representa una compra confirmada por un cliente.
/// </summary>
public class Compra
{
    /// <summary>
    /// Identificador único de la compra.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora en que se realizó la compra.
    /// </summary>
    public DateTime Fecha { get; set; } = DateTime.Now;

    /// <summary>
    /// Importe total de la compra.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Nombre del cliente que realizó la compra.
    /// </summary>
    public string NombreCliente { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del cliente que realizó la compra.
    /// </summary>
    public string ApellidoCliente { get; set; } = string.Empty;

    /// <summary>
    /// Email del cliente que realizó la compra.
    /// </summary>
    public string EmailCliente { get; set; } = string.Empty;

    /// <summary>
    /// Lista de ítems de la compra (productos y cantidades).
    /// </summary>
    public List<ItemCompra> ItemsCompra { get; set; } = new();
}
