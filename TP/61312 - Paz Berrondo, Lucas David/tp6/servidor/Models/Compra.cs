namespace servidor.Models;

/// <summary>
/// Modelo que representa una compra completada en el sistema.
/// Almacena información de la transacción y datos del cliente.
/// </summary>
public class Compra
{
    /// <summary>
    /// Identificador único de la compra. Clave primaria en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora cuando se realizó la compra.
    /// Se establece automáticamente al confirmar la compra.
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Monto total de la compra (suma de todos los items).
    /// Se calcula como: Σ(cantidad × precio_unitario) de todos los items.
    /// Utilizamos decimal para precisión en cálculos monetarios.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Nombre del cliente que realizó la compra.
    /// Campo obligatorio en el formulario de confirmación.
    /// </summary>
    public string NombreCliente { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del cliente que realizó la compra.
    /// Campo obligatorio en el formulario de confirmación.
    /// </summary>
    public string ApellidoCliente { get; set; } = string.Empty;

    /// <summary>
    /// Email del cliente para confirmación y comunicaciones.
    /// Campo obligatorio en el formulario de confirmación.
    /// </summary>
    public string EmailCliente { get; set; } = string.Empty;

    /// <summary>
    /// Colección de items que componen esta compra.
    /// Relación uno-a-muchos: una compra puede tener múltiples items.
    /// </summary>
    public List<ItemCompra> Items { get; set; } = new List<ItemCompra>();
}
