namespace servidor.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class CarritoDto
{
    public string Id { get; set; } = string.Empty;
    public List<ItemCarritoDto> Items { get; set; } = new List<ItemCarritoDto>();
    public decimal Total { get; set; }
    public int TotalItems { get; set; }
}

public class ItemCarritoDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class ActualizarItemCarritoDto
{
    public int Cantidad { get; set; }
}

public class ConfirmarCompraDto
{
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
}

public class CompraConfirmadaDto
{
    public int CompraId { get; set; }
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
    public string Mensaje { get; set; } = "Compra confirmada exitosamente";
}
