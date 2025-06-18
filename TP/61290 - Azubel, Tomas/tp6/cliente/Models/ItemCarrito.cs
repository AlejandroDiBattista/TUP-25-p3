namespace Cliente.Models;

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;

    public decimal Importe => PrecioUnitario * Cantidad;
}