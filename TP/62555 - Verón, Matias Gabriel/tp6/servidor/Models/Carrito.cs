using Servidor.Models;
namespace Servidor.Models;

public class Carrito
{
    public int Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new();
}
