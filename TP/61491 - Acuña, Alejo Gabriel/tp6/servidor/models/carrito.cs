namespace servidor.Models;

public class Carrito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ItemCarrito> Items { get; set; } = new();
    public bool Confirmado { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
