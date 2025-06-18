using servidor.Modelos;
namespace servidor.Modelos
{
    public class CarritoItem
    {
        public int Id { get; set; }
        public int CarritoId { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal Total => Cantidad * PrecioUnitario;
    }
}