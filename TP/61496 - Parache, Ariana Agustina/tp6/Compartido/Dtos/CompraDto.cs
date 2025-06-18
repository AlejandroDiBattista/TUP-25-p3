using Compartido.Models;

namespace Compartido.Dtos
{
    public class CompraDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public List<ItemCompraDto> Items { get; set; }
    }

    public class ItemCompraDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}