using System;
using System.Collections.Generic;

namespace servidor.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;

        // Relación con ItemsCompra
        public ICollection<ItemCompra> ItemsCompra { get; set; } = new List<ItemCompra>();
    }
}