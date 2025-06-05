namespace servidor.Models
{
    public class ItemCompra
    {
        public int ItemCompraId { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int Cantidad { get; set; }

        // Relación con la compra
        public int CompraId { get; set; }
        public Compra? Compra { get; set; }
    }
}
