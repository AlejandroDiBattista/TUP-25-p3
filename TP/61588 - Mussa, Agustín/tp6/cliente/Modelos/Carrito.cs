using System.Collections.Generic;

namespace cliente.Modelos
{
    public class Carrito
    {
        public int Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
    }
}