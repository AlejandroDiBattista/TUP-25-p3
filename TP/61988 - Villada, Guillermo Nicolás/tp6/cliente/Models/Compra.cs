namespace cliente.Models;

public class Compra
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Cliente Cliente { get; set; } = new();
    public List<ItemCarrito> Items { get; set; } = new();
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public decimal Total => Items.Sum(i => i.Cantidad * i.PrecioUnitario);
}
