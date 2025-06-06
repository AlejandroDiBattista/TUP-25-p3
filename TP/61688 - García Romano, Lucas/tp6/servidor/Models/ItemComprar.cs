namespace servidor.Models;
//representa la tienda y sus detalles 
public class ItemCompra
{
    public int Id { get; set; }

    public int ProductoId { get; set; }
    //Para que el producto no sea nulo si no tiene un id
    public Producto? Producto { get; set; }
    //relaciona la compra con el detalle de la compra
    public int CompraId { get; set; }
    public Compra? Compra { get; set; }
//Detalles del producto comprado
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    //Calcula el total
}