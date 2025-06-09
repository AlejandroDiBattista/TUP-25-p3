namespace servidor.Models;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public List<ItemCarrito> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Importe);
}
