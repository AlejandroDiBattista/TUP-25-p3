namespace cliente.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
}

public class VentaResponse
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<DetalleResponse> Detalles { get; set; } = new();
}

public class DetalleResponse
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public ProductoResponse Producto { get; set; }
}

public class ProductoResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; }
}