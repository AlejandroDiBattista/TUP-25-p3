public class Item
{
    public int Id { get; set; }

    // Clave foránea a Producto
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }

    // Clave foránea a Compra
    public int CompraId { get; set; }
    public Compra Compra { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioTotal { get; set; }
}