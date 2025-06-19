using System;
using System.Collections.Generic;

namespace Servidor.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string EmailCliente { get; set; }
        public ICollection<ItemCompra> Items { get; set; }
    }
}
