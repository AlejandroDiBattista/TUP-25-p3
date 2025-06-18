namespace cliente.Models;

/// <summary>
/// DTO para transferir datos de productos entre cliente y servidor.
/// Incluye toda la información necesaria para mostrar productos en el catálogo.
/// </summary>
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

/// <summary>
/// DTO para enviar datos del carrito al cliente.
/// Incluye items y totales calculados.
/// </summary>
public class CarritoDto
{
    public string Id { get; set; } = string.Empty;
    public List<ItemCarritoDto> Items { get; set; } = new List<ItemCarritoDto>();
    public decimal Total { get; set; }
    public int TotalItems { get; set; }
}

/// <summary>
/// DTO para items del carrito.
/// </summary>
public class ItemCarritoDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

/// <summary>
/// DTO para actualizar la cantidad de un producto en el carrito.
/// </summary>
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
    public string Mensaje { get; set; } = "Compra confirmada exitosamente";
}

/// <summary>
/// DTO para respuestas de API que contienen listas de productos.
/// </summary>
public class ProductosRespuestaDto
{
    public List<ProductoDto> Productos { get; set; } = new List<ProductoDto>();
    public int Total { get; set; }
    public string TerminoBusqueda { get; set; } = string.Empty;
}

/// <summary>
/// DTO para respuestas de creación de carrito.
/// </summary>
public class CarritoCreado
{
    public string CarritoId { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para respuestas estándar de operaciones exitosas.
/// </summary>
public class RespuestaOperacion
{
    public string Mensaje { get; set; } = string.Empty;
    public string CarritoId { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public int CantidadFinal { get; set; }
    public DateTime FechaActualizacion { get; set; }
}

/// <summary>
/// DTO para respuesta de stock disponible considerando carrito.
/// </summary>
public class StockDisponibleDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int StockTotal { get; set; }
    public int CantidadEnCarrito { get; set; }
    public int StockDisponible { get; set; }
}
