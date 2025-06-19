using System;
using System.Collections.Generic;

namespace Servidor.Modelo
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string EmailCliente { get; set; }

        public List<ItemCompra> Items { get; set; }
    }
}
