using System.Text.Json.Serialization;

namespace Servidor.Modelo
{
    public class ItemCarrito
    {
        public int Id { get; set; }

        public int CarritoId { get; set; }

        public Carrito Carrito { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
