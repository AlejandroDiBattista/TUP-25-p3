using System;
using System.Collections.Generic;

namespace servidor.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public List<ItemCompra> Items { get; set; }
        public decimal Total { get; set; }
    }
}
