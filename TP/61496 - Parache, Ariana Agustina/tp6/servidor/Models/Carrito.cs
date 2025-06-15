using Compartido.Models;
using Compartido.Dtos;

namespace servidor.Models


{
    public class Carrito
    {
        public int Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
        public bool Confirmado { get; set; } = false;

        public string? NombreCliente { get; set; } = string.Empty;
        public string? ApellidoCliente { get; set; } = string.Empty;
        public string? EmailCliente { get; set; } = string.Empty;
    }
}
