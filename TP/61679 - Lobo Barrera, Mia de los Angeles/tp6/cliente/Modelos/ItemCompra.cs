using cliente.Modelos;

namespace cliente.Modelos
{
    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Productos { get; set; } = null;
        public int CompraId { get; set; }
        public Compra Compras { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}