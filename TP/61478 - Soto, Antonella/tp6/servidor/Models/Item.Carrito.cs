using System.Text.Json.Serialization;

namespace Servidor.Models
{
    public class ItemCarrito
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int CarritoId { get; set; }

        [JsonIgnore] // 
        public Carrito? Carrito { get; set; }

        public int Cantidad { get; set; }

        public decimal Precio { get; set; } // Precio al momento de agregarlo

        public decimal Total => Precio * Cantidad; // Calcula el total usando el precio guardado
    }
}