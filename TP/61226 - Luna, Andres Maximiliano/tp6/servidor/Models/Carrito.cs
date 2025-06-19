using servidor.Models;

namespace servidor.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
