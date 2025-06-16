namespace cliente.Shared;

public class ItemCompra
{
    public int Id { get; set; }
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
