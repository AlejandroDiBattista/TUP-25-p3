using cliente.Models;

namespace cliente.Models
{
    public class CarritoItem
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}