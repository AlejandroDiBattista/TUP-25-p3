using System;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public List<ItemCompra> Items { get; set; } = new();

        
        public decimal Total => Items.Sum(i => i.Cantidad * i.Producto.Precio);
    }
}
