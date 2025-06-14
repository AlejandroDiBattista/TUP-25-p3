
namespace servidor.models
{
    public class Compras
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } // Propiedad de navegación
        public int CompraId { get; set; }
        public Compra Compra { get; set; } // Propiedad de navegación
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}