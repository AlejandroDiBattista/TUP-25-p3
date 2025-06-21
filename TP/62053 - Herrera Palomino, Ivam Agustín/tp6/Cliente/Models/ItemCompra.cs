using System.Text.Json.Serialization;


namespace TiendaOnline.Frontend.Models
{
    /// <summary>
    /// Representa el detalle de un producto comprado en una transacción de compra.
    /// </summary>
    public class ItemCompra
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int CompraId { get; set; }
        [JsonIgnore]
        public Compra? Compra { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

}