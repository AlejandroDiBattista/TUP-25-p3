using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor.Models
{
    public class ItemsCarrito
    {
        public int Id { get; set; }
        public int CarritoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; } // Precio del producto al momento de agregarlo

        // Relaciones de navegación
        public virtual Carrito Carrito { get; set; }
        public virtual Producto Producto { get; set; }
    }
}