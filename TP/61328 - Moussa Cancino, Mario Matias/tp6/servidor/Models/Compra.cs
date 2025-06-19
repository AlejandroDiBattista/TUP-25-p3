namespace servidor.Modelos;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public required string NombreCliente { get; set; }
    public required string ApellidoCliente { get; set; }
    public required string EmailCliente { get; set; }
    public List<ItemCompra> Items { get; set; } = new();
}