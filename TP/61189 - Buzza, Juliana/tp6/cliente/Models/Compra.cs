using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace cliente.Models 
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string? NombreCliente { get; set; }
        public string? ApellidoCliente { get; set; }
        public string? EmailCliente { get; set; }
        public string Status { get; set; } = string.Empty;

        public ICollection<ItemCompra> Items { get; set; } = new List<ItemCompra>();
    }
}
