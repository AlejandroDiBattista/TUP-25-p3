using System.ComponentModel.DataAnnotations;

namespace Servidor.Models
{
    public class Carrito
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
    }
}
