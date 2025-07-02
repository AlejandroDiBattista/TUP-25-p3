namespace cliente.Models
{
    public class ItemCompra
    {
        public int Id { get; set; }

        // FK a Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        // FK a Compra
        public int CompraId { get; set; }
        public Compra Compra { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
} 