namespace servidor.Modelos;

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    
    // Quitar estas propiedades de navegación que están causando problemas:
    // public Producto Producto { get; set; } = null!;
    // public Compra Compra { get; set; } = null!;
}