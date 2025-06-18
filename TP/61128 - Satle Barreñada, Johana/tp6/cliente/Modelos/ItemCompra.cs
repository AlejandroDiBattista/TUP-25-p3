namespace cliente.Modelos;

public class ItemCompra
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }

    // Opcional, si querés mostrar más info del producto en el carrito
    public Producto? Producto { get; set; }
}