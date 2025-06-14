namespace cliente.Modelos;

public class ItemCompra
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }

    public decimal Subtotal => PrecioUnitario * Cantidad;
}
