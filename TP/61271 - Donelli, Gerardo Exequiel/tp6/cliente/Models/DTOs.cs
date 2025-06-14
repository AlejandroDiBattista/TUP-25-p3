using System.ComponentModel.DataAnnotations;

namespace cliente.Models;

/// <summary>
/// DTO para representar un producto en el cliente
/// </summary>
public class ProductoDTO
{
    /// <summary>
    /// Identificador único del producto
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del producto
    /// </summary>
    public string Nombre { get; set; } = "";

    /// <summary>
    /// Descripción detallada del producto
    /// </summary>
    public string Descripcion { get; set; } = "";

    /// <summary>
    /// Precio unitario del producto
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cantidad disponible en stock
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// URL de la imagen del producto
    /// </summary>
    public string ImagenUrl { get; set; } = "";
}

/// <summary>
/// DTO para representar una compra realizada
/// </summary>
public class CompraDTO 
{
    /// <summary>
    /// Identificador único de la compra
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora en que se realizó la compra
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Monto total de la compra
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Nombre del cliente que realizó la compra
    /// </summary>
    public string NombreCliente { get; set; }

    /// <summary>
    /// Apellido del cliente que realizó la compra
    /// </summary>
    public string ApellidoCliente { get; set; }

    /// <summary>
    /// Correo electrónico del cliente
    /// </summary>
    public string EmailCliente { get; set; }

    /// <summary>
    /// Lista de items incluidos en la compra
    /// </summary>
    public List<ItemCompraDTO> Items { get; set; } = new();
}

/// <summary>
/// DTO para representar un item dentro de una compra
/// </summary>
public class ItemCompraDTO
{
    /// <summary>
    /// Identificador único del item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto asociado
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Cantidad del producto
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del producto al momento de la compra
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Información del producto asociado
    /// </summary>
    public ProductoDTO Producto { get; set; }
}

/// <summary>
/// Representa un item en el carrito de compras
/// </summary>
public class ItemCarrito
{
    /// <summary>
    /// Identificador del producto en el carrito
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Nombre del producto
    /// </summary>
    public string Nombre { get; set; } = "";

    /// <summary>
    /// Precio unitario del producto
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cantidad seleccionada del producto
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Calcula el subtotal multiplicando el precio por la cantidad
    /// </summary>
    public decimal Subtotal => Precio * Cantidad;
}

/// <summary>
/// DTO para enviar una orden de compra al servidor
/// </summary>
public class OrdenCompraDTO
{
    /// <summary>
    /// Lista de items a comprar
    /// </summary>
    public List<ItemOrdenDTO> Items { get; set; } = new();
}

/// <summary>
/// DTO para representar un item en una orden de compra
/// </summary>
public class ItemOrdenDTO
{
    /// <summary>
    /// Identificador del producto a comprar
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Cantidad deseada del producto
    /// </summary>
    public int Cantidad { get; set; }
}

/// <summary>
/// DTO para enviar los datos del cliente al realizar una compra
/// </summary>
public class DatosClienteDTO
{
    /// <summary>
    /// Nombre del cliente
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    public string Nombre { get; set; } = "";

    /// <summary>
    /// Apellido del cliente
    /// </summary>
    [Required(ErrorMessage = "El apellido es obligatorio")]
    [MinLength(2, ErrorMessage = "El apellido debe tener al menos 2 caracteres")]
    public string Apellido { get; set; } = "";

    /// <summary>
    /// Correo electrónico del cliente
    /// </summary>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    public string Email { get; set; } = "";
}
