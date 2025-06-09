namespace servidor.Models;

public class Compra
{
    public Guid Id { get; set; }
    public Cliente Cliente { get; set; } = new();
    public List<ItemCarrito> Items { get; set; } = new();
    public DateTime Fecha { get; set; }
}

