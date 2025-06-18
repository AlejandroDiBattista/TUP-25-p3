namespace servidor.Models
{
    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int CompraId { get; set; } // Foreign key para Compra
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Propiedades de navegación
        public Producto Producto { get; set; } = null!;
        public Compra Compra { get; set; } = null!;
    }
}