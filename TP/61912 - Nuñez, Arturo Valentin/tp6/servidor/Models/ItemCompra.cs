namespace Servidor.Models
{
    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Product Producto { get; set; }
        public int CompraId { get; set; }
        public Compra Compra { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
