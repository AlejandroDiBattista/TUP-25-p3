
using cliente.Models;

namespace Client.Models;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<ItemCompra> Items { get; set; } = new();
}
