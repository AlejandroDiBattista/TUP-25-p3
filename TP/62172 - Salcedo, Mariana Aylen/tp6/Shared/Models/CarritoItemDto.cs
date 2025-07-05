namespace Shared.Models;
public class CarritoItemDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public decimal Importe => Cantidad * PrecioUnitario;
}