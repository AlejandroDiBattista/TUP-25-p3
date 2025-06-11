using System;
using System.Collections.Generic;


namespace Servidor.Modelos  
{
    public class Compra 
    {
        public int Id { get; set; } 
        public DateTime Fecha { get; set; } 
        public decimal Total { get; set; } 
        public int ClienteId { get; set; }

        public List<ItemCompra> Items { get; set; } = new(); 
    }
 
}