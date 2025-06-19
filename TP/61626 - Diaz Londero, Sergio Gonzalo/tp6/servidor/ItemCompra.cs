// Modelo de ítem de compra (producto + cantidad + precio)
public class ItemCompra
{
    public int Id { get; set; } // Identificador único
    public int ProductoId { get; set; } // FK al producto
    public Producto Producto { get; set; } = null!; // Navegación
    public int CompraId { get; set; } // FK a la compra
    public Compra Compra { get; set; } = null!; // Navegación
    public int Cantidad { get; set; } // Cantidad comprada
    public decimal PrecioUnitario { get; set; } // Precio al momento de la compra
}
