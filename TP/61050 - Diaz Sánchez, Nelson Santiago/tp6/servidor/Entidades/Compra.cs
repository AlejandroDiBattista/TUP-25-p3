using System;
using System.Collections.Generic;
using servidor.Data;

namespace servidor.Entidades
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public List<ItemCompra> Items { get; set; }
        public double Total { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string EmailCliente { get; set; }
    }
}
