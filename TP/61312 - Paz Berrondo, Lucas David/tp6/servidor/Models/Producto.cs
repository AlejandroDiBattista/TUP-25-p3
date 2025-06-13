namespace servidor.Models;

/// <summary>
/// Modelo que representa un producto en la tienda online.
/// Contiene información básica del producto: identificación, descripción, precio, stock e imagen.
/// </summary>
public class Producto
{
    /// <summary>
    /// Identificador único del producto. Clave primaria en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del producto que se mostrará al cliente.
    /// Ejemplo: "rtx 5090 15", "Samsung m2"
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del producto con características principales.
    /// Ejemplo: "Smartphone con pantalla de 6.1 pulgadas, cámara de 48MP, 128GB"
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Precio unitario del producto en la moneda base del sistema.
    /// Utilizamos decimal para evitar problemas de precisión en cálculos monetarios.
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cantidad disponible en inventario. Se actualiza al agregar/quitar productos del carrito.
    /// Debe ser >= 0. Cuando llega a 0, el producto no puede agregarse al carrito.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// URL de la imagen del producto que se mostrará en el catálogo.
    /// Debe ser una URL válida accesible desde el cliente web.
    /// </summary>
    public string ImagenUrl { get; set; } = string.Empty;
}
