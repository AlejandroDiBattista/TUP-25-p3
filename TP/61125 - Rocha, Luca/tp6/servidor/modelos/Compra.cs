namespace servidor.Modelos;

public class Compra
{
    public int Id { get; set; }
    public List<ItemCompra> Items { get; set; } = new();
    public DateTime Fecha { get; set; }
    public decimal Total => Items.Sum(i => i.Subtotal);
}