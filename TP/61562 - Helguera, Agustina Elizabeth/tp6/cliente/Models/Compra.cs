using System;

namespace cliente.Models
{
    public class Compra
    {
        public DateTime Fecha { get; set; }
        public List<ItemCompra> Items { get; set; }
        public decimal Total { get; set; }
    }
}
