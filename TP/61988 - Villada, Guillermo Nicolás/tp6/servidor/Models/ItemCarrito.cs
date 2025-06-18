using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace servidor.Models
{
    public class ItemCarrito
    {
        [Key]
        public int ItemCarritoId { get; set; }

        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Relación con Producto (opcional)
        public Producto? Producto { get; set; }

        // Relación con Compra
        public Guid CompraId { get; set; }

        [JsonIgnore] // Esto evita el ciclo infinito al serializar a JSON
        public Compra? Compra { get; set; }
    }
}