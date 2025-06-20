using System;
using System.Collections.Generic;

namespace Servidor.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";

        public List<ItemCarrito> Items { get; set; } = new();
    }
}
