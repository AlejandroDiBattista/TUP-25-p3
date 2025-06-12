namespace servidor.Modelos;

public class ItemCompra
{
    public int Id { get; set; }
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => Producto.Precio * Cantidad;
    public Compra Compra { get; set; }
}