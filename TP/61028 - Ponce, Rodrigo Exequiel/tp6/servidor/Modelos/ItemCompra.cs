
using servidor.Modelos; // Asegúrate de que las clases Producto y Compra están en este namespace o cambia el namespace según corresponda    

namespace servidor.Modelos
{
    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int CompraId { get; set; }
        public Compra Compra { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
