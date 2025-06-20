using System.Collections.Generic;

namespace servidor.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }

        // Relación con ItemCompra
        public ICollection<ItemCompra> Items { get; set; } = new List<ItemCompra>();
    }
}