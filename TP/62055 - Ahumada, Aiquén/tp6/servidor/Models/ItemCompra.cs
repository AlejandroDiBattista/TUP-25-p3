namespace servidor.Models;

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}
