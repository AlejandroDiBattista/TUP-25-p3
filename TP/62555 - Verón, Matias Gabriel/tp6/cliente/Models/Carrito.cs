namespace cliente.Models;

public class Carrito
{
    public int Id { get; set; }
    public List<CarritoItem> Items { get; set; } = new();
}
