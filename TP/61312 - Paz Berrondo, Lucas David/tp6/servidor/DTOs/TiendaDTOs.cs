namespace servidor.DTOs;

// DTO para transferir datos de productos entre cliente y servidor
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

// DTO para enviar datos del carrito al cliente (incluye items y totales)
public class CarritoDto
{
    public string Id { get; set; } = string.Empty;
    public List<ItemCarritoDto> Items { get; set; } = new List<ItemCarritoDto>();
    public decimal Total { get; set; }
    public int TotalItems { get; set; }
}

// DTO para items individuales del carrito
public class ItemCarritoDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

// DTO para actualizar la cantidad de un producto en el carrito
public class ActualizarItemCarritoDto
{
    public int Cantidad { get; set; }
}

/// <summary>
/// DTO para confirmar una compra.
/// Incluye datos del cliente necesarios para completar la transacción.
/// </summary>
public class ConfirmarCompraDto
{
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
}

/// <summary>
/// DTO para respuesta después de confirmar una compra.
/// </summary>
public class CompraConfirmadaDto
{
    public int CompraId { get; set; }
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
    public string Mensaje { get; set; } = "Compra confirmada exitosamente";
}
