using cliente.Models;
using Compartido.Models;
using Compartido.Dtos;

namespace cliente.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
    }
}
