using System.Collections.Generic;
using cliente.Modelos;
namespace cliente.Modelos
{
    public class Carrito
    {
        public int Id { get; set; }
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}